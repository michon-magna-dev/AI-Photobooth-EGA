using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public enum DisplayMode
{
    Internal,
    External,
    Extend,
    Duplicate
}
public class SetDisplayMode : MonoBehaviour
{
    public DisplayMode m_displayMode = DisplayMode.Extend;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeDisplayMode(m_displayMode);
        }
    }

    private void ChangeDisplayMode(DisplayMode mode)
    {
        var proc = new Process();
        proc.StartInfo.FileName = "DisplaySwitch.exe";
        switch (mode)
        {
            case DisplayMode.External:
                proc.StartInfo.Arguments = "/external";
                break;
            case DisplayMode.Internal:
                proc.StartInfo.Arguments = "/internal";
                break;
            case DisplayMode.Extend:
                proc.StartInfo.Arguments = "/extend";
                break;
            case DisplayMode.Duplicate:
                proc.StartInfo.Arguments = "/clone";
                break;
        }
        proc.Start();
    }
   
}
