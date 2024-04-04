using Entities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResearchController
{
    #region Singleton
    private static ResearchController m_Instance;
    public static ResearchController Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = new ResearchController();
            return m_Instance;
        }
    }

    private ResearchController() { }
    #endregion

    #region Fields and properties
    public List<cResearchItem> ResearchItemsDB { get; set; }

    public List<ResearchItem> ResearchItems { get; set; }
    public List<cResearchItemRelation> Relations { get; set; }
    public List<cResearchItemWeapon> ItemWeapons { get; set; }

    public UIController UIController { get; set; }

    private GameObject ResearchItemPrefab { get; set; }
    private Transform InfantryContent { get; set; }
    private Transform ArtilleryContent { get; set; }
    private Transform ArmorContent { get; set; }
    private Transform AirContent { get; set; }
    private Transform NavyContent { get; set; }

    private float RowYChange = 0;
    private int[] columnPos = { -190, 0, 190 };
    private Vector3 StarterPos = new Vector3(0f, -150f);

    #endregion

    #region Methods

    public void Init(List<cResearchItem> researchItems, List<cResearchItemRelation> relations, List<cResearchItemWeapon> itemWeapons, SavedData data)
    {
        ResearchItemsDB = researchItems;
        Relations = relations;
        ItemWeapons = itemWeapons;

        RowYChange = (((RectTransform)ResearchItemPrefab.transform).rect.height + 70) * -1;

        LoadContent(data);
        GenerateResearchItems();
    }

    public void LoadContent(SavedData data)
    {
    }

    public void InitGameObjects(GameObject researchItemPrefab, GameObject ResearchPage)
    {
        ResearchItemPrefab = researchItemPrefab;

        Transform viewPort = ResearchPage.transform.Find("Viewport");
        InfantryContent = viewPort.Find("Infantry");
        ArtilleryContent = viewPort.Find("Artillery");
        ArmorContent = viewPort.Find("Armor");
        AirContent = viewPort.Find("Air");
        NavyContent = viewPort.Find("Navy");
    }

    public void GenerateResearchItems()
    {
        //ResearchEra currentEra = ResearchEra.PreWW1;
        ResearchItems = new List<ResearchItem>();

        foreach (cResearchItem item in ResearchItemsDB)
        {
            GameObject newResearchItem = null;
            Transform currentContent = null;
            switch (item.Type)
            {
                case WeaponType.Infantry: currentContent = InfantryContent; break;
                case WeaponType.Artillery: currentContent = ArtilleryContent; break;
                case WeaponType.Armor: currentContent = ArmorContent; break;
                case WeaponType.Air: currentContent = AirContent; break;
                case WeaponType.Navy: currentContent = NavyContent; break;
            }

            newResearchItem = GameObject.Instantiate(ResearchItemPrefab, currentContent);
            Vector3 newPos = StarterPos + new Vector3(columnPos[item.Column], RowYChange * item.Row);
            newResearchItem.transform.position = newResearchItem.transform.TransformPoint(newPos);

            ResearchItem resItemScript = newResearchItem.AddComponent<ResearchItem>();
            resItemScript.Parents = Relations.Where(r => r.ParentId == item.Id).ToList();
            resItemScript.Children = Relations.Where(r => r.ChildId == item.Id).ToList();
            List<int> WeaponIds = ItemWeapons.Where(w => w.ResearchItemId == item.Id).Select(w => w.WeaponId).ToList();
            resItemScript.Weapons = ProductionController.Instance.AllWeapons.Where(w => WeaponIds.Contains(w.Id)).ToList();

            if (resItemScript.Weapons.Count > 4)
                Debug.LogWarning("ResearchItem: " + item.Name + ", ID: " + item.Id + " has too many weapons assigned (over 4). This will cause UI issues");
            if (resItemScript.Weapons.Where(w => w.Type != item.Type).Any())
                Debug.LogWarning("ResearchItem: " + item.Name + ", ID: " + item.Id + " has links to weapons of other types, this is against game logic, fix data");

            resItemScript.Init(item);

            ResearchItems.Add(resItemScript);
        }

        ArtilleryContent.localScale = new Vector3(0, 1, 1);
        ArmorContent.localScale = new Vector3(0, 1, 1);
        AirContent.localScale = new Vector3(0, 1, 1);
        NavyContent.localScale = new Vector3(0, 1, 1);
    }

    #endregion 
}
