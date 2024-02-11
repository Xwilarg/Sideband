using RhythmEngine;
using RhythmEngine.Examples;
using RhythmJam2024.Player;
using RhythmJam2024.SO;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
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
        private TwoToneSong _song;

        [SerializeField]
        private RectTransform _centerContainer;

        [SerializeField]
        private HitArea[] _containers;

        [SerializeField]
        private GameObject _notePrefab;

        [SerializeField]
        private TMP_Text _waitingForPlayers;

        private Queue<SimpleManiaNote> _unspawnedNotes;

        private readonly List<NoteData> _spawnedNotes = new();

        private readonly List<PlayerInputUnit> _players = new();

        private const float FallDuration = 1f;

        private bool _didStart;

        public int PlayerCount => 1;

        private void Awake()
        {
            Instance = this;

            _unspawnedNotes = new Queue<SimpleManiaNote>(_song.NoteData.Notes.OrderBy(note => note.Time));

            _goodEngine.Engine.SetSong(new Song()
            {
                Clip = _song.GoodClip,
                BaseBpm = _song.NoteData.BaseBpm,
                FirstBeatOffset = 3000
            });
            _badEngine.Engine.SetSong(new Song()
            {
                Clip = _song.BadClip,
                BaseBpm = _song.NoteData.BaseBpm,
                FirstBeatOffset = 3000
            });

            _goodEngine.Engine.InitTime();
            _badEngine.Engine.InitTime();

            _goodEngine.SetVolume(.5f);
            _badEngine.SetVolume(.5f);
        }

        private void Update()
        {
            if (!_didStart)
            {
                return;
            }

            var time = _goodEngine.Engine.GetCurrentAudioTime();
            TrySpawningNotes(time);

            foreach (var note in _spawnedNotes)
            {
                note.CurrentTime += Time.deltaTime * 1f / FallDuration;

                note.RT.position = new(Mathf.Lerp(_centerContainer.position.x, note.TargetContainer.position.x, (float)note.CurrentTime), note.RT.position.y);

                if (note.CurrentTime > 1f + _info.HitInfo.Last().Distance)
                {
                    note.HitArea.ShowHitInfo(_info.MissInfo);

                    Destroy(note.GameObject);
                    note.GameObject = null;
                }
            }

            _spawnedNotes.RemoveAll(x => x.GameObject == null);
        }

        public void RegisterPlayer(PlayerInputUnit unit)
        {
            unit.Init(_containers[_players.Count]);
            _players.Add(unit);

            if (_players.Count == PlayerCount)
            {
                _waitingForPlayers.gameObject.SetActive(false);

                _goodEngine.Engine.Play();
                _badEngine.Engine.Play();

                _didStart = true;
            }
        }

        private void TrySpawningNotes(double currentTime)
        {
            if (_unspawnedNotes.Count == 0) return;

            var closestUnspawnedNote = _unspawnedNotes.Peek();

            if (currentTime > closestUnspawnedNote.Time - FallDuration)
            {
                _unspawnedNotes.Dequeue();
                SpawnNote(closestUnspawnedNote.Lane, currentTime - (closestUnspawnedNote.Time - FallDuration));
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

                    HitArea = container
                });
            }
        }

        public void HitNote(int line)
        {
            var targetNote = _spawnedNotes.FirstOrDefault(x => x.Line == line);
            if (targetNote == null) // No spawned note on this line
            {
                return;
            }

            var dist = Mathf.Abs(1f - (float)targetNote.CurrentTime);
            for (int i = _info.HitInfo.Length - 1; i >= 0; i--)
            {
                if (dist < _info.HitInfo[i].Distance)
                {
                    targetNote.HitArea.ShowHitInfo(_info.HitInfo[i]);

                    Destroy(targetNote.GameObject);
                    _spawnedNotes.Remove(targetNote);
                    break;
                }
            }
            // TODO: Did we hit the note
        }

        private class NoteData
        {
            public GameObject GameObject;
            public RectTransform RT;
            public double CurrentTime;
            public RectTransform TargetContainer;
            public int Line;

            public HitArea HitArea;
        }
    }
}
