using System;

namespace DTT.AudioRecording
{
    /// <summary>
    /// Manager class for the recordings.
    /// </summary>
    /// <typeparam name="T">Class that extends Recording.</typeparam>
    public class RecordingsManager<T> where T : Recording
    {
        /// <summary>
        /// Event invoked when the recordings have been loaded.
        /// </summary>
        public event Action<T[]> RecordingsLoaded;

        /// <summary>
        /// Event invoked when the recording has been saved.
        /// </summary>
        public event Action<T> RecordingSaved;

        /// <summary>
        /// Event invoked when the recording has been deleted.
        /// </summary>
        public event Action RecordingDeleted;

        /// <summary>
        /// The saver interface used to load, save and delete data.
        /// </summary>
        private readonly IRecordingSaver<T> _saver;

        /// <summary>
        /// Constructor that initializes your recordings saver.
        /// </summary>
        /// <param name="saver">Any class that implements IRecordingSaver. A class that extends Recording should be passed as a generic argument.</param>
        public RecordingsManager(IRecordingSaver<T> saver) => _saver = saver;

        /// <summary>
        /// Loads the recordings.
        /// </summary>
        /// <param name="path">The folder path.</param>
        public void LoadRecordings(string path) => _saver.Load(path, RecordingsLoaded);

        /// <summary>
        /// Saves a new recording 
        /// </summary>
        /// <param name="path">File path.</param>
        /// <param name="recording">Recording to save.</param>
        public void SaveRecording(string path, T recording) => _saver.Save(path, recording, RecordingSaved);

        /// <summary>
        /// Deletes a recording.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <param name="recording">Recording to delete.</param>
        public void DeleteRecording(string path, T recording) => _saver.Delete(path, recording, RecordingDeleted);
    }
}