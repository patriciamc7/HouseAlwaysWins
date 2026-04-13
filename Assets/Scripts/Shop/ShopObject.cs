using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public enum objectType
{
    olive,
    oliveOil,
    calçot,
    cannelloni,
    stew,
    fuet,
    serranoHam,
    bread,
    spanishOmelette,
}

[Serializable]
public class shopObject : MonoBehaviour
{
    public objectType objectType;
    public int price;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }
    void OnClick()
    {
        FindObjectOfType<GameManager>().OnShopItemClick(this.gameObject);
    }
}