using UnityEngine;

namespace RhythmEngine.Player
{
    public class HitArea : MonoBehaviour
    {
        [SerializeField]
        private RectTransform[] _hits;

        public int LineCount => _hits.Length;

        public RectTransform GetHit(int index) => _hits[index];
    }
}
