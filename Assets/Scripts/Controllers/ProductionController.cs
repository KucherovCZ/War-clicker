using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

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
    public int[] UsedFactories = { 0, 0, 0, 0, 0};
    #endregion

    #region Methods

    public void Init(List<cWeapon> weapons, SavedData data)
    {
        AllWeapons = weapons;
        ProdItems = new List<ProductionItem>();
        StartPosition.y = -95;
        PosYChange = ((RectTransform)ProductionItemPrefab.transform).rect.height + 10;
        PosYChange *= -1;

        LoadContent(data);
        GenerateWeaponContent();
    }

    public void LoadContent(SavedData data)
    {
        // TODO load all factories from PlayerPrefs (or sql, doesnt matter)
        //Factories = data.factories;

        // TEMP
        Factories[(int)WeaponType.Infantry] = 5;
        Factories[(int)WeaponType.Artillery] = 5;
        Factories[(int)WeaponType.Armor] = 10;
        // TEMP
    }

    public void InitGameObjects(GameObject prefab, Transform infantry, Transform artillery, Transform armor, Transform air, Transform navy)
    {
        ProductionItemPrefab = prefab;
        InfantryContent = infantry;
        ArtilleryContent = artillery;
        ArmorContent = armor;
        AirContent = air;
        NavyContent = navy;
    }

    public void GenerateWeaponContent()
    {
        int infCount = 0;
        int artCount = 0;
        int armCount = 0;
        int airCount = 0;
        int navCount = 0;

        foreach (cWeapon weapon in AllWeapons)
        {
            GameObject newProductionItem = null;
            switch (weapon.Type)
            {
                case WeaponType.Infantry:
                    {
                        newProductionItem = GameObject.Instantiate(ProductionItemPrefab, InfantryContent);
                        Vector3 newPos = StartPosition + new Vector3(0f, PosYChange * infCount, 0f);
                        newProductionItem.transform.position = newProductionItem.transform.TransformPoint(newPos);
                        infCount++;
                        break;
                    }

                case WeaponType.Artillery:
                    {
                        newProductionItem = GameObject.Instantiate(ProductionItemPrefab, ArtilleryContent);
                        Vector3 newPos = StartPosition + new Vector3(0f, PosYChange * artCount, 0f);
                        newProductionItem.transform.position = newProductionItem.transform.TransformPoint(newPos);
                        artCount++;
                        break;
                    }

                case WeaponType.Armor:
                    {
                        newProductionItem = GameObject.Instantiate(ProductionItemPrefab, ArmorContent);
                        Vector3 newPos = StartPosition + new Vector3(0f, PosYChange * armCount, 0f);
                        newProductionItem.transform.position = newProductionItem.transform.TransformPoint(newPos);
                        armCount++;
                        break;
                    }

                case WeaponType.Air:
                    {
                        newProductionItem = GameObject.Instantiate(ProductionItemPrefab, AirContent);
                        Vector3 newPos = StartPosition + new Vector3(0f, PosYChange * airCount, 0f);
                        newProductionItem.transform.position = newProductionItem.transform.TransformPoint(newPos);
                        airCount++;
                        break;
                    }

                case WeaponType.Navy:
                    {
                        newProductionItem = GameObject.Instantiate(ProductionItemPrefab, NavyContent);
                        Vector3 newPos = StartPosition + new Vector3(0f, PosYChange * navCount, 0f);
                        newProductionItem.transform.position = newProductionItem.transform.TransformPoint(newPos);
                        navCount++;
                        break;
                    }
            }

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
    #endregion 
}
