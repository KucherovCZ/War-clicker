using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class Serializer
{
    public static string SavedDataPath = Application.persistentDataPath + "/save.dat";

    public static void SaveData(SavedData data = null)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(SavedDataPath, FileMode.Create);

        if(data == null)
            data = new SavedData();

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SavedData LoadData()
    {
        if (File.Exists(SavedDataPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(SavedDataPath, FileMode.Open);

            SavedData data = formatter.Deserialize(stream) as SavedData;
            stream.Close();

            return data;
        }
        else if (!PlayerPrefs.HasKey("FirstLaunchDone"))
        {
            PlayerPrefs.SetInt("FirstLaunchDone", 1);
            SavedData NewData = new SavedData(true);
            SaveData(NewData);
            return NewData;
        }
        else
        {
            Debug.LogWarning("SavedData not found at " + SavedDataPath);
            return new SavedData();
        }
    }
}