using UnityEditor;
using UnityEngine;

namespace RhythmEngine
{
    [CustomPropertyDrawer(typeof(BeatSequenceChange))]
    public class BeatSequenceChangeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //draw both fields on the same line

            var savedLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 100;

            var rect = EditorGUI.IndentedRect(position);
            rect.width *= 0.5f;
            rect.width -= 8;
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("Time"));
            rect.x += rect.width + 8;
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("SequenceIndex"));

            EditorGUIUtility.labelWidth = savedLabelWidth;

            var beatSequencer = property.serializedObject.targetObject as BeatSequencer;
            var song = property.serializedObject.targetObject as BeatSequencedSong;
            if (beatSequencer == null && song == null) return;

            var beatSequenceCount = beatSequencer != null ? beatSequencer.BeatSequences.BeatSequences.Count : song.BeatSequences.BeatSequences.Count;

            var sequenceIndex = property.FindPropertyRelative("SequenceIndex").intValue;

            if (sequenceIndex < 0 || sequenceIndex >= beatSequenceCount)
            {
                var rect2 = EditorGUI.IndentedRect(position);
                rect2.y += EditorGUIUtility.singleLineHeight * 1.15f;
                rect2.height = EditorGUIUtility.singleLineHeight;
                rect2.width -= 8;

                if (sequenceIndex < -1)
                    EditorGUI.HelpBox(rect2, "Sequence index out of range.", MessageType.Error);
                else
                    EditorGUI.HelpBox(rect2, "Sequence index -1 is an empty sequence.", MessageType.Info);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var baseHeight = base.GetPropertyHeight(property, label);

            var beatSequencer = property.serializedObject.targetObject as BeatSequencer;
            var song = property.serializedObject.targetObject as BeatSequencedSong;
            if (beatSequencer == null && song == null) return baseHeight;

            var beatSequenceCount = beatSequencer != null ? beatSequencer.BeatSequences.BeatSequences.Count : song.BeatSequences.BeatSequences.Count;

            var sequenceIndex = property.FindPropertyRelative("SequenceIndex").intValue;
            if (sequenceIndex < 0 || sequenceIndex >= beatSequenceCount)
            {
                return baseHeight + EditorGUIUtility.singleLineHeight * 1.3f;
            }

            return baseHeight;
        }
    }
}
