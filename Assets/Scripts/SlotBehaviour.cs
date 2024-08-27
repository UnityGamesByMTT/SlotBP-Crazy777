using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System;

public class SlotBehaviour : MonoBehaviour
{
    #region VARIABLES_AND_REFERENCES

    [Header("Audio Controller...")]
    [SerializeField]
    private AudioController audioController;

    [Header("SockerManager")]
    [SerializeField]
    private SocketIOManager SocketManager;

    [Header("UI Manager")]
    [SerializeField]
    private UIManager m_UIManager;

    [Header("Key")]
    private KeyStruct m_Key;


    #region Arrays
    [Header("Sprites")]
    [SerializeField]
    private Sprite[] myImages;

    [Header("Slots Objects")]
    [SerializeField]
    private GameObject[] Slot_Objects;

    [Header("Slots Transforms")]
    [SerializeField]
    private Transform[] Slot_Transform;
    #endregion


    #region Lists
    [Header("Slot Images")]
    [SerializeField]
    private List<SlotImage> images;

    [SerializeField]
    private List<SlotImage> Tempimages;

    [Header("Tweeners")]
    [SerializeField]
    private List<Tweener> alltweens = new List<Tweener>();

    [Header("Image Animations")]
    [SerializeField]
    private List<ImageAnimation> TempList;
    #endregion


    #region Booleans
    private bool IsAutoSpin = false;
    private bool IsFreeSpin = false;
    private bool IsSpinning = false;
    internal bool CheckPopups = false;
    #endregion

    #region Numbers
    private int BetCounter = 0;
    private double currentBalance = 0;
    private double currentTotalBet = 0;
    int tweenHeight = 0;
    int Lines = 1;

    [SerializeField]
    private int numberOfSlots = 4;
    [SerializeField]
    private int IconSizeFactor = 100;
    [SerializeField]
    private int SpaceFactor = 0;
    [SerializeField]
    int verticalVisibility = 3;
    #endregion

    #region Coroutines
    Coroutine AutoSpinRoutine = null;
    Coroutine tweenroutine = null;
    Coroutine FreeSpinRoutine = null;
    #endregion

    #endregion

    private void Awake()
    {
        m_Key = new KeyStruct();
    }

    private void Start()
    {
        InitiateButtons();
        tweenHeight = (myImages.Length * IconSizeFactor) - 280;
    }

    private void InitiateButtons()
    {
        m_UIManager.GetButton(m_Key.m_button_spin).onClick.RemoveAllListeners();
        m_UIManager.GetButton(m_Key.m_button_spin).onClick.AddListener(delegate { StartSlots(); });

        m_UIManager.GetButton(m_Key.m_button_bet_button).onClick.RemoveAllListeners();
        m_UIManager.GetButton(m_Key.m_button_bet_button).onClick.AddListener(OnBetOne);

        m_UIManager.GetButton(m_Key.m_button_auto_spin).onClick.RemoveAllListeners();
        m_UIManager.GetButton(m_Key.m_button_auto_spin).onClick.AddListener(AutoSpin);

        m_UIManager.GetButton(m_Key.m_button_auto_spin_stop).onClick.RemoveAllListeners();
        m_UIManager.GetButton(m_Key.m_button_auto_spin_stop).onClick.AddListener(StopAutoSpin);
    }

    internal void SetInitialUI()
    {
        BetCounter = SocketManager.initialData.Bets.Count - 1;

        m_UIManager.GetText(m_Key.m_text_bet_amount).text = SocketManager.initialData.Bets[BetCounter].ToString();

        //TotalBet_text.text = (SocketManager.initialData.Bets[BetCounter] * Lines).ToString(); //To Be Implemented In Future

        m_UIManager.GetText(m_Key.m_text_win_amount).text = "0.00";

        m_UIManager.GetText(m_Key.m_text_balance_amount).text = SocketManager.playerdata.Balance.ToString("f2");

        currentBalance = SocketManager.playerdata.Balance;
        currentTotalBet = SocketManager.initialData.Bets[BetCounter] * Lines;

        //uiManager.InitialiseUIData(SocketManager.initUIData.AbtLogo.link, SocketManager.initUIData.AbtLogo.logoSprite, SocketManager.initUIData.ToULink, SocketManager.initUIData.PopLink, SocketManager.initUIData.paylines);
        //PayCalculator.LineList = SocketManager.initialData.LinesCount;

        CompareBalance();
    }

    internal void shuffleInitialMatrix()
    {
        for (int i = 0; i < Tempimages.Count; i++)
        {
            for (int j = 0; j < Tempimages[i].slotImages.Count; j++)
            {
                int randomIndex = UnityEngine.Random.Range(0, myImages.Length);
                Tempimages[i].slotImages[j].sprite = myImages[randomIndex];
            }
        }
    }

