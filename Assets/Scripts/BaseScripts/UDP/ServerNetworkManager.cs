using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerNetworkManager : MonoBehaviour
{
    [Header("Objs")]
    [SerializeField] UDPReceive m_receiver;
    [SerializeField] UDPSend m_sender;
    [Space]
    [Header("Current User")]
    [SerializeField] string currentUserName;
    [SerializeField] string currentUserGuid;
    [Space]
    [Header("Expected Messages")]
    [SerializeField] string m_selfieIdleMessage;
    [SerializeField] string m_selfieDataKeyword;
    [SerializeField] string m_playScreenSaverKeyword;
    [SerializeField] string m_connectionCheckKeyword;
    [Space]
    [Header("Connection Status")]
    bool isTabletConnected;
    public Text connectionTxt;

    private void Awake()
    {
        m_receiver.receiveUDP += OnReceiveSensorData;
    }

    void Start()
    {
        CheckTabletConnection();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F5))
        {
            CheckTabletConnection();
        }
#endif
    }

    private void FixedUpdate()
    {
        connectionTxt.text = $"Is Tablet Connected: {isTabletConnected}";
    }

    public void OnReceiveSensorData(string p_msge)
    {
        //On Receive Message.

        if (p_msge.StartsWith(m_selfieDataKeyword))
        {
            var splitMessage = p_msge.Split(':');
            //m_photoManager.ReceiveData(splitMessage[1]);
        }
        else if (p_msge.Equals(m_connectionCheckKeyword))
        {
            isTabletConnected = true;
        }
    }

    public void CheckTabletConnection()
    {
        //m_sender.DelayedSendUDP("STATUS");
        //isTabletConnected = false;
    }
}