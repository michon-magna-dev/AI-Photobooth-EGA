using System;
using UnityEditor;
using UnityEngine;

public class TouchDetector : MonoBehaviour
{
    public float m_widthSelectionRange = 300;
    public float m_xLeftLimit;
    public float m_xRightLimit;

    float _screenWidth;
    float _screenHeight;

    public Action OnPressLeft;
    public Action OnPressRight;

    public bool showDebug = false;
    private void OnValidate()
    {
        InitScreenSettings();
    }
    // Start is called before the first frame update
    void Start()
    {

#if !UNITY_EDITOR
    showDebug = false;
#endif


        InitScreenSettings();
    }

    void InitScreenSettings()
    {
        _screenWidth = Screen.width;
        _screenHeight = Screen.height;

        m_xLeftLimit = m_widthSelectionRange;
        m_xRightLimit = _screenWidth - m_widthSelectionRange;

    }

    // Update is called once per frame
    void Update()
    {
        var mousePosition = Input.mousePosition;
        if (Input.GetMouseButtonUp(0))
        {
            //Debug.Log(mousePosition);
            if (mousePosition.x < m_xLeftLimit && m_xLeftLimit > 0)
            {
                Debug.Log($"isLeft");
                OnPressLeft?.Invoke();
            }
            else if (mousePosition.x > m_xRightLimit)
            {
                Debug.Log($"is Right");
                OnPressRight?.Invoke();
            }
        }
    }

    private void OnGUI()
    {

        if (!showDebug)
        {
            return;
        }
#if UNITY_EDITOR
        Handles.BeginGUI();
        // Calculate the rect for the box
        Rect leftBoxRect = new Rect(0, 0, m_xLeftLimit, _screenHeight);
        Rect rightBoxRect = new Rect(m_xRightLimit, 0, m_widthSelectionRange, _screenHeight);

        // Draw the GUI box
        GUI.Box(leftBoxRect, "Left Box");
        GUI.Box(rightBoxRect, "Right Box");

        Handles.EndGUI();
#endif


    }

}