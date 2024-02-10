using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace RhythmJam2024.Player
{
    public class HitArea : MonoBehaviour
    {
        [SerializeField]
        private RectTransform[] _hits;

        private Image[] _images;

        public int LineCount => _hits.Length;

        public RectTransform GetHit(int index) => _hits[index];

        private void Awake()
        {
            _images = _hits.Select(x => x.GetComponent<Image>()).ToArray();
        }

        public void OnKeyDown(int line)
        {
            _images[line].color = Color.black;
        }

        public void OnKeyUp(int line)
        {
            _images[line].color = Color.white;
        }
    }
}
