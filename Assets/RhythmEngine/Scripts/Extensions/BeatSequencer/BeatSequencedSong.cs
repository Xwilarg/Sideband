using System.Collections.Generic;
using UnityEngine;

namespace RhythmEngine
{
    [CreateAssetMenu(fileName = "BeatSequencedSong", menuName = "RhythmEngine/Songs/BeatSequencedSong")]
    public class BeatSequencedSong : Song
    {
        [Space]

        [Tooltip("The list of BeatSequences to play.")]
        public BeatSequenceList BeatSequences = new();

        [Tooltip("The list of sequence changes throughout the song.")]
        public List<BeatSequenceChange> BeatSequenceChanges = new();
    }
}
