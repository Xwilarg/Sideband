using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RhythmEngine
{
    [CustomEditor(typeof(AudioBandListener))]
    public class AudioBandListenerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
            EditorGUILayout.LabelField("The AudioBandListener can be used to analyze the frequency data of the playing audio.", EditorStyles.Label);
            EditorGUILayout.LabelField("It can be used with components like ScaleToBand or ColorToBand for codeless audio-synced visuals.", EditorStyles.Label);
            EditorGUILayout.LabelField("Use the StandaloneMode if you want to use the AudioBandListener without the RhythmEngine setup.", EditorStyles.Label);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
            var standalone = serializedObject.FindProperty("StandaloneMode").boolValue;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("StandaloneMode"));
            EditorGUILayout.PropertyField(!standalone ? serializedObject.FindProperty("RhythmEngine") : serializedObject.FindProperty("AudioSource"));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // Draw the rest of the inspector
            var property = serializedObject.GetIterator();
            var next = property.NextVisible(true);
            while (next)
            {
                if (property.name != "m_Script" && property.name != "StandaloneMode" && property.name != "RhythmEngine" && property.name != "AudioSource")
                {
                    EditorGUILayout.PropertyField(property);
                }

                next = property.NextVisible(false);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
