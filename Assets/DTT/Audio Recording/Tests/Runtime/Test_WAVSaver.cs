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
    /// Test class for the <see cref="WAVSaver"/>.
    /// </summary>
    public class Test_WAVSaver
    {
        /// <summary>
        /// Setup a file for load testing.
        /// </summary>
        [OneTimeSetUp]
        public void Setup()
        {
            Action<AudioClipRecording> callback = null;
            GameObject gameObject = new GameObject();
            WAVSaver saver = gameObject.AddComponent<WAVSaver>();
            AudioClip sampleClip = AudioClip.Create("Test_WAVSaverLoad", 1, 1, 44100, false);
            AudioClip sampleClip2 = AudioClip.Create("Test_WAVSaverDelete", 1, 1, 44100, false);
            AudioClipRecording newRecording = new AudioClipRecording("Test_WAVSaverLoad", 1, sampleClip);
            AudioClipRecording newRecording2 = new AudioClipRecording("Test_WAVSaverDelete", 1, sampleClip2);

            string path = Application.persistentDataPath;

            saver.Save(path, newRecording, callback);
            saver.Save(path, newRecording2, callback);
        }

        /// <summary>
        /// Tests the saving of recordings.
        /// </summary>
        [Test]
        public void Test_Save()
        {
            // Arrange.
            Action<AudioClipRecording> callback = null;
            GameObject gameObject = new GameObject();
            WAVSaver saver = gameObject.AddComponent<WAVSaver>();
            AudioClip sampleClip = AudioClip.Create("Test_WAVSaverSave", 1, 1, 44100, false);
            AudioClipRecording newRecording = new AudioClipRecording("Test_WAVSaverSave", 1, sampleClip);
            string path = Application.persistentDataPath;

            // Act.
            saver.Save(path, newRecording, callback);

            // Assert.
            Assert.IsTrue(File.Exists(path + "/Test_WAVSaverSave.wav"));
        }

        /// <summary>
        /// Tests the loading of recordings.
        /// </summary>
        [Test]
        public void Test_Load()
        {
            // Arrange.
            Action<AudioClipRecording[]> callback = null;
            GameObject gameObject = new GameObject();
            WAVSaver saver = gameObject.AddComponent<WAVSaver>();
            string path = Application.persistentDataPath;

            // Act.
            saver.Load(path, callback);

            // Assert.
            Assert.IsTrue(saver.Recordings.Where(x => x.Name == "Test_WAVSaverLoad") != null);
        }

        /// <summary>
        /// Tests the deletion of a recording.
        /// </summary>
        [Test]
        public void Test_Delete()
        {
            // Arrange.
            GameObject gameObject = new GameObject();
            WAVSaver saver = gameObject.AddComponent<WAVSaver>();
            RecordingsManager<AudioClipRecording> recordingsManager = new RecordingsManager<AudioClipRecording>(saver);
            AudioClip sampleClip = AudioClip.Create("Test_WAVSaverDelete.wav.wav", 1, 1, 44100, false);
            AudioClipRecording newRecording = new AudioClipRecording("Test_WAVSaverDelete.wav", 1, sampleClip);
            string path = Application.persistentDataPath;

            // Act.
            recordingsManager.DeleteRecording(path, newRecording);

            // Assert.
            Assert.IsFalse(File.Exists(path + "/Test_WAVSaverDelete.wav")); ;
        }

        /// <summary>
        /// Delete the testing files.
        /// </summary>
        [OneTimeTearDown]
        public void Cleanup()
        {
            File.Delete(Application.persistentDataPath + "/Test_WAVSaverDelete.wav");
            File.Delete(Application.persistentDataPath + "/Test_WAVSaverLoad.wav");
            File.Delete(Application.persistentDataPath + "/Test_WAVSaverSave.wav");
        }
    }
}

#endif