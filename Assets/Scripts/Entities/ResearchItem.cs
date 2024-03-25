using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Entities
{
    public class ResearchItem : MonoBehaviour
    {
        #region Properties
        cResearchItem researchItem;

        public List<cResearchItemRelation> Parents { get; set; }
        public List<cResearchItemRelation> Children { get; set; }
        public List<cWeapon> Weapons { get; set; }

        private Image Icon { get; set; }
        public Sprite Sprite { get; set; }
        private TextMeshProUGUI DisplayNameLabel { get; set; }
        private TextMeshProUGUI PriceLabel { get; set; }
        private Button ResItemButton { get; set; }

        #endregion

        #region Methods
        public void Init(cResearchItem item)
        {
            researchItem = item;

            InitUI();
        }

        public void InitUI()
        {
            // if item.researched show as finished (green?)
            // if all parents are researched, show as unlocked
            // else show as locked

            // button
            ResItemButton = transform.GetComponent<Button>();
            ResItemButton.onClick.AddListener(OnResearchItemButtonClick);

            // labels
            Icon = transform.Find("IconBackground").Find("Icon").GetComponent<Image>();
            DisplayNameLabel = transform.Find("DisplayName").GetComponent<TextMeshProUGUI>();
            PriceLabel = transform.Find("Price").GetComponent<TextMeshProUGUI>();

            Sprite = UIController.Instance.GetResearchIcon(researchItem.Name);
            Icon.sprite = Sprite;
            DisplayNameLabel.text = researchItem.DisplayName;
            PriceLabel.text = CustomUtils.FormatNumber(researchItem.Price);

            UpdateItem();
        }

        public void UpdateItem()
        {
            if (researchItem.Researched)
            {
                transform.GetComponent<Image>().color = new Color(0.25f, 0.55f, 0.2f, 1f);
                transform.Find("Background").GetComponent<Image>().color = new Color(0.2f, 0.4392157f, 0.1607843f, 1f);
            }


            // TODO update all lines coming to and out of researchItem
        }

        public void OnResearchItemButtonClick()
        {
            ProductionController.Instance.UIController.OpenResearchDetail(researchItem, this);
        }

        /// <summary>
        /// Tries to unlock researchItem and update according Weapons
        /// </summary>
        /// <returns>TRUE when item is researched FALSE when player doesnt have enough warfunds</returns>
        public bool UnlockResearchItem()
        {
            if (PlayerController.Instance.WarFunds < researchItem.Price) return false;

            PlayerController.Instance.AddWarFunds(researchItem.Price * -1);

            researchItem.Researched = true;

            // set color to green (as unlocked)
            UpdateItem();

            ProductionController.Instance.ChangeProductionItemState(
                Weapons.Select(w => w.Id).ToList(),
                WeaponState.Researched);

            return true;
        }
        #endregion
    }
}