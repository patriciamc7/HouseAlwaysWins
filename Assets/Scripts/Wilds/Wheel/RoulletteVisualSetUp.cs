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

    //public void Start()
    //{
    //    GenerateSlots();
    //}
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
            GameObject slotGO = GameObject.CreatePrimitive(PrimitiveType.Quad);
            slotGO.name = "Slot " + (i + 1);
            slotGO.transform.SetParent(transform);
            slotGO.transform.localPosition = Vector3.zero;

            // rotar para que forme el círculo
            slotGO.transform.localRotation = Quaternion.Euler(0f, 0f, i * angleStep);

            // escalar para que se vea como sector
            slotGO.transform.localScale = new Vector3(slotWidth, radius, 1f);

            // mover hacia el borde
            slotGO.transform.localPosition = slotGO.transform.up * radius / 2f;

            // asignar color
            Renderer rend = slotGO.GetComponent<Renderer>();
            rend.material = (i % 2 == 0) ? redMaterial : blackMaterial;
        }
    }
}