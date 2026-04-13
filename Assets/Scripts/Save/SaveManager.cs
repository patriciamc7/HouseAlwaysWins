using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int coins;
    public int currentDay;
    public int currentEvent;
    public float stressLevel;
    public DeckBias bias;
    public List<SpanishCard> handCards;
    public List<SpanishCard> bankHandCards;
    public List<SpanishCard> deck;
    public WildType wildType;

    public List<shopObject> shopItems;
    public List<shopObject> currentShop;
    public List<shopObject> itemsBought;
}

public static class SaveManager
{
    static string path = Application.persistentDataPath + "/save.json";

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    public static SaveData Load()
    {
        if (!File.Exists(path))
        {
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