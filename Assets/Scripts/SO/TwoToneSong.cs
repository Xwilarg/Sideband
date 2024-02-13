using RhythmEngine.Examples;
using UnityEngine;

namespace RhythmJam2024.SO
{
    [CreateAssetMenu(fileName = "TwoToneSong", menuName = "RhythmJam2024/TwoToneSong")]
    public class TwoToneSong : ScriptableObject
    {
        public AudioClip GoodClip, BadClip;
        public string GoodName, BadName;

        public int Bpm;
        public string Author;

        public SimpleManiaSong NoteData;
    }
}