using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RhythmEngine
{
    [CustomEditor(typeof(ParticlesToBand))]
    public class ParticlesToBandEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();


            EditorGUILayout.PropertyField(serializedObject.FindProperty("AudioBandListener"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Band"));

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("UseBufferedValues"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("LerpSpeed"));

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("AffectBaseSettings"));
            var baseSettings = serializedObject.FindProperty("AffectBaseSettings").boolValue;
            if (baseSettings)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("StartLifetime"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("StartSpeed"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("StartSize"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("StartRotation"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("StartColor"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("GravityModifier"));

                EditorGUILayout.Space();
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("AffectEmission"));
            var emission = serializedObject.FindProperty("AffectEmission").boolValue;
            if (emission)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("EmissionRateOverTime"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("EmissionRateOverDistance"));

                EditorGUILayout.Space();
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("AffectShape"));
            var shape = serializedObject.FindProperty("AffectShape").boolValue;
            if (shape)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ShapePosition"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ShapeRotation"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ShapeScale"));

                EditorGUILayout.Space();
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("AffectRuntime"));
            var runtime = serializedObject.FindProperty("AffectRuntime").boolValue;
            if (runtime)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ParticleSize"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ParticleRotation"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ParticleColor"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
