using System;

namespace DTT.AudioRecording
{
    /// <summary>
    /// Base class for all recordings.
    /// </summary>
    public abstract class Recording
    {
        /// <summary>
        /// Name of the recording.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Duration of the recording.
        /// </summary>
        public float Duration { get; private set; }

        /// <summary>
        /// Constructor with a custom name.
        /// </summary>
        /// <param name="name">Name of the recording.</param>
        /// <param name="duration">Duration of the recording.</param>
        public Recording(string name, float duration)
        {
            Name = name;
            Duration = duration;
        }

        /// <summary>
        /// Constructor with a timestamp as a name.
        /// </summary>
        /// <param name="duration">Duration of the recording.</param>
        public Recording(float duration)
        {
            Name = DateTime.Now.ToString(@"yyyy-MM-dd HH-mm-ss");
            Duration = duration;
        }

        /// <summary>
        /// Displays recording details.
        /// </summary>
        /// <returns>String with name and duration.</returns>
        public override string ToString() => $"Name: {Name}, Duration: {Duration}";
    }
}