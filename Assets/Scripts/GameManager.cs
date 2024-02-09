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
        private RectTransform _leftNoteContainer, _rightNoteContainer;

        [SerializeField]
        private GameObject _notePrefab;

        private Queue<SimpleManiaNote> _unspawnedNotes;

        private readonly List<NoteData> _spawnedNotes = new();

        private const float SpeedMultiplier = 1f;

        private void Awake()
        {
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

                note.RT.anchoredPosition = new(Mathf.Lerp(0f, note.RT.position.x, Mathf.Clamp01((float)note.CurrentTime)), note.RT.anchoredPosition.y);

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
                SpawnNote(currentTime);
            }
        }

        private void SpawnNote(double currentTime)
        {
            var noteTransform = Instantiate(_notePrefab, _leftNoteContainer);
            var rt = (RectTransform)noteTransform.transform;
            rt.anchoredPosition = Vector2.zero; //new Vector3(LanePositions[note.Lane], SpawnHeight, 0); // Set the note's position to the correct lane and the spawn height

            _spawnedNotes.Add(new() {
                GameObject = noteTransform,
                RT = rt,
                CurrentTime = currentTime
            });
        }

        private record NoteData
        {
            public GameObject GameObject;
            public RectTransform RT;
            public double CurrentTime;
        }
    }
}
