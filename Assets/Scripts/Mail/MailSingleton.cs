using SENDEMAIL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Mail;
using UnityEngine;
using UnityEngine.UI;

public class MailSingleton : MonoBehaviour {

    public static MailSingleton Instance;
    string SMTPAddress = "smtp.gmail.com";
    int SMTPPort = 587;
   // string From = "magnacompany2022@gmail.com"; // Enter your gmail id here
    string From = "ega.ruya.careers@gmail.com"; // Enter your gmail id here
    // Enter the app password that you are getting from https://security.google.com/settings/security/apppasswords
    // Generate one with custom name 
    string Password = "hemtpdhsuknepfnk"; 

    bool EnableSSL = true;
    bool UseDefaultCredentials = false;

    public Text txtInfo;
    public bool isReady;

    MailSender mailSender;
 

    void OnEnable () {
        if (MailSingleton.Instance != null && MailSingleton.Instance != gameObject.GetComponent<MailSingleton>())
        {
            DestroyImmediate(gameObject);
        } else
        {
            DontDestroyOnLoad(gameObject);
            MailSingleton.Instance = gameObject.GetComponent<MailSingleton>();
        }

        mailSender = new MailSender(From, Password, SMTPAddress, SMTPPort, EnableSSL, UseDefaultCredentials);
    }

    public void SendMailWithAttachment(string from, string fromName, string to, string subject, string body, string attachment)
    {
        if (IsValidEmail(to))
        {
            mailSender.SendMail(from, fromName, to, subject, body, onAsyncComplete, attachment);
        }
        else
            Debug.Log("Not Email FormatFormat");

       
    }

    public void SendPlainMail(string from, string fromName, string to, string subject, string body)
    {
        mailSender.SendMail(from, fromName, to, subject, body, onAsyncComplete);
    }


    void onAsyncComplete(object sender, AsyncCompletedEventArgs completedEventArgs)
    {
        isReady = false;
        if (completedEventArgs.Error != null)
        {
            StartCoroutine(ShowInfo("Mail configuration error"));
            Debug.LogError(completedEventArgs.Error.Message);
            Debug.LogWarning("If you are using gmail please setup an application password to use with this application");
            return;
        }

        if (completedEventArgs.Cancelled)
        {
            StartCoroutine(ShowInfo("Sending cancelled"));
            Debug.LogWarning("Sending cancelled");
            return;
        }

        StartCoroutine(ShowInfo("Mail sent successfully"));
        Debug.Log("Mail sent successfully");

        SmtpClient sndr = (SmtpClient)sender;
        sndr.SendCompleted -= onAsyncComplete;

        StartCoroutine(waitForNextReady());
        
    }

    IEnumerator ShowInfo(string msg){
        // txtInfo.text = msg;
        Debug.Log(msg);
        yield return new WaitForSeconds(3);
        // txtInfo.text = "";
    }

    IEnumerator waitForNextReady()
    {
        yield return new WaitForSeconds(10f);
        isReady = true;
    }

    bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }


}
