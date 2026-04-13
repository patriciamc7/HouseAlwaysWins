using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.UI;

public enum DayResult
{
    None,
    ExactWin,
    Bust,
    Stand,
    Escape
}
public enum GameEventType
{
    ChanceGame,
    Transition,
    SevenAndHalf, 
    DeckModifier,
    Shop,
    FinalGame,
    EndRun
}

public enum WildType: int
{
    None = 0,
    Wheel = 1,
    Slot = 2,
    Coin =3,
}

public class GameManager : MonoBehaviour
{
    [SerializeField] CardDealer dealer;
    [SerializeField] GameFlow gameFlow;

    const float winPoints = 7.5f;

    #region UI
    //Debug
    [SerializeField] Text bankHandText;
    [SerializeField] Text totalText;
    [SerializeField] Text currentBiasText;


    [SerializeField] GameObject npcPhrases;
    [SerializeField] Text dayText;
    [SerializeField] Text coinsText;
    [SerializeField] GameObject coinsGameObject;

    [SerializeField] GameObject cardButtons;
    [SerializeField] ResultUI gameResult;

    [SerializeField] IrisTransition iris;
    #endregion

    int coins = 5;

    int currentDay = 0;
    int currentEventIndex = 0;

    bool waitingNextDay = false;

    DayResult lastDayResult = DayResult.None;

    #region Stress
    [SerializeField] StressSytem stressSystem;
    [SerializeField] Slider stressSlider;
    #endregion

    #region Wilds
    int remainingWilds = 3;
    WildType currentWild;

    #region Dime
    [SerializeField] CoinWildController coinWild;
    [SerializeField] GameObject coinPanel;
    #endregion

    #region Wheel
    [SerializeField] GameObject wheelPanel;
    [SerializeField] WheelWildLogic wheelGame;
    #endregion

    #region Slot
    [SerializeField] GameObject slotWildPanel;
    #endregion

    #endregion

    #region Probability
    [SerializeField] GameObject biasUI;
    DeckBias currentPlayerBias = DeckBias.None;
    #endregion

    #region NPC
    [SerializeField] Text bankHandTotal;
    #endregion

    #region Shop
    [SerializeField] GameObject shopUI;
    #endregion

    void Start()
    {
        stressSystem = new StressSytem();
        shopUI.GetComponent<ShopManager>().InitShop();

        if (GameState.loadSavedGame)
        {
            LoadGame();
        }
        else
        {
            NewGame();
        }
    }

    /// <summary>
    /// New game starts, reset everything and draw 2 cards
    /// </summary>
    public void NewGame()
    {
        currentDay = 0;
        currentEventIndex = 0;
        currentWild = WildType.None;

        shopUI.GetComponent<ShopManager>().NewGame();

        if (currentDay == 0)
            stressSystem.Reset();

        dealer.StartRun(currentPlayerBias);
        lastDayResult = DayResult.None;

        StartEvent();
        RefreshUI();

    }

    private void ResetHand()
    {
        dealer.ResetHand();
    }

    private void OnTransitionEnded()
    {
        NextEvent();
        iris.gameObject.SetActive(false);
    }

    public void StartEvent()
    {
        GameEventType currentEvent =
            GameFlowConfig.days[currentDay][currentEventIndex];

        Debug.Log("Día " + (currentDay + 1) + " - Evento: " + currentEvent);

        switch (currentEvent)
        {
            case GameEventType.ChanceGame:
                npcPhrases.GetComponent<PhrasesManagerNPC>().Hide();
                cardButtons.SetActive(false);
                StartChanceGame(GameState.loadSavedGame);
                break;

            case GameEventType.Transition:
                BlockButtons(false);
                iris.GoToScene(OnTransitionEnded);
                break;

            case GameEventType.SevenAndHalf:
                if (GameState.loadSavedGame)
                    break;
                ResetHand();
                StartSevenAndHalf();
                break;

            case GameEventType.DeckModifier:
                UpdateDeckBias();
                break;

            case GameEventType.Shop:
                OpenShop();
                break;

            case GameEventType.FinalGame:
                //StartFinalGame();
                StartSevenAndHalf();
                break;

            case GameEventType.EndRun:
                //TODO CELEBRATION AND GO TO MENU
                break;
        }

        GameState.loadSavedGame = false;
        SaveState();
    }

