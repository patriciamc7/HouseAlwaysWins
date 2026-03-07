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
    public float ballRadius = 153f;

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

        float finalWheelAngle = wheelStartAngle + 360f * wheelExtraTurns;

        float finalBallAngle = mapBallAngle(targetBallAngle) + 360f * ballExtraTurns + finalWheelAngle;

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
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return angle;
    }

    void SetBallAngle(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        ball.localPosition = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * ballRadius;
    }

    float mapBallAngle(float angle)
    {
        if (angle == 0)
            return 90;
        else if (angle == 30)
            return 60;
        else if (angle == 60f)
            return 30;
        else if (angle == 90)
            return 0;
        else if (angle == 120)
            return 330;
        else if (angle == 150)
            return 300;
        else if (angle == 180)
            return 270;
        else if (angle == 210)
            return 240;
        else if (angle == 240)
            return 210;
        else if (angle == 270)
            return 180;
        else if (angle == 300)
            return 150;
        else if (angle == 330)
            return 120;

        return 0;

    }
    float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }
}