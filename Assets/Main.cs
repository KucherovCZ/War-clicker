using System.Collections;
using System.Linq;
using UnityEngine;

public class Main : MonoBehaviour
{
    #region Unity properties
    [SerializeField]
    private GameObject ProductionItemPrefab;
    [SerializeField]
    private GameObject ProductionPage;

    [SerializeField]
    private GameObject ResearchItemPrefab;
    [SerializeField]
    private GameObject ResearchPage;
    #endregion

    #region Fields and properties
    private string connectionString;
    public Database Database { get; set; }
    public AssetReferences Assets { get; set; }
    #endregion

    #region Unity
    void Start()
    {
        InitDB();
        InitData();

        StartCoroutine(Autosave());
    }

    #endregion

    #region Methods
    public void InitDB()
    {
        connectionString = "URI=file:" + Application.dataPath + "/WarClicker_DB.s3db";
        Database = new Database(connectionString);

        Logger.DB = Database;
    }

    public void InitData()
    {
        // load serialized player information
        SavedData data = Serializer.LoadData();

        // Admin
        Translator.Init(Database.LoadTranslations(eLanguage.EN), Database.LoadTranslations(eLanguage.CZ)); // TODO - Change localLanguage argument based on PlayerPrefs Settings

        // Player
        PlayerController.Instance.Init(data);

        // Entities
        ProductionController.Instance.InitGameObjects(ProductionItemPrefab, ProductionPage);
        ProductionController.Instance.Init(Database.LoadWeapons().ToList(), data);

        ResearchController.Instance.InitGameObjects(ResearchItemPrefab, ResearchPage);
        ResearchController.Instance.Init(
            Database.LoadResearchItems().ToList(),
            Database.LoadResearchItemRelations().ToList(),
            Database.LoadResearchItemWeapon().ToList(),
            data);
    }

    public void SaveData()
    {
        Database.SaveWeapons(ProductionController.Instance.AllWeapons);
        Database.SaveResearchItems(ResearchController.Instance.ResearchItemsDB);
        Serializer.SaveData();
    }

    public IEnumerator Autosave()
    {
        yield return new WaitForSeconds(PlayerController.Instance.AutosaveInterval);

        SaveData();
    }

    public void ResetData()
    {
        Database.ResetWeapons();
        Database.ResetReserach();
        Serializer.ResetData();
        InitData();


    }
    #endregion

    #region EventHandlers

    public void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Logger.Log(LogLevel.INFO, "App paused - saving data", "");
            SaveData();
        }
    }

    #endregion
}
