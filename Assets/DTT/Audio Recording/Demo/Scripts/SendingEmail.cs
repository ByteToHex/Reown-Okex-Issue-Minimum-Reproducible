using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Networking;

namespace DTT.AudioRecording.Demo
{
    /// <summary>
    /// Class that handle sending email.
    /// </summary>
    public class SendingEmail : Email
    {
        /// <summary>
        /// Constructor for an email.
        /// </summary>
        /// <param name="emailTo">Destination mail address.</param>
        /// <param name="subject">Subject of the mail.</param>
        /// <param name="body">Body of the mail.</param>
        /// <param name="attachmentLink">Link to the sound file.</param>
        public SendingEmail(string emailTo, string subject, string body, string attachmentLink) : 
            base(emailTo, subject, body, attachmentLink) {}
        

        /// <summary>
        /// Open the mail app with the mailto URL.
        /// </summary>
        public override void SendEmail()
        {
            i_body += i_attachmentLink;
            Application.OpenURL("mailto:" + i_emailTo + "?subject=" + i_subject + "&body=" + i_body);
        }
        
    }
}
