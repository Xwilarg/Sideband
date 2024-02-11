using RhythmJam2024.SO;
using TMPro;
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

        [SerializeField]
        private TMP_Text _hitText;

        [SerializeField]
        private RectTransform _associatedScoreContainer;

        [SerializeField]
        private TMP_Text _scoreText;

        public RectTransform LinesRT => _linesRT;

        public int LineCount => _hits.Length;

        private int _score;
        public int Score
        {
            set
            {
                _score = value;
                _scoreText.text = $"{_score}";
            }
            get => _score;
        }

        public LineInfo GetLineInfo(int index) => _hits[index];

        private float _hitTextTimer;

        private void Awake()
        {
            foreach (var line in _hits)
            {
                line.BaseColor = line.Image.color;
                line.KeyImage = line.Hit.GetComponent<Image>();
            }
        }

        private void Update()
        {
            if (_hitTextTimer > 0f)
            {
                _hitTextTimer -= Time.deltaTime;
                if (_hitTextTimer <= 0f)
                {
                    _hitText.gameObject.SetActive(false);
                }
            }
        }

        public void SetScoreValue(float value)
        {
            _associatedScoreContainer.localScale = new(value, 1f);
        }

        public void OnKeyDown(int line)
        {
            var hit = _hits[line];
            hit.Image.color = _hits[line].PressedColor;
            if (hit.KeyImage != null)
            {
                hit.KeyImage.color = Color.black;
            }
        }

        public void OnKeyUp(int line)
        {
            var hit = _hits[line];
            hit.Image.color = _hits[line].BaseColor;
            if (hit.KeyImage != null)
            {
                hit.KeyImage.color = Color.white;
            }
        }

        public Color NoteColor(int line) => _hits[line].PressedColor;

        public void ShowHitInfo(HitInfo hit)
        {
            _hitText.text = hit.DisplayText;
            _hitText.color = hit.Color;

            _hitText.gameObject.SetActive(true);
            _hitTextTimer = 1f;
        }
    }

    [System.Serializable]
    public class LineInfo
    {
        public RectTransform Hit;
        public Color PressedColor;

        [HideInInspector]
        public Color BaseColor;

        [HideInInspector]
        public Image KeyImage;

        public Image Image;
    }
}
