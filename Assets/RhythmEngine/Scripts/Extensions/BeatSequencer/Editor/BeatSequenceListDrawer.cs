using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace RhythmEngine
{
    [CustomPropertyDrawer(typeof(BeatSequenceList))]
    public class BeatSequenceListDrawer : PropertyDrawer
    {
        private SerializedProperty _prop;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _prop = property;

            var list = property.FindPropertyRelative("BeatSequences");
            var beatSequences = BeatSequenceList(list);

            EditorGUI.BeginProperty(position, label, property);

            Label(ref position, "Beat sequences:");

            Line(ref position);
            position.y += EditorGUIUtility.singleLineHeight * 0.25f;

            if (list.arraySize == 0)
            {
                Label(ref position, "- Empty -");
            }
            else
            {
                for (int i = 0; i < list.arraySize; i++)
                {
                    DrawSequence(ref position, beatSequences, i);

                    position.y += EditorGUIUtility.singleLineHeight * 5;
                }
            }

            var buttonRect = EditorGUI.IndentedRect(position);
            buttonRect.x += buttonRect.width * 0.3f - 8;
            buttonRect.width *= 0.35f;
            buttonRect.height = EditorGUIUtility.singleLineHeight;

            if (GUI.Button(buttonRect, "Create New", ButtonStyles.UnselectedButton))
            {
                var path = EditorUtility.SaveFilePanelInProject("Create Beat Sequence", "New Beat Sequence", "asset", "Create a new beat sequence asset.");

                if (!string.IsNullOrWhiteSpace(path))
                {
                    var sequence = ScriptableObject.CreateInstance<BeatSequence>();
                    AssetDatabase.CreateAsset(sequence, path);
                    AssetDatabase.SaveAssets();

                    Undo.RecordObject(property.serializedObject.targetObject, "Add Beat Sequence");
                    beatSequences.Add(sequence);
                }

                GUIUtility.ExitGUI();
            }

            buttonRect.x += buttonRect.width + 8;

            if (GUI.Button(buttonRect, "Select Existing", ButtonStyles.UnselectedButton))
            {
                var existingSequenceAsset = EditorUtility.OpenFilePanelWithFilters("Select Beat Sequence", "Assets", new[] { "Beat Sequence", "asset" });

                if (!string.IsNullOrWhiteSpace(existingSequenceAsset))
                {
                    var relativePath = existingSequenceAsset.Replace(Application.dataPath, "Assets");
                    var sequence = AssetDatabase.LoadAssetAtPath<BeatSequence>(relativePath);

                    if (sequence != null)
                    {
                        Undo.RecordObject(property.serializedObject.targetObject, "Add Beat Sequence");
                        beatSequences.Add(sequence);
                    }
                }

                GUIUtility.ExitGUI();
            }

            position.y += EditorGUIUtility.singleLineHeight * 0.5f;

            Space(ref position);
            Line(ref position);

            position.y += EditorGUIUtility.singleLineHeight * 0.25f;

            EditorGUI.EndProperty();
        }

        private void DrawSequence(ref Rect position, BeatSequenceList beatSequences, int index = 0)
        {
            var sequence = beatSequences[index];

            var previewRect = EditorGUI.IndentedRect(position);
            previewRect.height = EditorGUIUtility.singleLineHeight * 4.25f;

            EditorGUI.HelpBox(previewRect, "", MessageType.None);

            var labelRect = EditorGUI.IndentedRect(position);
            var margin = previewRect.width * 0.01f;
            var width = previewRect.width - margin * 2;
            labelRect.height = EditorGUIUtility.singleLineHeight;
            labelRect.width = 145;
            labelRect.x += margin;

            var labelStyle = new GUIStyle(UnityEditor.EditorStyles.label)
            {
                fontSize = 10
            };

            var boldLabelStyle = new GUIStyle(UnityEditor.EditorStyles.label)
            {
                fontStyle = FontStyle.Bold
            };

            EditorGUI.LabelField(labelRect, index == 0 ? $"Sequence {index} (default)" : $"Sequence {index}", boldLabelStyle);

            labelRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(labelRect, $"Step count: {sequence.StepCount}", labelStyle);

            labelRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(labelRect, $"Instrument count: {sequence.InstrumentCount}", labelStyle);

            labelRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(labelRect, $"Sequence length: {sequence.SequenceLength} Bar(s)", labelStyle);

            var buttonRect = EditorGUI.IndentedRect(position);
            buttonRect.x += width * 0.8f - margin;
            buttonRect.y += EditorGUIUtility.singleLineHeight * 0.25f;
            buttonRect.width = width * 0.2f;
            buttonRect.height = EditorGUIUtility.singleLineHeight;

            var sequenceRect = EditorGUI.IndentedRect(position);
            sequenceRect.height = EditorGUIUtility.singleLineHeight * 4.25f;
            sequenceRect.width = width - labelRect.width - buttonRect.width - margin * 3;
            sequenceRect.x += labelRect.width + margin;

            EditorGUI.HelpBox(sequenceRect, "", MessageType.None);

            var stepCount = Mathf.Min(sequence.StepCount, 16);
            var buttonWidth = sequenceRect.width / stepCount;
            var buttonHeight = sequenceRect.height / 4;

            var labelStyle2 = new GUIStyle(UnityEditor.EditorStyles.label)
            {
                alignment = TextAnchor.MiddleCenter,
                normal =
                {
                    textColor = Color.grey
                },
                fontSize = 8
            };

            for (int i = 0; i < Mathf.Min(4, sequence.InstrumentCount); i++)
            {
                if (i < 3)
                {
                    for (int j = 0; j < stepCount - 1; j++)
                    {
                        var buttonX = sequenceRect.x + buttonWidth * j;
                        var buttonY = sequenceRect.y + buttonHeight * i;

                        var buttonRect2 = new Rect(buttonX, buttonY + 2, buttonWidth, buttonHeight);

                        var style = sequence[j, i] ? GUI.skin.button : EditorStyles.SequencerButtonOff;
                        GUI.Label(buttonRect2, "", style);
                    }

                    if (i == 1)
                    {
                        var labelX = sequenceRect.x + buttonWidth * 15.1f;
                        var labelY = sequenceRect.y;

                        var labelRect2 = new Rect(labelX, labelY, buttonWidth, sequenceRect.height);

                        EditorGUI.LabelField(labelRect2, "(...)", labelStyle2);
                    }
                }
                else
                {
                    var labelX = sequenceRect.x;
                    var labelY = sequenceRect.y + buttonHeight * i;

                    var labelRect2 = new Rect(labelX, labelY, sequenceRect.width, buttonHeight);

                    EditorGUI.LabelField(labelRect2, "(...)", labelStyle2);
                }
            }

            if (GUI.Button(buttonRect, "Edit", ButtonStyles.UnselectedButton))
            {
                EditorGUIUtility.PingObject(sequence);
                Selection.activeObject = sequence;

                GUIUtility.ExitGUI();
            }

            buttonRect.y += EditorGUIUtility.singleLineHeight * 2.75f;

            if (GUI.Button(buttonRect, "Remove", ButtonStyles.UnselectedButton))
            {
                Undo.RecordObject(_prop.serializedObject.targetObject, "Remove Beat Sequence");
                beatSequences.Remove(sequence);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var list = property.FindPropertyRelative("BeatSequences");
            var height = 0f;

            if (list.arraySize == 0)
            {
                height += EditorGUIUtility.singleLineHeight;
            }
            else
            {
                height += EditorGUIUtility.singleLineHeight * 4.25f * list.arraySize;
            }

            height += EditorGUIUtility.singleLineHeight * 4.5f + 2;

            return height;
        }

        private static BeatSequenceList BeatSequenceList(SerializedProperty prop)
        {
            var targetObject = prop.serializedObject.targetObject;
            var targetObjectClassType = targetObject.GetType();
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;

            var field = targetObjectClassType.GetFields(bindingFlags).FirstOrDefault(fieldInfo => fieldInfo.FieldType == typeof(BeatSequenceList));

            var attackList = (BeatSequenceList)field?.GetValue(targetObject);

            return attackList;
        }

        private static void Label(ref Rect position, string label)
        {
            var labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, label);
            position.y += EditorGUIUtility.singleLineHeight;
        }

        private static void Line(ref Rect position, int height = 1)
        {
            var rect = new Rect(position)
            {
                height = height
            };

            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
            position.y += height;
        }

        private static void Space(ref Rect position)
        {
            position.y += EditorGUIUtility.singleLineHeight;
        }
    }
}
