using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum DeckBias
{
    None,
    MoreFigures,
    HigherNumbers,
    LowerNumbers
}

[System.Serializable]
public enum Suit
{
    Clubs,
    Coin,
    Cup,
    Sword
}

[System.Serializable]
public class SpanishDeck
{
    const int totalCards = 10;
    public List<SpanishCard> cards = new List<SpanishCard>();

    DeckBias currentBias = DeckBias.None;

    /// <summary>
    /// Creates the deck with all the suits 
    /// </summary>
    public void CreateDeck()
    {
        cards.Clear();

        foreach (Suit suit in System.Enum.GetValues(typeof(Suit)))
        {
            for (int i = 1; i <= totalCards; i++)
            {
                SpanishCard card = new SpanishCard();
                card.suit = suit;
                card.value = i;
                cards.Add(card);
            }
        }
    }

    /// <summary>
    /// Suffles the cards
    /// </summary>
    public void Suffle()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            int rnd = Random.Range(i, cards.Count);
            var temp = cards[i];
            cards[i] = cards[rnd];
            cards[rnd] = temp;
        }
    }

    /// <summary>
    /// Draws the first card of the Deck
    /// </summary>
    /// <returns></returns>
    public SpanishCard Draw()
    {
        if (cards.Count == 0) return null;

        if (currentBias == DeckBias.None)
        {
            SpanishCard c = cards[0];
            cards.RemoveAt(0);
            return c;
        }

        int index = GetBiasedIndex();
        SpanishCard card = cards[index];
        cards.RemoveAt(index);
        return card;
    }

    public void SetBias(DeckBias bias)
    {
        currentBias = bias;
    }

    int GetBiasedIndex()
    {
        List<int> weightedIndexes = new List<int>();

        for (int i = 0; i < cards.Count; i++)
        {
            int weight = 1;
            int v = cards[i].value;

            switch (currentBias)
            {
                case DeckBias.MoreFigures:
                    if (v >= 10)
                        weight = 4;
                    break;
                case DeckBias.HigherNumbers:
                    if (v >= 5 && v<= 7)
                        weight = 4;
                    break;
                case DeckBias.LowerNumbers:
                    if (v >= 1 && v<= 3)
                        weight = 4;
                    break;
            }

            for (int j = 0; j < weight; j++)
                weightedIndexes.Add(i);
        }

        if (weightedIndexes.Count == 0)
            return Random.Range(0, cards.Count);

        return weightedIndexes[Random.Range(0, weightedIndexes.Count)];
    }

    public float GetTotal()
    {
        float total = 0;
        for (int i = 0;i < cards.Count;i++)
        {
            total += cards[i].value;
        }
        return total;
    }
}
