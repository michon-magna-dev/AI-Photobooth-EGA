using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace SENDEMAIL
{
    public class MailSender
    {

        private SmtpClient mSmtpServer;

        public MailSender(string username, string password, string smtpServerAddress, int port = 25, bool enableSSL = false, bool useDefaultCredentials = false)
        {
            
            mSmtpServer = new SmtpClient(smtpServerAddress);
            mSmtpServer.EnableSsl = enableSSL;
            mSmtpServer.UseDefaultCredentials = useDefaultCredentials;
            mSmtpServer.Port = port;
            mSmtpServer.Credentials = new System.Net.NetworkCredential(username, password) as ICredentialsByHost;

            // A trick to enbale SSL with Google, I didn't tried other servers
            if (enableSSL)
            {
                ServicePointManager.ServerCertificateValidationCallback =
                    delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                    { return true; };
            }
        }

        public void SendMail(string from, string fromName, string to, string subject, string body, SendCompletedEventHandler onComplete = null, string attachmentName = "", string attachmentKind = "image/jpg")
        {
            MailMessage mail = new MailMessage();
            
            mail.From = new MailAddress(from, fromName);
           
            mail.To.Add(to);
            mail.Subject = subject;

            string link = "https://www.instagram.com/fitbitmena/";
            string link2 = "http://bit.ly/fitbit30x30rules";

            body = " <html>" +
               " <body>" +
               " <br/>" +
               "Thank you for joining the Dubai Fitness Challenge with Fitbit at the Kite Beach Fitness Village. <br/>" +
                " <br/>" +
               "Please find attached your photo that was clicked at the Fitbit Zone! <br/>" +
                " <br/>" +
               "For your chance to win a brand-new Fitbit Inspire 3, every week, you could take part in the Fitbit 30x30 Dubai Fitness Challenge Competition 2022. <br/>" +
                " <br/>" +
               "To participate, just follow the steps mentioned below <br/>" +
                " <br/>" +
               "1. Post a photo or a video of one of the activities during DFC 2022 on your Instagram account <br/>" +
               "2. Use #Fitbit30x30 in the caption <br/>" +
               "3. Follow @FitbitMENA <br/>" +
               " <br/>" +
               "To know more, follow and stay tuned to the " +
               "<a href ='" + link + "'>" + "https://www.instagram.com/fitbitmena/" + "</a>" + "." +
               " <br/>" +
               " <br/>" +
               // "<font size='"+ "'-3>" +
               "<small>" +
               "<i> Disclaimer : Some selected images that are clicked at Fitbit Zone might feature on the " +
                "<a href ='" + link + "'>" + "Fitbit MENA instagram page" + "</a>" + "." +
               " In case you don't want a photo to be featured on the " +
               "<a href ='" + link + "'>" + "Fitbit MENA Instagram page" + "</a>" + "." +
               " please forward the particular photo to this email and include “Don’t feature on Instagram” as the subject line. <br/>" +
                " </i>" +
               " <br/>" +
               " <i>" +
               "Fitbit 30x30 Dubai Fitness Challenge Competition 2022 is open to UAE residents over 18 years of age, from 29 Oct to 27 Nov 2022. Refer to our Instagram posts each week for more details. Official rules apply; view the rules at " +
                "<a href ='" + link2 + "'>" + "bit.ly/fitbit30x30rules" + "</a>" +
               ". Any personal data collected in relation to the Fitbit activations at Kite Beach Fitness Village is being collected by Magna Innovations FZ LLC and Crayons Global FZ LLC and will be processed in accordance with applicable data protection law. Google LLC and/or its affiliates including Fitbit are not collecting any personal data and are not responsible for the same.  <br/>" +
               " </i>" +
               " </small>" +
               // " </font>" +
               " <br/>" +
               " </html>" +
               " </body>";

            mail.Body = body;
            mail.IsBodyHtml = true;

            if (attachmentName != "")
            {
                mail.Attachments.Add(new Attachment(attachmentName, attachmentKind));
            }

            if (onComplete != null)
            {
                mSmtpServer.SendCompleted += onComplete;
            }

            mSmtpServer.SendAsync(mail, null);
        }

      
    }
}
