using UnityEditor;
using UnityEngine;

namespace RhythmEngine
{
    [CustomEditor(typeof(AutoBeatSequencer))]
    public class AutoBeatSequencerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var scriptProperty = serializedObject.FindProperty("m_Script");
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(scriptProperty);
            EditorGUI.EndDisabledGroup();

            var rhythmEngineProperty = serializedObject.FindProperty("RhythmEngine");
            EditorGUILayout.PropertyField(rhythmEngineProperty);

            EditorGUILayout.Space();
            GUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
            GUILayout.Label("Automatic Beat Sequencer, takes the Beat Sequences and Beat Sequence Changes from the song", EditorStyles.Label);
            GUILayout.Label("The song supplied must be of type BeatSequencedSong or any derived type.", EditorStyles.Label);
            GUILayout.EndVertical();

            var rhythmEngine = (RhythmEngineCore)rhythmEngineProperty.objectReferenceValue;

            if (rhythmEngine != null && rhythmEngine.Song != null && GUILayout.Button("Edit Sequences in the Song Asset", ButtonStyles.UnselectedButton, GUILayout.MinHeight(25)))
            {
                Selection.activeObject = rhythmEngine.Song;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
