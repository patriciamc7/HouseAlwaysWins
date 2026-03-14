using System.Collections;
using UnityEngine;


public class Reel : MonoBehaviour
{
    [SerializeField] float spinTime = 3f;
    [SerializeField] float reelExtraTurns = 4f;
    public IEnumerator SpinReel(float targetAngle)
    {
        float reelStartAngle = this.transform.localEulerAngles.z;

        float finalWheelAngle = -360f * reelExtraTurns + targetAngle;

        float t = 0f;

        while (t < spinTime)
        {
            t += Time.deltaTime;

            float k = Mathf.Clamp01(t / spinTime);
            float eased = EaseOutCubic(k);


            float wheelAngle =
                Mathf.Lerp(reelStartAngle, finalWheelAngle, eased);

            this.transform.localRotation = Quaternion.Euler(0, 0, wheelAngle);

            yield return null;
        }

        this.transform.localRotation = Quaternion.Euler(0, 0, finalWheelAngle);
    }
    float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }
}
