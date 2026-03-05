using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class ReelSpin3x3 : MonoBehaviour
{
    [Header("Configuración")]
    public float spacing = 2f;             // distancia entre símbolos
    public float spinSpeed = 10f;          // velocidad de giro
       public int visibleCount = 3; 
    private List<Transform> symbols = new List<Transform>();
    [HideInInspector]
    public List<int> logicalIndices = new List<int>();

    private bool spinning = false;
    private bool stopping = false;
    private float velocity = 0f;
    public float stopSmoothTime = 0.3f;    // tiempo de rebote
    private float targetOffset = 0f;

    // Crear los símbolos en el reel
    public void Setup(List<GameObject> reelList)
    {
        // limpiar
        foreach (Transform t in transform)
            Destroy(t.gameObject);

        symbols.Clear();

        for (int i = 0; i < reelList.Count; i++)
        {
            GameObject s = Instantiate(reelList[i], transform);
            s.transform.localPosition = new Vector3(0, 0, -i * spacing);
            symbols.Add(s.transform);
        }
    }

    void Update()
    {
        if (!spinning) return;

        if (!stopping)
            ScrollSymbols();
        else
            SmoothStop();
    }

    void ScrollSymbols()
    {
        // mover todos los símbolos hacia atrás (simula scroll)
        for (int i = 0; i < symbols.Count; i++)
        {
            Transform s = symbols[i];
            s.localPosition += Vector3.back * spinSpeed * Time.deltaTime;

            // si pasa el final, reciclar al principio
            if (s.localPosition.z < -symbols.Count * spacing)
            {
                s.localPosition += Vector3.forward * symbols.Count * spacing;
            }
        }
    }

    void SmoothStop()
    {
        bool allAligned = true;

        for (int i = 0; i < symbols.Count; i++)
        {
            Transform s = symbols[i];
            float targetZ = s.localPosition.z + targetOffset;
            float newZ = Mathf.SmoothDamp(s.localPosition.z, targetZ, ref velocity, stopSmoothTime);
            s.localPosition = new Vector3(s.localPosition.x, s.localPosition.y, newZ);

            if (Mathf.Abs(newZ - targetZ) > 0.01f)
                allAligned = false;
        }

        if (allAligned)
        {
            spinning = false;
            stopping = false;
        }
    }
    public void StartSpin()
    {
        spinning = true;
        stopping = false;
    } 

    public void StopSpin(List<int> result)
    {
        stopping = true;
        
        
        // calcular offset Z para que el símbolo del medio quede centrado
        int middleIndex = logicalIndices[1]; // índice del símbolo central de la fila
        float currentZ = symbols[middleIndex].localPosition.z;
        float desiredZ = -spacing * 1; // posición central del reel
        float extraTurns = spacing * symbols.Count * 2; // 2 vueltas extras para efecto natural
        targetOffset = currentZ - desiredZ + extraTurns;
    }

    public List<string> GetCenterSymbols()
    {
        List<string> center = new List<string>();
        int centerStart = 0; // el índice de los símbolos visibles de la fila central
        for (int i = centerStart; i < centerStart + visibleCount; i++)
            center.Add(symbols[i % symbols.Count].name);
        return center;

    }
}