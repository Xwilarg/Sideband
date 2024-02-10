using UnityEngine;
using UnityEngine.UI;

namespace RhythmJam2024.Player
{
    public class HitArea : MonoBehaviour
    {
        [SerializeField]
        private LineInfo[] _hits;

        [SerializeField]
        private RectTransform _linesRT;
        public RectTransform LinesRT => _linesRT;

        public int LineCount => _hits.Length;

        public RectTransform GetHit(int index) => _hits[index].Hit;

        private void Awake()
        {
            foreach (var line in _hits)
            {
                line.BaseColor = line.Image.color;
            }
        }

        public void OnKeyDown(int line)
        {
            _hits[line].Image.color = _hits[line].PressedColor;
        }

        public void OnKeyUp(int line)
        {
            _hits[line].Image.color = _hits[line].BaseColor;
        }
    }

    [System.Serializable]
    public class LineInfo
    {
        public RectTransform Hit;
        public Color PressedColor;

        [HideInInspector]
        public Color BaseColor;

        public Image Image;
    }
}
