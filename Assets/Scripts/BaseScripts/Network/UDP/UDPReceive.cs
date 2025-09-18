

using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPReceive : MonoBehaviour
{
    public bool isDebug = true;
    [SerializeField] int m_port = 4244;
    Thread receiveThread;
    UdpClient client;

    public string lastReceivedUDPPacket = "";

    public delegate void OnReceiveUDP(string p_msge);
    public event OnReceiveUDP receiveUDP;

    void Awake()
    {
        init();
        UnityThread.initUnityThread();
    }

    private void init()
    {
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    // receive thread
    private void ReceiveData()
    {
        client = new UdpClient(m_port);

        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, m_port);
                byte[] data = client.Receive(ref anyIP);
                string text = Encoding.UTF8.GetString(data);

                lastReceivedUDPPacket = text;

                //if (receiveUDP != null)
                //    receiveUDP(text);

                Action received = OnReceive;
                UnityThread.executeInUpdate(received);

                if (isDebug)
                    print(lastReceivedUDPPacket);
            }
            catch
            {
                //Debug.Log("err: " + err);
                //print(err.ToString());
            }
        }
    }

    void OnReceive()
    {
        if (receiveUDP != null)
            receiveUDP(lastReceivedUDPPacket);
    }

    public string getLatestUDPPacket()
    {
        return lastReceivedUDPPacket;
    }

    public void SetPort(int p_port)
    {
        receiveThread.Abort();
        if (client != null)
            client.Close();
        m_port = p_port;
        init();
    }

    void OnDestroy()
    {
        receiveThread.Abort();
        if (client != null)
            client.Close();
    }

    private void OnApplicationQuit()
    {
        receiveThread.Abort();
        if (client != null)
            client.Close();
    }
}
