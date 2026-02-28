using System.Collections;
using UnityEngine;

public class CardMoveAnimation : MonoBehaviour
{
    public float moveTime = 0.25f;

    public void MoveTo (Vector3 targetPosition, Quaternion targetRotation, Transform newParent)
    {
        StartCoroutine(MoveRoutine(targetPosition, targetRotation, newParent));
    }

    IEnumerator MoveRoutine (Vector3 targetPosition, Quaternion targetRotation, Transform newParent)
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

        transform.SetParent(newParent);
    }

    public void FlipTo(Quaternion targetRotation, float time = 0.2f, float lift = 0.03f)
    {
        StartCoroutine(FlipRoutine(targetRotation, time, lift));
    }

    IEnumerator FlipRoutine(Quaternion targetRotation, float time, float lift)
    {
        Quaternion startRot = transform.rotation;
        Vector3 startPos = transform.position;

        Vector3 upPos = startPos + transform.up * lift;

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
    }
}
