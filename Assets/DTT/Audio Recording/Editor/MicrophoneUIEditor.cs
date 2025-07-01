using DTT.PublishingTools;
using UnityEditor;
using DTT.AudioRecording.Demo;

namespace DTT.AudioRecording.Editor
{
    /// <summary>
    /// Custom editor for the MicrophoneUI class.
    /// </summary>
    [DTTHeader("dtt.audio-recording", "Microphone UI")]
    [CustomEditor(typeof(MicrophoneUI), true)]
    public class MicrophoneUIEditor : DTTInspector
    {
        /// <summary>
        /// Places a DTT header in the inspector and keeps the default body.
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawDefaultInspector();
        }
    }
}