using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSCReceiver : MonoBehaviour
{
    public OSC osc;
    bool isConnected = false;
    void Start()
    {
        string str, str1, str2;
        str = ConfigManager.GetInstance().GetStringValue("ADDRESS_FOR_RESOLUME_CONTENT_FINISHED");
        str1 = ConfigManager.GetInstance().GetStringValue("ADDRESS_FOR_ROTATING_SCREEN_CONNECT");
        str2 = ConfigManager.GetInstance().GetStringValue("ADDRESS_FOR_RESOLUME_CONTENT_CONNECT");
        //osc.SetAllMessageHandler(OnConnect);
        osc.SetAddressHandler(str, OnReceive);
        osc.SetAddressHandler(str1, OnConnectZero);
        osc.SetAddressHandler(str2, OnConnectOne);
    }


    private void OnConnectZero(OscMessage message)
    {
        //Debug.Log("Received OSC message: " + message.address);
        isConnected = false;
    }
    private void OnConnectOne(OscMessage message)
    {
        Invoke("IsConnected", 0.5f);
        //Debug.Log("Received OSC message: " + message.address);
    }
    void OnReceive(OscMessage message)
    {
        if (isConnected)
        {
            //ResolumeSender._instance.TriggerColumnVideo(1);
            //Debug.Log("Received OSC message: " + message.address);
        }
    }
    void IsConnected()
    {
        isConnected = true;
        // print(isConnected);
    }
}