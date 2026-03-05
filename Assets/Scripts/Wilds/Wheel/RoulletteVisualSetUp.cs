using UnityEngine;

[ExecuteInEditMode]
public class WheelVisualSetup : MonoBehaviour
{
    public int totalSlots = 12;          // número de casillas
    public float radius = 1f;            // distancia desde el centro para los quads
    public float slotWidth = 0.3f;       // ancho del sector
    public Material redMaterial;         // material rojo
    public Material blackMaterial;       // material negro

    public bool autoClearOldSlots = true; // borra slots anteriores al generar

    public void Awake()
    {
        GenerateSlots();
    }

    public void GenerateSlots()
    {
        if (autoClearOldSlots)
        {
            // eliminar hijos antiguos
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }

        float angleStep = 360f / totalSlots;

        for (int i = 0; i < totalSlots; i++)
        {
            GameObject slot = GameObject.CreatePrimitive(PrimitiveType.Quad);
            slot.name = "Slot_" + i;

            slot.transform.SetParent(transform);

            float angle = i * angleStep;
            float rad = angle * Mathf.Deg2Rad;

            // posición exacta en el círculo
            Vector3 pos = new Vector3(
                Mathf.Cos(rad) * radius,
                Mathf.Sin(rad) * radius,
                0f
            );

            slot.transform.localPosition = pos;

            // rotar para que mire hacia fuera
            slot.transform.localRotation = Quaternion.Euler(0, 0, angle - 90f);

            // tamaño del sector
            slot.transform.localScale = new Vector3(slotWidth, slotWidth, 1);

            Renderer r = slot.GetComponent<Renderer>();
            r.material = (i % 2 == 0) ? redMaterial : blackMaterial;
        }
    }
}