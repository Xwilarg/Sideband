using RhythmJam2024.SO;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace RhythmJam2024.Menu
{
    public class LevelSelector : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text[] _before, _after;

        [SerializeField]
        private TMP_Text _mainAuthor, _mainTitle, _mainBpm, _mainDifficulty;

        [SerializeField]
        private AudioSource _source;

        private int _currIndex;

        private void Awake()
        {
            SceneManager.LoadScene("SongData", LoadSceneMode.Additive);
        }

        private void Start()
        {
            UpdateUI();
        }

        private void Update()
        {
            if (_source.time > _source.clip.length / 2f + 20f)
            {
                _source.time = _source.clip.length / 2f;
            }
        }

        private void UpdateUI()
        {
            var songs = SongManager.Instance.Songs;

            _mainTitle.text = songs[_currIndex].Name;
            _mainBpm.text = songs[_currIndex].Bpm.ToString();
            _mainAuthor.text = songs[_currIndex].Author;
            _mainDifficulty.text = songs[_currIndex].Difficulty;

            var index = _currIndex;
            for (var i = 0; i < _before.Length; i++)
            {
                index--;
                if (index < 0)
                {
                    index = songs.Length - 1;
                }
                _before[i].text = songs[index].Name;
            }

            index = _currIndex;
            for (var i = 0; i < _after.Length; i++)
            {
                index++;
                if (index > songs.Length - 1)
                {
                    index = 0;
                }
                _after[i].text = songs[index].Name;
            }

            _source.Stop();
            _source.clip = songs[_currIndex].GoodClip;
            _source.time = _source.clip.length / 2f;
            _source.Play();
        }

        public void OnValidateInput(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                StaticData.SongSelected = _currIndex;
                SceneManager.LoadScene("Main");
            }
        }

        private int _lastDir = 0;
        public void OnMoveY(InputAction.CallbackContext value)
        {
            if (value.performed || value.phase == InputActionPhase.Canceled)
            {
                var tmp = Mathf.RoundToInt(value.ReadValue<float>());
                int dir = 0;
                if (tmp > .5f) dir = -1;
                else if (tmp < -.5f) dir = 1;

                if (_lastDir != dir)
                {
                    _lastDir = dir;

                    if (dir != 0)
                    {
                        _currIndex += dir;

                        if (_currIndex < 0) _currIndex = SongManager.Instance.Songs.Length - 1;
                        else if (_currIndex > SongManager.Instance.Songs.Length - 1) _currIndex = 0;

                        UpdateUI();
                    }
                }
            }
        }

        public void OnExitMenu(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
}
