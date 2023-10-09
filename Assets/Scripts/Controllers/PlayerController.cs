using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
    public long Money { get; set; }
    public long WarFunds { get; set; }

    //private double MoneyPerSecond { get; set; }
    //private double WarFundsPerSecond { get; set; }
    #endregion

    #region Methods

    public void Init()
    {
        // TODO load Money-Warfund from PlayerPrefs or SerializedFile
        Money = 100;
        WarFunds = 500;

        // try to get player with his GUID from server (to check his real stats) -- AFTER LEADERBOARDS UPDATE
    }
    public void SaveContent()
    {
        // TODO save Money, Warfund to PLayerPrefs or SerializedFile
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

