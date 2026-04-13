using UnityEngine;
using UnityEngine.UI;

public class RandomPhraseManager : MonoBehaviour
{
    public Text phraseText;
    public string fileName;

    private string[] phrases;

    void OnEnable()
    {
        LoadPhrases();
        ShowRandomPhrase();
    }

    private void LoadPhrases()
    {
        string savedLang = PlayerPrefs.GetString("Language", "en");
        TextAsset textFile = Resources.Load<TextAsset>("phrases/"+fileName+savedLang);

        if (textFile == null)
            return;

        phrases = textFile.text.Split(
            new[] { '\n', '\r' },
            System.StringSplitOptions.RemoveEmptyEntries
        );
    }

    private void ShowRandomPhrase()
    {
        if (phrases == null || phrases.Length == 0)
            return;

        int randomIndex = Random.Range(0, phrases.Length);
        phraseText.text = phrases[randomIndex].Trim();
    }
}