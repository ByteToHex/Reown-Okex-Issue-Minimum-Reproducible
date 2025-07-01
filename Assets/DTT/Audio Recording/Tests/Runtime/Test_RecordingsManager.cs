#if TEST_FRAMEWORK

using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace DTT.AudioRecording.Tests.Runtime
{
    /// <summary>
    /// Test class for the <see cref="RecordingsManager{AudioClipRecording}"/>.
    /// </summary>
    public class Test_RecordingsManager
    {
        /// <summary>
        /// Setup a file for load and delete testing.
        /// </summary>
        [OneTimeSetUp]
        public void Setup()
        {
            Action<AudioClipRecording> callback = null;
            GameObject gameObject = new GameObject();
            WAVSaver saver = gameObject.AddComponent<WAVSaver>();
            AudioClip sampleClip = AudioClip.Create("Test_RecordingsManagerLoad", 1, 1, 44100, false);
            AudioClip sampleClip2 = AudioClip.Create("Test_RecordingsManagerDelete", 1, 1, 44100, false);
            AudioClipRecording newRecording = new AudioClipRecording("Test_RecordingsManagerLoad", 1, sampleClip);
            AudioClipRecording newRecording2 = new AudioClipRecording("Test_RecordingsManagerDelete", 1, sampleClip2);

            string path = Application.persistentDataPath;

            saver.Save(path, newRecording, callback);
            saver.Save(path, newRecording2, callback);
        }

        /// <summary>
        /// Tests the saving of recordings.
        /// </summary>
        [Test]
        public void Test_SaveRecording()
        {
            // Arrange.
            GameObject gameObject = new GameObject();
            WAVSaver saver = gameObject.AddComponent<WAVSaver>();
            RecordingsManager<AudioClipRecording> recordingsManager = new RecordingsManager<AudioClipRecording>(saver);
            AudioClip sampleClip = AudioClip.Create("Test_RecordingsManagerSaveRecording", 1, 1, 44100, false);
            AudioClipRecording newRecording = new AudioClipRecording("Test_RecordingsManagerSaveRecording", 1, sampleClip);
            string path = Application.persistentDataPath;

            // Act.
            recordingsManager.SaveRecording(path, newRecording);

            // Assert.
            Assert.IsTrue(File.Exists(path + "/Test_RecordingsManagerSaveRecording.wav"));
        }

        /// <summary>
        /// Tests the loading of recordings.
        /// </summary>
        [Test]
        public void Test_LoadRecordings()
        {
            // Arrange.
            GameObject gameObject = new GameObject();
            WAVSaver saver = gameObject.AddComponent<WAVSaver>();
            RecordingsManager<AudioClipRecording> recordingsManager = new RecordingsManager<AudioClipRecording>(saver);
            string path = Application.persistentDataPath;

            // Act.
            recordingsManager.LoadRecordings(path);

            // Assert.
            Assert.IsTrue(saver.Recordings.Where(x => x.Name == "Test_RecordingsManagerLoad") != null);
        }

        /// <summary>
        /// Tests the deletion of a recording.
        /// </summary>
        [Test]
        public void Test_DeleteRecording()
        {
            // Arrange.
            GameObject gameObject = new GameObject();
            WAVSaver saver = gameObject.AddComponent<WAVSaver>();
            RecordingsManager<AudioClipRecording> recordingsManager = new RecordingsManager<AudioClipRecording>(saver);
            AudioClip sampleClip = AudioClip.Create("Test_RecordingsManagerDelete.wav", 1, 1, 44100, false);
            AudioClipRecording newRecording = new AudioClipRecording("Test_RecordingsManagerDelete.wav", 1, sampleClip);
            string path = Application.persistentDataPath;

            // Act.
            recordingsManager.DeleteRecording(path, newRecording);

            // Assert.
            Assert.IsFalse(File.Exists(path + "/Test_RecordingsManagerDelete.wav")); ;
        }

        /// <summary>
        /// Delete the testing files.
        /// </summary>
        [OneTimeTearDown]
        public void Cleanup()
        {
            File.Delete(Application.persistentDataPath + "/Test_RecordingsManagerDelete.wav");
            File.Delete(Application.persistentDataPath + "/Test_RecordingsManagerLoad.wav");
            File.Delete(Application.persistentDataPath + "/Test_RecordingsManagerSaveRecording.wav");
        }
    }
}

#endif