using UnityEngine;
using DTT.PublishingTools;
using UnityEditor;
using DTT.AudioRecording.Demo;
using System.IO;

namespace DTT.AudioRecording.Editor
{
    /// <summary>
    /// Custom editor for the RecordingSettings class.
    /// </summary>
    [DTTHeader("dtt.audio-recording", "Recording Settings")]
    [CustomEditor(typeof(RecordingSettings), true)]
    public class RecordingSettingsEditor : DTTInspector
    {
        /// <summary>
        /// Position used for the ScrollView.
        /// </summary>
        private Vector2 _scrollPosition = Vector2.zero;

        /// <summary>
        /// Folder where new recordings are being saved.
        /// </summary>
        private SerializedProperty _exportFolder;

        /// <summary>
        /// The default folder to save recordings.
        /// </summary>
        private string _defaultExportFolder = "Recordings";

        /// <summary>
        /// Styling for the folder icon button.
        /// </summary>
        private GUIStyle _iconButtonStyle;

        /// <summary>
        /// Gets the reqired properties.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            _exportFolder = serializedObject.FindProperty("_exportFolder");

            _defaultExportFolder = "Recordings";
        }

        /// <summary>
        /// Draws the export folder path and controls.
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            DrawPropertiesExcluding(serializedObject, "m_Script", "_exportFolder");

            // Initialize the button style.
            _iconButtonStyle = new GUIStyle(GUI.skin.button)
            {
                padding = new RectOffset()
            };

            // If the path is empty assign it to the persistend data path.
            if (_exportFolder.stringValue == "")
                _exportFolder.stringValue = _defaultExportFolder;

            EditorGUILayout.Space(20);

            EditorGUILayout.BeginHorizontal();
            {
                GUIContent content = new GUIContent("Relative export folder:", "The folder at Application.persistentDataPath where recording are saved.");
                EditorGUILayout.PrefixLabel(content);

                    _exportFolder.stringValue = EditorGUILayout.TextArea(_exportFolder.stringValue);
                if (GUILayout.Button("Reset", GUILayout.Width(50), GUILayout.Height(22)))
                    _exportFolder.stringValue = _defaultExportFolder;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();

                // Fixes the exception that is thrown when switching custom folder locations.
                GUIUtility.ExitGUI();
            }
        }
    }
}