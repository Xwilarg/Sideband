using RhythmEngine;
using RhythmEngine.Examples;
using RhythmJam2024.SO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RhythmJam2024
{
    public class GameManager : MonoBehaviour
    {   
        [SerializeField]
        private ToneAudioManager _goodEngine, _badEngine;

        [SerializeField]
        private TwoToneSong _song;

        [SerializeField]
        private RectTransform _centerContainer;

        [SerializeField]
        private RectTransform _leftNoteContainer, _rightNoteContainer;

        [SerializeField]
        private GameObject _notePrefab;

        private Queue<SimpleManiaNote> _unspawnedNotes;

        private readonly List<NoteData> _spawnedNotes = new();

        private const float SpeedMultiplier = 1f;

        private const int LineCount = 4;
        private readonly List<int> _linesPos = new();

        private void Awake()
        {
            var d = _centerContainer.sizeDelta.y / LineCount;
            var half = _centerContainer.sizeDelta.y / 2f;
            for (int i = 0; i < LineCount; i++)
            {
                _linesPos.Add(Mathf.RoundToInt(d * i - half));
            }

            _unspawnedNotes = new Queue<SimpleManiaNote>(_song.NoteData.Notes.OrderBy(note => note.Time));

            _goodEngine.Engine.SetSong(new Song()
            {
                Clip = _song.GoodClip,
                BaseBpm = _song.NoteData.BaseBpm
            });
            _badEngine.Engine.SetSong(new Song()
            {
                Clip = _song.BadClip,
                BaseBpm = _song.NoteData.BaseBpm
            });

            _goodEngine.Engine.InitTime();
            _badEngine.Engine.InitTime();

            _goodEngine.Engine.Play();
            _badEngine.Engine.Play();

            _goodEngine.SetVolume(.5f);
            _badEngine.SetVolume(.5f);
        }

        private void Update()
        {
            var time = _goodEngine.Engine.GetCurrentAudioTime();
            TrySpawningNotes(time);

            foreach (var note in _spawnedNotes)
            {
                note.CurrentTime += Time.deltaTime * SpeedMultiplier;

                note.RT.position = new(Mathf.Lerp(_centerContainer.position.x, note.TargetContainer.position.x, Mathf.Clamp01((float)note.CurrentTime)), note.RT.position.y);

                if (note.CurrentTime > 1f)
                {
                    Destroy(note.GameObject);
                    note.GameObject = null;
                }
            }

            _spawnedNotes.RemoveAll(x => x.GameObject == null);
        }

        private void TrySpawningNotes(double currentTime)
        {
            if (_unspawnedNotes.Count == 0) return;

            var closestUnspawnedNote = _unspawnedNotes.Peek();

            if (currentTime > closestUnspawnedNote.Time - SpeedMultiplier)
            {
                _unspawnedNotes.Dequeue();
                SpawnNote(closestUnspawnedNote.Lane, currentTime - (closestUnspawnedNote.Time - SpeedMultiplier));
            }
        }

        private void SpawnNote(int line, double currentTime)
        {
            var containers = new[] { _leftNoteContainer, _rightNoteContainer };

            foreach (var container in containers)
            {
                var noteTransform = Instantiate(_notePrefab, container);
                var rt = (RectTransform)noteTransform.transform;
                rt.position = new(_centerContainer.position.x, _centerContainer.position.y + _linesPos[line]);

                _spawnedNotes.Add(new()
                {
                    GameObject = noteTransform,
                    RT = rt,
                    CurrentTime = currentTime,
                    TargetContainer = container,
                    Line = line
                });
            }
        }

        private class NoteData
        {
            public GameObject GameObject;
            public RectTransform RT;
            public double CurrentTime;
            public RectTransform TargetContainer;
            public int Line;
        }
    }
}
