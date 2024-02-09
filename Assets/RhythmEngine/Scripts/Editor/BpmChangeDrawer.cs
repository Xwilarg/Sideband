using UnityEditor;
using UnityEngine;

namespace RhythmEngine
{
    [CustomPropertyDrawer(typeof(Song.BpmChange))]
    public class BpmChangeDrawer : PropertyDrawer
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
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("Bpm"));

            EditorGUIUtility.labelWidth = savedLabelWidth;
        }
    }
}
