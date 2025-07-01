using System;

namespace DTT.AudioRecording
{
    /// <summary>
    /// Implement this interface when creating your custom saver.
    /// </summary>
    /// <typeparam name="T">Class that extends Recording.</typeparam>
    public interface IRecordingSaver<T> where T : Recording
    {
        /// <summary>
        /// Loads the data from a given path and invokes a callback event with an array of recordings.
        /// </summary>
        /// <param name="path">Folder path.</param>
        /// <param name="callback">Callback event.</param>
        void Load(string path, Action<T[]> callback);

        /// <summary>
        /// Saves a new recording and invokes a callback event with the saved Recording.
        /// </summary>
        /// <param name="path">File path for saving.</param>
        /// <param name="recording">Recording to save.</param>
        /// <param name="callback">Callback event to invoke.</param>
        void Save(string path, T recording, Action<T> callback);

        /// <summary>
        /// Deletes a recording and invokes a callback event.
        /// </summary>
        /// <param name="path">File path of the recording.</param>
        /// <param name="recording">Recording to delete.</param>
        /// <param name="callback">Callback event to invoke.</param>
        void Delete(string path, T recording, Action callback);
    }
}