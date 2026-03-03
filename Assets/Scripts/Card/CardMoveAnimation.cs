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

        while (t < 1f)
        {
            t += Time.deltaTime / moveTime;

            transform.position = Vector3.Lerp(startPos, targetPosition, t);
            transform.rotation = Quaternion.Slerp(startRot, targetRotation, t);

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

        while (t < 1f)
        {
            t += Time.deltaTime / time;
            float h = Mathf.Sin(t * Mathf.PI);

            transform.position = Vector3.Lerp(startPos, upPos, h);
            transform.rotation = Quaternion.Slerp(startRot, targetRotation, t);

            yield return null;
        }

        transform.position = startPos;
        transform.rotation = targetRotation;

        onComplete?.Invoke();
    }
}
