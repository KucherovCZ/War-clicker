[System.Serializable]
public class SavedData
{
    public SavedData()
    {
        money = PlayerController.Instance.Money;
        warFunds = PlayerController.Instance.WarFunds;

        factories = ProductionController.Instance.Factories;
    }

    public long money;
    public long warFunds;
    public int[] factories;
}