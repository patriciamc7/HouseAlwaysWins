using System;
using System.Collections;
using UnityEngine;

public class WheelWildAnimation : MonoBehaviour
{
    [Header("References")]
    public Transform wheel;
    public Transform ball;
    public Transform center;

    [Header("Ball orbit")]
    public float ballRadius = 0.22f;

    [Header("Spin")]
    public float spinTime = 3f;
    public float wheelExtraTurns = 4f;
    public float ballExtraTurns = 6f;

    float ballStartAngle;
    float wheelStartAngle;

    public void Play(float targetBallAngle, Action onFinished)
    {
        ball.gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(SpinRoutine(targetBallAngle, onFinished));
    }

    IEnumerator SpinRoutine(float targetBallAngle, Action onFinished)
    {
        ballStartAngle = GetBallAngle();
        wheelStartAngle = wheel.localEulerAngles.z;

        float finalBallAngle =
            ballStartAngle + 360f * ballExtraTurns + targetBallAngle;

        float finalWheelAngle =
            wheelStartAngle + 360f * wheelExtraTurns + targetBallAngle * 0.5f;

        float t = 0f;

        while (t < spinTime)
        {
            t += Time.deltaTime;

            float k = Mathf.Clamp01(t / spinTime);
            float eased = EaseOutCubic(k);

            float ballAngle =
                Mathf.Lerp(ballStartAngle, finalBallAngle, eased);

            float wheelAngle =
                Mathf.Lerp(wheelStartAngle, finalWheelAngle, eased);

            SetBallAngle(ballAngle);
            wheel.localRotation = Quaternion.Euler(0, 0, wheelAngle);

            yield return null;
        }

        SetBallAngle(finalBallAngle);
        wheel.localRotation = Quaternion.Euler(0, 0, finalWheelAngle);

        onFinished?.Invoke();
    }

    float GetBallAngle()
    {
        Vector2 dir = ball.localPosition;
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

    void SetBallAngle(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        Vector3 localPos = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * ballRadius;
        ball.localPosition = localPos; // Z = 0 siempre
    }

    float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }
}