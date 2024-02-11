﻿using RhythmJam2024.SO;
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

        public RectTransform LinesRT => _linesRT;

        public int LineCount => _hits.Length;

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

        public void OnKeyDown(int line)
        {
            _hits[line].Image.color = _hits[line].PressedColor;
        }

        public void OnKeyUp(int line)
        {
            _hits[line].Image.color = _hits[line].BaseColor;
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

        public Image Image;
    }
}
