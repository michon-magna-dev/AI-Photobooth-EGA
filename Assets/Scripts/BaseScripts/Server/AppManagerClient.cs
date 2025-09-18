using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppManagerClient : MonoBehaviour
{
    [SerializeField] UDPSend m_udpSend;

    public void PlayMediaPlayer()
    {
        m_udpSend.SendUDPMsg("Play");
    }

    public void PauseMediaPlayer()
    {
        m_udpSend.SendUDPMsg("Pause");
    }

    public void StopMediaPlayer()
    {
        m_udpSend.SendUDPMsg("Stop");
    }
}
