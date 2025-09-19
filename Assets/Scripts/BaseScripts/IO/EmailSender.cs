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

    [Header("Gmail Settings")]
    public string fromEmail = "michon.magna@gmail.com";
    public string toEmail = "mbojador@gmail.com";
    public string appPassword = "cwbnbtooinxcdxmg"; // NO spaces

    public void SendTestEmail()
    {
        // Call async method so we don't freeze Unity
        Debug.LogAssertion("Sending test email...");
        SendEmailAsync("Unity Test Email", "This is a test email sent from Unity!");
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SendTestEmail();
        }
    }

    public async void SendEmailAsync(string subject, string body)
    {
        await Task.Run(() =>
        {
            try
            {
                // Enable multiple TLS versions
                ServicePointManager.SecurityProtocol =
                    SecurityProtocolType.Tls12 |
                    SecurityProtocolType.Tls11 |
                    SecurityProtocolType.Tls;

                // Bypass certificate validation (good for testing; remove for production)
                ServicePointManager.ServerCertificateValidationCallback =
                    delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                    {
                        return true;
                    };

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(fromEmail);
                    mail.To.Add(toEmail);
                    mail.Subject = subject;
                    mail.Body = body;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(fromEmail, appPassword);
                        smtp.EnableSsl = true;
                        smtp.Send(mail);

                        Debug.Log("Email sent successfully.");
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Failed to send email: " + ex);
                if (ex.InnerException != null)
                {
                    Debug.LogError("Inner Exception: " + ex.InnerException);
                }
            }

        }
        );
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
        if (GUILayout.Button("Send Email")) SendTestEmail();
        //}

        //if (!string.IsNullOrEmpty(result))
        //{
        //    GUILayout.TextField(result);
        //}
    }

}