using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideMouse : MonoBehaviour
{
    bool isMouseVisible = false;
    public GameObject m_visualizer;

    void Start()
    {
#if UNITY_EDITOR
        Cursor.visible = true;
#else
        Cursor.visible = false;
#endif
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            isMouseVisible = !isMouseVisible;
            Cursor.visible = isMouseVisible;
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            m_visualizer.SetActive(!m_visualizer.activeInHierarchy);
        }
    }
}
