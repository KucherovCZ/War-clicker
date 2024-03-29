﻿using Entities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class ProductionController
{
    #region Singleton
    private static ProductionController m_Instance;
    public static ProductionController Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = new ProductionController();
            return m_Instance;
        }
    }

    private ProductionController() { }
    #endregion

    #region Fields and properties
    public List<cWeapon> AllWeapons { get; set; }
    public List<ProductionItem> ProdItems { get; set; }
    public UIController UIController { get; set; }

    private GameObject ProductionItemPrefab { get; set; }
    private Transform InfantryContent { get; set; }
    private Transform ArtilleryContent { get; set; }
    private Transform ArmorContent { get; set; }
    private Transform AirContent { get; set; }
    private Transform NavyContent { get; set; }

    private float PosYChange = 0;
    private Vector3 StartPosition = Vector3.zero;

    public int[] Factories = { 0, 0, 0, 0, 0 };
    public int[] UsedFactories = { 0, 0, 0, 0, 0 };
    #endregion

    #region Methods

    public void Init(List<cWeapon> weapons, SavedData data)
    {
        AllWeapons = weapons;
        ProdItems = new List<ProductionItem>();
        StartPosition.y = -95;
        PosYChange = (((RectTransform)ProductionItemPrefab.transform).rect.height + 10) * -1;

        LoadContent(data);
        GenerateWeaponContent();
    }

    public void LoadContent(SavedData data)
    {
        // TODO load all factories from PlayerPrefs (or sql, doesnt matter)
        Factories = data.factories;

        // TEMP
        //Factories[(int)WeaponType.Infantry] = 5;
        //Factories[(int)WeaponType.Artillery] = 5;
        //Factories[(int)WeaponType.Armor] = 10;
        // TEMP
    }

    public void InitGameObjects(GameObject prefab, GameObject productionPage)
    {
        ProductionItemPrefab = prefab;

        Transform viewPort = productionPage.transform.Find("Viewport");
        InfantryContent = viewPort.Find("Infantry");
        ArtilleryContent = viewPort.Find("Artillery");
        ArmorContent = viewPort.Find("Armor");
        AirContent = viewPort.Find("Air");
        NavyContent = viewPort.Find("Navy");
    }

    public void GenerateWeaponContent()
    {
        int[] weaponCounts = new int[] { 0, 0, 0, 0, 0 };

        foreach (cWeapon weapon in AllWeapons)
        {
            GameObject newProductionItem = null;
            Transform currentContent = null;
            switch (weapon.Type)
            {
                case WeaponType.Infantry: currentContent = InfantryContent; break;
                case WeaponType.Artillery: currentContent = ArtilleryContent; break;
                case WeaponType.Armor: currentContent = ArmorContent; break;
                case WeaponType.Air: currentContent = AirContent; break;
                case WeaponType.Navy: currentContent = NavyContent; break;
            }

            newProductionItem = GameObject.Instantiate(ProductionItemPrefab, currentContent);
            Vector3 newPos = StartPosition + new Vector3(0f, PosYChange * weaponCounts[(int)weapon.Type], 0f);
            newProductionItem.transform.position = newProductionItem.transform.TransformPoint(newPos);
            weaponCounts[(int)weapon.Type]++;
            
            if (newProductionItem == null) continue;

            newProductionItem.name = weapon.Name;
            ProductionItem prItemScript = newProductionItem.AddComponent<ProductionItem>();
            prItemScript.Init(weapon);

            ProdItems.Add(prItemScript);
        }
    }

    public int GetFactories(WeaponType type)
    {
        return Factories[(int)type];
    }

    public int GetFreeFactories(WeaponType type)
    {
        return Factories[(int)type] - UsedFactories[(int)type];
    }

    public void UseFactories(WeaponType type, int amount)
    {
        UsedFactories[(int)type] += amount;
    }

    public void ChangeProductionItemState(List<int> itemIds, WeaponState state)
    {
        List<ProductionItem> itemsToUpdate = ProdItems.Where(pr => itemIds.Contains(pr.Weapon.Id)).ToList();
        foreach (ProductionItem item in itemsToUpdate)
        {
            item.UpdateWeaponState(state);
        }
    }
    #endregion 
}
