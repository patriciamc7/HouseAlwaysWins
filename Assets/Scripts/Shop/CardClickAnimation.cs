using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

public class CardClickAnimation : MonoBehaviour
{
    #region Flip
    private float flipDuration = 0.8f;
    private int numTurns = 3;
    #endregion

    #region Move
    private float moveDuration = 0.2f;
    private float distance = 50f;
    #endregion

    #region Scale
    private float impactScale = 1.2f;      
    private float shrinkDuration = 0.1f; 
    private float impactDuration = 0.25f;
    private float finalScale = 1.0f; 
    #endregion

    private RectTransform _rect;
    private Vector2 _originalPosition;
    private Vector3 _originalScale;
    private bool _isAnimating = false;

    void Start()
    {
        _rect = GetComponent<RectTransform>();
        _originalPosition = _rect.anchoredPosition;
        _originalScale = _rect.localScale;
    }

    public void OnCardClick()
    {
        if (_isAnimating || !this.gameObject.GetComponent<Button>().interactable) return;
        StartCoroutine(PlayCardAnimation());
    }


    private IEnumerator PlayCardAnimation()
    {
        _isAnimating = true;

        yield return StartCoroutine(SmallScale());

        //Coroutine move = StartCoroutine(Move());
        //Coroutine flip = StartCoroutine(FlipCard());

        //yield return move;
        //yield return flip;
        //yield return StartCoroutine(MoveBack());

        //Coroutine scale = StartCoroutine(ImpactScale());

        //yield return scale;

        _isAnimating = false;
        this.gameObject.SetActive(false);
    }


    private IEnumerator FlipCard()
    {
        float t = 0f;
        float totalDegrees = 360f * numTurns;

        while (t < 1f)
        {
            t += Time.deltaTime / flipDuration;
            float currentAngle = Mathf.Lerp(0f, totalDegrees, EaseInOutCubic(t));
            _rect.localRotation = Quaternion.Euler(0f, currentAngle, 0f);
            yield return null;
        }

        _rect.localRotation = Quaternion.identity;
    }

    private IEnumerator Move()
    {
        float t = 0f;
        Vector2 startPos = _rect.anchoredPosition;
        Vector2 finalPos = startPos + new Vector2(0, distance);

        while (t < 1f)
        {
            t += Time.deltaTime / moveDuration;
            _rect.anchoredPosition = Vector2.Lerp(startPos, finalPos, EaseInOut(t));
            yield return null;
        }
    }

   
    private IEnumerator ImpactScale()
    {
        // Crecer
        float t = 0f;
        Vector3 startScale = _originalScale;
        Vector3 bigScale = _originalScale * impactScale;

        while (t < 1f)
        {
            t += Time.deltaTime / impactDuration;
            _rect.localScale = Vector3.Lerp(startScale, bigScale, EaseOut(t));
            yield return null;
        }

        t = 0f;
        Vector3 endScale = _originalScale * finalScale;

        while (t < 1f)
        {
            t += Time.deltaTime / shrinkDuration;
            _rect.localScale = Vector3.Lerp(bigScale, endScale, EaseIn(t));
            yield return null;
        }

    }

    private IEnumerator SmallScale()
    {
        float t = 0f;
        Vector3 startScale = _originalScale;
        Vector3 bigScale = _originalScale * 0.98f;

        while (t < 1f)
        {
            t += Time.deltaTime / 0.1f;
            _rect.localScale = Vector3.Lerp(startScale, bigScale, EaseOut(t));
            yield return null;
        }

        t = 0f;
        Vector3 endScale = _originalScale * finalScale;

        while (t < 1f)
        {
            t += Time.deltaTime / 0.1f;
            _rect.localScale = Vector3.Lerp(bigScale, endScale, EaseIn(t));
            yield return null;
        }
    }

    private IEnumerator MoveBack()
    {
        float t = 0f;
        Vector2 startPos = _rect.anchoredPosition;

        while (t < 1f)
        {
            t += Time.deltaTime / moveDuration;
            _rect.anchoredPosition = Vector2.Lerp(startPos, _originalPosition, EaseInOut(t));
            yield return null;
        }
    }

    
    private float EaseInOut(float t) => t < 0.5f ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;
    private float EaseOut(float t) => 1 - Mathf.Pow(1 - t, 3);
    private float EaseIn(float t) => t * t * t;
    private float EaseInOutCubic(float t) => t < 0.5f? 4 * t * t * t: 1 - Mathf.Pow(-2 * t + 2, 3) / 2;
}