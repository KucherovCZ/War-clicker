using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

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

            Icon.sprite = UIController.Instance.GetResearchIcon(researchItem.Name);
            DisplayNameLabel.text = researchItem.DisplayName;
            PriceLabel.text = CustomUtils.FormatNumber(researchItem.Price);
        }

        public void OnResearchItemButtonClick()
        {
            ProductionController.Instance.UIController.OpenResearchDetail(researchItem, this);
        }
        #endregion
    }
}