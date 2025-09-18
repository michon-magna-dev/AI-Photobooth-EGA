using System;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class EmailSender : MonoBehaviour
{
    private static EmailSender _instance;
    public static EmailSender GetInstance => _instance;

    [Header("Debug")]
    [SerializeField] string fromEmail;
    [SerializeField] string toEmail;
    [SerializeField] string subject;
    [SerializeField] string password;
    [SerializeField] string m_filePath;

    System.Net.Mail.Attachment m_attachment;
    [Space]
    [Header("Debug")]
    [SerializeField] string d_title;
    [SerializeField] string d_body;
    public string result = "";

    string lastEmail;
    string lastPath;
    Thread t;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        //fromEmail = ConfigManager.GetInstance().GetStringValue("From_Email");
        //subject = ConfigManager.GetInstance().GetStringValue("Subject");
        //password = ConfigManager.GetInstance().GetStringValue("Password");
        //body = ConfigManager.GetInstance().GetStringValue("Body");
        //Debug.Log(fromEmail);
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.F1))
        {
            //Selfie:jzkdi,Visitor,v,michon@sparkslab.me,7c9372b1-5c34-49b2-a71d-9d120e52e8a3
            //SendEmailThread();
            SendEmail("Michon", toEmail, m_filePath);
            //SendEmail("Michon", "michon.magna@sparkslab.me", "\\\\MICHON-DEV-PC\\Shared Folder\\adro\\29Apr22\\jzkdi_7c9372b1-5c34-49b2-a71d-9d120e52e8a3.png");
        }

    }

    void SendEmailThread()
    {
        Thread t = new Thread(() =>
        {
            //SendEmail("test", "c_nitish.prasad@digitaldubai.ae", "\\\\MICHON-DEV-PC\\Shared Folder\\adro\\29Apr22\\jzkdi_7c9372b1-5c34-49b2-a71d-9d120e52e8a3.png");
        });
        t.Start();
    }

    private void OnDestroy()
    {
        try
        {
            t.Abort();
        }
        catch (Exception)
        {

        }
    }

    public void ResendEmail()
    {
        SendEmail(lastEmail, lastPath);
    }

    public void SendEmailThread(string p_email, string p_imgPath)
    {
        Thread t = new Thread(() =>
        {
            SendEmail(p_email, p_imgPath);
        });
        t.Start();
    }


    public async Task SendEmailAsync(string p_email, string p_imgPath)
    {
        await Task.Run(() =>
        {
            SendEmail(p_email, p_imgPath);
        });
    }

    public void SendEmail(string p_email, string p_imgPath)
    {
        lastEmail = p_email;
        lastPath = p_imgPath;

        //try
        //{
        //    toEmail = p_email;
        //    MailMessage mail = new MailMessage();
        //    mail.From = new MailAddress(fromEmail);
        //    mail.To.Add(toEmail);
        //    mail.Subject = subject;
        //    string body = $"We hope this message finds you well and that you had a fantastic time at GITEX 2023! " +
        //    $"We were thrilled to have you visit the Dubai Digital Authority booth and participate in our cutting-edge Holo LCD Box Photo Booth experience. " +
        //    $"Your presence made our event truly special.\r\n\r\nWe're excited to share your memorable photo booth moment with you." +
        //    $" Please find attached your unique holographic LCD box photo as a token of our appreciation.\r\n\r\n[Attach Photo]\r\n\r" +
        //    $"\nFeel free to download and share this fantastic memory with your friends, family, and colleagues on your social media profiles. " +
        //    $"Don't forget to tag us @DubaiDigitalAuthority and use the hashtag #GITEX2023 to let everyone know about your GITEX experience." +
        //    $"\r\n\r\nThank you once again for being a part of GITEX 2023 and visiting our booth. We look forward to staying connected with you and bringing more innovative experiences your way.";

        //    mail.Body = body;
        //    // mail.AlternateViews.Add(GetEmbeddedImage(m_filePath));
        //    m_attachment = new System.Net.Mail.Attachment(p_imgPath);
        //    mail.Attachments.Add(m_attachment);

        //    SmtpClient smtpServer = new SmtpClient("smtp.dubai.gov.ae", 25);
        //    smtpServer.Credentials = new System.Net.NetworkCredential(fromEmail, password) as ICredentialsByHost;
        //    smtpServer.EnableSsl = false;
        //    ServicePointManager.ServerCertificateValidationCallback =
        //    delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        //    { return true; };
        //    smtpServer.Send(mail);

        //    Debug.LogAssertion($"Sent email to {p_email}: Successful");
        //}
        //catch (Exception)
        //{
        //    Debug.LogAssertion($"Sent email to {p_email} : Unsuccessful");
        //}
    }

    public void SendEmail(string p_name, string p_email, string p_imgPath)// if need to chang Body Email =LOOK HERE
    {
        Debug.Log("Sending Email;");
        lastEmail = p_email;
        lastPath = p_imgPath;

        try
        {
            toEmail = p_email;
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(fromEmail);
            mail.To.Add(toEmail);
            mail.Subject = subject;
            string body = $"Dear Attendee,\n\nGreetings from Digital Dubai!\n\nWe hope you enjoyed your time at the Dubai Government Pavilion" +
                $" during Gitex Global 2023! We want to express our gratitude for visiting our booth and capturing a Holo Picture at the Holo LCD Box." +
                $" Attached, you'll find your photo, which we hope brings a smile to your face.\n\nFeel free to share this experience on social media and tag us " +
                $"@digitaldubai.\n\nBest regards,\n\nDigital\u00a0Dubai";
            mail.Body = body;

            // mail.AlternateViews.Add(GetEmbeddedImage(m_filePath));

            m_attachment = new System.Net.Mail.Attachment(p_imgPath);
            mail.Attachments.Add(m_attachment);
            m_attachment = new System.Net.Mail.Attachment(p_imgPath);
            mail.Attachments.Add(m_attachment);
            
            //SmtpClient smtpServer = new SmtpClient("smtp.dubai.gov.ae", 25);
            ////smtpServer.Credentials = new System.Net.NetworkCredential(fromEmail, password) as ICredentialsByHost;
            //smtpServer.EnableSsl = false;
            //ServicePointManager.ServerCertificateValidationCallback =
            //delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            //{ return true; };
            //smtpServer.Send(mail);

            Debug.LogAssertion($"Sent email to {p_email}: Successful");
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            Debug.LogAssertion($"Sent email to {p_email} : Unsuccessful");
        }
    }
    public void EmailSending()
    {
        try
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(fromEmail);
            mail.To.Add(toEmail);
            mail.Subject = subject;
            mail.Body = d_body;

            // Optional: Add inline image or attachment
            if (!string.IsNullOrEmpty(m_filePath))
            {
                mail.AlternateViews.Add(GetEmbeddedImage(m_filePath));
                m_attachment = new Attachment(m_filePath);
                mail.Attachments.Add(m_attachment);
            }

            using (SmtpClient smtpServer = new SmtpClient("smtp.gmail.com", 587))
            {
                smtpServer.Credentials = new NetworkCredential(fromEmail, password);
                smtpServer.EnableSsl = true;

                // (Optional) bypass SSL errors if needed:
                ServicePointManager.ServerCertificateValidationCallback =
                    (s, cert, chain, sslPolicyErrors) => true;

                smtpServer.Send(mail);
            }

            Console.WriteLine("Email sent successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
        }
    }

    //public void EmailSending()
    //{
    //    //toEmail = m_email.text;
        
    //    MailMessage mail = new MailMessage();
    //    mail.From = new MailAddress(fromEmail);
    //    mail.To.Add(toEmail);
    //    mail.Subject = subject;
    //    mail.Body = d_body;
    //    mail.AlternateViews.Add(GetEmbeddedImage(m_filePath));
    //    m_attachment = new System.Net.Mail.Attachment(m_filePath);
    //    mail.Attachments.Add(m_attachment);


    //    SmtpClient smtpServer = new SmtpClient("smtp.gmail.com", 587);
    //    smtpServer.Credentials = new System.Net.NetworkCredential(fromEmail, password) as ICredentialsByHost;
    //    smtpServer.EnableSsl = true;
    //    ServicePointManager.ServerCertificateValidationCallback =
    //    delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    //    { return true; };
    //    smtpServer.Send(mail);
    //    smtpServer.SendCompleted += PrintResult;
    //}

    private void PrintResult(object sender, AsyncCompletedEventArgs e)
    {
        if (e.Error != null)
        {
            Debug.Log($"Email failed to send: {e.Error.Message}");
        }
        else if (e.Cancelled)
        {
            Debug.Log("Email sending was cancelled.");
        }
        else
        {
            Debug.Log("Email sent successfully.");
        }
    }
    private AlternateView GetEmbeddedImage(string filePath)
    {
        LinkedResource res = new LinkedResource(filePath);
        res.ContentId = Guid.NewGuid().ToString();
        string htmlBody = @"<img src='cid:" + res.ContentId + @"'/>";
        AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
        alternateView.LinkedResources.Add(res);
        return alternateView;
    }

    protected void OnGUI()
    {
        //if (request != null && request.IsRunning)
        //{
        //    GUILayout.Label($"Loading: {request.Progress:P2}");
        //}
        //else
        //{
            toEmail = GUILayout.TextField(toEmail);
            if (GUILayout.Button("Send Email")) EmailSending();
        //}

        if (!string.IsNullOrEmpty(result))
        {
            GUILayout.TextField(result);
        }
    }

}