using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] List<shopObject> objects = new List<shopObject>();
    [SerializeField] List<Transform> objectsPosition = new List<Transform>();
    [SerializeField] Transform parent;

    private int objectsInShop;
    public List<shopObject> currentList = new List<shopObject>();
    public List<shopObject> currentShop = new List<shopObject>();
    public List<shopObject> currentBought = new List<shopObject>();

    public void InitShop()
    {
        objectsInShop = objectsPosition.Count;
        currentList = new List<shopObject>(objects);
    }

    public void NewGame()
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    public void StartShop()
    {
        currentShop = new List<shopObject>();
        currentBought = new List<shopObject>();

        for (int i = 0; i < objectsInShop; i++)
        {
            int index = Random.Range(0, currentList.Count);
            GameObject newObject = Instantiate(currentList[index].gameObject, objectsPosition[i].position, objectsPosition[i].rotation);
            newObject.transform.SetParent(parent, true);

            currentShop.Add(currentList[index]);
            currentList.RemoveAt(index);
        }
    }

    public void LoadShop()
    {
        for (int j = 0; j < objectsInShop; j++)
        {
            if (currentBought == null || currentBought.Count == 0 || !currentBought.Contains(currentShop[j]))
            {
                GameObject newObject = Instantiate(currentShop[j].gameObject, objectsPosition[j].position, objectsPosition[j].rotation);
                newObject.transform.SetParent(parent, true);
            }
        }
    }

    public void BuyItem(shopObject obj)
    {
        currentBought.Add(obj);
    }
}
