using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.AudioRecording.Demo
{
    /// <summary>
    /// UI manager class for the microphone <see cref="Recorder"/>.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class MicrophoneUI : MonoBehaviour
    {
        /// <summary>
        /// The settings for the recorder.
        /// </summary>
        [Tooltip("The settings for the recorder.")]
        [SerializeField]
        private RecordingSettings _settings;

        /// <summary>
        /// The microphone recorder.
        /// </summary>
        [Tooltip("The microphone recorder.")]
        [SerializeField]
        private Recorder _recorder;

        /// <summary>
        /// Dropdown to show the available input devices.
        /// </summary>
        [Tooltip("Dropdown to show the available input devices.")]
        [SerializeField]
        private Dropdown _deviceDropdown;

        /// <summary>
        /// InputField for the name of the recording.
        /// </summary>
        [Tooltip("InputField for the name of the recording.")]
        [SerializeField]
        private InputField _nameInput;

        /// <summary>
        /// Slider for controlling the audioclip.
        /// </summary>
        [Tooltip("Slider for controlling the audioclip.")]
        [SerializeField]
        private ClipControlSlider _clipControlSlider;

        /// <summary>
        /// Button to start and stop the recording.
        /// </summary>
        [Tooltip("Button to start and stop the recording.")]
        [SerializeField]
        private Button _recordButton;

        /// <summary>
        /// Image component of the button's icon.
        /// </summary>
        [Tooltip("Image component of the button's icon.")]
        [SerializeField]
        private Image _buttonIcon;

        /// <summary>
        /// Icon sprite when the recording is off.
        /// </summary>
        [Tooltip("Sprite when the recording is off.")]
        [SerializeField]
        private Sprite _recordIcon;

        /// <summary>
        ///  Icon sprite when the recording is on.
        /// </summary>
        [Tooltip("Icon sprite when the recording is on.")]
        [SerializeField]
        private Sprite _stopIcon;

        /// <summary>
        /// Icon sprite to play the recording.
        /// </summary>
        [Tooltip("Icon sprite to play the recording.")]
        [SerializeField]
        private Sprite _playIcon;

        /// <summary>
        /// Text that shows the max duration of the AudioClip.
        /// </summary>
        [Tooltip("Text that shows the max duration of the AudioClip.")]
        [SerializeField]
        private Text _maxDurationText;

        /// <summary>
        /// Text for the timer that counts the seconds since the recording started.
        /// </summary>
        [Tooltip("Text for the timer that counts the seconds since the recording started.")]
        [SerializeField]
        private Text _recordingTimerText;

        /// <summary>
        /// Parent GameObject for the recording controls.
        /// </summary>
        [Tooltip("Parent GameObject for the recording controls.")]
        [SerializeField]
        private GameObject _menu;

        /// <summary>
        /// The sample rate of the AudioClip produced by the recording.
        /// </summary>
        private int _frequency;

        /// <summary>
        /// The length of the AudioClip produced by the recording.
        /// </summary>
        private int _duration;

        /// <summary>
        /// Recording manager for AudioClipRecordings.
        /// </summary>
        private RecordingsManager<AudioClipRecording> _recordingsManageer;

        /// <summary>
        /// Timer counting the time passed since starting the recording.
        /// </summary>
        private float _timer;

        /// <summary>
        /// Whether the recording must be started.
        /// </summary>
        private bool _recordingCommand;

        /// <summary>
        /// The path to the folder where new recordings are saved.
        /// </summary>
        public string ExportPath { get; private set; }

        /// <summary>
        /// Checks if the recording should be stopped automatically.
        /// Assigns the correct icon.
        /// Updates timer.
        /// </summary>
        private void FixedUpdate()
        {
            if (_recorder.GetAllDevices().Length <= 0)
                return;

            // Automatically stop recording if the max duration is reached.
            if (_recordingCommand && !_recorder.DeviceIsRecording(_deviceDropdown.options[_deviceDropdown.value].text))
                StopRecording();

            // Automatically stop playng the audio if the max duration is reached.
            if (_menu.activeSelf && _buttonIcon.sprite != _playIcon && !_clipControlSlider.AudioSource.isPlaying)
                StopAudio();

            if (_menu.activeSelf && _clipControlSlider.AudioSource.isPlaying)
                _recordingTimerText.text = TimeSpan.FromSeconds(_clipControlSlider.AudioSource.time).ToString(@"mm\:ss");

            UpdateTimer();
        }

        /// <summary>
        /// Initializes the recorder.
        /// </summary>
        /// <param name="recordingsManager">Recording manager for AudioClipRecordings.</param>
        public void Initialize(RecordingsManager<AudioClipRecording> recordingsManager)
        {
            ExportPath = Path.Combine(Application.persistentDataPath, _settings.ExportFolder);

            _clipControlSlider.ClipSlider.maxValue = _settings.MaxRecordingDuration;

            _recordingsManageer = recordingsManager;
            _maxDurationText.text = $"max.  {_settings.MaxRecordingDuration} sec";
            _frequency = _settings.Frequency;
            _duration = _settings.MaxRecordingDuration;

            _recordButton.onClick.AddListener(StartRecording);

            if (_recorder.GetAllDevices().Length <= 0)
            {
                Debug.Log("You have NO input devices to Record Sound!");
            }
            else
            {
                Debug.Log("You have " + _recorder.GetAllDevices().Length + " input device/s");
                _deviceDropdown.AddOptions(_recorder.GetAllDevices().ToList());
            }
        }

        /// <summary>
        /// Starts the recording and updates the UI.
        /// </summary>
        public void StartRecording()
        {
            if (_recorder.GetAllDevices().Length <= 0)
                return;

            _clipControlSlider.gameObject.SetActive(true);
            // Blocks the mouse from controlling the slider.
            _recordingTimerText.raycastTarget = true;

            _recorder.StartRecording(_deviceDropdown.options[_deviceDropdown.value].text, false, _duration, _frequency);

            _recordButton.onClick.RemoveAllListeners();
            _recordButton.onClick.AddListener(StopRecording);

            _recordButton.image.color = new Color32(16, 166, 210, 255);
            _buttonIcon.sprite = _stopIcon;

            _recordingCommand = true;
        }

        /// <summary>
        /// Stops the recording and updates the UI.
        /// </summary>
        public void StopRecording()
        {
            _recorder.StopRecording(_deviceDropdown.options[_deviceDropdown.value].text);

            _recordButton.onClick.RemoveAllListeners();
            _recordButton.onClick.AddListener(PlayRecording);

            _clipControlSlider.Initialize(_recorder.RecordedClip);

            _recordingTimerText.raycastTarget = false;

            _recordButton.image.color = new Color32(230, 85, 65, 255);
            _buttonIcon.sprite = _playIcon;
            _menu.SetActive(true);

            _recordingCommand = false;
        }

        /// <summary>
        /// Stops the playing audio and resets the recorder.
        /// </summary>
        public void DeleteRecording()
        {
            _clipControlSlider.StopAudio();
            ResetRecorder();
        }

        /// <summary>
        /// Saves the recording and resets the recorder.
        /// </summary>
        public void SaveRecording()
        {
            AudioClipRecording newRecording;

            if (_nameInput.text == "")
                newRecording = new AudioClipRecording(_recorder.RecordedClip.length, _recorder.RecordedClip);
            else
                newRecording = new AudioClipRecording(_nameInput.text, _recorder.RecordedClip.length, _recorder.RecordedClip);

            _recordingsManageer.SaveRecording(ExportPath, newRecording);

            ResetRecorder();
        }

        /// <summary>
        /// Plays the recorded AudioClip and updates the UI.
        /// </summary>
        public void PlayRecording()
        {
            _recordButton.onClick.RemoveAllListeners();
            _recordButton.onClick.AddListener(StopAudio);

            _recordButton.image.color = new Color32(16, 166, 210, 255);
            _buttonIcon.sprite = _stopIcon;
            _clipControlSlider.PlayAudio();
        }

        /// <summary>
        /// Stops the playing AudioClip and updates the UI.
        /// </summary>
        public void StopAudio()
        {
            _recordButton.onClick.RemoveAllListeners();
            _recordButton.onClick.AddListener(PlayRecording);

            _recordButton.image.color = new Color32(230, 85, 65, 255);
            _buttonIcon.sprite = _playIcon;

            _recordingTimerText.text = _recordingTimerText.text = TimeSpan.FromSeconds(Mathf.Round(_clipControlSlider.AudioSource.clip.length)).ToString(@"mm\:ss");
            _clipControlSlider.StopAudio();
        }

        /// <summary>
        /// Resets the UI and the recorded AudioClip.
        /// </summary>
        private void ResetRecorder()
        {
            _menu.SetActive(false);
            _recorder.ResetAudioClip();

            _recordButton.onClick.RemoveAllListeners();
            _recordButton.onClick.AddListener(StartRecording);

            _buttonIcon.sprite = _recordIcon;
            _recordingTimerText.text = "00:00";
            _clipControlSlider.ClipSlider.maxValue = _settings.MaxRecordingDuration;
            _clipControlSlider.gameObject.SetActive(false);
            _timer = 0;
        }

        /// <summary>
        /// Updates the timer that tracks the recording time.
        /// </summary>
        private void UpdateTimer()
        {
            if (_recorder.DeviceIsRecording(_deviceDropdown.options[_deviceDropdown.value].text))
            {
                UpdateSlider();
                _timer += Time.deltaTime;
                _recordingTimerText.text = TimeSpan.FromSeconds(_timer).ToString(@"mm\:ss");
            }
        }

        /// <summary>
        /// Updates the recording progress bar.
        /// </summary>
        private void UpdateSlider() => _clipControlSlider.ClipSlider.value = _timer;
    }
}