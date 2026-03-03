using System.Collections;
using UnityEngine;

public class CardMoveAnimation : MonoBehaviour
{
    public void MoveToPosition(Vector3 targetPosition, Quaternion targetRotation, Transform newParent, System.Action onComplete, float moveTime = 0.25f)
    {
        StartCoroutine(MoveRoutine(targetPosition, targetRotation, newParent, onComplete, moveTime));
    }

    IEnumerator MoveRoutine(Vector3 targetPosition, Quaternion targetRotation, Transform newParent, System.Action onComplete, float moveTime)
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        float t = 0f;

        while (t < moveTime)
        {
            t += Time.deltaTime;


            float k = Mathf.Clamp01(t / moveTime);
            float eased = EaseOutCubic(k);

            transform.position = Vector3.Lerp(startPos, targetPosition, eased);
            transform.rotation = Quaternion.Slerp(startRot, targetRotation, eased);

            yield return null;
        }

        transform.position = targetPosition;
        transform.rotation = targetRotation;

        transform.SetParent(newParent, true);

        onComplete?.Invoke();
    }

    public void FlipTo(Quaternion targetRotation, System.Action onComplete, float time = 0.25f, float lift = 1f)
    {
        StartCoroutine(FlipRoutine(targetRotation, time, lift, onComplete));
    }

    IEnumerator FlipRoutine(Quaternion targetRotation, float time, float lift, System.Action onComplete)
    {
        Quaternion startRot = transform.rotation;
        Vector3 startPos = transform.position;

        Vector3 liftDir = transform.parent != null
        ? transform.parent.up
        : transform.up;

        Vector3 upPos = startPos + liftDir * lift;
        float t = 0f;

        while (t < time)
        {
            t += Time.deltaTime;

            float k = Mathf.Clamp01(t / time);
            float eased = EaseOutCubic(k);

            float h = Mathf.Sin(k * Mathf.PI);

            transform.position = Vector3.Lerp(startPos, upPos, h);
            transform.rotation = Quaternion.Slerp(startRot, targetRotation, eased);

            yield return null;
        }

        transform.position = startPos;
        transform.rotation = targetRotation;

        onComplete?.Invoke();
    }

    public void MoveRelayout(Vector3 targetPos, float time)
    {
        StopAllCoroutines();
        StartCoroutine(RelayoutRoutine(targetPos, time));
    }

    IEnumerator RelayoutRoutine(Vector3 targetPos, float time)
    {
        Vector3 startPos = transform.position;

        float t = 0f;

        while (t < time)
        {
            t += Time.deltaTime;

            float k = Mathf.Clamp01(t / time);

            float eased = k * k * (3f - 2f * k); // smoothstep manual

            transform.position = Vector3.Lerp(startPos, targetPos, eased);

            yield return null;
        }

        transform.position = targetPos;
    }

    float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }
}
