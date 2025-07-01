using DTT.PublishingTools;
using UnityEditor;
using DTT.AudioRecording.Demo;

namespace DTT.AudioRecording.Editor
{
    /// <summary>
    /// Custom editor for the BoardUI class.
    /// </summary>
    [DTTHeader("dtt.audio-recording", "Board")]
    [CustomEditor(typeof(BoardUI), true)]
    public class BoardUIEditor : DTTInspector
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
