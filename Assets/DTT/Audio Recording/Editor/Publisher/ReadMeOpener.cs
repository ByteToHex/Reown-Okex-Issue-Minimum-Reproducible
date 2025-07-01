#if UNITY_EDITOR

using DTT.PublishingTools;
using UnityEditor;

namespace DTT.AudioRecording.Editor
{
    /// <summary>
    /// Class that handles opening the editor window for the package Audio Recording.
    /// </summary>
    internal static class ReadMeOpener
    {
        /// <summary>
        /// Opens the readme for this package.
        /// </summary>
        [MenuItem("Tools/DTT/Audio Recording/ReadMe")]
        private static void OpenReadMe() => DTTEditorConfig.OpenReadMe("dtt.audio-recording");
    }
}
#endif