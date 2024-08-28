using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour
{
    #region EVENTS_AND_DELEGATES
    internal event Action OnSpinClicked;
    internal event Action OnAutoSpinClicked;
    internal event Action OnSettingsButtonClicked;
    internal event Action OnBetButtonClicked;
    internal event Action OnSpinSuccess;
    internal event Action OnBonusReceived;
    internal event Action OnFreeSpinReceived;
    #endregion

    private void Start()
    {
        SubscribeToEvents();
    }

    private void SpinClickedAction() { }

    private void AutoSpinClickedAction() { }

    private void SettingsClickedAction() { }

    private void BetButtonClickedAction() { }

    private void SpinSuccessAction() { }

    private void BonusSuccessAction() { }

    private void FreeSpinReceivedAction() { }

    internal void SubscribeToEvents()
    {
        OnSpinClicked += delegate { SpinClickedAction(); };
        OnAutoSpinClicked += delegate { AutoSpinClickedAction(); };
        OnSettingsButtonClicked += delegate { SettingsClickedAction(); };
        OnBetButtonClicked += delegate { BetButtonClickedAction(); };
        OnSpinSuccess += delegate { SpinSuccessAction(); };
        OnBonusReceived += delegate { BonusSuccessAction(); };
        OnFreeSpinReceived += delegate { FreeSpinReceivedAction(); };
    }

    internal void UnSubscribeToEvents()
    {
        OnSpinClicked -= delegate { SpinClickedAction(); };
        OnAutoSpinClicked -= delegate { AutoSpinClickedAction(); };
        OnSettingsButtonClicked -= delegate { SettingsClickedAction(); };
        OnBetButtonClicked -= delegate { BetButtonClickedAction(); };
        OnSpinSuccess -= delegate { SpinSuccessAction(); };
        OnBonusReceived -= delegate { BonusSuccessAction(); };
        OnFreeSpinReceived -= delegate { FreeSpinReceivedAction(); };
    }

    internal void ResetAllEvents()
    {
        UnSubscribeToEvents();
    }
}

[System.Serializable]
public struct SlotImage
{
    public List<Image> slotImages;
}