    public void SaveState()
    {
        SaveData data = new SaveData();
        data.coins = coins;
        data.currentDay = currentDay;
        data.bias = currentPlayerBias;
        data.currentEvent = currentEventIndex;
        data.bankHandCards = dealer.bankHand.cards;
        data.handCards = dealer.hand.cards;
        data.deck = dealer.deck.cards;
        data.stressLevel = stressSystem.currentStress;
        data.wildType = currentWild;
        data.shopItems = shopUI.GetComponent<ShopManager>().currentList;
        data.itemsBought = shopUI.GetComponent<ShopManager>().currentBought;
        data.currentShop = shopUI.GetComponent<ShopManager>().currentShop;

        SaveManager.Save(data);
    }

    void LoadGame()
    {
        SaveData data = SaveManager.Load();

        currentWild = data.wildType;
        stressSystem.currentStress = data.stressLevel;
        coins = data.coins;
        currentDay = data.currentDay;
        currentEventIndex = data.currentEvent;
        currentPlayerBias = data.bias;
        dealer.StartRun(currentPlayerBias);
        dealer.bankHand.cards = data.bankHandCards;
        dealer.hand.cards = data.handCards;
        dealer.loadInstance();
        dealer.deck.cards = data.deck;

        RefreshUI();
        lastDayResult = DayResult.None;

        shopUI.GetComponent<ShopManager>().currentList  = data.shopItems;
        shopUI.GetComponent<ShopManager>().currentBought = data.itemsBought;
        shopUI.GetComponent<ShopManager>().currentShop = data.currentShop;

        StartEvent();
    }

    public void NextEvent()
    {
        RefreshUI();
        currentEventIndex++;

        if (currentEventIndex >= GameFlowConfig.days[currentDay].Count)
        {
            currentDay++;
            currentEventIndex = 0;

            if (currentDay >= GameFlowConfig.days.Count)
            {
                return;
            }
        }

        StartEvent();
    }

    private void StartSevenAndHalf()
    {
        npcPhrases.GetComponent<PhrasesManagerNPC>().Show();
        cardButtons.SetActive(true);
        gameFlow.StartTurn(dealer, () => BlockButtons(true));
        RefreshUI();
    }
    public void DrawBank()
    {
        dealer.DrawBankCard();

        StringBuilder sb = new StringBuilder();

        foreach (var c in dealer.bankHand.cards)
            sb.AppendLine(c.ToString());

        bankHandTotal.text = sb.ToString();
    }

