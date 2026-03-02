using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Text.RegularExpressions;

public enum DayResult
{
    None,
    ExactWin,
    Bust,
    Stand,
    Escape
}

public class GameManager : MonoBehaviour
{
    const float winPoints = 7.5f;

    #region UI
    public Text handText;
    public Text totalText;
    public Text resultText;
    public Text dayText;
    public Text coinsText;
    public Text currentBiasText;

    public Button drawButton;
    public Button standButton;
    #endregion

    public int coins = 0;

    bool roundFinished = false;

    public int currentDay = 1;
    public int maxDays = 7;

    bool waitingNextDay = false;

    DayResult lastDayResult = DayResult.None;

    CardDealer dealer;

    #region Stress
    public StressSytem stressSystem;
    public Slider stressSlider;
    #endregion

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
    public GameObject biasUI;
    DeckBias currentPlayerBias = DeckBias.None;
    #endregion

    #region NPC
    public BankNPC bankNPC;
    public Image bankCardImage;
    public Text bankHandTotal;
    #endregion

    #region Shop
    public GameObject shopUI;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        stressSystem = new StressSytem();

        wheelGame = new WheelGame();
        slotSystem = new SlotGame();
        dimeSystem = new DimeGame();

        bankNPC = new BankNPC();

        NewGame();
        ResetWilds();
    }

    /// <summary>
    /// New game starts, reset everything and draw 2 cards
    /// </summary>
    public void NewGame()
    {
        if (currentDay == 1)
            stressSystem.Reset();

        dealer.StartRun(currentPlayerBias);

        bankNPC.Clear();

        lastDayResult = DayResult.None;
        roundFinished = false;
        resultText.text = "";

        DrawBank();

        DrawCard();

        RefreshUI();
        BlockButtons(true);

        RefreshWildsByStress();
    }

    public void DrawBank() //TODO
    {
        //if (roundFinished) return;

        //var card = deck.Draw();
        //if (card != null)
        //    bankNPC.Add(card);

        //StringBuilder sb = new StringBuilder();

        //foreach (var c in bankNPC.cards)
        //    sb.AppendLine(c.ToString());

        //bankHandTotal.text = sb.ToString();
    }

    /// <summary>
    /// Checks if the game has end and draw card
    /// </summary>
    public void DrawCard()
    {
        if (roundFinished) return;

        //Draw card dealer;

        CheckResult();
        stressSystem.ProcessCardResult(dealer.hand.cards.Count, dealer.hand.GetTotal());

        if (stressSystem.IsCollapsed())
        {
            resultText.text = "Mental collapse";
            return;
        }

        RefreshUI();
    }

    /// <summary>
    /// Refresh the ui as the player draws cards
    /// </summary>
    void RefreshUI()
    {
        StringBuilder sb = new StringBuilder();

        foreach (var c in dealer.hand.cards)
            sb.AppendLine(c.ToString());

        handText.text = sb.ToString();
        totalText.text = "Total: " + dealer.hand.GetTotal();
        dayText.text = currentDay + " / " + maxDays;
        coinsText.text = "Coins: " + coins;
        currentBiasText.text = "Current bias: " + currentPlayerBias.ToReadable();

        RefreshStressUI();
    }

    /// <summary>
    /// Check results, win lose
    /// </summary>
    void CheckResult()
    {
        if (roundFinished) return;

        float total = dealer.hand.GetTotal();

        if (total == winPoints)
        {
            BlockButtons(false);
            resultText.text = "WIN";
            EndRound(DayResult.ExactWin);
        }
        else if (total > winPoints)
        {
            BlockButtons(false);
            resultText.text = "Lose with " + total + " points";
            EndRound(DayResult.Bust);
            //TODo GAMEOVER
        }
        else
        {
            resultText.text = "";
        }
    }

    void BlockButtons(bool interactable)
    {
        standButton.interactable = interactable;
        drawButton.interactable = interactable;
    }

    public void Stand()
    {
        if (roundFinished) return;

        DayResult result = ResolveStand();
        EndRound(result);
    }

    DayResult ResolveStand()
    {
        float total = dealer.hand.GetTotal();
        float totalBank = bankNPC.GetTotal();

        if (total <= totalBank)
        {
            resultText.text = "Lose with " + total;
            coins = 0;
            return DayResult.Bust;
        }
        else
        {
            resultText.text = "Win with " + total;
            return DayResult.Stand;
        }
    }

    void EndRound(DayResult result)
    {
        lastDayResult = result;
        roundFinished = true;
        waitingNextDay = result == DayResult.ExactWin || result == DayResult.Stand;

        if (waitingNextDay)
            OpenShop();
    }

    public void NextDay()
    {
        shopUI.SetActive(false);

        if (!waitingNextDay) return;

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

        if (stressSystem.IsCollapsed())
        {
            resultText.text = "Mental collapse";
            return;
        }

        RefreshStressUI();
    }

    public void UseDimeWild()
    {
        dimeWildButton.interactable = false;
        roundFinished = true;

        standButton.interactable = false;
        drawButton.interactable = false;

        if (remainingWilds <= 0)
            return;

        if (dimeSystem.isWin())
        {
            coins /= 2;
            EndRound(DayResult.Escape);
            NewGame();//TODO GO BACK TO MENU
        }

        RefreshWildsByStress();
        RefreshUI();
        OnWildPress();
    }

    public void UseSlotWild()
    {
        slotWildButton.interactable = false;

        if (roundFinished) return;

        if (remainingWilds <= 0) return;

        if (dealer.hand.cards.Count == 0) return;

        if (slotSystem.isWin())
        {
            //TODO
            //var newCard = deck.Draw();
            //if (newCard == null) return;

            //TODO CLICK
            //int index = Random.Range(0, dealer.hand.cards.Count);

            //dealer.hand.cards[index] = newCard;
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
        OnWildPress();
    }

    void ApplyDayReward()
    {
        int reward = 0;

        switch (lastDayResult)
        {
            case DayResult.ExactWin:
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
                    currentPlayerBias = DeckBias.HigherNumbers;
                    break;
                case 1:
                    currentPlayerBias = DeckBias.MoreFigures;
                    break;
                case 2:
                    currentPlayerBias = DeckBias.LowerNumbers;
                    break;
            }
        }
        else
        {
            biasUI.SetActive(true);
        }
    }

    public void PlayerChangeHouseBias(string bias)
    {
        if (System.Enum.TryParse<DeckBias>(bias, out currentPlayerBias))
            NextDay();

        biasUI.SetActive(false);
    }

    void OpenShop()
    {
        shopUI.SetActive(true);

        //TODO
        //RefreshShopItems();
    }

    public void OnShopClosed()
    {
        shopUI.SetActive(false);
        UpdateHouseBias();
    }


}
public static class EnumExtensions
{
    public static string ToReadable(this System.Enum value)
    {
        return Regex.Replace(value.ToString(), "(\\B[A-Z])", " $1");
    }
}
