using UnityEngine;

public class LanguageSelector : MonoBehaviour
{
    public void SetSpanish() => LocalizationManager.Instance.LoadLanguage("es");
    public void SetEnglish() => LocalizationManager.Instance.LoadLanguage("en");
    public void SetCatalan() => LocalizationManager.Instance.LoadLanguage("cat");
    public void SetNihon() => LocalizationManager.Instance.LoadLanguage("ni");
    public void SetPortuges() => LocalizationManager.Instance.LoadLanguage("po");

    public void OnDropdownChanged(int index)
    {
        string[] langs = { "es", "en", "fr" };
        if (index < langs.Length)
            LocalizationManager.Instance.LoadLanguage(langs[index]);
    }
}