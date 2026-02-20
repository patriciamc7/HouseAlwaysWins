using UnityEngine;
using UnityEngine.UI;
using System.Text;

public enum DayResult
{
    None,
    Win,
    Bust,
    Stand,
    Escape
}

public class GameManager : MonoBehaviour
{
    const float winPoints = 7.5f;

    //UI
    public Text handText;
    public Text totalText;
    public Text resultText;
    public Text dayText;
    public Text coinsText;

    SpanishDeck deck;
    PlayerHand hand;

    public int coins = 0;

    bool roundFinished = false;

    public int currentDay = 1;
    public int maxDays = 7;

    bool waitingNextDay = false;

    DayResult lastDayResult = DayResult.None;

    //Stress
    StressSytem stressSystem;
    public Slider stressSlider;

    #region Wilds
    public int remainingWilds = 3;

    //Dime
    DimeGame dimeSystem;
    public Button dimeWildButton;
    public float dimeBlockStress = 60f;

    //Slot
    SlotGame slotSystem;
    public Button slotWildButton;

    //Wheel
    WheelGame wheelGame;
    public Button wheelWildButton;
    #endregion

    #region Probability
    DeckBias currentPlayerBias = DeckBias.None;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        stressSystem = new StressSytem();

        wheelGame = new WheelGame();
        slotSystem = new SlotGame();
        dimeSystem = new DimeGame();

        deck = new SpanishDeck();
        hand = new PlayerHand();

        NewGame();
    }

    /// <summary>
    /// New game starts, reset everything and draw 2 cards
    /// </summary>
    public void NewGame()
    {
        if (currentDay == 1)
            stressSystem.Reset();

        deck.CreateDeck();
        deck.Suffle();
        hand.Clear();

        lastDayResult = DayResult.None;
        roundFinished = false;
        resultText.text = "";

        deck.SetBias(currentPlayerBias);
        DrawCard();

        RefreshUI();

        RefreshWildsByStress();

        ResetWilds();
    }

    /// <summary>
    /// Checks if the game has end and draw card
    /// </summary>
    public void DrawCard()
    {
        if (roundFinished) return;

        var card = deck.Draw();
        if (card != null)
            hand.Add(card);

        CheckResult();
        stressSystem.ProcessCardResult(hand.cards.Count, hand.GetTotal());
        RefreshUI();
    }

    /// <summary>
    /// Refresh the ui as the player draws cards
    /// </summary>
    void RefreshUI()
    {
        StringBuilder sb = new StringBuilder();

        foreach (var c in hand.cards)
            sb.AppendLine(c.ToString());

        handText.text = sb.ToString();
        totalText.text = "Total: " + hand.GetTotal();
        dayText.text = currentDay + " / " + maxDays;
        coinsText.text = "Coins: " + coins;

        RefreshStressUI();
    }

    /// <summary>
    /// Check results, win lose
    /// </summary>
    void CheckResult()
    {
        if (roundFinished) return;

        float total = hand.GetTotal();

        if (total == winPoints)
        {
            resultText.text = "WIN";
            EndRound(DayResult.Win);
        }
        else if (total > winPoints)
        {
            resultText.text = "Lose with " + total + " points";
            EndRound(DayResult.Bust);
        }
        else
        {
            resultText.text = "";
        }
    }

    public void Stand()
    {
        if (roundFinished) return;

        ResolveStand();
        EndRound(DayResult.Stand);
    }

    void ResolveStand()
    {
        float total = hand.GetTotal();

        if (total <= winPoints)
        {
            resultText.text = "Stand with " + total;
        }
        else
        {
            resultText.text = "Lose with " + total;
        }
    }

    void EndRound(DayResult result)
    {
        lastDayResult = result;
        roundFinished = true;
        waitingNextDay = result == DayResult.Win;
    }

    public void NextDay()
    {
        if (!waitingNextDay) return;

        UpdateHouseBias();

        stressSystem.ProcessDayResult(lastDayResult, currentDay);
        ApplyDayReward();
        RefreshWildsByStress();

        if (stressSystem.IsCollapsed())
        {
            resultText.text = "Mental collapse";
            return;
        }

        currentDay++;

        if (currentDay > maxDays) //TODO poner al final de la ronda
        {
            resultText.text = "You survived 7 days!!";
            return;
        }

        waitingNextDay = false;

        NewGame();
    }

    void RefreshStressUI()
    {
        if (stressSlider == null || stressSystem == null) return;

        stressSlider.maxValue = stressSystem.maxStress;
        stressSlider.value = stressSystem.currentStress;
    }

    void RefreshWildsByStress()
    {
        if (dimeWildButton == null || stressSystem == null) return;

        bool blocked = stressSystem.currentStress >= dimeBlockStress;

        dimeWildButton.interactable = !blocked;
    }

    void OnWildPress()
    {
        remainingWilds--;
        stressSystem.ProcessWilds(remainingWilds);
        RefreshStressUI();
    }

    public void UseDimeWild()
    {
        dimeWildButton.interactable = false;

        if (roundFinished) return;

        if (remainingWilds <= 0) return;

        if (dimeSystem.isWin())
        {
            coins /= 2;
            EndRound(DayResult.Escape);
            RefreshWildsByStress();
        }

        OnWildPress();
    }

    public void UseSlotWild()
    {
        slotWildButton.interactable = false;

        if (roundFinished) return;

        if (remainingWilds <= 0) return;

        if (hand.cards.Count == 0) return;

        if (slotSystem.isWin())
        {
            var newCard = deck.Draw();
            if (newCard == null) return;

            //TODO CLICK
            int index = Random.Range(0, hand.cards.Count);

            hand.cards[index] = newCard;
        }

        RefreshUI();
        OnWildPress();
    }

    public void useWheelWild()
    {
        wheelWildButton.interactable = false;

        if (roundFinished) return;

        if (remainingWilds <= 0) return;

        bool isWin = wheelGame.isWin();
        if (isWin)
        {
            coins *= 2;
        }
        else
        {
            coins = 0;
        }

        RefreshUI();
        OnWildPress();
    }

    void ApplyDayReward()
    {
        int reward = 0;

        switch (lastDayResult)
        {
            case DayResult.Win:
                reward = 10;
                break;

            case DayResult.Stand:
                reward = 5;
                break;

            case DayResult.Bust:
            case DayResult.Escape:
                reward = 0;
                break;
        }

        coins += reward;
    }

    void ResetWilds()
    {
        dimeWildButton.interactable = true;
        slotWildButton.interactable = true;
        wheelWildButton.interactable = true;
    }

    void UpdateHouseBias()
    {
        if (lastDayResult == DayResult.Bust)
        {
            int r = Random.Range(0, 3); // TODO PONER EL QUE MAS LE CONVENGA A LA CASA

            switch (r)
            {
                case 0:
                    currentPlayerBias = DeckBias.MoreHighNumbers;
                    break;
                case 1:
                    currentPlayerBias = DeckBias.MoreFigures;
                    break;
                case 2:
                    currentPlayerBias = DeckBias.MoreLowNumbers;
                    break;
            }
            Debug.Log("La casa ha cambiado las probabilidades: " + currentPlayerBias);
        }
        else
        {
            currentPlayerBias = DeckBias.None;
        }
    }
}
