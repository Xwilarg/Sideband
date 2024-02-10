using UnityEngine;

namespace RhythmJam2024.Player
{
    public class HitArea : MonoBehaviour
    {
        [SerializeField]
        private RectTransform[] _hits;

        public int LineCount => _hits.Length;

        public RectTransform GetHit(int index) => _hits[index];
    }
}
