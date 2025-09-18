using System;
using System.Linq;
using UnityEngine;

public enum CURRENT_SCREEN
{
    STANDBY,
    //REGISTRATION, //REMOVED REGISTRATION 
    PICTURE_MODE,
    RETAKE_MODE,
    FINAL
};

[Serializable]
public class LedUIObjects
{
    public string name;
    public GameObject[] display1Objs;//led
    public GameObject[] display2Objs;//touchscreen

    //public void HideObjects()
    //{
    //    var allDisplayObjs = display1Objs.Concat(display2Objs);
    //    foreach (var obj in allDisplayObjs)
    //    {
    //        obj.gameObject.SetActive(false);
    //    }
    //}

    public void SetActive(bool p_show)
    {
        var allDisplayObjs = display1Objs.Concat(display2Objs);
        foreach (var obj in allDisplayObjs)
        {
            obj.gameObject.SetActive(p_show);
        }
    }
}

public class HUDManager : MonoBehaviour
{
    private static HUDManager _instance;
    public static HUDManager GetInstance() => _instance;

    private CURRENT_SCREEN _currentScreen = CURRENT_SCREEN.STANDBY;

    public CURRENT_SCREEN CurrentScreen
    {
        get { return _currentScreen; }
        set
        {
            if (_currentScreen != value)
            {
                _currentScreen = value;
                OnScreenChange?.Invoke(_currentScreen);
                Debug.Log($"ScreenChanged Invoked");
            }
        }
    }

    public AnimationManager animationManager;
    public ScreenSaverAnimation screenSaverManager;
    public LedUIObjects[] displayUIObjs;
    public GameObject ypoSpriteMask;
    [SerializeField] GameObject photoEnvironmentParent;
    public CURRENT_SCREEN defaultScreenStartup = CURRENT_SCREEN.STANDBY;

    [Space]
    //public GameObject finalImage;
    //public GameObject finalImage2; 
    public GameObject[] finalImages;//touchscreen

    [Header("Debug")]
    public bool showDebugger = true;
    public int debugButtonHeight = 150;

    public Rect standbyButtonRect = new Rect();
    public Rect registrationButtonRect = new Rect();
    public Rect selfieButtonRect = new Rect();
    public Rect finalButtonRect = new Rect();
    public Vector2 debugButtonPosition = new Vector2(50, 50);
    public Vector2 debugButtonSize = new Vector2(50, 50);

    public Action<CURRENT_SCREEN> OnScreenChange;

    public void ShowYPOMask(bool p_showMask)
    {
        ypoSpriteMask.SetActive(p_showMask);
    }

    public void ChangeUIState(CURRENT_SCREEN p_screen)
    {
        CurrentScreen = p_screen;
    }

    #region LifeCycles

    public void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {

#if !UNITY_EDITOR
    showDebugger = false;
#endif


        SwitchScreen((int)defaultScreenStartup);
        //uiObjs[0].SetActive(true);
        ShowEnvironmentObjects(false);
        ShowFinalPreviewImages(false);
        ShowYPOMask(false);
        OnScreenChange += OnScreenChanged;
    }

    #endregion

    #region ChangeScreens

    public void OnScreenChanged(CURRENT_SCREEN p_currentScreen)
    {
        switch (p_currentScreen)
        {
            case CURRENT_SCREEN.STANDBY:
                ShowEnvironmentObjects(false);
                ShowFinalPreviewImages(false);
                SwitchScreen(0);
                break;
            //case CURRENT_SCREEN.REGISTRATION:
            //    SwitchScreen(1);
            //    break;
            case CURRENT_SCREEN.PICTURE_MODE:
                ShowEnvironmentObjects(true);
                ShowFinalPreviewImages(false);
                SwitchScreen(1);
                break;
            case CURRENT_SCREEN.RETAKE_MODE:
                //ShowEnvironmentObjects(true);
                ShowFinalPreviewImages();
                SwitchScreen(2);
                break;
            case CURRENT_SCREEN.FINAL:
                //ShowEnvironmentObjects(true);
                ShowFinalPreviewImages();
                SwitchScreen(3);
                break;
            default:
                break;
        }
        Debug.Log($"ScreenChanged Invoked 2");
    }

