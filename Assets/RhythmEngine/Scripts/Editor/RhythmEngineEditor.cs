using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace RhythmEngine
{
    [CustomEditor(typeof(RhythmEngineCore))]
    public class RhythmEngineEditor : Editor
    {
        private RhythmEngineCore _script;

        private bool ManualMode => _script.ManualMode;

        private bool _songFoldout;
        private Editor _songEditor;

        private void OnEnable()
        {
            _script = (RhythmEngineCore)target;

            var audioSource = _script.GetComponent<AudioSource>();
            audioSource.hideFlags = HideFlags.HideInInspector;

            _songEditor = CreateEditor(_script.Song);

            Repaint();
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

            serializedObject.Update();

            GUILayout.BeginHorizontal();

            var height = ManualMode ? GUILayout.MinHeight(10) : GUILayout.MinHeight(20);
            if (GUILayout.Button("Automatic mode", ManualMode ? ButtonStyles.UnselectedButton : ButtonStyles.SelectedButton, height))
            {
                Undo.RecordObject(_script, "Set Automatic Mode");

                _script.ManualMode = false;
                EditorUtility.SetDirty(_script);
            }

            height = ManualMode ? GUILayout.MinHeight(20) : GUILayout.MinHeight(10);
            if (GUILayout.Button("Manual mode", ManualMode ? ButtonStyles.SelectedButton : ButtonStyles.UnselectedButton, height))
            {
                Undo.RecordObject(_script, "Set Manual Mode");

                _script.ManualMode = true;
                _script.SetSong(null, false);
                EditorUtility.SetDirty(_script);
            }

            GUILayout.EndHorizontal();

            if (!ManualMode) DrawAutomaticMode();
            else DrawManualMode();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            GUILayout.Space(75);
            if (GUILayout.Button("Add Extensions", ButtonStyles.UnselectedButton, GUILayout.MinHeight(30)))
            {
                //Todo: search provider for extensions
                var provider = CreateInstance<ExtensionSearchProvider>();
                provider.Init(type =>
                {
                    var extension = (RhythmEngineExtension)Undo.AddComponent(_script.gameObject, type);
                    extension.RhythmEngine = _script;
                    EditorUtility.SetDirty(_script);
                    EditorUtility.SetDirty(extension);
                });
                SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), provider);
            }
            GUILayout.Space(75);
            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawAutomaticMode()
        {
            GUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
            GUILayout.Label("Automatic mode - given a song, automatically play it on Awake().", EditorStyles.Label);
            GUILayout.EndVertical();
            EditorGUILayout.Space();

            GUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
            DrawPropertiesExcluding(serializedObject, "m_Script", "SongToPlay");
            EditorGUILayout.Space();

            if (_script.Song == null)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("SongToPlay"));

                EditorGUILayout.BeginHorizontal();
                EditorUtils.Warning("No song set.", false);
                if (GUILayout.Button("New Song", GUILayout.Width(150)))
                {
                    var path = EditorUtility.SaveFilePanelInProject("Create New Song", "New Song", "asset", "Create New Song");
                    if (!string.IsNullOrEmpty(path))
                    {
                        var song = CreateInstance<Song>();
                        AssetDatabase.CreateAsset(song, path);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        _script.SetSong(song, false);
                        EditorUtility.SetDirty(_script);
                    }
                    else
                    {
                        return;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUI.indentLevel++;
                _songFoldout = EditorGUILayout.Foldout(_songFoldout, "Song To Play");
                EditorGUI.indentLevel--;

                EditorGUILayout.PropertyField(serializedObject.FindProperty("SongToPlay"), GUIContent.none);
                EditorGUILayout.EndHorizontal();

                if (_songFoldout)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
                    _songEditor.serializedObject.Update();
                    _songEditor.DrawDefaultInspector();
                    _songEditor.serializedObject.ApplyModifiedProperties();
                    EditorGUILayout.Space();
                    EditorGUILayout.EndVertical();
                    EditorGUI.indentLevel--;
                }

                if (_script.Song.Clip == null)
                {
                    EditorGUILayout.Space();
                    EditorUtils.Warning("No clip set.");
                }
            }

            DrawMusicSourceWarning();

            if (_script.MusicSource != null && _script.Song != null)
            {
                EditorGUILayout.Space();
            }

            GUILayout.EndVertical();
        }

        private void DrawManualMode()
        {
            GUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
            EditorGUILayout.LabelField("Manual mode - you need to call SetSong(), InitTime(), and Play() yourself.\nPlay() should be called last.", EditorStyles.Label);
            EditorGUILayout.LabelField("Play() can be called whenever you like, but SetSong/SetClip() and InitTime() should be called in Awake().", EditorStyles.Label);
            EditorGUILayout.LabelField("You can also just call SetClip() instead of SetSong(), and create your own gameplay back-end logic for songs.", EditorStyles.Label);
            GUILayout.EndVertical();
            EditorGUILayout.Space();

            GUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
            DrawPropertiesExcluding(serializedObject, "m_Script", "SongToPlay");
            DrawMusicSourceWarning();
            GUILayout.EndVertical();
        }

        private void DrawMusicSourceWarning()
        {
            if (_script.MusicSource == null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorUtils.Warning("No music audio source set.");

                if (GUILayout.Button("Add as Child", GUILayout.Width(150)))
                {
                    var child = new GameObject("Music Audio Source");
                    child.transform.SetParent(_script.transform);
                    Undo.RegisterCreatedObjectUndo(child, "Add Music Audio Source");
                    var audioSource = Undo.AddComponent<AudioSource>(child.gameObject);
                    audioSource.playOnAwake = false;
                    audioSource.loop = false;
                    _script.MusicSource = audioSource;
                    EditorUtility.SetDirty(_script);
                }

                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
