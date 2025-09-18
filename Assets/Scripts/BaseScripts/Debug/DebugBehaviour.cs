using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugBehaviour : MonoBehaviour
{

    public static bool debugModeOn = false;
    public KeyCode debugKey = KeyCode.F12;
    // Start is called before the first frame update
    void Start()
    {
        debugModeOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(debugKey))
        {
            debugModeOn = !debugModeOn;
        }
    }
}
