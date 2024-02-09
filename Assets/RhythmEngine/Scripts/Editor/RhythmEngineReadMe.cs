#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace RhythmEngine
{
    public class RhythmEngineReadMe : ScriptableObject
    {

    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(RhythmEngineReadMe))]
    public class RhythmEngineReadMeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //icon
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/RhythmEngine/Scripts/Editor/Textures/Logo.png"), GUILayout.Width(64), GUILayout.Height(64));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            //text field
            var style = new GUIStyle(UnityEditor.EditorStyles.wordWrappedLabel)
            {
                alignment = TextAnchor.MiddleCenter
            };
            GUILayout.Label("Rhythm Engine 1.1.1", style);
            GUILayout.Space(16);
            GUILayout.Label("Thank you for using Rhythm Engine!", style);
            GUILayout.Label("Be sure to check out the documentation for more information on how to use the Rhythm Engine!", style);
            GUILayout.Label("In case you have any problems or questions, feel free to contact me on Discord @qer24.", style);
            style.alignment = TextAnchor.MiddleRight;
            GUILayout.Label("Happy jamming~", style);
            GUILayout.Label("- Arcy", style);
            GUILayout.Space(16);
            if (GUILayout.Button("Open Online Documentation"))
            {
                Application.OpenURL("https://rhythm-engine.gitbook.io/");
            }
            if (GUILayout.Button("Remove ReadMe Asset"))
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(target));
            }
        }
    }
    #endif
}
