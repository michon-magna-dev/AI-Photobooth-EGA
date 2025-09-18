using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OSC_Sender : MonoBehaviour
{
    [SerializeField] OSC osc;

    #region Singleton
    public static OSC_Sender _instance;
  
    public OSC_Sender Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log(message: "Instance is null");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    #endregion

    public void TriggerCueVideo(int p_buttonIndex)
    {
        OscMessage message = new OscMessage();
        message.address = $"/composition/columns/{p_buttonIndex}/connect";
        message.values.Add(1);
        osc.Send(message);
        Debug.LogAssertion("Button Index:" + p_buttonIndex);
    }

    public void VolumeControl()
    {
        //OscMessage message = new OscMessage();
        //message.address = "/composition/selectedlayer/audio/volume";
        //var value = m_volume.currentValue / 100;
        //message.values.Add(value*2);
        //m_value.text=$"{(m_volume.currentValue * 2)}%";
        //osc.Send(message);
    }

}