    private void StartSlots(bool autoSpin = false)
    {
        if (audioController) audioController.PlaySpinButtonAudio();
        //if (gambleController) gambleController.toggleDoubleButton(false);
        //gambleController.GambleTweeningAnim(false);
        if (!autoSpin)
        {
            if (AutoSpinRoutine != null)
            {
                StopCoroutine(AutoSpinRoutine);
                StopCoroutine(tweenroutine);
                tweenroutine = null;
                AutoSpinRoutine = null;
            }

        }
        //PayCalculator.DontDestroyLines.Clear();

        if (TempList.Count > 0)
        {
            StopGameAnimation();
        }
        //PayCalculator.ResetStaticLine();
        tweenroutine = StartCoroutine(TweenRoutine());
    }

    private void AutoSpin()
    {
        if (audioController) audioController.PlaySpinButtonAudio();
        if (!IsAutoSpin)
        {
            IsAutoSpin = true;
            m_UIManager.GetButton(m_Key.m_button_auto_spin_stop).gameObject.SetActive(true);
            m_UIManager.GetButton(m_Key.m_button_auto_spin).gameObject.SetActive(false);

            if (AutoSpinRoutine != null)
            {
                StopCoroutine(AutoSpinRoutine);
                AutoSpinRoutine = null;
            }
            AutoSpinRoutine = StartCoroutine(AutoSpinCoroutine());
        }
    }

    private void StopAutoSpin()
    {
        if (audioController) audioController.PlaySpinButtonAudio();
        if (IsAutoSpin)
        {
            IsAutoSpin = false;
            m_UIManager.GetButton(m_Key.m_button_auto_spin_stop).gameObject.SetActive(false);
            m_UIManager.GetButton(m_Key.m_button_auto_spin).gameObject.SetActive(true);
            StartCoroutine(StopAutoSpinCoroutine());
        }
    }

    private IEnumerator AutoSpinCoroutine()
    {

        while (IsAutoSpin)
        {
            StartSlots(IsAutoSpin);
            yield return tweenroutine;
        }
    }

    private IEnumerator StopAutoSpinCoroutine()
    {
        yield return new WaitUntil(() => !IsSpinning);
        ToggleButtonGrp(true);
        if (AutoSpinRoutine != null || tweenroutine != null)
        {
            StopCoroutine(AutoSpinRoutine);
            StopCoroutine(tweenroutine);
            tweenroutine = null;
            AutoSpinRoutine = null;
            StopCoroutine(StopAutoSpinCoroutine());
        }
    }

    private void CompareBalance()
    {
        if (currentBalance < currentTotalBet)
        {
            //uiManager.LowBalPopup();
            m_UIManager.GetButton(m_Key.m_button_auto_spin).interactable = false;
            m_UIManager.GetButton(m_Key.m_button_spin).interactable = false;
        }
        else
        {
            m_UIManager.GetButton(m_Key.m_button_auto_spin).interactable = true;
            m_UIManager.GetButton(m_Key.m_button_spin).interactable = true;
        }
    }

    void OnBetOne()
    {
        if (audioController) audioController.PlayButtonAudio();

        if (BetCounter < SocketManager.initialData.Bets.Count - 1)
        {
            BetCounter++;
        }
        else
        {
            BetCounter = 0;
        }
        m_UIManager.GetText(m_Key.m_text_bet_amount).text = SocketManager.initialData.Bets[BetCounter].ToString();
        //if (TotalBet_text) TotalBet_text.text = (SocketManager.initialData.Bets[BetCounter] * Lines).ToString(); // To Be Implemented
        currentTotalBet = SocketManager.initialData.Bets[BetCounter] * Lines;
        CompareBalance();
    }

