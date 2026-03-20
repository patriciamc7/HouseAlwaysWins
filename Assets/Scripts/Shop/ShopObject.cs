using UnityEngine;
using UnityEngine.UI;

public enum objectType
{
    olive,
    ham,
    bread,
    omelette,
}

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