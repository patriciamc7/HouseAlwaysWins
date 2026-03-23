using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] Text resultText;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip winSound;
    [SerializeField] AudioClip loseSound;

    public void ShowWin(Action onComplete)
    {
        resultText.text = "GANASTE";
        resultText.color = Color.green;
        panel.SetActive(true);
        StartCoroutine(WinAnimation(onComplete));
        audioSource.PlayOneShot(winSound);
    }

    public void ShowLose(System.Action onComplete)
    {
        resultText.text = "PERDISTE";
        resultText.color = Color.red;
        panel.SetActive(true);
        StartCoroutine(LoseAnimation(onComplete));
        audioSource.PlayOneShot(loseSound);
    }

    IEnumerator WinAnimation(Action onComplete)
    {
        canvasGroup.alpha = 0;
        transform.localScale = Vector3.zero;

        float t = 0f;
        float duration = 0.4f;

        // fade + scale up
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = t / duration;

            canvasGroup.alpha = p;
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 1.2f, p);

            yield return null;
        }

        // pequeño rebote
        t = 0f;
        Vector3 start = transform.localScale;
        Vector3 end = Vector3.one;

        while (t < 0.2f)
        {
            t += Time.deltaTime;
            float p = t / 0.2f;

            transform.localScale = Vector3.Lerp(start, end, p);
            yield return null;
        }

        onComplete?.Invoke();
    }

    IEnumerator LoseAnimation(Action onComplete)
    {
        canvasGroup.alpha = 1;
        transform.localScale = Vector3.one;

        float duration = 0.5f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;

            // shake
            float shake = 10f;
            transform.localPosition = new Vector3(
                UnityEngine.Random.Range(-shake, shake),
                UnityEngine.Random.Range(-shake, shake),
                0);

            yield return null;
        }

        transform.localPosition = Vector3.zero;

        // fade out
        t = 0f;
        while (t < 0.3f)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = 1 - (t / 0.3f);
            yield return null;
        }

        onComplete?.Invoke();
    }
}