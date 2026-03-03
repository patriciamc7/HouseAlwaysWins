using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;


public class CardFront : MonoBehaviour
{
    [SerializeField] List<TextMeshPro> valueText;

    public void SetValue(int value)
    {
        foreach (TextMeshPro text in valueText)
        {
            text.text = value.ToString();
        }
    }
}