    internal void LayoutReset(int number)
    {
        //if (Slot_Elements[number]) Slot_Elements[number].ignoreLayout = true;
        m_UIManager.GetButton(m_Key.m_button_spin).interactable = true;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            if (!IsSpinning)
            {
                if (audioController) audioController.StopWLAaudio();
            }
        }
    }

    private IEnumerator TweenRoutine()
    {
        //gambleController.GambleTweeningAnim(false);
        //TriggerWinImageAnimation(false);
        if (currentBalance < currentTotalBet && !IsFreeSpin)
        {
            CompareBalance();
            StopAutoSpin();
            yield return new WaitForSeconds(1);
            yield break;
        }
        if (audioController) audioController.PlayWLAudio("spin");
        IsSpinning = true;
        ToggleButtonGrp(false);
        for (int i = 0; i < numberOfSlots; i++)
        {
            InitializeTweening(Slot_Transform[i]);
            yield return new WaitForSeconds(0.1f);
        }

        if (!IsFreeSpin)
        {
            double bet = 0;
            double balance = 0;
            try
            {
                //bet = double.Parse(TotalBet_text.text); // To Be Implemented
            }
            catch (Exception e)
            {
                Debug.Log("Error while conversion " + e.Message);
            }

            try
            {
                balance = double.Parse(m_UIManager.GetText(m_Key.m_text_balance_amount).text);
            }
            catch (Exception e)
            {
                Debug.Log("Error while conversion " + e.Message);
            }
            double initAmount = balance;

            balance = balance - bet;

            DOTween.To(() => initAmount, (val) => initAmount = val, balance, 0.8f).OnUpdate(() =>
            {
                m_UIManager.GetText(m_Key.m_text_balance_amount).text = initAmount.ToString("f2");
            });
        }
        SocketManager.AccumulateResult(BetCounter);
        print("before result");
        yield return new WaitUntil(() => SocketManager.isResultdone);

        for (int j = 0; j < SocketManager.resultData.ResultReel.Count; j++)
        {
            List<int> resultnum = SocketManager.resultData.FinalResultReel[j]?.Split(',')?.Select(Int32.Parse)?.ToList();
            for (int i = 0; i < 4; i++)
            {
                if (images[i].slotImages[images[i].slotImages.Count - 4 + j])
                    images[i].slotImages[images[i].slotImages.Count - 4 + j].sprite = myImages[resultnum[i]];
            }
        }

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < numberOfSlots; i++)
        {
            yield return StopTweening(5, Slot_Transform[i], i);
        }

        yield return new WaitForSeconds(0.3f);

        CheckPayoutLineBackend(SocketManager.resultData.linesToEmit, SocketManager.resultData.FinalsymbolsToEmit, SocketManager.resultData.jackpot);
        KillAllTweens();

        CheckPopups = true;

        m_UIManager.GetText(m_Key.m_text_win_amount).text = SocketManager.playerdata.currentWining.ToString("f2");
        m_UIManager.GetText(m_Key.m_text_balance_amount).text = SocketManager.playerdata.Balance.ToString("f2");
        if (SocketManager.resultData.isBonus)
        {
            CheckBonusGame();
        }
        else
        {
            CheckWinPopups();
        }

        yield return new WaitUntil(() => !CheckPopups);
        if (!IsAutoSpin)
        {
            ActivateGamble();
            ToggleButtonGrp(true);
            IsSpinning = false;
        }
        else
        {
            ActivateGamble();
            yield return new WaitForSeconds(2f);
            IsSpinning = false;
        }
        //if (SocketManager.resultData.freeSpins > 0  && !IsFreeSpin)
        //{
        //    uiManager.FreeSpinProcess((int)SocketManager.resultData.freeSpins);
        //}
    }

    private void ActivateGamble()
    {
        if (SocketManager.playerdata.currentWining > 0 && SocketManager.playerdata.currentWining <= SocketManager.GambleLimit)
        {
            //gambleController.GambleTweeningAnim(true);
            //gambleController.toggleDoubleButton(true);
            IsSpinning = false;
        }
    }

    internal void DeactivateGamble()
    {
        StopAutoSpin();
    }

    internal void GambleCollect()
    {
        SocketManager.GambleCollectCall();
    }

    internal void CheckWinPopups()
    {
        if (SocketManager.resultData.WinAmout >= currentTotalBet * 10 && SocketManager.resultData.WinAmout < currentTotalBet * 15)
        {
            //uiManager.PopulateWin(1, SocketManager.resultData.WinAmout);
        }
        else if (SocketManager.resultData.WinAmout >= currentTotalBet * 15 && SocketManager.resultData.WinAmout < currentTotalBet * 20)
        {
            //uiManager.PopulateWin(2, SocketManager.resultData.WinAmout);
        }
        else if (SocketManager.resultData.WinAmout >= currentTotalBet * 20)
        {
            //uiManager.PopulateWin(3, SocketManager.resultData.WinAmout);
        }
        else
        {
            CheckPopups = false;
        }
    }

    internal void CheckBonusGame()
    {
        //bonusController.maxBreakCount = SocketManager.resultData.BonusResult.Count;
        //bonusController.StartBonus(SocketManager.resultData.BonusResult);
    }

    void ToggleButtonGrp(bool toggle)
    {

        m_UIManager.GetButton(m_Key.m_button_spin).interactable = toggle;
        m_UIManager.GetButton(m_Key.m_button_bet_button).interactable = toggle;
        //if (MaxBet_Button) MaxBet_Button.interactable = toggle;
        //if (Double_button) Double_button.interactable = toggle;
        m_UIManager.GetButton(m_Key.m_button_auto_spin).interactable = toggle;

    }

    internal void updateBalance()
    {
        m_UIManager.GetText(m_Key.m_text_balance_amount).text = SocketManager.playerdata.Balance.ToString("f2");
        m_UIManager.GetText(m_Key.m_text_win_amount).text = SocketManager.playerdata.currentWining.ToString("f2");
    }

    private void StartGameAnimation(GameObject animObjects)
    {
        //if (animObjects.transform.GetComponent<ImageAnimation>().isActiveAndEnabled)
        //{

        animObjects.transform.GetChild(0).gameObject.SetActive(true);
        animObjects.transform.GetChild(1).gameObject.SetActive(true);
        //}

        ImageAnimation temp = animObjects.transform.GetChild(0).GetComponent<ImageAnimation>();

        temp.StartAnimation();
        TempList.Add(temp);
    }

    private void StopGameAnimation()
    {
        for (int i = 0; i < TempList.Count; i++)
        {
            TempList[i].StopAnimation();
            if (TempList[i].transform.parent.childCount > 0)
                TempList[i].transform.parent.GetChild(1).gameObject.SetActive(false);
        }
        TempList.Clear();
        TempList.TrimExcess();
    }

    internal void CallCloseSocket()
    {
        SocketManager.CloseSocket();
    }

    private void CheckPayoutLineBackend(List<int> LineId, List<string> points_AnimString, double jackpot = 0)
    {
        List<int> points_anim = null;
        if (LineId.Count > 0)
        {
            if (audioController) audioController.PlayWLAudio("win");

            //TriggerWinImageAnimation(true);

            for (int i = 0; i < LineId.Count; i++)
            {
                //PayCalculator.DontDestroyLines.Add(LineId[i]);
                //PayCalculator.GeneratePayoutLinesBackend(LineId[i]);
            }

            if (jackpot > 0)
            {
                for (int i = 0; i < Tempimages.Count; i++)
                {
                    for (int k = 0; k < Tempimages[i].slotImages.Count; k++)
                    {
                        StartGameAnimation(Tempimages[i].slotImages[k].gameObject);
                    }
                }
            }
            else
            {
                for (int i = 0; i < points_AnimString.Count; i++)
                {
                    points_anim = points_AnimString[i]?.Split(',')?.Select(Int32.Parse)?.ToList();

                    for (int k = 0; k < points_anim.Count; k++)
                    {
                        print(points_anim.Count);
                        if (points_anim[k] >= 10)
                        {
                            StartGameAnimation(Tempimages[(points_anim[k] / 10) % 10].slotImages[points_anim[k] % 10].gameObject);
                        }
                        else
                        {
                            StartGameAnimation(Tempimages[0].slotImages[points_anim[k]].gameObject);
                        }
                    }
                }
            }
        }
        else
        {

            if (audioController) audioController.StopWLAaudio();
        }
    }

    #region TweeningCode
    private void InitializeTweening(Transform slotTransform)
    {
        slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, 0);
        Tweener tweener = slotTransform.DOLocalMoveY(-tweenHeight, 0.2f).SetLoops(-1, LoopType.Restart).SetDelay(0);
        tweener.Play();
        alltweens.Add(tweener);
    }

    private IEnumerator StopTweening(int reqpos, Transform slotTransform, int index)
    {
        alltweens[index].Pause();
        int tweenpos = (reqpos * (IconSizeFactor + SpaceFactor)) - (IconSizeFactor + (2 * SpaceFactor));
        alltweens[index] = slotTransform.DOLocalMoveY(-tweenpos + 100 + (SpaceFactor > 0 ? SpaceFactor / 4 : 0), 0.5f).SetEase(Ease.OutElastic);
        yield return new WaitForSeconds(0.2f);
    }

    //private void TriggerWinImageAnimation(bool _config_tf)
    //{
    //    if (_config_tf)
    //    {
    //        Your_Win_GameObject.GetComponent<ImageAnimation>().StartAnimation();
    //    }
    //    else
    //    {
    //        Your_Win_GameObject.GetComponent<ImageAnimation>().StopAnimation();
    //    }
    //}

    private void KillAllTweens()
    {
        for (int i = 0; i < numberOfSlots; i++)
        {
            alltweens[i].Kill();
        }
        alltweens.Clear();
    }
    #endregion
}
