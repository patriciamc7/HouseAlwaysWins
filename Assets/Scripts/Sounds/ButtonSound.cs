using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSound : MonoBehaviour, IPointerDownHandler
{
    public AudioClip clickSound;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("PointerDown");
        AudioManager.Instance.PlaySFX(clickSound);
    }

    //void Start()
    //{
    //    GetComponent<Button>().onClick.AddListener(() =>
    //        AudioManager.Instance.PlaySFX(clickSound)
    //    );
    //}
}