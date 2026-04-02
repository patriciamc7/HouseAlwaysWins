using System;
using System.Collections;
using UnityEngine;

public enum CoinSide
{
    Head,
    Tails
}

public class CoinWildController : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] GameObject coinPrefab;
    [SerializeField] Transform spawnPoint;

    GameObject currentCoin;

    public void UseCoinWild(CoinSide playerChoice, Action<bool> onFinished)
    {
        currentCoin = Instantiate(coinPrefab, spawnPoint.position, coinPrefab.transform.rotation);

        StartCoroutine(FlipRoutine(playerChoice, onFinished));
    }

    IEnumerator FlipRoutine(CoinSide playerChoice, Action<bool> onFinished)
    {
        CoinSide result = UnityEngine.Random.value < 0.5f ? CoinSide.Head : CoinSide.Tails;
        bool win = result == playerChoice;

        float t = 0f;

        // vueltas aleatorias (pero controladas)
        int spins = UnityEngine.Random.Range(3, 6);   // 3,4 o 5 vueltas
        float flipTime = UnityEngine.Random.Range(1.5f, 2f); // no muy largo

        float totalRot = 360f * spins;

        Quaternion startRot = currentCoin.transform.rotation;

        while (t < flipTime)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / flipTime);

            float eased = 1f - Mathf.Pow(1f - k, 3f);//cubic easing

            float rotX = Mathf.Lerp(0, totalRot, eased);

            currentCoin.transform.rotation =
                startRot * Quaternion.Euler(rotX, 0, 0);

            yield return null;
        }

        if (result == CoinSide.Head)
            currentCoin.transform.rotation = startRot;// cara
        else
            currentCoin.transform.rotation = Quaternion.Euler(90, 0, 0); // cruz

        yield return new WaitForSeconds(0.4f);

        onFinished?.Invoke(win);
    }

    public void DestroyCoin()
    {
        if (currentCoin != null)
            Destroy(currentCoin);
    }
}