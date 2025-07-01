using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace DTT.AudioRecording
{
    /// <summary>
    /// Recording saver for the WAV format.
    /// </summary>
    public class WAVSaver : MonoBehaviour, IRecordingSaver<AudioClipRecording>
    {
        /// <summary>
        /// Collection with all AudioClipRecordings.
        /// </summary>
        public List<AudioClipRecording> Recordings { get; private set; }

        /// <summary>
        /// Deletes a recording and invokes a callback event.
        /// </summary>
        /// <param name="path">File path of the recording.</param>
        /// <param name="recording">Recording to delete.</param>
        /// <param name="callback">Callback event to invoke.</param>
        public void Delete(string path, AudioClipRecording recording, Action callback)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            FileInfo[] file = directoryInfo.GetFiles(recording.Name);

            if (file.Length <= 0)
            {
                Debug.LogError("Recording not found.");
                return;
            }

            File.Delete(file[0].ToString());

            callback?.Invoke();
        }

        /// <summary>
        /// Saves a new recording and invokes a callback event with the saved Recording.
        /// </summary>
        /// <param name="path">File path for saving.</param>
        /// <param name="recording">Recording to save.</param>
        /// <param name="callback">Callback event to invoke.</param>
        public void Save(string path, AudioClipRecording recording, Action<AudioClipRecording> callback)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            SavWav.Save(recording.Name, path, recording.Clip);
            callback?.Invoke(recording);
        }

        /// <summary>
        /// Loads the data from a given path and invokes a callback event with an array of recordings.
        /// </summary>
        /// <param name="path">Folder path.</param>
        /// <param name="callback">Callback event.</param>
        public void Load(string path, Action<AudioClipRecording[]> callback)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            Recordings = new List<AudioClipRecording>();
            StartCoroutine(GetAudioClips(path, callback));
        }

        /// <summary>
        /// Coroutine that searches through the given directory.
        /// </summary>
        /// <param name="path">Directory path.</param>
        /// <param name="callback">Callback event for the loaded recordings.</param>
        /// <returns>StartCoroutine(SendWebRequest(wavFile.FullName, wavFile.Name))</returns>
        private IEnumerator GetAudioClips(string path, Action<AudioClipRecording[]> callback)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            FileInfo[] wavFiles = directoryInfo.GetFiles("*.wav", SearchOption.AllDirectories).OrderBy(x => x.CreationTime).ToArray();

            // Sends a web request for each found WAV file.
            foreach (FileInfo wavFile in wavFiles)
                yield return StartCoroutine(SendWebRequest(wavFile.FullName, wavFile.Name));

            callback?.Invoke(Recordings.ToArray());
        }

        /// <summary>
        /// Coroutine that tries to load an AudioClip from the WAV files.
        /// </summary>
        /// <param name="uri">File path.</param>
        /// <param name="fileName">File name.</param>
        /// <returns>request.SendWebRequest()</returns>
        private IEnumerator SendWebRequest(string uri, string fileName)
        {
            UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip("file:///" + uri, AudioType.WAV);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(request.error);
                yield break;
            }

            AudioClip myClip = DownloadHandlerAudioClip.GetContent(request);
            AudioClipRecording newRecording = new AudioClipRecording(fileName, myClip.length, myClip);
            Recordings.Add(newRecording);
        }
    }
}