using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Entities
{
    /// <summary>
    /// Class for each production item
    /// </summary>
    public class ProductionItem : MonoBehaviour
    {
        public cWeapon Weapon { get; set; }

        private Image Icon { get; set; }
        public Sprite Sprite { get; set; }
        private TextMeshProUGUI DisplayNameLabel { get; set; }
        private TextMeshProUGUI FlagsLabel { get; set; }
        private TextMeshProUGUI TimeLabel { get; set; }
        private TextMeshProUGUI PriceLabel { get; set; }
        private TextMeshProUGUI StoredLabel { get; set; }
        private Button ProdItemButton { get; set; }
        private Slider LoadingBar { get; set; }

        public void Init(cWeapon weapon)
        {
            Weapon = weapon;

            InitUI();

            if (Weapon.FactoriesAssigned != 0)
            {
                ProductionController.Instance.UseFactories(Weapon.Type, Weapon.FactoriesAssigned);
                StartProduction(Weapon.ProductionTime);
            }
        }

        private void InitUI()
        {
            // button
            ProdItemButton = transform.GetComponent<Button>();
            ProdItemButton.onClick.AddListener(OnProdItemButtonClick);

            // labels
            Icon = transform.Find("IconBackground").Find("Icon").GetComponent<Image>();
            DisplayNameLabel = transform.Find("DisplayName").GetComponent<TextMeshProUGUI>();
            FlagsLabel = transform.Find("Flags").GetComponent<TextMeshProUGUI>();
            TimeLabel = transform.Find("Time").GetComponent<TextMeshProUGUI>();
            PriceLabel = transform.Find("Price").GetComponent<TextMeshProUGUI>();
            StoredLabel = transform.Find("Stored").GetComponent<TextMeshProUGUI>();
            LoadingBar = transform.Find("LoadingBar").GetComponent<Slider>();

            Sprite = UIController.Instance.GetWeaponIcon(Weapon.Name);
            Icon.sprite = Sprite;
            DisplayNameLabel.text = Weapon.DisplayName;
            FlagsLabel.text = Weapon.FlagsString;
            PriceLabel.text = CustomUtils.FormatNumber(Weapon.SellPrice);
            StoredLabel.text = Weapon.Stored.ToString();

            UpdateWeaponState(Weapon.State);
        }

        public void UpdateWeaponState(WeaponState newState)
        {
            Weapon.State = newState;

            switch (newState)
            {
                case WeaponState.Locked:
                    // show overlay for production item: locked
                    break;
                case WeaponState.Researched:
                    // show overlay for prodctuon item: researched, but disabled
                    break;
                case WeaponState.Active:
                    // dont show any overlay
                    break;
            }
        }

        // this method is called 50 times / second (every 20 miliseconds)
        int counter = 0;
        int framesPerUpdate = 50 / CustomUtils.UpdateFrequency;
        float timeChange = 1f / CustomUtils.UpdateFrequency;
        private void FixedUpdate()
        {
            if (ActiveCoroutine != null)
            {
                counter++;
                if (counter == framesPerUpdate)
                {
                    counter = 0;
                    remainingTime -= timeChange;


                    TimeLabel.text = CustomUtils.FormatTime(remainingTime);
                    StoredLabel.text = Weapon.Stored.ToString();
                    LoadingBar.value = 1.1f - (remainingTime / Weapon.ProductionTime);
                }
            }
        }

        #region ActiveProduction
        public Coroutine ActiveCoroutine = null;

        public float remainingTime = 0;

        public void StartProduction(float prodTime)
        {
            if (ActiveCoroutine == null)
            {
                ActiveCoroutine = StartCoroutine(ProductionCoroutine(prodTime));
                remainingTime = prodTime;
            }
        }

        public void StopProduction()
        {
            if (ActiveCoroutine != null)
            {
                StopCoroutine(ActiveCoroutine);
                remainingTime = 0;
                ActiveCoroutine = null;
                TimeLabel.text = "";
                LoadingBar.value = 0.1f;
            }
        }

        public void FactoriesUpdated(int change)
        {
            float remainingPercentage = Weapon.ProductionTime == 0 ? 1 : remainingTime / Weapon.ProductionTime;

            Weapon.FactoriesAssigned += change;
            ProductionController.Instance.UseFactories(Weapon.Type, change);

            if (Weapon.FactoriesAssigned == 0)
            {
                Weapon.ProductionTime = 0;
                StopProduction();
                return;
            }

            Weapon.ProductionTime = 0; // set to zero, time gets recalculated automatically
            remainingTime = Weapon.ProductionTime * remainingPercentage;

            ResetProduction(remainingTime);
        }

        public void ResetProduction(float prodTime)
        {
            StopProduction();
            StartProduction(prodTime);
        }

        private IEnumerator ProductionCoroutine(float prodTime)
        {
            WaitForSeconds wait = new WaitForSeconds(prodTime);
            do
            {
                remainingTime = prodTime;

                yield return wait;

                if (Weapon.Autosell)
                {
                    PlayerController.Instance.AddMoney(Weapon.SellPrice);
                }
                else
                {
                    Weapon.Stored++;
                    UIController.Instance.UpdateDetailStored();
                }
            } while (prodTime == Weapon.ProductionTime);

            ResetProduction(Weapon.ProductionTime);
        }

        #endregion

        #region EventHandler

        public void OnProdItemButtonClick()
        {
            ProductionController.Instance.UIController.OpenWeaponDetail(Weapon, this);
        }

        #endregion
    }
}
