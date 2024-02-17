using RhythmJam2024.SO;
using System.Collections;
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
        private RectTransform _background;

        [SerializeField]
        private TMP_Text _scoreText;

        [SerializeField]
        private Animator _playerAnim;

        [SerializeField]
        private TMP_Text _comboText;

        private int _combo;

        public bool IsAIController { set; get; }

        public bool IsReversed { set; get; }

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

        public void IncreaseComboText()
        {
            _combo++;
            _comboText.gameObject.SetActive(_combo >= 10);
            if (_combo == 10)
            {
                _playerAnim.SetBool("Combo", true);
            }
            _comboText.text = _combo.ToString();
        }

        public void ResetCombo()
        {
            _playerAnim.SetBool("Combo", false);
            _combo = 0;
            _comboText.gameObject.SetActive(false);
        }

        public void SetScoreValue(float value)
        {
            _associatedScoreContainer.localScale = new(value, 1f);
            _background.localScale = new(value, 1f);
        }

        public void OnKeyDownSpring(int line)
        {
            StartCoroutine(OnKeyDownSpringEnumerator(line));
        }
        private IEnumerator OnKeyDownSpringEnumerator(int line)
        {
            OnKeyDown(line);
            yield return new WaitForSeconds(.25f);
            OnKeyUp(line);
        }

        public void OnKeyDown(int line)
        {
            var hit = _hits[line];
            hit.Image.color = _hits[line].PressedColor;
        }

        public void OnKeyUp(int line)
        {
            var hit = _hits[line];
            hit.Image.color = _hits[line].BaseColor;
        }

        public Color NoteColor(int line) => _hits[line].PressedColor;

        public void ShowHitInfo(HitInfo hit)
        {
            if (hit.DoesBreakCombo)
            {
                ResetCombo();
            }
            else
            {
                IncreaseComboText();
            }

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

        public Image Image;
    }
}
