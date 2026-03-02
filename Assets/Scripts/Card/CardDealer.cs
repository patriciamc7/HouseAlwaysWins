using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDealer : MonoBehaviour
{
    [SerializeField] GameObject ClubsCard;
    [SerializeField] GameObject CoinCard;
    [SerializeField] GameObject CupCard;
    [SerializeField] GameObject SwordCard;

    [SerializeField] Transform deckPoint;
    [SerializeField] Transform handRoot;

    [SerializeField] float distanceBetweenCards = 0.5f;

    private List<Transform> cardsInHand = new List<Transform>();
    private int currentSlot = 0;

    [SerializeField] SpanishDeck deck;
    public PlayerHand hand;

    public void StartRun(DeckBias bias)
    {
        deck = new SpanishDeck();
        hand = new PlayerHand();

        deck.CreateDeck();
        deck.Suffle();
        deck.SetBias(bias);

        hand.Clear();
    }
    public void StartDay()
    {
        DrawCardAtStart();
    }

    void DrawCardAtStart()
    {
        SpanishCard data = deck.Draw();
        DrawCard(data);
    }

    void DrawCard(SpanishCard data)
    {
        var cardValue = deck.Draw();
        if (cardValue != null)
            hand.Add(cardValue);

        GameObject prefab = GetPrefab(data.suit);

        if (prefab != null)
        {
            Debug.LogError("There's no pregab for suit: " + data.suit);
            return;
        }

        GameObject card = Instantiate(prefab, deckPoint.position, prefab.transform.rotation);

        CardFront front = card.GetComponentInChildren<CardFront>();
        front.SetValue(data.value);

        Vector3 targetPos = handRoot.position + Vector3.right * distanceBetweenCards * currentSlot;
        currentSlot++;

        cardsInHand.Add(card.transform);

        CardMoveAnimation moveAnimation = card.GetComponent<CardMoveAnimation>();
        moveAnimation.MoveToPosition(targetPos, card.transform.rotation, handRoot, () =>
        {
            Quaternion rotation = Quaternion.Euler(0f, 180f, 0f) * handRoot.rotation;
            moveAnimation.FlipTo(rotation, 0.25f, 0.03f);
        });
    }

    GameObject GetPrefab(Suit suit)
    {
        switch (suit)
        {
            case Suit.Cup:
                return CupCard;
            case Suit.Sword:
                return SwordCard;
            case Suit.Clubs:
                return ClubsCard;
            case Suit.Coin:
                return CoinCard;
            default: return null;
        }
    }

    public void ResetHand()
    {
        foreach (Transform t in cardsInHand)
        {
            Destroy(t.gameObject);
        }

        cardsInHand.Clear();
        currentSlot = 0;
    }
}
