using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace DTT.AudioRecording.Demo
{
    /// <summary>
    /// UI manager class for the recordings.
    /// </summary>
    public class BoardUI : MonoBehaviour
    {
        /// <summary>
        /// UI manager class for the recorder.
        /// </summary>
        [Tooltip("UI manager class for the recorder.")]
        [SerializeField]
        private MicrophoneUI _microphoneUI;

        /// <summary>
        /// The recording saver.
        /// </summary>
        [Tooltip("The recording saver.")]
        [SerializeField]
        private WAVSaver _wavSaver;

        /// <summary>
        /// Prefab for the recording entries.
        /// </summary>
        [Tooltip("Prefab for the recording entries.")]
        [SerializeField]
        private RecordingHeader _recordingPrefab;

        /// <summary>
        /// ScrollRect of the recording container.
        /// </summary>
        [Tooltip("ScrollRect of the recording container.")]
        [SerializeField]
        private ScrollRect _scrollRect;

        /// <summary>
        /// Parent object for the rows.
        /// </summary>
        [SerializeField]
        [Tooltip("Parent object of the rows.")]
        private GameObject _rowContainer;

        /// <summary>
        /// Object for the delete menu.
        /// </summary>
        [SerializeField]
        [Tooltip("Object for the delete menu.")]
        private GameObject _deleteRecordingPopup;
        
        /// <summary>
        /// Object for the sharing not supported menu.
        /// </summary>
        [SerializeField]
        [Tooltip("Object for the sharing not supported menu.")]
        private GameObject _sharingNotSupportedPopUp;

        /// <summary>
        /// Recording manager class for AudioClipRecordings.
        /// </summary>
        private RecordingsManager<AudioClipRecording> _recordingsManager;

        /// <summary>
        /// All instantiated Recording objects.
        /// </summary>
        private List<RecordingHeader> _recordingObjecs;

        /// <summary>
        /// Recording that is about to be deleted.
        /// Used for the delete popup menu.
        /// </summary>
        private AudioClipRecording _recordingToDelete;

        /// <summary>
        /// Used to share an audio recording
        /// </summary>
        private SharingAudioRecording _sharingAudioRecording;

        /// <summary>
        /// Initializes the necessary objects.
        /// </summary>
        private void Awake()
        {
            _recordingsManager = new RecordingsManager<AudioClipRecording>(_wavSaver);
            _recordingObjecs = new List<RecordingHeader>();
            _sharingAudioRecording = new SharingAudioRecording(_sharingNotSupportedPopUp);

            _microphoneUI.Initialize(_recordingsManager);

            _recordingsManager.RecordingsLoaded += OnRecordingsLoaded;
            _recordingsManager.RecordingSaved += OnRecordingSaved;

            GenerateBoard();
        }

        /// <summary>
        /// Clears the board and loads the recordings.
        /// </summary>
        public void GenerateBoard()
        {
            ClearBoard();

            _deleteRecordingPopup.SetActive(false);

            _recordingsManager.LoadRecordings(_microphoneUI.ExportPath);
        }

        /// <summary>
        /// Destroys each instantiated recording object and clears the list.
        /// </summary>
        public void ClearBoard()
        {
            foreach (RecordingHeader recordingHeader in _recordingObjecs)
                Destroy(recordingHeader.gameObject);

            _recordingObjecs.Clear();
        }

        /// <summary>
        /// Used in the pop up menu to delete the selected recording.
        /// </summary>
        public void ConfirmDeletion()
        {
            _recordingsManager.DeleteRecording(_microphoneUI.ExportPath, _recordingToDelete);
            GenerateBoard();
        }

        /// <summary>
        /// SharingAudioRecording a recording.
        /// </summary>
        /// <param name="_recordingToShare">Recording to share.</param>
        private void OnShare(AudioClipRecording recordingToShare)
        {
            _sharingAudioRecording.Sharing(recordingToShare, _microphoneUI.ExportPath);
        }
        
        /// <summary>
        /// Cancel the sharing.
        /// </summary>
        public void CancelSharing() => _sharingAudioRecording.CancelSharing();
        
        
        /// <summary>
        /// Used in the pop up menu to cancel deleting a selected recording.
        /// </summary>
        public void CancelDeletion()
        {
            _deleteRecordingPopup.SetActive(false);
            _recordingToDelete = null;
        }

        /// <summary>
        /// Generates the board when the recordings have been loaded.
        /// </summary>
        /// <param name="recordings">Collection with loaded recordings.</param>
        private void OnRecordingsLoaded(AudioClipRecording[] recordings)
        {
            foreach (AudioClipRecording recording in recordings)
            {
                RecordingHeader recordingHeader = Instantiate(_recordingPrefab, _rowContainer.transform, false);
                recordingHeader.Initialize(recording);
                _recordingObjecs.Add(recordingHeader);
            }

            // Subscribes to the delete event.
            foreach (RecordingHeader recordingHeader in _recordingObjecs)
            {
                recordingHeader.DeletedRecording += OnDeletedRecording;
                recordingHeader.ShareRecording += OnShare;
            }

            _scrollRect.verticalNormalizedPosition = 1;
        }
        
        /// <summary>
        /// Refreshes the board when a new recording is saved.
        /// </summary>
        /// <param name="recording">The saved recording.</param>
        private void OnRecordingSaved(AudioClipRecording recording) => GenerateBoard();

        /// <summary>
        /// Listens to the delete event of the recording.
        /// Deletes the selected recording and refreshes the board.
        /// </summary>
        /// <param name="recording">Recording to delete.</param>
        private void OnDeletedRecording(AudioClipRecording recording)
        {
            _deleteRecordingPopup.SetActive(true);
            _recordingToDelete = recording;
        }

        /// <summary>
        /// Unsubscribes from the events.
        /// </summary>
        private void OnDisable()
        {
            _recordingsManager.RecordingsLoaded -= OnRecordingsLoaded;
            _recordingsManager.RecordingSaved -= OnRecordingSaved;

            foreach (RecordingHeader recordingHeader in _recordingObjecs)
                recordingHeader.DeletedRecording -= OnDeletedRecording;
        }
    }
}

