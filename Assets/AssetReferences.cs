using System.Collections;
using System.Linq;
using UnityEngine;

public class AssetReferences : MonoBehaviour
{
    #region Singleton
    public static AssetReferences Instance;
    public void Awake()
    {
        if (Instance != null)
            Logger.Log(LogLevel.ERROR, "Instance of AssetReferences already exists", "");

        Instance = this;
    }
    #endregion

    [SerializeField]
    public GameObject ResearchBackgroundPrefab;
}
