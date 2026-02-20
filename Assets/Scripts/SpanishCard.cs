using System;
using UnityEngine;

[Serializable]
public class SpanishCard
{
    public string suit;
    public int value;

    /// <summary>
    /// Gets the points of the cards
    /// </summary>
    /// <returns></returns>
    public float GetPoints()
    {
        if (value > 7)
            return 0.5f;

        return value;
    }

    /// <summary>
    /// Gets the value and suit of the card
    /// </summary>
    /// <returns></returns>
    public  override string ToString()
    {
        return value + " de " + suit; //todo cambiar por idioma
    }
}
