using UnityEngine;
using System.IO;

[System.Serializable]
public class SaveData
{
    public int coins;
    public int currentDay;
    public int currentEvent;
    public CardDealer dealer;
}

public static class SaveManager
{
    static string path = Application.persistentDataPath + "/save.json";

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log("Guardado en: " + path);
    }

    public static SaveData Load()
    {
        if (!File.Exists(path))
        {
            Debug.Log("No hay save, creando nuevo");
            return new SaveData();
        }

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static void DeleteSave()
    {
        if (File.Exists(path))
            File.Delete(path);
    }
}