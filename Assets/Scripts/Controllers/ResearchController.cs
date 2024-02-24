using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
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
    public List<cResearchItem> ResearchItems { get; set; }
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
    private int[] columnPos = { 0, 100, 200 };

    #endregion

    #region Methods

    public void Init(List<cResearchItem> researchItems, List<cResearchItemRelation> relations, List<cResearchItemWeapon> itemWeapons, SavedData data)
    {
        ResearchItems = researchItems;
        Relations = relations;
        ItemWeapons = itemWeapons;

        RowYChange = (((RectTransform)ResearchItemPrefab.transform).rect.height + 10) * -1;

        LoadContent(data);
        GenerateResearchItems();
    }

    public void LoadContent(SavedData data)
    {
    }

    public void InitGameObjects(GameObject researchItemPrefab, GameObject ResearchPage)
    {
        ResearchItemPrefab = researchItemPrefab;
    }


    public void GenerateResearchItems()
    {
        ResearchEra currentEra = ResearchEra.PreWW1;

        List<ResearchItem> Items = new List<ResearchItem>();
        foreach (cResearchItem item in ResearchItems)
        {
            ResearchItem newItem = new ResearchItem(item);
            newItem.Parents = Relations.Where(r => r.ParentId == newItem.Id).ToList();
            newItem.Children = Relations.Where(r => r.ChildId == newItem.Id).ToList();
            newItem.Weapons = ItemWeapons.Where(w => w.ResearchItemId == newItem.Id).ToList();
        }

        foreach (var item in Items)
        { 
            
        }

        
        //int infCount = 0;
        //int artCount = 0;
        //int armCount = 0;
        //int airCount = 0;
        //int navCount = 0;

        //foreach (cWeapon weapon in AllWeapons)
        //{
        //    GameObject newProductionItem = null;
        //    switch (weapon.Type)
        //    {
        //        case WeaponType.Infantry:
        //            {
        //                newProductionItem = GameObject.Instantiate(ProductionItemPrefab, InfantryContent);
        //                Vector3 newPos = StartPosition + new Vector3(0f, PosYChange * infCount, 0f);
        //                newProductionItem.transform.position = newProductionItem.transform.TransformPoint(newPos);
        //                infCount++;
        //                break;
        //            }

        //        case WeaponType.Artillery:
        //            {
        //                newProductionItem = GameObject.Instantiate(ProductionItemPrefab, ArtilleryContent);
        //                Vector3 newPos = StartPosition + new Vector3(0f, PosYChange * artCount, 0f);
        //                newProductionItem.transform.position = newProductionItem.transform.TransformPoint(newPos);
        //                artCount++;
        //                break;
        //            }

        //        case WeaponType.Armor:
        //            {
        //                newProductionItem = GameObject.Instantiate(ProductionItemPrefab, ArmorContent);
        //                Vector3 newPos = StartPosition + new Vector3(0f, PosYChange * armCount, 0f);
        //                newProductionItem.transform.position = newProductionItem.transform.TransformPoint(newPos);
        //                armCount++;
        //                break;
        //            }

        //        case WeaponType.Air:
        //            {
        //                newProductionItem = GameObject.Instantiate(ProductionItemPrefab, AirContent);
        //                Vector3 newPos = StartPosition + new Vector3(0f, PosYChange * airCount, 0f);
        //                newProductionItem.transform.position = newProductionItem.transform.TransformPoint(newPos);
        //                airCount++;
        //                break;
        //            }

        //        case WeaponType.Navy:
        //            {
        //                newProductionItem = GameObject.Instantiate(ProductionItemPrefab, NavyContent);
        //                Vector3 newPos = StartPosition + new Vector3(0f, PosYChange * navCount, 0f);
        //                newProductionItem.transform.position = newProductionItem.transform.TransformPoint(newPos);
        //                navCount++;
        //                break;
        //            }
        //    }

        //    if (newProductionItem == null) continue;

        //    newProductionItem.name = weapon.Name;
        //    ProductionItem prItemScript = newProductionItem.AddComponent<ProductionItem>();
        //    prItemScript.Init(weapon);

        //    ProdItems.Add(prItemScript);
        //}
    }

    #endregion 
}
