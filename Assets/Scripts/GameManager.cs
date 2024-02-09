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

        public float SpeedUnit => SpeedMultiplier * _song.NoteData.BaseBpm;

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
        }

        private void TrySpawningNotes(double currentTime)
        {
            if (_unspawnedNotes.Count == 0) return;

            float dist = _rightNoteContainer.position.x; // Distance between where we hit and the middle of the screen

            var closestUnspawnedNote = _unspawnedNotes.Peek();

            var offset = closestUnspawnedNote.Time - currentTime;
            if (offset < dist)
            {
                _unspawnedNotes.Dequeue();
                SpawnNote(closestUnspawnedNote, (float)offset);
            }

            // Note: we spawn the notes {NoteFallTime} seconds before their actual time so they can fall from the top of the screen to the bottom.
        }

        private void SpawnNote(SimpleManiaNote note, float offset)
        {
            var noteTransform = Instantiate(_notePrefab, _leftNoteContainer);
            var rt = (RectTransform)noteTransform.transform;
            rt.anchoredPosition = new(offset, 0f); //new Vector3(LanePositions[note.Lane], SpawnHeight, 0); // Set the note's position to the correct lane and the spawn height

            _spawnedNotes.Add(new() {
                GameObject = noteTransform,
                RT = rt
            });
            //_spawnedNotes.Add(new SpawnedNote(noteTransform, note, currentTime, NoteFallTime)); // Add the note to the list of spawned notes
        }

        private record NoteData
        {
            public GameObject GameObject;
            public RectTransform RT;
        }
    }
}
