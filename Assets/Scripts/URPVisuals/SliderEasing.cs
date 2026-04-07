using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SliderEasing : MonoBehaviour
{
    public enum EaseType { OutBack, OutElastic }

    [SerializeField] Slider slider;
    [SerializeField] float duration = 0.5f;
    [SerializeField] EaseType easeType = EaseType.OutElastic;
    [SerializeField] Image fill;

    Coroutine _current;

    void Update()
    {
        fill.material.SetFloat("_Value", slider.value);
    }

    public void AnimateTo(float targetValue)
    {
        if (_current != null) StopCoroutine(_current);
        _current = StartCoroutine(EaseSlider(targetValue));
    }

    IEnumerator EaseSlider(float target)
    {
        float start = slider.value;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = Evaluate(t);
            slider.value = Mathf.Lerp(start, target, eased);
            yield return null;
        }

        slider.value = target;
    }

    float Evaluate(float t)
    {
        switch (easeType)
        {
            case EaseType.OutBack:
                float c1 = 1.70158f;
                float c3 = c1 + 1f;
                return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);

            case EaseType.OutElastic:
                float c4 = (2f * Mathf.PI) / 2f;
                if (t == 0f) return 0f;
                if (t == 1f) return 1f;
                return Mathf.Pow(2f, -15f * t) * Mathf.Sin((t * 10f - 0.75f) * c4) + 1f;

            default:
                return t;
        }
    }

}