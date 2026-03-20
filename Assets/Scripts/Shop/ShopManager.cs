using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] List<shopObject> objects = new List<shopObject>();
    [SerializeField] List<Transform> objectsPosition = new List<Transform>();
    [SerializeField] Transform parent;

    private int objectsInShop;
    private List<shopObject> currentList = new List<shopObject>();

    public void InitShop()
    {
        objectsInShop = objects.Count;
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
        for (int i = 0; i < objectsInShop; i++)
        {
            int index = Random.Range(0, currentList.Count);
            GameObject newObject = Instantiate(currentList[index].gameObject, objectsPosition[i].position, objectsPosition[i].rotation);
            newObject.transform.SetParent(parent, true);
            currentList.RemoveAt(index);
        }
    }
}
