using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class RegistrationHandler : MonoBehaviour
{
    [SerializeField] TMP_InputField nameInputField;
    [SerializeField] TMP_InputField phoneNumberInputField;
    [SerializeField] TMP_InputField emailInputField;
    [SerializeField] TMP_Text m_emailPlaceholder;

    [SerializeField] UDPSend m_udpSend;
    [SerializeField] GameObject m_startPanel;
    [SerializeField] GameObject m_registrationPanel;
    [SerializeField] GameObject m_endObjects;
    [SerializeField] GameObject m_takePictureObjects;
    [SerializeField] GameObject m_endPanel;
    [SerializeField]
    Button[] m_endButtons;
    [SerializeField] string m_ip;

    [SerializeField] string m_currentUserDetails;

    private string filePath = "data.csv";

    private void Start()
    {
        m_ip = ConfigManager.GetInstance().GetStringValue("UDP_SEND_IP");
        filePath = Path.Combine(Application.streamingAssetsPath, "data.csv");
        if (!File.Exists(filePath))
        {
            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                writer.WriteLine("Name,Phone Number,Email,GUID"); 
            }
        }
    }

    public void StartButton()
    {
        m_startPanel.SetActive(false);
        m_registrationPanel.SetActive(true);
        m_udpSend.SendUDPMsg("Start", m_ip);
    }

    public void SaveDataToCSV()
    {
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            writer.WriteLine(m_currentUserDetails);
        }

        Debug.Log("Data has been saved to " + filePath);
       
        nameInputField.text = null;
        phoneNumberInputField.text = null;
        emailInputField.text = null;

    }

    public void SubmitButton()
    {
        m_emailPlaceholder.gameObject.SetActive(false);
        string name = nameInputField.text;
        string phoneNumber = phoneNumberInputField.text;
        string email = emailInputField.text;
        var guid = GUID_Generator.GetInstance().CreateNewUserGUID();

        if (email.Contains("@") && email.Contains("."))
        {
            m_currentUserDetails = $"{name},{phoneNumber},{email},{guid}";

            var udpSignal = $"SELFIE: {m_currentUserDetails}";
            m_udpSend.SendUDPMsg(udpSignal,m_ip);

            m_endPanel.SetActive(true);
            m_registrationPanel.SetActive(false);
            m_takePictureObjects.SetActive(true);
      
        }
        else
        {
            emailInputField.text="";
            m_emailPlaceholder.gameObject.SetActive(true);
        }
    }

    public void RefreshGUID()
    {
        string name = nameInputField.text;
        string phoneNumber = phoneNumberInputField.text;
        string email = emailInputField.text;
        var guid = GUID_Generator.GetInstance().CreateNewUserGUID();

        m_currentUserDetails = $"{name},{phoneNumber},{email},{guid}";
        var udpSignal = $"SELFIE: {m_currentUserDetails}";
        m_udpSend.SendUDPMsg(udpSignal,m_ip);
        m_takePictureObjects.SetActive(true);
        m_endObjects.SetActive(false);

    }

    public void TakePicture()
    {
        m_udpSend.SendUDPMsg("Take Picture",m_ip);
        m_endObjects.SetActive(true);
        m_takePictureObjects.SetActive(false);
    }

    public void OnPictureTaken()
    {
        foreach (var button in m_endButtons)
            button.interactable = true;
    }

    public void DoneButton()
    {
        m_udpSend.SendUDPMsg("Experience Completed", m_ip);
        m_endPanel.SetActive(false);
        m_endObjects.SetActive(false);
        m_startPanel.SetActive(true);
        SaveDataToCSV();
        foreach (var button in m_endButtons)
            button.interactable = false;
    }
}
