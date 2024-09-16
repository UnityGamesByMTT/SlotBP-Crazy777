using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    #region EVENTS_AND_DELEGATES
    internal event Action OnSpinClicked;
    internal event Action OnAutoSpinClicked;
    internal event Action OnAutoSpinStopClicked;
    internal event Action OnBetButtonClicked;
    internal event Action OnSpinSuccess;
    internal event Action OnBonusReceived;
    internal event Action OnFreeSpinReceived;
    internal event Action OnFreeSpinEnded;
    #endregion

    #region MANAGER_REFERENCES
    [SerializeField]
    private UIManager m_UIManager;
    [SerializeField]
    private SlotBehaviour m_SlotBehaviour;
    [SerializeField]
    private SocketIOManager m_SocketManager;
    #endregion

    #region SERIALIZED_BOOLEANS
    [SerializeField]
    protected internal bool TurboSpin = false;
    [SerializeField]
    protected internal bool DemoFreeSpin = false;
    #endregion

    #region SERIALIZED_INTEGERS
    [SerializeField] internal int StopPos_Plus = 50;
    [SerializeField] internal int AutoSpin_Count = 10;
    #endregion

    #region SERIALIZED_ARRAYS_AND_LISTS
    [SerializeField]
    private List<GameObject> m_Initial_Animation;
    #endregion

    private Coroutine M_Initial_Animation = null;
    private bool m_SettingsClicked = false;

    private KeyStruct m_Key;

    private void OnEnable()
    {
        OnFreeSpinReceived += FreeSpinAction;
        OnFreeSpinEnded += FreeSpinStopAction;
    }

    private void Awake()
    {
        m_Key = new KeyStruct();
        M_Initial_Animation = StartCoroutine(InitialAnimation());
    }

    private void Start()
    {
        InitialSetup();

        InitiateButtons();
    }

    private void InitialSetup()
    {
        m_UIManager.GetText(m_Key.m_text_total_auto_spin).text = AutoSpin_Count.ToString();
    }

    private void InitiateButtons()
    {
        m_UIManager.GetButton(m_Key.m_button_spin).onClick.RemoveAllListeners();
        m_UIManager.GetButton(m_Key.m_button_spin).onClick.AddListener(delegate { OnSpinClicked?.Invoke(); });

        m_UIManager.GetButton(m_Key.m_button_bet_button).onClick.RemoveAllListeners();
        m_UIManager.GetButton(m_Key.m_button_bet_button).onClick.AddListener(delegate { OnBetButtonClicked?.Invoke(); });

        m_UIManager.GetButton(m_Key.m_button_auto_spin).onClick.RemoveAllListeners();
        m_UIManager.GetButton(m_Key.m_button_auto_spin).onClick.AddListener(delegate { OnAutoSpinClicked?.Invoke(); });

        m_UIManager.GetButton(m_Key.m_button_auto_spin_stop).onClick.RemoveAllListeners();
        m_UIManager.GetButton(m_Key.m_button_auto_spin_stop).onClick.AddListener(delegate { OnAutoSpinStopClicked?.Invoke(); });

        m_UIManager.GetButton(m_Key.m_button_turbo_spin).onClick.RemoveAllListeners();
        m_UIManager.GetButton(m_Key.m_button_turbo_spin).onClick.AddListener(TurboSpinClickedAction);

        m_UIManager.GetButton(m_Key.m_button_turbo_spin_stop).onClick.RemoveAllListeners();
        m_UIManager.GetButton(m_Key.m_button_turbo_spin_stop).onClick.AddListener(TurboSpinClickedAction);

        m_UIManager.GetButton(m_Key.m_button_settings).onClick.RemoveAllListeners();
        m_UIManager.GetButton(m_Key.m_button_settings).onClick.AddListener(SettingsButtonClickedAction);

        m_UIManager.GetButton(m_Key.m_button_music).onClick.RemoveAllListeners();
        m_UIManager.GetButton(m_Key.m_button_music).onClick.AddListener(OnSettingButtonClicked);

        m_UIManager.GetButton(m_Key.m_button_info).onClick.RemoveAllListeners();
        m_UIManager.GetButton(m_Key.m_button_info).onClick.AddListener(OnInfoButtonClicked);

        m_UIManager.GetButton(m_Key.m_button_music_exit).onClick.RemoveAllListeners();
        m_UIManager.GetButton(m_Key.m_button_music_exit).onClick.AddListener(delegate { ExitPopup("music"); });

        m_UIManager.GetButton(m_Key.m_button_info_exit).onClick.RemoveAllListeners();
        m_UIManager.GetButton(m_Key.m_button_info_exit).onClick.AddListener(delegate { ExitPopup("info"); });

        m_UIManager.GetButton(m_Key.m_auto_spin_setting_done).onClick.RemoveAllListeners();
        m_UIManager.GetButton(m_Key.m_auto_spin_setting_done).onClick.AddListener(AutoSpinSettingsConfig);

        m_UIManager.GetButton(m_Key.m_button_auto_spin_plus).onClick.RemoveAllListeners();
        m_UIManager.GetButton(m_Key.m_button_auto_spin_plus).onClick.AddListener(delegate { AutoSpinPlusMinus(true); });

        m_UIManager.GetButton(m_Key.m_button_auto_spin_minus).onClick.RemoveAllListeners();
        m_UIManager.GetButton(m_Key.m_button_auto_spin_minus).onClick.AddListener(delegate { AutoSpinPlusMinus(false); });

        m_UIManager.GetButton(m_Key.m_button_max_auto_spin).onClick.RemoveAllListeners();
        m_UIManager.GetButton(m_Key.m_button_max_auto_spin).onClick.AddListener(AutoSpinMax);

        m_UIManager.GetButton(m_Key.m_button_music_on).onClick.RemoveAllListeners();
        m_UIManager.GetButton(m_Key.m_button_music_on).onClick.AddListener(OnMusicButtonClicked);

        m_UIManager.GetButton(m_Key.m_button_music_off).onClick.RemoveAllListeners();
        m_UIManager.GetButton(m_Key.m_button_music_off).onClick.AddListener(OnMusicButtonClicked);
    }

    private IEnumerator InitialAnimation()
    {
        //m_UIManager.GetGameObject(m_Key.m_object_start_animation_panel).SetActive(true);
        m_UIManager.GetGameObject(m_Key.m_object_game_buttons_panel).SetActive(false);

        //m_Initial_Animation[0].SetActive(true);
        //m_Initial_Animation[0].GetComponent<ImageAnimation>().StartAnimation();
        //m_Initial_Animation[0].transform.DOLocalMoveX(550, 0.8f).SetEase(Ease.Linear);

        yield return new WaitForSeconds(.2f);

        //for(int i = 1; i < m_Initial_Animation.Count; i++)
        //{
        //    GameObject _m = m_Initial_Animation[i];
        //    _m.SetActive(true);
        //    _m.GetComponent<ImageAnimation>().StartAnimation();
        //    _m.transform.DOLocalMoveY(-180, 0.8f).SetEase(Ease.Linear);
        //    yield return new WaitForSeconds(.2f);
        //}

        yield return new WaitForSeconds(1f);

        //foreach(var i in m_Initial_Animation)
        //{
        //    i.SetActive(false);
        //    i.GetComponent<ImageAnimation>().StopAnimation();
        //}

        yield return new WaitForSeconds(.2f);
        yield return new WaitUntil(() => m_SocketManager.isLoaded);

        //m_UIManager.GetGameObject(m_Key.m_object_start_animation_panel).SetActive(false);
        m_UIManager.GetGameObject(m_Key.m_object_game_buttons_panel).SetActive(true);

        StopCoroutine(M_Initial_Animation);
    }

    // This is the method use to trigger and off the turbo spin
    internal void TurboSpinClickedAction()
    {
        if (TurboSpin)
        {
            TurboSpin = !TurboSpin;
            m_UIManager.GetButton(m_Key.m_button_turbo_spin).gameObject.SetActive(true);
            m_UIManager.GetButton(m_Key.m_button_turbo_spin_stop).gameObject.SetActive(false);
        }
        else
        {
            TurboSpin = !TurboSpin;
            m_UIManager.GetButton(m_Key.m_button_turbo_spin).gameObject.SetActive(false);
            m_UIManager.GetButton(m_Key.m_button_turbo_spin_stop).gameObject.SetActive(true);
        }
    }

    private void SettingsButtonClickedAction()
    {
        if (m_SettingsClicked)
        {
            DeanimateInfoMusicButton();
        }
        else
        {
            AnimateInfoMusicButton();
        }
    }

    private void FreeSpinAction()
    {
        m_UIManager.GetGameObject(m_Key.m_object_reel_ref).GetComponent<Image>().sprite = m_UIManager.GetSprite(m_Key.m_sprite_free_spin_reel);
        m_UIManager.GetGameObject(m_Key.m_object_reel_ref).transform.GetChild(1).GetComponent<Image>().color = new Color(255, 255, 0, 255);
    }

    private void FreeSpinStopAction()
    {
        m_UIManager.GetGameObject(m_Key.m_object_reel_ref).GetComponent<Image>().sprite = m_UIManager.GetSprite(m_Key.m_sprite_normal_reel);
        m_UIManager.GetGameObject(m_Key.m_object_reel_ref).transform.GetChild(1).GetComponent<Image>().color = new Color(255, 255, 255, 255);
    }

    internal void InvokeFreeSpin()
    {
        //m_SlotBehaviour.FreeSpin(UnityEngine.Random.Range(1, 6));
        m_SlotBehaviour.FreeSpin(m_SocketManager.resultData.freeSpinCount);
        OnFreeSpinReceived?.Invoke();
    }

    internal void InvokeFreeSpinEnd()
    {
        OnFreeSpinEnded?.Invoke();
    }

    private void OnInfoButtonClicked()
    {
        DeanimateInfoMusicButton();
        m_UIManager.GetGameObject(m_Key.m_object_popup_panel).SetActive(true);
        m_UIManager.GetGameObject(m_Key.m_object_paytable_popup).SetActive(true);
    }

    private void OnSettingButtonClicked()
    {
        DeanimateInfoMusicButton();
        m_UIManager.GetGameObject(m_Key.m_object_popup_panel).SetActive(true);
        m_UIManager.GetGameObject(m_Key.m_object_settings_popup).SetActive(true);
    }

    private void OnDisable()
    {
        OnFreeSpinReceived -= FreeSpinAction;
        OnFreeSpinEnded -= FreeSpinStopAction;
    }

    private void AnimateInfoMusicButton()
    {
        m_UIManager.GetButton(m_Key.m_button_info).GetComponent<RectTransform>().DOLocalMoveY(150f, 0.2f);
        m_UIManager.GetButton(m_Key.m_button_music).GetComponent<RectTransform>().DOLocalMoveY(100f, 0.2f);
        m_UIManager.GetGameObject(m_Key.m_object_music_button_holder).transform.DOLocalMoveY(50f, 0.2f);

        m_SettingsClicked = !m_SettingsClicked;
    }

    private void DeanimateInfoMusicButton()
    {
        m_UIManager.GetButton(m_Key.m_button_info).GetComponent<RectTransform>().DOLocalMoveY(-15f, 0.2f);
        m_UIManager.GetButton(m_Key.m_button_music).GetComponent<RectTransform>().DOLocalMoveY(-15f, 0.2f);
        m_UIManager.GetGameObject(m_Key.m_object_music_button_holder).transform.DOLocalMoveY(-15f, 0.2f);

        m_SettingsClicked = !m_SettingsClicked;
    }

    private void ExitPopup(string m_Config_String)
    {
        switch (m_Config_String)
        {
            case "info":
                m_UIManager.GetGameObject(m_Key.m_object_paytable_popup).SetActive(false);
                m_UIManager.GetGameObject(m_Key.m_object_popup_panel).SetActive(false);
                break;
            case "music":
                m_UIManager.GetGameObject(m_Key.m_object_settings_popup).SetActive(false);
                m_UIManager.GetGameObject(m_Key.m_object_popup_panel).SetActive(false);
                break;
        }
    }

    private void AutoSpinSettingsConfig()
    {
        ExitPopup("music");
    }

    private void AutoSpinPlusMinus(bool m_config)
    {
        AutoSpin_Count = int.Parse(m_UIManager.GetText(m_Key.m_text_total_auto_spin).text);
        if (m_config)
        {
            if(AutoSpin_Count < 10)
            {
                AutoSpin_Count++;
                m_UIManager.GetText(m_Key.m_text_total_auto_spin).text = AutoSpin_Count.ToString();
            }
        }
        else
        {
            if (AutoSpin_Count > 5)
            {
                AutoSpin_Count--;
                m_UIManager.GetText(m_Key.m_text_total_auto_spin).text = AutoSpin_Count.ToString();
            }
        }
    }

    private void OnMusicButtonClicked()
    {
        //DeanimateInfoMusicButton();

        GameObject m_music_obj = m_UIManager.GetButton(m_Key.m_button_music_on).gameObject;
        if (m_music_obj.activeSelf)
        {
            m_music_obj.SetActive(false);
            m_UIManager.GetButton(m_Key.m_button_music_off).gameObject.SetActive(true);
        }
        else
        {
            m_music_obj.SetActive(true);
            m_UIManager.GetButton(m_Key.m_button_music_off).gameObject.SetActive(false);
        }
    }

    private void AutoSpinMax()
    {
        AutoSpin_Count = 10;
        m_UIManager.GetText(m_Key.m_text_total_auto_spin).text = AutoSpin_Count.ToString();
    }
}

[System.Serializable]
public struct SlotImage
{
    public List<Image> slotImages;
}