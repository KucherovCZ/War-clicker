﻿using Entities;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
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
    public List<DbResearchItem> ResearchItemsDB { get; set; }

    public List<ResearchItem> ResearchItems { get; set; }
    public List<DbResearchItemRelation> Relations { get; set; }
    public List<DbResearchItemWeapon> ItemWeapons { get; set; }
    public ResearchFilter Filter { get; set; }

    //public Dictionary<WeaponType, List<ResearchEra>> CompletedEras = new Dictionary<WeaponType, List<ResearchEra>>();

    public UIController UIController { get; set; }

    private GameObject ResearchItemPrefab { get; set; }

    public Transform InfantryContent { get; set; }
    private Transform ArtilleryContent { get; set; }
    private Transform ArmorContent { get; set; }
    private Transform AirContent { get; set; }
    private Transform NavyContent { get; set; }

    public WeaponType ActiveType { get; set; }

    private float RowYChange = 0; // calculated at start
    private int[] columnPos = { -190, 0, 190 };
    private Vector3 StarterPos = new Vector3(0f, -120);
    private Vector3 EraGap = new Vector3(0f, 80f);
    private Vector3 ResItemGap = new Vector3(0f, 80f);

    #endregion

    #region Methods

    public void Init(List<DbResearchItem> researchItems, List<DbResearchItemRelation> relations, List<DbResearchItemWeapon> itemWeapons, SavedData data)
    {
        ResearchItemsDB = researchItems;
        Relations = relations;
        ItemWeapons = itemWeapons;

        Filter = new ResearchFilter(data);
        UIController.UpdateFilterButtons(Filter);
        ResearchItems = new List<ResearchItem>();

        RowYChange = (((RectTransform)ResearchItemPrefab.transform).rect.height + ResItemGap.y) * -1;

        LoadContent(data);
        RemoveResearchItems();
        GenerateResearchItems();

        SetDefaultContent();
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

    public void LoadContent(SavedData data)
    {
        // Not used
    }

    private void SetDefaultContent()
    {
        ArtilleryContent.localScale = new Vector3(0, 1, 1);
        ArmorContent.localScale = new Vector3(0, 1, 1);
        AirContent.localScale = new Vector3(0, 1, 1);
        NavyContent.localScale = new Vector3(0, 1, 1);
    }

    private void GenerateResearchItems(WeaponType? limitType = null)
    {
        // split research items by ERA, and calculate each size, then adjust background size by total calculation
        Dictionary<WeaponType, List<DbResearchItem>> ResearchItemsByType = ResearchItemsDB.GroupBy(t => t.Type).ToDictionary(group => group.Key, group => group.ToList());
        if (limitType != null)
            ResearchItemsByType = ResearchItemsByType.Where(g => g.Key == limitType.Value).ToDictionary(group => group.Key, group => group.Value);


        // For each UI tab (by weapon type)
        foreach (var typeGroup in ResearchItemsByType)
        {
            Dictionary<ResearchEra, List<DbResearchItem>> ResearchItemsByEra = typeGroup.Value.GroupBy(t => t.Era).ToDictionary(group => group.Key, group => group.ToList());

            Transform currentContent = GetContent(typeGroup.Key);

            int typeStartRow = typeGroup.Value.Min(r => r.Row);
            int typeRowCount = typeGroup.Value.Max(r => r.Row) - typeStartRow + 1;

            float yOffset = 0;

            foreach (var eraGroup in ResearchItemsByEra)
            {
                if (!Filter.EraState[eraGroup.Key])
                    continue;
                    
                //if (eraGroup.Value.Where(r => !r.Researched).Count() == 0)
                //    CompletedEras[typeGroup.Key].Add(eraGroup.Key);

                int eraStartRow = eraGroup.Value.Min(r => r.Row);
                int eraRowCount = eraGroup.Value.Max(r => r.Row) - eraStartRow + 1;

                yOffset -= EraGap.y;

                GameObject background = CreateBackground(eraGroup.Key.ToString(), currentContent, eraStartRow, eraRowCount, eraGroup.Key, yOffset);

                foreach (var item in eraGroup.Value)
                {
                    ResearchItems.Add(CreateResearchItem(item, currentContent, yOffset));
                }
            }

            AdjustContentSize(currentContent as RectTransform, typeRowCount, yOffset);
        }
    }

    private void RemoveResearchItems(WeaponType? limitType = null)
    {
        List<Transform> contents = null;

        if (limitType != null)
        {
            contents = new List<Transform>() { GetContent(limitType.Value) };
        }
        else
        {
            contents = new List<Transform>() { InfantryContent, ArtilleryContent, ArmorContent, AirContent, NavyContent };
        }

        foreach (Transform content in contents)
        {
            foreach (Transform item in CustomUtils.GetAllChildren(content))
            {
                if (item.name.Contains("Research"))
                    ResearchItems.Remove(item.GetComponent<ResearchItem>());

                GameObject.Destroy(item.gameObject);
            }
        }
    }

    /// <summary>
    /// Updates filter, redraws UI and returns new state of selected filter
    /// </summary>
    /// <param name="era"></param>
    /// <returns></returns>
    public bool UpdateFilter(ResearchEra era)
    {
        Filter.EraState[era] = !Filter.EraState[era];

        RemoveResearchItems(ActiveType);
        GenerateResearchItems(ActiveType);

        return Filter.EraState[era];
    }

    private void AdjustContentSize(RectTransform content, int rowCount, float yOffset)
    {
        content.position += new Vector3(0, -10000); // -10000 so its scrolled all the way up

        if (rowCount > 3)
            content.sizeDelta = new Vector3(content.sizeDelta.x, content.sizeDelta.y + (-1 * rowCount * RowYChange) + yOffset * -1);
        else
            // if too few rows, get size from parent Viewport
            content.sizeDelta = new Vector3(content.sizeDelta.x, ((RectTransform)(content.parent.transform)).sizeDelta.y);
    }

    private GameObject CreateBackground(string name, Transform parent, int startRow, int rowCount, ResearchEra era, float yOffset)
    {
        // create new background gameobject
        GameObject background = GameObject.Instantiate(AssetReferences.Instance.ResearchBackgroundPrefab, parent);
        background.name = ($"{name}_Background");
        background.transform.Find("Label").Find("Text").GetComponent<TextMeshProUGUI>().text = EnumUtils.GetEnumDescription(era);
        Image bgImage = background.GetComponent<Image>();
        bgImage.color = CustomMapping.EraColorMap[era];
        RectTransform rectTransform = background.transform as RectTransform;

        if (startRow == 0)
        {
            rectTransform.sizeDelta = new Vector2((rectTransform.parent as RectTransform).sizeDelta.x, (rowCount * RowYChange * -1) + EraGap.y);
            rectTransform.localPosition = new Vector3(0, (yOffset + EraGap.y) + rectTransform.sizeDelta.y / 2 * -1);
        }
        else
        {
            rectTransform.sizeDelta = new Vector2((rectTransform.parent as RectTransform).sizeDelta.x, rowCount * RowYChange * -1 + EraGap.y);
            rectTransform.localPosition = new Vector3(0, (yOffset + EraGap.y) + startRow * RowYChange + (rectTransform.sizeDelta.y / 2 * -1));
        }

        return background;
    }

    private ResearchItem CreateResearchItem(DbResearchItem item, Transform currentContent, float yOffset)
    {
        // Set researchItem transform
        GameObject newResearchItem = GameObject.Instantiate(ResearchItemPrefab, currentContent);
        Vector3 newPos = StarterPos + new Vector3(columnPos[item.Column], RowYChange * item.Row + yOffset);
        newResearchItem.transform.position = newResearchItem.transform.TransformPoint(newPos);

        // Load research links
        ResearchItem resItemScript = newResearchItem.AddComponent<ResearchItem>();
        resItemScript.Parents = ResearchItems.Where(it => Relations.Where(r => r.ChildId == item.Id).Select(p => p.ParentId).Contains(it.researchItem.Id)).ToList();
        resItemScript.Children = Relations.Where(r => r.ChildId == item.Id).ToList();

        // Load weapon links
        List<int> WeaponIds = ItemWeapons.Where(w => w.ResearchItemId == item.Id).Select(w => w.WeaponId).ToList();
        resItemScript.Weapons = ProductionController.Instance.AllWeapons.Where(w => WeaponIds.Contains(w.Id)).ToList();

        resItemScript.Init(item);

        return resItemScript;
    }

    public Transform GetContent(WeaponType type)
    {
        switch (type)
        {
            case WeaponType.Infantry: return InfantryContent;
            case WeaponType.Artillery: return ArtilleryContent;
            case WeaponType.Armor: return ArmorContent;
            case WeaponType.Air: return AirContent;
            case WeaponType.Navy: return NavyContent;
            default: return null;
        }
    }

    #endregion 
}
