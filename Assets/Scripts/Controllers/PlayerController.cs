public class PlayerController
{
    #region Singleton
    private static PlayerController m_Instance;
    public static PlayerController Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = new PlayerController();
            return m_Instance;
        }
    }

    //private PlayerController() { }
    #endregion

    #region Fields and properties

    public UIController UIController { get; set; }
    public long Money { get; private set; } = 500;
    public long WarFunds { get; private set; } = 0;
    #endregion

    #region Methods

    public void Init(SavedData data)
    {
        Money = data.money;
        WarFunds = data.warFunds;

        Money = 500000;
        WarFunds = 1000000;

        // try to get player with his GUID from server (to check his real stats) -- AFTER LEADERBOARDS UPDATE
    }

    public void AddMoney(long amount)
    {
        Money += amount;
    }

    public void AddWarFunds(long amount)
    {
        WarFunds += amount;
    }

    #endregion
}

