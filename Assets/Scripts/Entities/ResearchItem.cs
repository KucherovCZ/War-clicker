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
        public DbResearchItem researchItem;

        public List<ResearchItem> Parents { get; set; }
        public List<DbResearchItemRelation> Children { get; set; }
        public List<DbWeapon> Weapons { get; set; }
        public bool IsUnlocked {
            get {
                return Parents.Where(p => !p.researchItem.Researched).Count() == 0;
            }
        }

        private Image Icon { get; set; }
        public Sprite Sprite { get; set; }
        private TextMeshProUGUI DisplayNameLabel { get; set; }
        private TextMeshProUGUI PriceLabel { get; set; }
        private Button ResItemButton { get; set; }

        public List<GameObject> Lines { get; set; }
        public List<GameObject> LinesOut { get; set; }
        private Transform TopAnchor { get; set; }
        private Transform BottomAnchor { get; set; }
        private static Vector3 LineChange {
            get {
                return new Vector3(0f, 84f);
            }
        }

        #endregion

        #region Methods
        public void Init(DbResearchItem item)
        {
            researchItem = item;
            Lines = new();
            LinesOut = new();

            if (Weapons.Count > 4)
                Logger.Log(LogLevel.WARNING, "ResearchItem: " + item.Name + ", ID: " + item.Id + " has too many weapons assigned (over 4). This will cause UI issues", "");
            if (Weapons.Where(w => w.Type != item.Type).Any())
                Logger.Log(LogLevel.WARNING, "ResearchItem: " + item.Name + ", ID: " + item.Id + " has links to weapons of other types, this is against game logic, fix data", "");

            InitUI();
        }

        public void InitUI()
        {
            // button
            ResItemButton = transform.GetComponent<Button>();
            ResItemButton.onClick.AddListener(OnResearchItemButtonClick);

            // labels
            Icon = transform.Find("IconBackground").Find("Icon").GetComponent<Image>();
            DisplayNameLabel = transform.Find("DisplayName").GetComponent<TextMeshProUGUI>();
            PriceLabel = transform.Find("Price").GetComponent<TextMeshProUGUI>();

            //Sprite = UIController.Instance.GetResearchIcon(researchItem.Name);
            //Icon.sprite = Sprite;
            Debug.LogWarning("ResearchItem.InitUI() is using default research icon");
            //Logger.Log(LogLevel.WARNING, "ResearchItem.InitUI() is using default research icon", "");

            DisplayNameLabel.text = researchItem.DisplayName;
            PriceLabel.text = CustomUtils.FormatNumber(researchItem.Price);

            TopAnchor = transform.Find("TopAnchor");
            BottomAnchor = transform.Find("BottomAnchor");

            foreach (ResearchItem item in Parents)
            {
                if (this.researchItem.Researched) transform.SetAsFirstSibling();
                Color color = this.researchItem.Researched ? new Color(0.25f, 0.55f, 0.2f, 1f) : item.researchItem.Researched ? Color.yellow : Color.white;
                List<GameObject> newLines = CreateLines(item.BottomAnchor.position, this.TopAnchor.position, color);
                item.LinesOut.AddRange(newLines);
                Lines.AddRange(newLines);
            }

            UpdateItem();
        }

        public void UpdateItem()
        {
            if (researchItem.Researched)
            {
                transform.SetAsLastSibling();

                transform.GetComponent<Image>().color = new Color(0.25f, 0.55f, 0.2f, 1f);
                transform.Find("Background").GetComponent<Image>().color = new Color(0.2f, 0.4392157f, 0.1607843f, 1f);

                foreach (GameObject line in Lines)
                    line.GetComponent<Image>().color = new Color(0.25f, 0.55f, 0.2f, 1f);

                foreach (GameObject line in LinesOut)
                    line.GetComponent<Image>().color = Color.yellow;
            }
        }

        public void OnResearchItemButtonClick()
        {
            ProductionController.Instance.UIController.OpenResearchDetail(researchItem, this);
        }

        public List<GameObject> CreateLines(Vector3 from, Vector3 to, Color color)
        {
            List<GameObject> returnList = new();

            if (from.x == to.x)
            {
                returnList.Add(CreateLine(from, to, color));
                return returnList;
            }

            Vector3 FromMidPoint = from - LineChange; // new Vector3(from.x, from.y + (to.y - from.y) / 2);
            Vector3 ToMidPoint = new Vector3(to.x, from.y - LineChange.y); // new Vector3(to.x, to.y - (to.y - from.y) / 2);

            returnList.Add(CreateLine(from, FromMidPoint, color));
            returnList.Add(CreateLine(FromMidPoint, ToMidPoint, color));
            returnList.Add(CreateLine(ToMidPoint, to, color));

            return returnList;
        }

        /// <summary>
        /// Generates lines with NULL sprite between two world points
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="color"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public GameObject CreateLine(Vector3 from, Vector3 to, Color color)
        {
            GameObject line = new GameObject();
            line.name = "ResItem line";

            Image newImage = line.AddComponent<Image>();
            newImage.sprite = Sprite.Create(null, new Rect(0, 0, 1, 1), new Vector2());
            newImage.color = color;

            RectTransform rect = line.GetComponent<RectTransform>();
            rect.SetParent(transform);
            rect.localScale = Vector3.one;

            rect.position = (from + to) / 2;
            Vector3 dif = from - to;
            rect.sizeDelta = new Vector3(Vector3.Distance(from, to) * 0.42f, 5);
            rect.rotation = dif.x == 0 ? Quaternion.Euler(0, 0, 90) : Quaternion.Euler(0, 0, 0);

            return line;
        }

        /// <summary>
        /// Tries to unlock researchItem and update according Weapons
        /// </summary>
        /// <returns>TRUE when item is researched FALSE when player doesnt have enough warfunds</returns>
        public void Unlock()
        {
            researchItem.Researched = true;

            // set color to green (as unlocked)
            UpdateItem();

            ProductionController.Instance.ChangeProductionItemState(
                Weapons.Select(w => w.Id).ToList(),
                WeaponState.Researched);
        }

        public bool CanUnlock()
        { 
            return PlayerController.Instance.TryBuyWarFunds(researchItem.Price);
        }
        #endregion
    }
}