using System.Collections.Generic;
using UnityEngine;

public class CardDrawTest : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform deckPoint;
    public Transform handRoot;
    public float distanceBetweenCards = 0.5f;

    List<Transform> cardsInHand  =new List<Transform>();

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            DrawCard();

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (cardsInHand.Count > 0)
            {
                Transform lastCard = cardsInHand[cardsInHand.Count - 1];
                Quaternion rotation = Quaternion.Euler(0f, 180f, 0f) * handRoot.rotation;

                lastCard.GetComponent<CardMoveAnimation>().FlipTo(rotation, 0.25f, 0.03f);
            }
        }
    }

    void DrawCard()
    {
        GameObject card = Instantiate(cardPrefab, deckPoint.position, deckPoint.rotation);
        card.transform.rotation = Quaternion.Euler(0f, 180f, 180f) * handRoot.rotation;

        cardsInHand.Add(card.transform);
        RelayoutHand();
    }

    void RelayoutHand()
    {
        int total = cardsInHand.Count;

        for (int i = 0; i < total; i++)
        {
            float offset = i - (total - 1) * 0.5f;

            Vector3 targetPos = handRoot.position + (handRoot.right * distanceBetweenCards * offset) + (handRoot.forward * 0.002f * i);
            Quaternion targetRot = cardsInHand[i].transform.rotation; // handRoot.rotation;

            var anim = cardsInHand[i].GetComponent<CardMoveAnimation>();
            
            if (i == total - 1)
            {
                anim.MoveToPosition(targetPos, targetRot, handRoot, () =>
                {
                    Quaternion rotation = Quaternion.Euler(0f, 180f, 0f) * handRoot.rotation;
                    anim.FlipTo(rotation);
                });
            }
        }
    }
}
