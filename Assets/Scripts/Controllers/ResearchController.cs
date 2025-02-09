using Entities;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

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
        RemoveResearchItems();
        GenerateResearchItems();
    }

    public void LoadContent(SavedData data)
    {
        // stays here, will be used after ERA implementation
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
        // split research items by ERA, and calculate each size, then adjust background size by total calculation
        Dictionary<WeaponType, List<cResearchItem>> ResearchItemsByType = ResearchItemsDB.GroupBy(t => t.Type).ToDictionary(group => group.Key, group => group.ToList());

        foreach (var typeGroup in ResearchItemsByType)
        { 
            Dictionary<ResearchEra, List<cResearchItem>> ResearchItemsByEra = typeGroup.Value.GroupBy(t => t.Era).ToDictionary(group => group.Key, group => group.ToList());
            
            Transform currentContent = null;
            switch (typeGroup.Key)
            {
                case WeaponType.Infantry: currentContent = InfantryContent; break;
                case WeaponType.Artillery: currentContent = ArtilleryContent; break;
                case WeaponType.Armor: currentContent = ArmorContent; break;
                case WeaponType.Air: currentContent = AirContent; break;
                case WeaponType.Navy: currentContent = NavyContent; break;
            }

            foreach (var eraGroup in ResearchItemsByEra)
            {
                // create new background gameobject
                GameObject background = new GameObject($"Background{eraGroup.Key}");
                background.transform.parent = currentContent;
                Image bgImage = background.AddComponent<Image>(); // Canvas renderer is auto added with image

                RectTransform rectTransform = background.transform as RectTransform;
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(1, 1);
                rectTransform.localScale = new Vector3(1, 1, 1);

                bgImage.color = new Color(0.5f, 0.8f, 1f);


                // set gameobject height by items
                int rowCount = eraGroup.Value.Max(r => r.Row) - eraGroup.Value.Min(r => r.Row);



                rectTransform.sizeDelta = new Vector3(1, 1); //RowYChange * rowCount * -1);
                rectTransform.localPosition = new Vector3(0, 0);
                //Vector3 newPos = StarterPos + new Vector3(rectTransform.position.x, RowYChange * rowCount * -1);
                //rectTransform.position = rectTransform.TransformPoint(newPos);
            }

        }

        ResearchItems = new List<ResearchItem>();

        foreach (cResearchItem item in ResearchItemsDB)
        {
            Transform currentContent = null;
            switch (item.Type)
            {
                case WeaponType.Infantry: currentContent = InfantryContent; break;
                case WeaponType.Artillery: currentContent = ArtilleryContent; break;
                case WeaponType.Armor: currentContent = ArmorContent; break;
                case WeaponType.Air: currentContent = AirContent; break;
                case WeaponType.Navy: currentContent = NavyContent; break;
            }

            // Set researchItem transform
            GameObject newResearchItem = GameObject.Instantiate(ResearchItemPrefab, currentContent);
            Vector3 newPos = StarterPos + new Vector3(columnPos[item.Column], RowYChange * item.Row);
            newResearchItem.transform.position = newResearchItem.transform.TransformPoint(newPos);

            // Load research links
            ResearchItem resItemScript = newResearchItem.AddComponent<ResearchItem>();
            resItemScript.Parents = ResearchItems.Where(it => Relations.Where(r => r.ChildId == item.Id).Select(p => p.ParentId).Contains(it.researchItem.Id)).ToList();
            resItemScript.Children = Relations.Where(r => r.ChildId == item.Id).ToList();

            // Load weapon links
            List<int> WeaponIds = ItemWeapons.Where(w => w.ResearchItemId == item.Id).Select(w => w.WeaponId).ToList();
            resItemScript.Weapons = ProductionController.Instance.AllWeapons.Where(w => WeaponIds.Contains(w.Id)).ToList();

            resItemScript.Init(item);

            ResearchItems.Add(resItemScript);
        }

        ArtilleryContent.localScale = new Vector3(0, 1, 1);
        ArmorContent.localScale = new Vector3(0, 1, 1);
        AirContent.localScale = new Vector3(0, 1, 1);
        NavyContent.localScale = new Vector3(0, 1, 1);
    }

    public void RemoveResearchItems()
    {
        List<Transform> contents = new List<Transform>() { InfantryContent, ArtilleryContent, ArmorContent, AirContent, NavyContent };
        foreach (Transform content in contents)
        {
            foreach (Transform item in CustomUtils.GetAllChildren(content))
            { 
                GameObject.Destroy(item.gameObject);
            }
        }
    }

    #endregion 
}
