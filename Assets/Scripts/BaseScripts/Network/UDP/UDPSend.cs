using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPSend : MonoBehaviour
{
    [SerializeField] private string m_ip;
    [SerializeField] int m_port;  // define in init
    [SerializeField] bool m_showDebugUi = true;

    //Connections
    IPEndPoint remoteEndPoint;
    UdpClient client;

    // start from unity3d
    public void Start()
    {
        client = new UdpClient();
    }

    public void SetPort(int p_port)
    {
        m_port = p_port;
    }

    // Send Data
    public void SendUDPMsg(string message,string p_ip = "")
    {
        Debug.Log(message);
        try
        {
            if(p_ip == "")
                remoteEndPoint = new IPEndPoint(IPAddress.Broadcast, m_port);
            else
                remoteEndPoint = new IPEndPoint(IPAddress.Parse(p_ip), m_port);
            byte[] data = Encoding.UTF8.GetBytes(message);
            client.Send(data, data.Length, remoteEndPoint);
            Debug.LogAssertion($"Sent UDP Message : {message}");
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }
}