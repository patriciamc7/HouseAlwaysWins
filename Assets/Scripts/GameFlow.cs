using System.Collections;
using UnityEngine;

public class GameFlow : MonoBehaviour
{
    private CardDealer cardDealer;

    public void StartTurn(CardDealer cardDealer, System.Action onComplete)
    {
        this.cardDealer = cardDealer;
        StartCoroutine(TurnSequence(onComplete));
    }

    private IEnumerator TurnSequence(System.Action onComplete)
    {
        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(DrawBankCardsRoutine());
        yield return new WaitForSeconds(0.3f);
        yield return StartCoroutine(DrawPlayerCardsRoutine());

        while (cardDealer.ShouldBankDraw())
        {
            yield return new WaitForSeconds(0.3f);
            yield return StartCoroutine(DrawBankCardsRoutine());
        }

        onComplete?.Invoke();
    }

    private IEnumerator DrawBankCardsRoutine()
    {
        cardDealer.DrawBankCard();
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator DrawPlayerCardsRoutine()
    {
        cardDealer.DrawCard();
        yield return new WaitForSeconds(0.5f);
    }

    public void OnStand()
    {
        StartCoroutine(StandSequence());
    }

    public IEnumerator StandSequence()
    {
        yield return StartCoroutine(RaisePlayerCards());
    }
    private IEnumerator RaisePlayerCards()
    {
        cardDealer.RaisePlayerCards();
        yield return new WaitForSeconds(0.5f);
    }
}