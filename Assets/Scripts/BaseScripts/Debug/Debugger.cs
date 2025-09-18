using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugger : MonoBehaviour
{
    private static Debugger _instance;

    public static Debugger GetInstance()=>_instance;

    [Header("Debug")]
    [SerializeField] bool enableDebug = false;
    [SerializeField] bool enableScaler = false;

    [SerializeField] GameObject m_pivot;
    [SerializeField] float m_scaleAmount=.3f;

    [SerializeField] static int BUTTON_PRESS_SENSITIVITY = 8;

    private void Awake()
    {
        if (_instance != null)
            Destroy(this);
        else
            _instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F10))
        {
            enableScaler = !enableScaler;
        }
       
        if (Input.GetKeyDown(KeyCode.F12))
        {
            enableDebug = !enableDebug;
        }

        if (!enableDebug)
            return;

        //ScaleButton();
        //AspectRatio();
        SetButtonSensitivity();
        SimulateButtonPress();
    }

    private void OnGUI()
    {
        if (enableDebug)
        {
            GUIStyle fontStyle = new GUIStyle();
            fontStyle.normal.background = null;
            fontStyle.normal.textColor = new Color(1, 0, 0);
            fontStyle.fontSize = 40;
            GUI.TextArea(new Rect(0, 0, 200, 40), " Debug Mode On", fontStyle);
        }
    }

    private void SimulateButtonPress()
    {
        //if (Input.GetKeyDown(KeyCode.F1))
        //{
        //    ButtonPlaybackBehaviour.GetInstance().OnButtonSelect(0);
        //    TopicButtonManager.GetInstance().SimulatePress(0);
        //}
        //if (Input.GetKeyDown(KeyCode.F2))
        //{
        //    ButtonPlaybackBehaviour.GetInstance().OnButtonSelect(1);
        //    TopicButtonManager.GetInstance().SimulatePress(1);
        //}
        //if (Input.GetKeyDown(KeyCode.F3))
        //{
        //    ButtonPlaybackBehaviour.GetInstance().OnButtonSelect(2);
        //    TopicButtonManager.GetInstance().SimulatePress(2);
        //}
        //if (Input.GetKeyDown(KeyCode.F4))
        //{
        //    ButtonPlaybackBehaviour.GetInstance().OnButtonSelect(3);
        //    TopicButtonManager.GetInstance().SimulatePress(3);
        //}
    }
    private void SetButtonSensitivity()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            BUTTON_PRESS_SENSITIVITY++;
#if UNITY_EDITOR
            Debug.Log(BUTTON_PRESS_SENSITIVITY);
#endif
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            BUTTON_PRESS_SENSITIVITY--;
#if UNITY_EDITOR
            Debug.Log(BUTTON_PRESS_SENSITIVITY);
#endif
        }
    }

    public int GetButtonSensitivity()
    {
        return BUTTON_PRESS_SENSITIVITY;
    }

    public void ScaleButton()
    {
        if (enableScaler)
        {
            if(Input.GetKeyDown(KeyCode.UpArrow))
            {
                m_pivot.transform.localScale = new Vector3(m_pivot.transform.localScale.x + m_scaleAmount, m_pivot.transform.localScale.y + m_scaleAmount, m_pivot.transform.localScale.z + m_scaleAmount);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                m_pivot.transform.localScale = new Vector3(m_pivot.transform.localScale.x - m_scaleAmount, m_pivot.transform.localScale.y - m_scaleAmount, m_pivot.transform.localScale.z - m_scaleAmount);
            }
        }
    }

}