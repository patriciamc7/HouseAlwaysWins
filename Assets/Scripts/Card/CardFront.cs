using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class CardFront : MonoBehaviour
{
    [SerializeField] List<Text> valueText;

    public void SetValue(int value)
    {
        foreach (Text text in valueText)
        {
            text.text = value.ToString();
        }
    }
}
