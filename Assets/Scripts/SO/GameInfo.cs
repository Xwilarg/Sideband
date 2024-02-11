using UnityEngine;

namespace RhythmJam2024.SO
{
    [CreateAssetMenu(fileName = "GameInfo", menuName = "RhythmJam2024/GameInfo")]
    public class GameInfo : ScriptableObject
    {
        [Header("Note info")]
        public HitInfo[] HitInfo;
        public HitInfo MissInfo;
    }

    [System.Serializable]
    public class HitInfo
    {
        public string DisplayText;
        public float Distance;
        public Color Color;
        public bool DoesBreakCombo;
        public bool DoesDisplayEarlyLate;

        public int Score;
    }
}