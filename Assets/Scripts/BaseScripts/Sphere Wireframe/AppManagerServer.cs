using UnityEngine;

public class AppManagerServer : MonoBehaviour
{
    //[SerializeField] VideoPlayerHandler m_videoHandler;
    [SerializeField] UDPReceive m_udpReceiver;

    void Awake()
    {
        m_udpReceiver.receiveUDP += OnReceive;
    }

    public void OnReceive(string p_data)
    {
        string[] data = p_data.Split('|');
        if (data.Length == 2) { }
        else
        {
            //m_videoHandler.PlayTopicAndScreenSaver(int.Parse(p_data));
            //m_videoHandler.PlayTopicVideo(int.Parse(p_data));
        }
    }

}