using UnityEngine;
using UnityEngine.UI;

public class RandomPhraseManager : MonoBehaviour
{
    [Header("Referencia al Text de UI")]
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
        TextAsset textFile = Resources.Load<TextAsset>("phrases/"+fileName);

        if (textFile == null)
        {
            Debug.LogError("No se encontró 'frases.txt' en Assets/Resources/");
            return;
        }

        // Divide por líneas y elimina vacías
        phrases = textFile.text.Split(
            new[] { '\n', '\r' },
            System.StringSplitOptions.RemoveEmptyEntries
        );
    }

    private void ShowRandomPhrase()
    {
        if (phrases == null || phrases.Length == 0)
        {
            Debug.LogWarning("La lista de frases está vacía.");
            return;
        }

        int randomIndex = Random.Range(0, phrases.Length);
        phraseText.text = phrases[randomIndex].Trim();
    }
}