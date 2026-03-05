using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotMachine : MonoBehaviour
{
    public ReelSpin3x3 reel1;
    public ReelSpin3x3 reel2;
    public ReelSpin3x3 reel3;

    public List<GameObject> symbolPrefabs;

    public void Spin()
    {
        // generar resultados aleatorios (3 símbolos visibles por reel)
        List<int> result1 = GenerateResult();
        List<int> result2 = GenerateResult();
        List<int> result3 = GenerateResult();

        // iniciar animación
        reel1.StartSpin();
        reel2.StartSpin();
        reel3.StartSpin();

        // parar cada reel con delay para efecto natural
        StartCoroutine(StopReels(result1, result2, result3));
    }

    IEnumerator StopReels(List<int> r1, List<int> r2, List<int> r3)
    {
        yield return new WaitForSeconds(1f);
        reel1.StopSpin(r1);
        yield return new WaitForSeconds(0.5f);
        reel2.StopSpin(r2);
        yield return new WaitForSeconds(0.5f);
        reel3.StopSpin(r3);

        yield return new WaitForSeconds(0.5f);
        CheckResult();
    }

    List<int> GenerateResult()
    {
        List<int> res = new List<int>();
        int visibleCount = 3;
        for (int i = 0; i < visibleCount; i++)
        {
            int r = Random.Range(0, symbolPrefabs.Count);
            res.Add(r);
        }
        return res;
    }

    void CheckResult()
    {
        List<string> c1 = reel1.GetCenterSymbols();
        List<string> c2 = reel2.GetCenterSymbols();
        List<string> c3 = reel3.GetCenterSymbols();

        Debug.Log("Fila central:");
        for (int i = 0; i < c1.Count; i++)
        {
            Debug.Log(c1[i] + " | " + c2[i] + " | " + c3[i]);
        }

        // ejemplo simple: fila central solo
        if (c1[1] == c2[1] && c2[1] == c3[1])
            Debug.Log("JACKPOT!");
        else
            Debug.Log("No hay jackpot");
    }
}