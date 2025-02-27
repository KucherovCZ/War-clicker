using System.Linq;

[System.Serializable]
public class SavedData
{
    public SavedData()
    {
        money = PlayerController.Instance.Money;
        warFunds = PlayerController.Instance.WarFunds;
        autosaveInterval = PlayerController.Instance.AutosaveInterval;

        factories = ProductionController.Instance.Factories;
        factoryLevels = ProductionController.Instance.FactoryLevel;
        warehouses = ProductionController.Instance.WarehouseCapacity;
        warehouseLevels = ProductionController.Instance.WarehouseLevel;

        eraFilter = ResearchController.Instance.Filter.EraState.Values.ToArray();
    }

    public SavedData(bool defaultValues)
    {
        money = 500;
        warFunds = 0;
        autosaveInterval = 300;

        factories = new[] { 1, 0, 0, 0, 0};
        factoryLevels = new[] { 1, 0, 0, 0, 0 };
        warehouses = new[] { 50, 0, 0, 0, 0 };
        warehouseLevels = new[] { 1, 0, 0, 0, 0 };

        eraFilter = new[] { true, true, true, true, true, true, true };
    }

    public long money;
    public long warFunds;
    public int autosaveInterval;

    public int[] factories;
    public int[] factoryLevels;
    public int[] warehouses;
    public int[] warehouseLevels;

    public bool[] eraFilter;

}