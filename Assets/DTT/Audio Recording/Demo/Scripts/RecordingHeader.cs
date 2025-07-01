using UnityEngine;
using UnityEngine.UI;
using System;

namespace DTT.AudioRecording.Demo
{
    /// <summary>
    /// UI class used to display a recording.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class RecordingHeader : MonoBehaviour
    {
        /// <summary>
        /// Text component for the name.
        /// </summary>
        [Tooltip("Text component for the name.")]
        [SerializeField]
        private Text _nameText;

        /// <summary>
        /// Slider for controlling the audioclip.
        /// </summary>
        [Tooltip("Slider for controlling the audioclip.")]
        [SerializeField]
        private ClipControlSlider _clipControlSlider;

        /// <summary>
        /// Button to start playing the recording.
        /// </summary>
        [Tooltip("Button to start playing the recording.")]
        [SerializeField]
        private Button _playButton;

        /// <summary>
        /// Icon sprite to start playing the recording.
        /// </summary>
        [Tooltip("Icon sprite to start playing the recording.")]
        [SerializeField]
        private Sprite _playIcon;

        /// <summary>
        /// Icon sprite to stop playing the recording.
        /// </summary>
        [Tooltip("Icon sprite to stop playing the recording.")]
        [SerializeField]
        private Sprite _stopIcon;

        /// <summary>
        /// Text component to show the recording duration.
        /// </summary>
        [Tooltip("Text component to show the recording duration.")]
        [SerializeField]
        private Text _durationText;

        /// <summary>
        /// Event invoked when the recording should be deleted.
        /// </summary>
        public event Action<AudioClipRecording> DeletedRecording;

        /// <summary>
        /// Event invoked when the recording should be shared by mail.
        /// </summary>
        public event Action<AudioClipRecording> ShareRecording; 

        /// <summary>
        /// Data class of the recording.
        /// </summary>
        private AudioClipRecording _recordingData;

        /// <summary>
        /// Used in the toggle method.
        /// </summary>
        private bool _playCommand;

        /// <summary>
        /// Initializes the necessary components.
        /// </summary>
        /// <param name="recording">Recording data class.</param>
        public void Initialize(AudioClipRecording recording)
        {
            _recordingData = recording;
            _playCommand = false;
            _nameText.text = recording.Name;
            string formattedTime = TimeSpan.FromSeconds(Mathf.Round(recording.Duration)).ToString(@"mm\:ss");
            _durationText.text = $"{formattedTime}";

            _clipControlSlider.Initialize(recording.Clip);
        }

        /// <summary>
        /// Stops the audio automatically and forces the slider to track the playing recording.
        /// </summary>
        private void FixedUpdate()
        {
            if (_playButton.image.sprite != _playIcon && !_clipControlSlider.AudioSource.isPlaying)
                StopAudio();
        }

        /// <summary>
        /// Plays the recording and updates the UI.
        /// </summary>
        public void PlayAudio()
        {
            _playCommand = true;
            _playButton.image.sprite = _stopIcon;

            _clipControlSlider.PlayAudio();
            _durationText.color = Color.white;
        }

        /// <summary>
        /// Stops the recording and updates the UI.
        /// </summary>
        public void StopAudio()
        {
            _playCommand = false;
            _playButton.image.sprite = _playIcon;

            _clipControlSlider.StopAudio();
            _durationText.color = new Color32(255, 255, 255, 163);
        }

        /// <summary>
        /// Invokes the DeletedRecording event.
        /// </summary>
        public void Delete() => DeletedRecording?.Invoke(_recordingData);

        /// <summary>
        /// Invokes the sharing event.
        /// </summary>
        public void OnShare() => ShareRecording?.Invoke(_recordingData);
        
        /// <summary>
        /// Toggles between play and stop audio.
        /// </summary>
        private void TogglePlay()
        {
            if (_playCommand)
                StopAudio();
            else
                PlayAudio();
        }

        /// <summary>
        /// Subscribes to the toggle event.
        /// </summary>
        private void OnEnable() => _playButton.onClick.AddListener(TogglePlay);

        /// <summary>
        /// Unsubscribes from the toggle event.
        /// </summary>
        private void OnDisable() => _playButton.onClick.RemoveListener(TogglePlay);
    }
}