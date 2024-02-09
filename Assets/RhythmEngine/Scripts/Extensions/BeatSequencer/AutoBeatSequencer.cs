using UnityEngine;

namespace RhythmEngine
{
    [AddComponentMenu("Rhythm Engine/Extensions/Auto Beat Sequencer")]
    public class AutoBeatSequencer : BeatSequencer
    {
        protected override void Start()
        {
            if (RhythmEngine.Song == null && RhythmEngine.ManualMode)
            {
                Debug.LogError("You should call SetSong/SetClip() and InitTime() in Awake() first!");
                return;
            }

            var song = RhythmEngine.Song as BeatSequencedSong;
            if (song == null)
            {
                Debug.LogError("AutoBeatSequencer requires a BeatSequencedSong or a Song derived from that class to be played.");
                return;
            }

            BeatSequences = song.BeatSequences;
            BeatSequenceChanges = song.BeatSequenceChanges;

            base.Start();
        }
    }
}
