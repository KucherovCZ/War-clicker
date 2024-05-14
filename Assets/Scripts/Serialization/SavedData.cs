[System.Serializable]
public class SavedData
{
    public SavedData()
    {
        money = PlayerController.Instance.Money;
        warFunds = PlayerController.Instance.WarFunds;

        factories = ProductionController.Instance.Factories;
        factoryLevels = ProductionController.Instance.FactoryLevel;
        warehouses = ProductionController.Instance.WarehouseCapacity;
        warehouseLevels = ProductionController.Instance.WarehouseLevel;
    }

    public long money;
    public long warFunds;

    public int[] factories;
    public int[] factoryLevels;
    public int[] warehouses;
    public int[] warehouseLevels;
}