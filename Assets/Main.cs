using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Main : MonoBehaviour
{
    #region Unity properties
    [SerializeField]
    private GameObject ProductionItemPrefab;
    [SerializeField]
    private Transform InfantryContent;
    [SerializeField]
    private Transform ArtilleryContent;
    [SerializeField]
    private Transform ArmorContent;
    [SerializeField]
    private Transform AirContent;
    [SerializeField]
    private Transform NavyContent;
    #endregion

    #region Fields and properties
    private string connectionString;
    public Database Database { get; set; }
    #endregion

    #region Unity
    void Start()
    {
        InitDB();
        InitData();
    } 

    #endregion

    #region Methods
    public void InitDB()
    {
        connectionString = "URI=file:" + Application.dataPath + "/WarClicker_DB.s3db";
        Database = new Database(connectionString);
    }

    public void InitData()
    {
        // Admin
        Translator.Init(Database.LoadTranslations(eLanguage.EN), Database.LoadTranslations(eLanguage.CZ)); // TODO - Change localLanguage argument based on PlayerPrefs

        // Player
        PlayerController.Instance.Init();

        // Entities
        ProductionController.Instance.InitGameObjects(ProductionItemPrefab, InfantryContent, ArtilleryContent, ArmorContent, AirContent, NavyContent);
        ProductionController.Instance.Init(Database.LoadWeapons().ToList());
    }

    public void SaveData()
    {
        Database.SaveWeapons(ProductionController.Instance.AllWeapons);
    }
    #endregion

    #region EventHandlers

    public void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Debug.Log("App paused - saving data");
            SaveData();
        }
    }

    #endregion
}
