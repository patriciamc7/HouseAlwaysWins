using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class IrisTransition : MonoBehaviour
{
    private Material material;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float pauseInBlack = 0.5f;

    void Start()
    {
        material = GetComponent<Image>().material;
    }

    public void GoToScene(System.Action onComplete)
    {
        this.gameObject.SetActive(true);
        material = GetComponent<Image>().material;
        StartCoroutine(Transition(onComplete));
    }

    private IEnumerator Transition(System.Action onComplete)
    {
        yield return StartCoroutine(AnimateIris(1f, 0f));
        yield return new WaitForSeconds(pauseInBlack);
        //SceneManager.LoadScene(sceneName);
        yield return StartCoroutine(AnimateIris(0f, 1f, onComplete));
    }

    public void CloseIris()
    {
        StartCoroutine(AnimateIris(1f, 0f));
    }

    public void OpenIris()
    {
        StartCoroutine(AnimateIris(0f, 1f));
    }

    private IEnumerator AnimateIris(float from, float to, System.Action onComplete = null)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float radius = Mathf.Lerp(from, to, elapsed / duration);
            material.SetFloat("_Radius", radius);
            yield return null;
        }

        onComplete?.Invoke();
    }
}