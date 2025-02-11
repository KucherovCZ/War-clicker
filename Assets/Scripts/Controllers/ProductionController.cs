using Entities;
using System.Collections.Generic;
using System.Linq;
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
    public List<DbWeapon> AllWeapons { get; set; }
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

    public int[] FactoryLevel = { 1, 0, 0, 0, 0 };
    public int[] Factories = { 1, 0, 0, 0, 0 };
    public int[] UsedFactories = { 0, 0, 0, 0, 0 };

    public int[] WarehouseLevel = { 1, 0, 0, 0, 0 };
    public int[] WarehouseUsed = { 50, 0, 0, 0, 0 };
    public int[] WarehouseCapacity = { 0, 0, 0, 0, 0 };
    #endregion

    #region Methods

    public void Init(List<DbWeapon> weapons, SavedData data)
    {
        AllWeapons = weapons;
        ProdItems = new List<ProductionItem>();
        StartPosition.y = -90;
        PosYChange = (((RectTransform)ProductionItemPrefab.transform).rect.height + 10) * -1;

        LoadContent(data);
        RemoveWeaponContent();
        GenerateWeaponContent();
    }

    public void LoadContent(SavedData data)
    {
        Factories = data.factories ??  new int[5];
        FactoryLevel = data.factoryLevels ?? new int[5];
        WarehouseCapacity = data.warehouses ?? new int[5];
        WarehouseLevel = data.warehouseLevels ?? new int[5];

        // foreach weapontype
        for (int i = 0; i < 5; i++)
        {
            WarehouseUsed[i] = AllWeapons.Where(w => (int)w.Type == i).Sum(w => w.Stored);
        }

        // TEMP
        //Factories[(int)WeaponType.Infantry] = 5;
        //Factories[(int)WeaponType.Artillery] = 5;
        //Factories[(int)WeaponType.Armor] = 10;
        // TEMP

        UIController.Instance.UpdateProductionUI();
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

        Dictionary<WeaponType, List<DbWeapon>> WeaponsByType = AllWeapons.GroupBy(w => w.Type).ToDictionary(group => group.Key, group => group.ToList());

        foreach(var  weaponGroup in WeaponsByType)
        {
            Transform currentContent = null;
            switch (weaponGroup.Key)
            {
                case WeaponType.Infantry: currentContent = InfantryContent; break;
                case WeaponType.Artillery: currentContent = ArtilleryContent; break;
                case WeaponType.Armor: currentContent = ArmorContent; break;
                case WeaponType.Air: currentContent = AirContent; break;
                case WeaponType.Navy: currentContent = NavyContent; break;
            }

            AdjustContentSize(currentContent as RectTransform, weaponGroup.Value.Count());

            foreach (DbWeapon weapon in weaponGroup.Value)
            {
                GameObject newProductionItem = null;
                newProductionItem = GameObject.Instantiate(ProductionItemPrefab, currentContent);
                if (newProductionItem == null) continue;

                // update item position
                Vector3 newPos = StartPosition + new Vector3(0f, PosYChange * weaponCounts[(int)weapon.Type], 0f);
                newProductionItem.transform.position = newProductionItem.transform.TransformPoint(newPos);
                weaponCounts[(int)weapon.Type]++;

                newProductionItem.name = weapon.Name;
                ProductionItem prItemScript = newProductionItem.AddComponent<ProductionItem>();
                prItemScript.Init(weapon);

                ProdItems.Add(prItemScript);
            }
        }
    }

    public void RemoveWeaponContent()
    {
        List<Transform> contents = new List<Transform>() { InfantryContent, ArtilleryContent, ArmorContent, AirContent, NavyContent };
        foreach (Transform content in contents)
        {
            int itemCounter = 0;
            foreach (Transform item in CustomUtils.GetAllChildren(content))
            {
                itemCounter++;
                GameObject.Destroy(item.gameObject);
            }

            RectTransform rectContent = ((RectTransform)content);
            rectContent.sizeDelta = new Vector3(rectContent.sizeDelta.x, rectContent.sizeDelta.y + itemCounter * PosYChange); // vrati zvetseni contentu o pocet itemu 
        }
    }

    private void AdjustContentSize(RectTransform content, int itemCount)
    {
        content.position += new Vector3(0, -10000);
        
        if (itemCount < 5) // clamps minimal size for content
        {
            content.sizeDelta = new Vector3(content.sizeDelta.x, (int)(PosYChange * 4.5f * -1));
            return;
        }

        content.sizeDelta = new Vector3(content.sizeDelta.x, PosYChange * itemCount * -1 + 15);

        //currentRectContent.sizeDelta = new Vector3(currentRectContent.sizeDelta.x, currentRectContent.sizeDelta.y + -1 * PosYChange); // poschange is negated prItem size + space between
        //currentContent.position += new Vector3(0, -5000);
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

    #region Factories
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

    public int GetNewFactoryPrice(WeaponType type)
    {
        int newPrice = (int)(10000 * Mathf.Pow(1.25f, FactoryLevel[(int)type] + 1));
        return newPrice;
    }

    public int GetNewFactoryAmount(WeaponType type)
    {
        int newAmount = 1;
        return newAmount;
    }

    public int GetCurrentFactoryPrice(WeaponType type)
    {
        int newPrice = (int)(10000 * Mathf.Pow(1.25f, FactoryLevel[(int)type]));
        return newPrice;
    }
    #endregion

    #region Warehouse

    public int GetNewWarehousePrice(WeaponType type)
    {
        int newPrice = (int)(10000 * Mathf.Pow(1.25f, WarehouseLevel[(int)type] + 1));
        return newPrice;
    }

    public int GetNewWarehouseAmount(WeaponType type)
    {
        int newAmount = 1000;
        return newAmount;
    }

    public int GetCurrentWarehousePrice(WeaponType type)
    {
        int newPrice = (int)(10000 * Mathf.Pow(1.25f, WarehouseLevel[(int)type]));
        return newPrice;
    }

    #endregion
}
