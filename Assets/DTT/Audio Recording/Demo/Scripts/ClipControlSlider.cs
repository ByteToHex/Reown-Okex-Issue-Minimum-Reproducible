using UnityEngine;
using UnityEngine.UI;

namespace DTT.AudioRecording.Demo
{
    public class ClipControlSlider : MonoBehaviour
    {
        /// <summary>
        /// Slider used to control the AudioClip.
        /// </summary>
        [Tooltip("Slider used to control the AudioClip.")]
        [SerializeField]
        private Slider _clipSlider;

        /// <summary>
        /// AudioSource component used to play the AudioClip.
        /// </summary>
        [Tooltip("AudioSource component used to play the AudioClip.")]
        [SerializeField]
        private AudioSource _audioSource;

        /// <summary>
        /// Slider used to control the AudioClip.
        /// </summary>
        public Slider ClipSlider => _clipSlider;

        /// <summary>
        /// AudioSource component used to play the AudioClip.
        /// </summary>
        public AudioSource AudioSource => _audioSource;

        /// <summary>
        /// Used to force the slider to follow the mouse when clicked.
        /// </summary>
        private bool _sliderClicked = false;

        /// <summary>
        /// Initializes clip and slider.
        /// </summary>
        /// <param name="audioClip">Audio clip to control.</param>
        public void Initialize(AudioClip audioClip)
        {
            _clipSlider.maxValue = audioClip.length;
            _audioSource.clip = audioClip;
            _clipSlider.value = 0;
        }

        /// <summary>
        /// Tracks the palying audio.
        /// </summary>
        private void FixedUpdate()
        {
            if (!_sliderClicked && _audioSource.isPlaying)
                _clipSlider.value = _audioSource.time;
        }

        /// <summary>
        /// Plays the clip from the selected starting point.
        /// </summary>
        public void PlayAudio()
        {
            _audioSource.Play();
            _audioSource.time = _clipSlider.value;
        }

        /// <summary>
        /// Stops playing the clip and resets the slider.
        /// </summary>
        public void StopAudio()
        {
            _audioSource.Stop();
            _clipSlider.value = 0;
        }

        /// <summary>
        /// Checks if the PointedDown event has been triggered on the slider.
        /// </summary>
        public void OnPointerDown() => _sliderClicked = true;

        /// <summary>
        /// Checks if the PointedUp event has been triggered on the slider and skips the track to the selected time.
        /// </summary>
        public void OnPointerUp()
        {
            _audioSource.time = _clipSlider.value;
            _sliderClicked = false;
        }
    }
}