    /// <summary>
    /// Checks if the game has end and draw card
    /// </summary>
    public void DrawCard()
    {
        BlockButtons(false);
        dealer.DrawCard(() => BlockButtons(true));

        CheckResult();
        stressSystem.ProcessCardResult(dealer.hand.cards.Count, dealer.hand.GetTotal());

        if (stressSystem.IsCollapsed())
        {
            //resultText.text = "Mental collapse";
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
        StringBuilder bsb = new StringBuilder();

        foreach (var c in dealer.hand.cards)
            sb.AppendLine(c.ToString());

        foreach (var c in dealer.bankHand.cards)
            bsb.AppendLine(c.ToString());

        bankHandText.text = bsb.ToString();
        totalText.text = "Total: " + dealer.hand.GetTotal();
        dayText.text = currentDay + 1 + " / " + GameFlowConfig.days.Count;
        coinsText.text = coins.ToString();
        currentBiasText.text = "Current bias: " + currentPlayerBias.ToReadable();

        RefreshStressUI();
    }

    /// <summary>
    /// Check results, win lose
    /// </summary>
    void CheckResult()
    {
        float total = dealer.hand.GetTotal();

        if (total == winPoints)
        {
            BlockButtons(false);
            StartCoroutine(ShowResult(DayResult.ExactWin));
        }
        else if (total > winPoints)
        {
            BlockButtons(false);
            StartCoroutine(ShowResult(DayResult.Bust));
        }
    }

    void BlockButtons(bool interactable)
    {
        foreach(Button but in cardButtons.GetComponentsInChildren<Button>())
        {
            but.interactable = interactable;
        }
    }

    public void Stand()
    {
        BlockButtons(false);
        gameFlow.OnStand();
        ResolveStand();
    }

    void ResolveStand()
    {
        float total = dealer.hand.GetTotal();
        float totalBank = dealer.bankHand.GetTotal();

        if (total <= totalBank && totalBank <= 7.5f)
        {
            coins = 0;
            StartCoroutine(ShowResult(DayResult.Bust));
        }
        else
        {
            StartCoroutine(ShowResult(DayResult.Stand));
        }
    }

    public void EndRound(DayResult result)
    {
        lastDayResult = result;
        waitingNextDay = result == DayResult.ExactWin || result == DayResult.Stand;

        NextEvent();
    }

    public void NextDay()
    {
        if (!waitingNextDay) return;

        stressSystem.ProcessDayResult(lastDayResult, currentDay);
        ApplyDayReward();

        if (stressSystem.IsCollapsed())
        {
            return;
        }

        waitingNextDay = false;
    }

    void RefreshStressUI()
    {
        if (stressSlider == null || stressSystem == null) return;

        stressSlider.maxValue = stressSystem.maxStress;
        stressSlider.GetComponent<SliderEasing>().AnimateTo(stressSystem.currentStress);
    }

    void OnWildPress()
    {
        remainingWilds--;
        stressSystem.ProcessWilds(remainingWilds);

        if (stressSystem.IsCollapsed())
        {
            return;
        }

        RefreshStressUI();
    }

    void StartChanceGame(bool isLoad)
    {
        int r = UnityEngine.Random.Range(1, 4);
        if (isLoad) 
            r = (int)currentWild;

        switch (r)
        {
            case (int)WildType.Wheel:
                currentWild = WildType.Wheel;
                useWheelWild();
                break;
            case (int)WildType.Slot:
                currentWild = WildType.Slot;
                UseSlotWild();
                break;
            case (int)WildType.Coin:
                currentWild = WildType.Coin;
                UseCoinWild();
                break;
        }
    }

    #region CoinWild
    public void UseCoinWild()
    {
        foreach (Button but in coinPanel.GetComponentsInChildren<Button>())
        {
            but.interactable = true;
            but.gameObject.SetActive(true);
        }
        coinPanel.SetActive(true);
    }

    public void SideChoose(string playerChoice)
    {
        foreach (Button but in coinPanel.GetComponentsInChildren<Button>())
        {
            if (but.name == playerChoice)
                but.interactable = false;
            else
                but.gameObject.SetActive(false);
        }

        CoinSide coinSide;
        if (System.Enum.TryParse<CoinSide>(playerChoice, out coinSide))
        {
            coinWild.UseCoinWild(coinSide, OnCoinWildResult);
        }
    }

    void OnCoinWildResult(bool isWin)
    {
        coins /= 2;
        if (isWin)
        {
           // EndRound(DayResult.Escape);
            //NewGame();//TODO GO BACK TO MENU
        }

        RefreshUI();
        OnWildPress();

        coinWild.DestroyCoin();
        coinPanel.SetActive(false);
        currentWild = WildType.None;
        NextEvent();
    }
    #endregion

    #region WheelWild
    public void useWheelWild()
    {
        foreach (Button but in wheelPanel.GetComponentsInChildren<Button>())
        {
            but.interactable = true;
            but.gameObject.SetActive(true);
        }
        wheelPanel.SetActive(true);
    }

    public void SideColor(string playerChoice)
    {
        foreach (Button but in wheelPanel.GetComponentsInChildren<Button>())
        {
            if (but.name == playerChoice)
                but.interactable = false;
            else
                but.gameObject.SetActive(false);
        }

        Color playerColor;
        if (playerChoice == "Red")
            playerColor = Color.red;
        else
            playerColor = Color.green;

        wheelGame.PlayWheel(playerColor, OnWheelWildResult);
    }

    void OnWheelWildResult(bool isWin)
    {
        if (isWin)
            coins *= 2;
        else
            coins = 0;
        
        RefreshUI();
        OnWildPress();

        NextEvent();
        currentWild = WildType.None;
        wheelPanel.SetActive(false);
    }

    #endregion

    #region SlotWild
    public void UseSlotWild()
    {
        foreach (Button but in slotWildPanel.GetComponentsInChildren<Button>())
        {
            but.interactable = true;
            but.gameObject.SetActive(true);
        }
        slotWildPanel.SetActive(true);
    }

    public void SpinSlot()
    {
        foreach (Button but in slotWildPanel.GetComponentsInChildren<Button>())
        {
            but.gameObject.SetActive(false);
        }

        slotWildPanel.GetComponent<reelSpin>().Spin( () => OnSlotWildResult());
    }

    void OnSlotWildResult()
    {
        RefreshUI();
        OnWildPress();
        currentWild = WildType.None;
        slotWildPanel.SetActive(false);
        NextEvent();
    }
    #endregion

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

    void UpdateDeckBias()
    {
        if (lastDayResult == DayResult.Bust)
        {
            int r = UnityEngine.Random.Range(0, 3); // TODO PONER EL QUE MAS LE CONVENGA A LA CASA

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
            npcPhrases.GetComponent<PhrasesManagerNPC>().Hide();
            biasUI.SetActive(true);
            cardButtons.SetActive(false);
        }
    }

    public void PlayerChangeHouseBias(string bias)
    {
        if (System.Enum.TryParse<DeckBias>(bias, out currentPlayerBias))
        {
            biasUI.SetActive(false);

            NextEvent();
        }
            NextDay();
    }

    #region Shop
    void OpenShop()
    {
        shopUI.SetActive(true);
        cardButtons.SetActive(false);
        npcPhrases.GetComponent<PhrasesManagerNPC>().Hide();

        if (GameState.loadSavedGame)
        {
            shopUI.GetComponent<ShopManager>().LoadShop();
        }
        else
        {
            shopUI.GetComponent<ShopManager>().StartShop();
        }
    }

    public void OnShopClosed()
    {
        shopUI.SetActive(false);
        NextEvent();
    }

    public void OnShopItemClick(GameObject currentObject)
    {
        shopObject shopItem = currentObject.GetComponent<shopObject>();
        if (coins >= shopItem.price)
        {
            coins = coins - shopItem.price;
            CoinUpdate();
            RefreshUI();
            currentObject.GetComponent<CardClickAnimation>().OnCardClick();
            shopUI.GetComponent<ShopManager>().BuyItem(shopItem);
            //TODO APPLY EFFECTS
        }
        else
        {
            //NO SE PUEDE COMPRAR SONIDO
        }
    }

    #region CoinAnimation
    void CoinUpdate()
    {
        StartCoroutine(ScaleCoins());
    }

    IEnumerator ScaleCoins()
    {
        Coroutine text = StartCoroutine(ScaleText());
        Coroutine coins = StartCoroutine(ScaleCoinGO());

        yield return text;
        yield return coins;
    }

    IEnumerator ScaleCoinGO()
    {
        float t = 0f;
        Vector3 _originalScale = coinsGameObject.transform.localScale;
        Vector3 bigScale = _originalScale * 0.5f;

        while (t < 1f)
        {
            t += Time.deltaTime / 0.1f;
            coinsGameObject.transform.localScale = Vector3.Lerp(_originalScale, bigScale, EaseOut(t));
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.1f;
            coinsGameObject.transform.localScale = Vector3.Lerp(bigScale, _originalScale, EaseIn(t));
            yield return null;
        }

        coinsGameObject.transform.localScale = _originalScale;
    }
    IEnumerator ScaleText()
    {
        float t = 0f;
        int _originalScale = coinsText.fontSize;
        float bigScale = _originalScale * 0.5f;

        while (t < 1f)
        {
            t += Time.deltaTime / 0.1f;
            coinsText.fontSize = (int)Mathf.Lerp(_originalScale, bigScale, EaseOut(t));
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.1f;
            coinsText.fontSize = (int)Mathf.Lerp(bigScale, _originalScale, EaseIn(t));
            yield return null;
        }

        coinsText.fontSize = _originalScale;
    }
    private float EaseOut(float t) => 1 - Mathf.Pow(1 - t, 3);
    private float EaseIn(float t) => t * t * t;
    #endregion

    #endregion



    IEnumerator ShowResult(DayResult playerWins)
    {
        Time.timeScale = 0.5f;
        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 1f;
        if (playerWins == DayResult.ExactWin || playerWins == DayResult.Stand)
            gameResult.ShowWin(() => EndRound(playerWins));
        else
            gameResult.ShowLose(() => EndRound(playerWins));
    }
}
public static class EnumExtensions
{
    public static string ToReadable(this System.Enum value)
    {
        return Regex.Replace(value.ToString(), "(\\B[A-Z])", " $1");
    }
}
