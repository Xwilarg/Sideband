using UnityEditor;
using UnityEngine;

namespace RhythmEngine
{
    [CustomEditor(typeof(BeatSequence))]
    public class BeatSequenceEditor : Editor
    {
        private BeatSequence _sequence;
        private int _selectedInstrument = 0;

        private void OnEnable()
        {
            _sequence = (BeatSequence)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
            EditorGUILayout.LabelField($"• Setup your beat sequence here, below is a representation of {_sequence.GetSequenceLength()} bar(s) of the song.", EditorStyles.Label);
            EditorGUILayout.LabelField("• Each row represents an instrument, such as a kick or a snare.", EditorStyles.Label);
            var barFraction = _sequence.GetLength(0) / Mathf.Max(0.001f, _sequence.GetSequenceLength());
            EditorGUILayout.LabelField($"• Each step represents 1/{barFraction}th of a bar", EditorStyles.Label);
            EditorGUILayout.LabelField("• Click on a step to toggle it on or off.", EditorStyles.Label);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("StepCount"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("InstrumentCount"));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("SequenceLength"));
            GUILayout.Space(16);
            EditorUtils.Info("The length of the sequence in bars.", false);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (_sequence.GetLength(0) <= 0 || _sequence.GetLength(1) <= 0 || _sequence.GetSequenceLength() <= 0f)
            {
                EditorGUILayout.HelpBox("Set the Length Per Bar, Instrument Count and Sequence Length to a value greater than 0.", MessageType.Error);

                serializedObject.ApplyModifiedProperties();
                return;
            }

            if (_sequence.Length() != _sequence.GetLength(1) * _sequence.GetLength(0))
            {
                //_sequence.SetBeats(new bool[_sequence.GetLength(1) * _sequence.GetLength(0)]);
                _sequence.ChangeStepsLength(_sequence.GetLength(1) * _sequence.GetLength(0));
                _selectedInstrument = 1;
            }

            EditorGUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);

            for (int y = -1; y < _sequence.GetLength(1); y++)
            {
                EditorGUILayout.BeginHorizontal();

                for (int x = 0; x < _sequence.GetLength(0); x++)
                {
                    if (y == -1)
                    {
                        if (x % 4 == 0)
                        {
                            var labelStyle = new GUIStyle(UnityEditor.EditorStyles.label)
                            {
                                alignment = TextAnchor.MiddleCenter,
                                normal =
                                {
                                    textColor = Color.gray
                                },
                            };

                            GUILayout.Label($"{x}-{x + 3}", labelStyle, GUILayout.MinWidth(0));
                        }

                        continue;
                    }

                    if (x % 4 == 0 && x != 0)
                    {
                        var labelStyle = new GUIStyle(UnityEditor.EditorStyles.label)
                        {
                            alignment = TextAnchor.MiddleLeft,
                            normal =
                            {
                                textColor = Color.gray
                            },
                            fontStyle = FontStyle.Bold
                        };

                        GUILayout.Label("|", labelStyle, GUILayout.MinWidth(4f), GUILayout.MaxWidth(8f));
                    }

                    var style = _sequence[x, y] ? GUI.skin.button : EditorStyles.SequencerButtonOff;
                    if (GUILayout.Button(GUIContent.none, style, GUILayout.MinWidth(0), GUILayout.MinHeight(0)))
                    {
                        Undo.RecordObject(_sequence, "Toggle Beat");
                        _sequence[x, y] = !_sequence[x, y];
                        EditorUtility.SetDirty(_sequence);
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Edit instruments", EditorStyles.Label);
            EditorGUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
            _selectedInstrument = EditorGUILayout.IntSlider("Selected Instrument", _selectedInstrument, 1, _sequence.GetLength(1));
            DrawInstrumentControls();
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawInstrumentControls()
        {
            EditorGUILayout.BeginHorizontal();

            void Clear()
            {
                for (int x = 0; x < _sequence.GetLength(0); x++)
                {
                    _sequence[x, _selectedInstrument - 1] = false;
                }
            }

            void Fill(int n)
            {
                for (int x = 0; x < _sequence.GetLength(0); x++)
                {
                    if (n == 0 || x % n == 0)
                    {
                        _sequence[x, _selectedInstrument - 1] = true;
                    }
                }
            }

            if (GUILayout.Button("Fill each 2 steps"))
            {
                Clear();
                Fill(2);
            }

            if (GUILayout.Button("Fill each 4 steps"))
            {
                Clear();
                Fill(4);
            }

            if (GUILayout.Button("Fill each 8 steps"))
            {
                Clear();
                Fill(8);
            }

            if (GUILayout.Button("Fill every step"))
            {
                Clear();
                Fill(0);
            }

            if (GUILayout.Button("Clear"))
            {
                Clear();
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
