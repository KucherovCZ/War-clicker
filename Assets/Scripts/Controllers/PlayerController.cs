﻿public class PlayerController
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

    //private double MoneyPerSecond { get; set; }
    //private double WarFundsPerSecond { get; set; }
    #endregion

    #region Methods

    public void Init(SavedData data)
    {
        Money = data.money;
        WarFunds = data.warFunds;

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
    //public void AddMoneyPerSecond(long oldProduction, long newProduction, int time)
    //{
    //    double difference = ((double)newProduction) - ((double)oldProduction);
    //    AddMoneyPerSecond(difference / (double)time);
    //}

    //public void AddMoneyPerSecond(double diffPerSecond)
    //{
    //    MoneyPerSecond += diffPerSecond;
    //}

    //public void AddWarFundPerSecond(long oldProduction, long newProduction, int time)
    //{
    //    double difference = ((double)newProduction) - ((double)oldProduction);
    //    AddWarFundPerSecond(difference / (double)time);
    //}

    //public void AddWarFundPerSecond(double diffPerSecond)
    //{
    //    WarFundsPerSecond += diffPerSecond;
    //}

    #endregion
}

