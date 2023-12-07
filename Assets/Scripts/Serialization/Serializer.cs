using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class Serializer
{
    public static string SavedDataPath = Application.persistentDataPath + "/save.dat";

    public static void SaveData()
    { 
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(SavedDataPath, FileMode.Create);

        SavedData data = new SavedData();

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SavedData LoadData()
    { 
        if(File.Exists(SavedDataPath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(SavedDataPath, FileMode.Open);

            SavedData data = formatter.Deserialize(stream) as SavedData;
            stream.Close();

            return data;
        }
        Debug.LogWarning("SavedData not found at " + SavedDataPath);
        return new SavedData();
    }
}