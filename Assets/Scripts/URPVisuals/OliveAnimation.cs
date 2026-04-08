using System.Collections;
using UnityEngine;

public class OliveAnimation : MonoBehaviour
{
    [SerializeField] float duration = 0.5f;
    [SerializeField] float intensity = 0.05f;
    [SerializeField] float velocity = 40f;

    private Vector3 originalPos;
    private bool shake = false;

    private void OnMouseDown()
    {
        ActivarVibracion();
    }

    public void ActivarVibracion()
    {
        if (!shake)
            StartCoroutine(Vibrar());
    }

    private IEnumerator Vibrar()
    {
        shake = true;
        originalPos = transform.localPosition;

        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < duration)
        {
            tiempoTranscurrido += Time.deltaTime;

            float progreso = tiempoTranscurrido / duration;

            float desvanecimiento = 1f - progreso;

            float desplazamientoX = Mathf.Sin(tiempoTranscurrido * velocity) * intensity * desvanecimiento;
            float desplazamientoY = Mathf.Sin(tiempoTranscurrido * velocity * 1.3f) * intensity * 0.5f * desvanecimiento;

            transform.localPosition = originalPos + new Vector3(desplazamientoX, desplazamientoY, 0f);

            yield return null;
        }

        transform.localPosition = originalPos;
        shake = false;
    }
}