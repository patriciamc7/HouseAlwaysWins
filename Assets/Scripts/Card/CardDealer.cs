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
    [SerializeField] Transform bankHandRoot;

    [SerializeField] float distanceBetweenCards = 1f;
    [SerializeField] float heightBetweenCards = 0.03f;

    private List<Transform> cardsInHand = new List<Transform>();
    private List<Transform> cardsInBankHand = new List<Transform>();

    private int currentSlot = 0;
    private int currentBankSlot = 0;

    [SerializeField] SpanishDeck deck;
    public PlayerHand hand;
    public PlayerHand bankHand;

    public void StartRun(DeckBias bias)
    {
        deck = new SpanishDeck();
        bankHand = new PlayerHand();
        hand = new PlayerHand();

        deck.CreateDeck();
        deck.Suffle();
        deck.SetBias(bias);

        ResetHand();
    }

    public void DrawBankCard()
    {
        var data = deck.Draw();
        if (data != null)
            bankHand.Add(data);

        GameObject prefab = GetPrefab(data.suit);
        GameObject card = Instantiate(prefab, deckPoint.position, prefab.transform.rotation);

        // Configura el valor en el front
        CardFront front = card.GetComponentInChildren<CardFront>();
        front.SetValue(data.value);

        // Animación hacia la mano
        Vector3 targetPos = bankHandRoot.position
                        + Vector3.right * distanceBetweenCards * currentBankSlot
                        + Vector3.up * heightBetweenCards * currentBankSlot;

        currentBankSlot++;
        cardsInBankHand.Add(card.transform);

        CardMoveAnimation moveAnim = card.GetComponent<CardMoveAnimation>();
        moveAnim.MoveToPosition(targetPos, card.transform.rotation, bankHandRoot, () =>
        {
            if (cardsInBankHand.Count == 1)
                moveAnim.FlipTo(Quaternion.Euler(0f, 0f, 180f) * card.transform.rotation, () => { RelayoutBankHand(); }, 0.25f, 1f);
            else
                RelayoutBankHand();
        });
    }

    public void DrawCard()
    {
        var cardValue = deck.Draw();
        if (cardValue != null)
            hand.Add(cardValue);

        GameObject prefab = GetPrefab(cardValue.suit);
        GameObject card = Instantiate(prefab, deckPoint.position, prefab.transform.rotation * Quaternion.Euler(0f, 180f, 0f));
        CardFront front = card.GetComponentInChildren<CardFront>();
        front.SetValue(cardValue.value);

        Vector3 targetPos = handRoot.position + Vector3.right * distanceBetweenCards * currentSlot + Vector3.up * heightBetweenCards * currentSlot;
        currentSlot++;

        cardsInHand.Add(card.transform);

        CardMoveAnimation moveAnimation = card.GetComponent<CardMoveAnimation>();
        moveAnimation.MoveToPosition(targetPos, card.transform.rotation, handRoot, () =>
        {
            Quaternion rotation = Quaternion.Euler(0f, 0f, 180f) * card.transform.rotation;
            moveAnimation.FlipTo(rotation, () => { RelayoutHand(); }, 0.25f, 1f);
        });
    }

    public void RelayoutBankHand()
    {
        if (cardsInBankHand.Count == 0) return;

        // Calcula el ancho total de la mano
        float totalWidth = distanceBetweenCards * (cardsInBankHand.Count - 1);

        // Centro base de la banca
        Vector3 centerPos = bankHandRoot.position;

        // Posición inicial de la primera carta
        Vector3 startPos = centerPos - Vector3.right * (totalWidth / 2f);

        for (int i = 0; i < cardsInBankHand.Count; i++)
        {
            Transform card = cardsInBankHand[i];

            Vector3 targetPos = startPos + Vector3.right * distanceBetweenCards * i
                                + Vector3.up * heightBetweenCards * i;

            // Mueve cada carta suavemente a su posición
            CardMoveAnimation moveAnim = card.GetComponent<CardMoveAnimation>();
            moveAnim.MoveRelayout(targetPos, 0.1f);
        }
    }

    public void RelayoutHand()
    {
        if (cardsInHand.Count == 0) return;

        // Calcula el ancho total de la mano
        float totalWidth = distanceBetweenCards * (cardsInHand.Count - 1);

        // Centro base (handRoot)
        Vector3 centerPos = handRoot.position;

        // Posición inicial de la primera carta
        Vector3 startPos = centerPos - Vector3.right * (totalWidth / 2f);

        for (int i = 0; i < cardsInHand.Count; i++)
        {
            Transform card = cardsInHand[i];
            Vector3 targetPos = startPos + Vector3.right * distanceBetweenCards * i
                                + Vector3.up * heightBetweenCards * i;
            CardMoveAnimation moveAnim = card.GetComponent<CardMoveAnimation>();
            moveAnim.MoveRelayout(targetPos, 0.1f);
        }
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
        hand.Clear();
        bankHand.Clear();

        foreach (Transform t in cardsInHand)
        {
            Destroy(t.gameObject);
        }

        foreach (Transform t in cardsInBankHand)
        {
            Destroy(t.gameObject);
        }

        cardsInHand?.Clear();
        cardsInBankHand?.Clear();

        currentBankSlot = 0;
        currentSlot = 0;
    }
}
