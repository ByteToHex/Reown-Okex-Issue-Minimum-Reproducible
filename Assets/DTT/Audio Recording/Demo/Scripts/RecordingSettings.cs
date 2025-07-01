using UnityEngine;

namespace DTT.AudioRecording.Demo
{
    /// <summary>
    /// ScriptableObject for the settings.
    /// </summary>
    [CreateAssetMenu(menuName = "DTT/AudioRecording/RecordingSettings", fileName = "RecordingSettings")]
    public class RecordingSettings : ScriptableObject
    {
        /// <summary>
        /// The length of the AudioClip produced by the recording.
        /// </summary>
        [Tooltip("The max duration a recording can have.")]
        [SerializeField]
        private int _maxRecordingDuration;

        /// <summary>
        /// The sample rate of the AudioClip produced by the recording.
        /// </summary>
        [Tooltip("The sample rate of the AudioClip produced by the recording.")]
        [SerializeField]
        private int _frequency = 44100;

        /// <summary>
        /// The folder at Application.persistentDataPath where recording are saved.
        /// </summary>
        [Tooltip("The folder at Application.persistentDataPath where recording are saved.")]
        [SerializeField]
        private string _exportFolder;

        /// <summary>
        /// The length of the AudioClip produced by the recording.
        /// </summary>
        public int MaxRecordingDuration => _maxRecordingDuration;

        /// <summary>
        /// The sample rate of the AudioClip produced by the recording.
        /// </summary>
        public int Frequency => _frequency;

        /// <summary>
        /// The folder where recordings are saved.
        /// </summary>
        public string ExportFolder => _exportFolder;
    }
}