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
        public DbWeapon Weapon { get; set; }

        private Image Icon { get; set; }
        public Sprite Sprite { get; set; }
        private TextMeshProUGUI DisplayNameLabel { get; set; }
        private TextMeshProUGUI FlagsLabel { get; set; }
        private TextMeshProUGUI TimeLabel { get; set; }
        private TextMeshProUGUI PriceLabel { get; set; }
        private TextMeshProUGUI StoredLabel { get; set; }
        private Button ProdItemButton { get; set; }
        private Slider LoadingBar { get; set; }
        private GameObject LockedOverlay { get; set; }
        private GameObject ResearchedOverlay { get; set; }
        private Button BuyEquipButton { get; set; }
        private TextMeshProUGUI UnlockPriceLabel { get; set; }

        public void Init(DbWeapon weapon)
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
            // gameObjects
            LockedOverlay = transform.Find("LockedOverlay").gameObject;
            ResearchedOverlay = transform.Find("ResearchedOverlay").gameObject;

            // button
            ProdItemButton = transform.GetComponent<Button>();
            ProdItemButton.onClick.AddListener(OnProdItemButtonClick);

            BuyEquipButton = ResearchedOverlay.transform.Find("BuyEquipButton").GetComponent<Button>();
            BuyEquipButton.onClick.AddListener(OnBuyEquipButtonClick);

            // labels
            Icon = transform.Find("IconBackground").Find("Icon").GetComponent<Image>();
            DisplayNameLabel = transform.Find("DisplayName").GetComponent<TextMeshProUGUI>();
            FlagsLabel = transform.Find("Flags").GetComponent<TextMeshProUGUI>();
            TimeLabel = transform.Find("Time").GetComponent<TextMeshProUGUI>();
            PriceLabel = transform.Find("Price").GetComponent<TextMeshProUGUI>();
            StoredLabel = transform.Find("Stored").GetComponent<TextMeshProUGUI>();
            LoadingBar = transform.Find("LoadingBar").GetComponent<Slider>();
            UnlockPriceLabel = ResearchedOverlay.transform.Find("BuyEquipButton").Find("UnlockPrice").GetComponent<TextMeshProUGUI>();

            Sprite = UIController.Instance.GetWeaponIcon(Weapon.Name);
            Icon.sprite = Sprite;
            DisplayNameLabel.text = Weapon.DisplayName;
            FlagsLabel.text = Weapon.FlagsString;
            PriceLabel.text = CustomUtils.FormatNumber(Weapon.SellPrice);
            StoredLabel.text = Weapon.Stored.ToString();
            UnlockPriceLabel.text = CustomUtils.FormatNumber(Weapon.UnlockPrice);

            UpdateWeaponState(Weapon.State);
        }

        public void UpdateWeaponState(WeaponState newState)
        {
            Weapon.State = newState;

            LockedOverlay.SetActive(newState == WeaponState.Locked);
            ResearchedOverlay.SetActive(newState == WeaponState.Researched);
            LoadingBar.gameObject.SetActive(newState == WeaponState.Active);
            ProdItemButton.enabled = (newState == WeaponState.Active);
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
                    if (ProductionController.Instance.WarehouseCapacity[(int)Weapon.Type] - ProductionController.Instance.WarehouseUsed[(int)Weapon.Type] <= 0)
                    {
                        Weapon.Autosell = true;
                        PlayerController.Instance.AddMoney(Weapon.SellPrice);
                    }
                    else
                    {
                        Weapon.Stored++;
                        ProductionController.Instance.WarehouseUsed[(int)Weapon.Type]++;
                        UIController.Instance.UpdateDetailStored();
                    }
                }
            } while (prodTime == Weapon.ProductionTime);

            ResetProduction(Weapon.ProductionTime);
        }

        #endregion

        #region EventHandler

        public void OnProdItemButtonClick()
        {
            UIController.Instance.OpenWeaponDetail(Weapon, this);
        }

        public void OnBuyEquipButtonClick()
        {
            if (PlayerController.Instance.TryBuyMoney(Weapon.UnlockPrice))
            {
                UpdateWeaponState(WeaponState.Active);
                Weapon.Autosell = CustomUtils.DefaultAutosellSettings;
            }
        }

        #endregion
    }
}
