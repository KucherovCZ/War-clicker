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
    #endregion

    public UIController UIController { get; set; }

    #region Player
    public long Money { get; private set; } = 500;
    public long WarFunds { get; private set; } = 0;

    public void Init(SavedData data)
    {
        Money = data.money;
        WarFunds = data.warFunds;
        AutosaveInterval = data.autosaveInterval;

        if (Application.isEditor)
        {
            //Money = 500000;
            WarFunds = 1000000;
        }

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

    public bool TryBuyMoney(long price)
    {
        if (price < Money)
        {
            AddMoney(-1 * price);
            return true;
        }
        else
        {
            UIController.Instance.PopupDialog("Not enough money", Color.red);
            return false;
        }
    }

    public bool TryBuyWarFunds(long price)
    {
        if (price < WarFunds)
        {
            AddWarFunds(-1 * price);
            return true;
        }
        else
        {
            UIController.Instance.PopupDialog("Not enough warfunds", Color.red);
            return false;
        }
    }

    #endregion

    #region Settings

    public int AutosaveInterval { get; set; } = 300;

    #endregion
}

