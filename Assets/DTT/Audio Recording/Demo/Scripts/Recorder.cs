using UnityEngine;

namespace DTT.AudioRecording.Demo
{
    /// <summary>
    /// Microphone recorder class.
    /// </summary>
    public class Recorder : MonoBehaviour
    {
        /// <summary>
        /// The AudioClip produced by the recording.
        /// </summary>
        public AudioClip RecordedClip { get; private set; }

        /// <summary>
        /// Starts recording with the given settings.
        /// </summary>
        /// <param name="device">Which input device to use.</param>
        /// <param name="loop">Indicates whether the recording should wrap around and record from the beginning of the AudioClip.</param>
        /// <param name="duration">The length of the AudioClip produced by the recording.</param>
        /// <param name="frequency">The sample rate of the AudioClip produced by the recording.</param>
        public void StartRecording(string device, bool loop, int duration, int frequency) => RecordedClip = Microphone.Start(device, loop, duration, frequency);

        /// <summary>
        /// Stops the recording. and trims the silence.
        /// </summary>
        public void StopRecording(string device)
        {
            Microphone.End(device);
            RecordedClip = SavWav.TrimSilence(RecordedClip, 0);
        }

        /// <summary>
        /// Resets the recorded AudioClip.
        /// </summary>
        public void ResetAudioClip() => RecordedClip = null;

        /// <summary>
        /// Checks if the device is currently recording.
        /// </summary>
        /// <param name="device">Input device to check.</param>
        /// <returns>True if the device is recording.</returns>
        public bool DeviceIsRecording(string device) => Microphone.IsRecording(device);

        /// <summary>
        /// Gets all found input devices.
        /// </summary>
        /// <returns>String array with all found input devices.</returns>
        public string[] GetAllDevices() => Microphone.devices;
    }
}