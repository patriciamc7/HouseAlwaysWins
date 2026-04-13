using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PhrasesManagerNPC : MonoBehaviour
{
    public Text textoUI;
    public Image background;

    public float tiempoEntreFrases = 30f;  
    public float tiempoDeFrases = 10f; 
    public float fadeDuration = 1f;    

    private string[] frases;
    private int frasesRestantes;
    private System.Random rng = new System.Random();

    void Start()
    {
        string savedLang = PlayerPrefs.GetString("Language", "en");
        TextAsset archivo = Resources.Load<TextAsset>("phrases/Npc_"+savedLang);

        if (archivo == null)
        {
            return;
        }

        frases = archivo.text.Split('\n');
        frasesRestantes = frases.Length;

        StartCoroutine(MostrarFrases());
    }

    IEnumerator MostrarFrases()
    {
        while (frasesRestantes > 0)
        {
            // --- 1. Obtener frase aleatoria sin repetir ---
            int index = rng.Next(frasesRestantes);
            string fraseElegida = frases[index].Trim();

            if (frasesRestantes == frases.Length)
                yield return new WaitForSeconds(tiempoEntreFrases);

            // Mover la frase usada al final y reducir el total
            frases[index] = frases[frasesRestantes - 1];
            frasesRestantes--;
            textoUI.text = fraseElegida;

            // --- 2. Fade IN ---
            yield return StartCoroutine(FadeBackground(0f, 0.5f, 0.5f));
            yield return StartCoroutine(FadeTexto(0f, 1f));

            // --- 3. Mantener visible ---
            yield return new WaitForSeconds(tiempoDeFrases);

            // --- 4. Fade OUT ---
            yield return StartCoroutine(FadeTexto(1f, 0f));
            yield return StartCoroutine(FadeBackground(0.5f, 0f, fadeDuration + 0.5f));

            // --- 3. Mantener visible ---
            yield return new WaitForSeconds(tiempoEntreFrases);

        }
    }

    IEnumerator FadeBackground(float inicio, float fin, float duration)
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(inicio, fin, t / duration);

            Color c = background.color;
            c.a = alpha;
            background.color = c;

            yield return null;
        }
    }


    IEnumerator FadeTexto(float inicio, float fin)
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(inicio, fin, t / fadeDuration);

            Color c = textoUI.color;
            c.a = alpha;
            textoUI.color = c;

            yield return null;
        }
    }

    public void Show()
    {
        textoUI.enabled = true;
        background.enabled = true;
    }

    public void Hide()
    {
        textoUI.enabled = false;
        background.enabled = false;
    }
}