using System.IO;
using UnityEngine;

namespace DTT.AudioRecording.Demo
{
    /// <summary>
    /// Handle sharing audio file for different platform.
    /// </summary>
    public class SharingAudioRecording : Share
    {
        /// <summary>
        /// Object for the sharing not supported menu.
        /// </summary>
        private GameObject _sharingNotSupportedPopUp;

        /// <summary>
        /// Constructor for the share class.
        /// </summary>
        /// <param name="iSharingNotSupportedPopUp">Popup for sharing not supported.</param>
        public SharingAudioRecording(GameObject SharingNotSupportedPopUp)
        {
            _sharingNotSupportedPopUp = SharingNotSupportedPopUp;
        }

        public override void CancelSharing() => _sharingNotSupportedPopUp.SetActive(false);

        public override void Sharing(AudioClipRecording recordingToShare, string exportPath)
        {
            p_recordingToShare = recordingToShare;
            p_exportPath = exportPath;
#if UNITY_IOS
            SharingIOS()
#elif Unity_ANDROID
            SharingAndroid();
#else
            SharingDesktop();
#endif
        }

        /// <summary>
        /// Share audio on IOS.
        /// </summary>
        protected void SharingIOS()
        {
            _sharingNotSupportedPopUp.SetActive(true);
            return;
        }

        /// <summary>
        /// Share audio on desktop.
        /// </summary>
        protected void SharingDesktop()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(p_exportPath);
            FileInfo[] file = directoryInfo.GetFiles(p_recordingToShare.Name);
            if (file.Length <= 0)
            {
                Debug.LogError("Recording not found.");
                return;
            }
            
            SendingEmail mailSender = new SendingEmail("",
                "Look at this audio!", 
                "You can attach the audio file stored at : ",
                file[0].ToString());
            mailSender.SendEmail();
        }

        /// <summary>
        /// Share audio on android.
        /// </summary>
        protected void SharingAndroid()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(p_exportPath);
            FileInfo[] file = directoryInfo.GetFiles(p_recordingToShare.Name);
            if (file.Length <= 0)
            {
                Debug.LogError("Recording not found.");
                return;
            }
            
            SendingEmail mailSender = new SendingEmail("",
                "Look at this audio!", 
                "You can attach the audio file stored at : ",
                file[0].ToString());
            mailSender.SendEmail();
        }
        
    }
}