    public void SwitchScreen(int p_index)
    {
        try
        {
            HidePanels();
            CurrentScreen = (CURRENT_SCREEN)Enum.ToObject(typeof(CURRENT_SCREEN), p_index);
            //uiObjs[p_index].SetActive(true);
            displayUIObjs[p_index].SetActive(true);
        }
        catch (Exception e)
        {

            Debug.LogError($"Error at Index {p_index}: \n {e.Message} ");
        }
    }

    public void ShowScreen(CURRENT_SCREEN screen, int uiObjIndex)
    {
        CurrentScreen = screen;
        HidePanels();

        //finalImage.SetActive(false);
        //finalImage2.SetActive(false);
        //ShowFinalPreviewImages(false);
        //uiObjs[uiObjIndex].SetActive(true);
    }

    public void ShowScreen(CURRENT_SCREEN screen)
    {
        CurrentScreen = screen;
        HidePanels();

        displayUIObjs[(int)screen].SetActive(true);
        //ShowFinalPreviewImages(trye);
        //finalImage.SetActive(false);
        //finalImage2.SetActive(false);
    }

    public void ShowFinalPreviewImages(bool p_show = true)
    {
        foreach (var item in finalImages)
        {
            item.SetActive(p_show);
        }
    }

    #endregion

    public void HidePanels()
    {
        foreach (var item in displayUIObjs)
        {
            item.SetActive(false);
        }
    }

    public void ShowEnvironmentObjects(bool p_show)
    {
        photoEnvironmentParent.SetActive(p_show);
        animationManager.HideImages();
    }

    #region OnDebug

    void OnGUI()
    {
        if (DebugBehaviour.debugModeOn)
        {
            debugButtonPosition.y = debugButtonHeight;

            if (GUI.Button(new Rect(debugButtonPosition.x, debugButtonPosition.y += 50, debugButtonSize.x, debugButtonSize.y), "Go To Standby"))
            {
                CurrentScreen = CURRENT_SCREEN.STANDBY;
                //SwitchScreen(0);
                SwitchScreen((int)CURRENT_SCREEN.STANDBY);
            }
            //if (GUI.Button(new Rect(debugButtonPosition.x, debugButtonPosition.y += 50, debugButtonSize.x, debugButtonSize.y), "Go To Registration"))
            //{
            //    CurrentScreen = CURRENT_SCREEN.REGISTRATION;
            //    SwitchScreen(1);
            //}
            if (GUI.Button(new Rect(debugButtonPosition.x, debugButtonPosition.y += 50, debugButtonSize.x, debugButtonSize.y), "Go To Take Picture"))
            {
                CurrentScreen = CURRENT_SCREEN.PICTURE_MODE;
                //SwitchScreen(2);
                SwitchScreen((int)CURRENT_SCREEN.PICTURE_MODE);
            }
            if (GUI.Button(new Rect(debugButtonPosition.x, debugButtonPosition.y += 50, debugButtonSize.x, debugButtonSize.y), "Go To Retake"))
            {
                CurrentScreen = CURRENT_SCREEN.RETAKE_MODE;
                SwitchScreen((int)CURRENT_SCREEN.RETAKE_MODE);
            }
            if (GUI.Button(new Rect(debugButtonPosition.x, debugButtonPosition.y += 50, debugButtonSize.x, debugButtonSize.y), "Go To Final"))
            {
                CurrentScreen = CURRENT_SCREEN.FINAL;
                SwitchScreen((int)CURRENT_SCREEN.FINAL);
            }
        }
    }

    #endregion
}