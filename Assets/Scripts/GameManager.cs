using RhythmEngine.Examples;
using RhythmJam2024.Player;
using RhythmJam2024.SO;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RhythmJam2024
{
    public class GameManager : MonoBehaviour
    {   
        public static GameManager Instance { private set; get; }

        [SerializeField]
        private GameInfo _info;

        [SerializeField]
        private ToneAudioManager _goodEngine, _badEngine;

        [SerializeField]
        private RectTransform _centerContainer;

        [SerializeField]
        private HitArea[] _containers;

        [SerializeField]
        private GameObject _notePrefab;

        [SerializeField]
        private TMP_Text _waitingForPlayers;

        [SerializeField]
        private PlayerInputManager _inputManager;

        [SerializeField]
        private GameObject _gameOverPanel;
        [SerializeField]
        private TMP_Text _gameOverText;

        [SerializeField]
        private AudioSource _victoryBgm;

        [SerializeField]
        private AudioClip _happyVClip, _horrorVClip;

        private TwoToneSong _song;

        private Queue<SimpleManiaNote> _unspawnedNotes;

        private readonly List<NoteData> _spawnedNotes = new();

        private readonly List<PlayerInputUnit> _players = new();

        private float _currFallDuration = 1f;

        private bool _didStart;

        private void Awake()
        {
            Instance = this;

            SceneManager.LoadScene("SongData", LoadSceneMode.Additive);

            if (StaticData.IsAgainstAI)
            {
                _containers[1].IsAIController = true;
            }
            _containers[1].IsReversed = true;

            _waitingForPlayers.text = "Waiting for Player 1 Input";
        }

        private void Start()
        {
            _song = SongManager.Instance.Songs[StaticData.SongSelected];

            _unspawnedNotes = new Queue<SimpleManiaNote>(_song.NoteData.Notes.OrderBy(note => note.Time));

            _goodEngine.SetClip(_song.GoodClip);
            _badEngine.SetClip(_song.BadClip);

            _goodEngine.SetVolume(.5f);
            _badEngine.SetVolume(.5f);
        }

        private void Update()
        {
            if (!_didStart)
            {
                return;
            }

            var time = _goodEngine.GetTime();
            TrySpawningNotes(time);

            foreach (var note in _spawnedNotes)
            {
                note.CurrentTime += Time.deltaTime * 1f / note.FallDuration;

                if (note.GameObject != null)
                {
                    note.RT.position = new(Mathf.Lerp(_centerContainer.position.x, note.TargetContainer.position.x, (float)note.CurrentTime), note.RT.position.y);
                }

                if (note.CurrentTime > 1f)
                {
                    if (note.GameObject != null)
                    {
                        Destroy(note.GameObject);
                        note.GameObject = null;
                    }

                    if (!note.PendingRemoval && note.HitArea.IsAIController && note.CurrentTime > note.AIHitTiming)
                    {
                        note.HitArea.OnKeyDownSpring(note.Line);
                        HitNote(note.Line, note.HitArea.GetInstanceID());
                    }

                    if (note.CurrentTime > 1f + _info.HitInfo[0].Distance)
                    {
                        note.HitArea.ShowHitInfo(_info.MissInfo);
                        note.PendingRemoval = true;
                    }
                }
            }

            _spawnedNotes.RemoveAll(x => x.PendingRemoval);

            if (!_spawnedNotes.Any() && !_unspawnedNotes.Any() && !_gameOverPanel.gameObject.activeInHierarchy)
            {
                _gameOverPanel.gameObject.SetActive(true);
                if (_containers[0].Score > _containers[1].Score)
                {
                    _gameOverText.color = new(0.9960784f, 0.4039216f, 0.7137255f);
                    _gameOverText.text = "Player One Win!";
                    _victoryBgm.clip = _happyVClip;
                }
                else if (_containers[0].Score < _containers[1].Score)
                {
                    _gameOverText.color = new(0.1960784f, 0.09411765f, 0.4627451f);
                    _gameOverText.text = "Player Two Win!";
                    _victoryBgm.clip = _horrorVClip;
                }
                else
                {
                    _gameOverText.color = Color.black;
                    _gameOverText.text = "It's a Draw!";
                    _victoryBgm.clip = _happyVClip;
                }
                _victoryBgm.Play();
            }
        }

        public void RegisterPlayer(PlayerInputUnit unit)
        {
            unit.Init(_containers[_players.Count]);
            _players.Add(unit);

            _waitingForPlayers.text = $"Waiting for Player {_players.Count + 1} Input";

            if (_players.Count == (StaticData.IsAgainstAI ? 1 : 2))
            {
                _waitingForPlayers.gameObject.SetActive(false);

                _goodEngine.Play(3f);
                _badEngine.Play(3f);

                _didStart = true;


                if (StaticData.IsAgainstAI)
                {
                    _inputManager.DisableJoining();
                }
            }
        }

        private void TrySpawningNotes(double currentTime)
        {
            if (_unspawnedNotes.Count == 0) return;

            var closestUnspawnedNote = _unspawnedNotes.Peek();

            if (currentTime > closestUnspawnedNote.Time - _currFallDuration)
            {
                _unspawnedNotes.Dequeue();
                SpawnNote(closestUnspawnedNote.Lane, currentTime - (closestUnspawnedNote.Time - _currFallDuration));
                TrySpawningNotes(currentTime);
            }
        }

        private void SpawnNote(int line, double currentTime)
        {
            foreach (var container in _containers)
            {
                var parent = container.GetLineInfo(line);
                var noteTransform = Instantiate(_notePrefab, parent.Hit);
                var rt = (RectTransform)noteTransform.transform;
                rt.sizeDelta = new(rt.sizeDelta.x, parent.Hit.rect.height);
                rt.position = new(_centerContainer.position.x, parent.Hit.position.y);

                noteTransform.GetComponent<Image>().color = container.NoteColor(line);

                _spawnedNotes.Add(new()
                {
                    GameObject = noteTransform,
                    RT = rt,
                    CurrentTime = currentTime,
                    TargetContainer = container.LinesRT,
                    Line = line,

                    AIHitTiming = Random.Range(.8f, 1.2f),

                    HitArea = container,

                    FallDuration = _currFallDuration
                });

                if (line == 3 && _currFallDuration > .2f)
                {
                    _currFallDuration -= .025f;
                }
            }
        }

        public void HitNote(int line, int id)
        {
            var targetNote = _spawnedNotes.FirstOrDefault(x => x.Line == line && x.HitArea.GetInstanceID() == id && !x.PendingRemoval);
            if (targetNote == null) // No spawned note on this line
            {
                return;
            }

            var dist = Mathf.Abs(1f - (float)targetNote.CurrentTime);
            for (int i = _info.HitInfo.Length - 1; i >= 0; i--)
            {
                var info = _info.HitInfo[i];
                if (dist < info.Distance)
                {
                    targetNote.HitArea.ShowHitInfo(info);

                    targetNote.HitArea.Score += info.Score;

                    Destroy(targetNote.GameObject);
                    targetNote.GameObject = null;
                    targetNote.PendingRemoval = true;

                    var diff = _containers[0].Score - _containers[1].Score;

                    var delta = Mathf.Clamp(diff * .5f / _info.MaxScoreDiff, -.5f, .5f) + .5f;

                    _containers[0].SetScoreValue(delta);
                    _containers[1].SetScoreValue(1f - delta);
                    _goodEngine.SetVolume(delta);
                    _badEngine.SetVolume(1f - delta);
                    break;
                }
            }
        }

        public void BackToMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public void Restart()
        {
            SceneManager.LoadScene("Main");
        }

        public void BackToLevelSelection()
        {
            SceneManager.LoadScene("LevelSelector");
        }

        public void OnRestart(InputAction.CallbackContext value)
        {
            if (value.performed && _gameOverPanel.gameObject.activeInHierarchy)
            {
                Restart();
            }
        }

        public void OnBackToMenu(InputAction.CallbackContext value)
        {
            if (value.performed && _gameOverPanel.gameObject.activeInHierarchy)
            {
                BackToMenu();
            }
        }

        public void OnLevelSelection(InputAction.CallbackContext value)
        {
            if (value.performed && _gameOverPanel.gameObject.activeInHierarchy)
            {
                BackToLevelSelection();
            }
        }

        private class NoteData
        {
            public GameObject GameObject;
            public RectTransform RT;
            public double CurrentTime;
            public RectTransform TargetContainer;
            public int Line;

            public float AIHitTiming;
            public HitArea HitArea;

            public bool PendingRemoval;

            public float FallDuration;
        }
    }
}
