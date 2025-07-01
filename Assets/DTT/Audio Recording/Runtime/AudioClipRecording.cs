using UnityEngine;

namespace DTT.AudioRecording
{
    /// <summary>
    /// Data class for Recordings with an AudioClip.
    /// </summary>
    public class AudioClipRecording : Recording
    {
        /// <summary>
        /// AudioClip property.
        /// </summary>
        public AudioClip Clip { get; private set; }

        /// <summary>
        /// Constructor with a custom name.
        /// </summary>
        /// <param name="name">AudioClip name.</param>
        /// <param name="duration">AudioClip duration.</param>
        /// <param name="clip">The AudioClip itself.</param>
        public AudioClipRecording(string name, float duration, AudioClip clip) : base(name, duration) => Clip = clip;

        /// <summary>
        /// Constructor with a timestamp as a name.
        /// </summary>
        /// <param name="duration">AudioClip duration.</param>
        /// <param name="clip">The AudioClip itself.</param>
        public AudioClipRecording(float duration, AudioClip clip) : base(duration) => Clip = clip;
    }
}
