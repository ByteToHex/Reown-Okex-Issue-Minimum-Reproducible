using UnityEngine.Networking;

namespace DTT.AudioRecording.Demo
{
    public abstract class Email
    {
        /// <summary>
        /// Destination mail address.
        /// </summary>
        internal string i_emailTo;
        
        /// <summary>
        /// Subject of the email.
        /// </summary>
        internal string i_subject;
        
        /// <summary>
        /// Body of the email.
        /// </summary>
        internal string i_body;
        
        /// <summary>
        /// Link to the attachment file.
        /// </summary>
        internal string i_attachmentLink;
        
        /// <summary>
        /// Constructor for the sending mail class.
        /// </summary>
        /// <param name="emailTo">Destination mail address.</param>
        /// <param name="subject">Subject of the mail.</param>
        /// <param name="body">Body of the mail.</param>
        /// <param name="attachmentLink">Link to the sound file.</param>
        public Email(string emailTo, string subject, string body,string attachmentLink)
        {
            i_emailTo = EscapeURL(emailTo);
            i_subject = EscapeURL(subject);
            i_body = EscapeURL(body);
            i_attachmentLink = attachmentLink;
        }
        
        /// <summary>
        /// Transform url into a mailto URL.
        /// </summary>
        /// <param name="url">URL to be transformed.</param>
        /// <returns>Transformed URL.</returns>
        public static string EscapeURL(string url) => UnityWebRequest.EscapeURL(url).Replace("+", "%20");
        
        /// <summary>
        /// Open the mail app with the mailto URL.
        /// </summary>
        public abstract void SendEmail();


    }
}