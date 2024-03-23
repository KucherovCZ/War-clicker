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
        Serializer.SaveData();
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
