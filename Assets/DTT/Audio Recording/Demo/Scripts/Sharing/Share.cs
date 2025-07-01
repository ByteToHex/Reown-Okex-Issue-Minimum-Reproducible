using UnityEngine;

namespace DTT.AudioRecording.Demo
{
    /// <summary>
    /// Base class for a sharing audio class.
    /// </summary>
    public abstract class Share
    {

        /// <summary>
        /// Export path where the audio file is saved.
        /// </summary>
        protected string p_exportPath;

        /// <summary>
        /// Audio recording to share.
        /// </summary>
        protected AudioClipRecording p_recordingToShare;

        /// <summary>
        /// Cancel the sharing.
        /// </summary>
        public abstract void CancelSharing();

        /// <summary>
        /// SharingAudioRecording an audio clip.
        /// </summary>
        /// <param name="recordingToShare">Audio recording to share.</param>
        /// <param name="exportPath">Export path of the recording.</param>
        public abstract void Sharing(AudioClipRecording recordingToShare, string exportPath);
    }
}