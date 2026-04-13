using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerHand : MonoBehaviour
{
    public List<SpanishCard> cards = new List<SpanishCard>();

    /// <summary>
    /// Add one cadr to the player hand
    /// </summary>
    /// <param name="card"></param>
    public void Add(SpanishCard card)
    {
        cards.Add(card);
    }

    /// <summary>
    /// Count the total points of the player hand
    /// </summary>
    /// <returns></returns>
    public float GetTotal()
    {
        float total = 0;

        foreach (var c in cards)
            total += c.GetPoints();

        return total;
    }

    /// <summary>
    /// Clears the player hand
    /// </summary>
    public void Clear()
    {
        cards.Clear();
    }
}
