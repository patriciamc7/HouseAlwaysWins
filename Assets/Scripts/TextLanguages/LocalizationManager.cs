using UnityEngine;
using System.Collections.Generic;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }
    public static event System.Action OnLanguageChanged;

    private Dictionary<string, string> _translations = new();
    public string CurrentLanguage { get; private set; } = "en";

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        string savedLang = PlayerPrefs.GetString("Language", "en");
        LoadLanguage(savedLang);
    }

    public void LoadLanguage(string lang)
    {
        TextAsset file = Resources.Load<TextAsset>($"Languages/{lang}");

        if (file == null)
        {
            return;
        }

        _translations.Clear();
        CurrentLanguage = lang;

        string json = file.text.Trim().TrimStart('{').TrimEnd('}');
        foreach (string line in json.Split(','))
        {
            string[] parts = line.Split(':');
            if (parts.Length < 2) continue;

            string key = parts[0].Trim().Trim('"');
            string value = string.Join(":", parts[1..]).Trim().Trim('"');
            _translations[key] = value;
        }

        PlayerPrefs.SetString("Language", lang);
        PlayerPrefs.Save();

        OnLanguageChanged?.Invoke();
    }

    public string Get(string key)
    {
        if (_translations.TryGetValue(key, out string value))
            return value;

        return key;
    }
}