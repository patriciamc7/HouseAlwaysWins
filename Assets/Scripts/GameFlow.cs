using System.Collections;
using UnityEngine;

public class GameFlow : MonoBehaviour
{
    private CardDealer cardDealer;

    public void StartTurn(CardDealer cardDealer)
    {
        this.cardDealer = cardDealer;
        StartCoroutine(TurnSequence());
    }

    private IEnumerator TurnSequence()
    {
        yield return new WaitForSeconds(0.3f); // espera que se anime
        yield return StartCoroutine(DrawBankCardsRoutine());
        yield return new WaitForSeconds(0.3f); // espera que se anime
        yield return StartCoroutine(DrawPlayerCardsRoutine());
    }

    private IEnumerator DrawBankCardsRoutine()
    {
        cardDealer.DrawBankCard();
        yield return new WaitForSeconds(0.5f); // espera que se anime

        //cardDealer.DrawBankCard(deck.DrawCard(), false);
        //yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator DrawPlayerCardsRoutine()
    {
        cardDealer.DrawCard();
        yield return new WaitForSeconds(0.5f); // espera animación
    }
}