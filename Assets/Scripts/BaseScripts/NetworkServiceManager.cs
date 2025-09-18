using System;
using System.Collections;
using UnityEngine;

public class NetworkServiceManager : MonoBehaviour
{
    private static NetworkServiceManager _instance;
    public static NetworkServiceManager GetInstance() => _instance;

    [SerializeField] UDPReceive m_receiver;
    [SerializeField] UDPSend m_sender;

    [SerializeField] ScreenShotEditable m_captureManager;
    [SerializeField] CountdownTimer m_countdown;

    [SerializeField] string UDP_ACTION_STARTSELFIE;
    [SerializeField] string UDP_ACTION_TAKEPICTURE;
    [SerializeField] string UDP_ACTION_EXPERIENCE_COMPLETE;
    [SerializeField] string m_selfieDataKeyword = "SELFIE";
    [Space]
    bool isProcessingImage = false;

    [Header("INFO")]
    [SerializeField] string name;
    [SerializeField] string phoneNumber;
    [SerializeField] string receivedEmail;
    [SerializeField] string guid;
    [Space]
    [Header("INFO")]
    [SerializeField] KeyCode msg1Key;
    [SerializeField] KeyCode msg2Key;
    [SerializeField] KeyCode msg3Key;

    public string[] GetUserDetails()
    {
        return new string[] { name, phoneNumber, receivedEmail, guid };
    }

    #region Lifecycles

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        m_receiver.receiveUDP += OnUDPReceive;
    }

    private void Update()
    {
        DebugReceiveMessages();
    }

    #endregion


    #region UDP Signals
    public void SendUDPMessage(string p_msg)
    {
        m_sender.SendUDPMsg(p_msg);
        Debug.Log($"Sent UDP Message {p_msg}");
    }

    private void OnUDPReceive(string p_msge)
    {
        Debug.Log($"Received {p_msge}");
        //On Receive Message.
        //Data Sample: Selfie:Michon Bojador,Visitor, I am a resident, mbojador@gmail.com,ABCD-AS123123
        // Selfie:Fajel,Resident, Love to Live,fajel@sparkslab.me,9c3dca0c-de73-4577-a207-a4d6b8ca8a4c

        if (p_msge.StartsWith(m_selfieDataKeyword))
        {
            GetSelfieMessage(p_msge);
        }
        else if (p_msge.Equals(UDP_ACTION_TAKEPICTURE))
        {
            StartCountdown();
        }
        else if (p_msge.Equals(UDP_ACTION_EXPERIENCE_COMPLETE))
        {
            FinishExperience();
        }
    }

    #endregion

    void GetSelfieMessage(string p_message)
    {
        var splitMessage = p_message.Split(':');
        string[] userDetails = splitMessage[1].Split(',');

        name = userDetails[0];
        phoneNumber = userDetails[1];
        receivedEmail = userDetails[2];
        guid = userDetails[3];

        //HUDManager.GetInstance().ChangeUIState(CURRENT_SCREEN.PICTURE_MODE);
        HUDManager.GetInstance().ShowScreen(CURRENT_SCREEN.PICTURE_MODE);
    }

    public void StartCountdown()
    {
        m_countdown.StartCountDown();
    }

    void FinishExperience()
    {
        if (!isProcessingImage)
            StartCoroutine(ProcessImage());

        SlideshowManager.GetInstance().ReLoadImages(true);
        HUDManager.GetInstance().ShowScreen(CURRENT_SCREEN.STANDBY);

        //try
        //{
        //    EmailSender.GetInstance.SendEmailThread(receivedEmail, ScreenShotEditable.GetInstance().GetImageTaken());
        //    PrinterManager.GetInstance().PrintImageThread(ScreenShotEditable.GetInstance().GetImageTaken());
        //    SlideshowManager.GetInstance().ReLoadImages(true);
        //    HUDManager.GetInstance().ShowStandbyMode();
        //}

        //catch (Exception ex)
        //{
        //    // Handle any exceptions that may occur during email sending or printing.
        //    Console.WriteLine($"Error: {ex.Message}");
        //    SlideshowManager.GetInstance().ReLoadImages(true);
        //    HUDManager.GetInstance().ShowStandbyMode();
        //}
    }

    private IEnumerator ProcessImage()
    {
        yield return null;
        EmailSender.GetInstance.SendEmailThread(receivedEmail, ScreenShotEditable.GetInstance().GetImageTaken());
        yield return null;
        PrinterManager.GetInstance().PrintImageThread(ScreenShotEditable.GetInstance().GetImageTaken());
    }

    #region Debug

    private void DebugReceiveMessages()
    {
        //if (Input.GetKeyDown(msg1Key))
        //{
        //    GetSelfieMessage("SELFIE: Michon,456432132,michon@sparkslab.me,asdasdasd");
        //}
        //if (Input.GetKeyDown(msg2Key))
        //{
        //    StartCountdown();
        //}
        //if (Input.GetKeyDown(msg3Key))
        //{
        //    FinishExperience();
        //}
    }

    #endregion

    /*
        R[10.39.2.24:53527]: Start
        R[10.39.2.24:53527]: SELFIE: FaJeL,456432132,@.,b62a09c3-3f58-42e0-a1a3-58aecd78235a
        R[10.39.2.24:53527]: Take Picture
        R[10.39.2.24:53527]: SELFIE: FaJeL,456432132,@.,502e88d3-5dd5-4485-a94f-663fc9c5fee0
        R[10.39.2.24:53527]: Take Picture
        R[10.39.2.24:53527]: Experience Completed
    */

}