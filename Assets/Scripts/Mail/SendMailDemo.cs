using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class SendMailDemo : MonoBehaviour {

    string From = "ega.ruya.careers@gmail.com";
    string Name = "Photo ready!";

    //public InputField To;
    //public InputField Subject;
    //public InputField Message;

    public string To;
    public string Subject;
    public string Message;
    public string AttachmentFilename = "test.jpg";

    public List<string> emailUser = new List<string>();
    public List<string> photo = new List<string>();

    #region Lifecycle
    
    private void Start()
    {
        InvokeRepeating("SendMessageIsReady", 5f, 5f);
    }
    
    private void Update()
    {
        if (CheckInternetConnection())
        {
            if (MailSingleton.Instance.isReady)
            {
                if (emailUser.Count > 0 && photo.Count > 0)
                {
                    SendMailWithAttachment(photo[0], emailUser[0]);
                    MailSingleton.Instance.isReady = false;
                    RemoveAnItem(0);
                }
            }

        }
        else
        {
            MailSingleton.Instance.isReady = false;
        }
    }

    #endregion


    void SendMessageIsReady()
    {
        MailSingleton.Instance.isReady = true;
    }

 
    public bool CheckInternetConnection()
    {
        return !(Application.internetReachability == NetworkReachability.NotReachable);
    }

    public void SendMailWithAttachment(string filename, string to)
    {
        To = to;
        if (CheckInternetConnection())
        {

            MailSingleton.Instance.SendMailWithAttachment(
                From,
                Name,
                //To.text, 
                //Subject.text, 
                //Message.text, 
                To,
                Subject,
                Message,
                // Application.streamingAssetsPath + "/" + AttachmentFilename

                Application.streamingAssetsPath + "/" + filename
            );
        }
        else
        {
            emailUser.Add(to);
            photo.Add(filename);
        }

    }

    public void RemoveAnItem(int index)
    {
        // RemoveAnItem(emailUser.IndexOf(index));
        emailUser.RemoveAt(index);
        photo.RemoveAt(index);
    }
    
    public void SendPlainMail()
    {
        MailSingleton.Instance.SendPlainMail(
            From, 
            Name,
              //To.text, 
              //Subject.text, 
              //Message.text

              To,
            Subject,
            Message
        );
    }

}