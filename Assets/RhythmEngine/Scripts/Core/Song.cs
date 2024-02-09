using System;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmEngine
{
    /// <summary>
    /// Core class used by a lot of components in Rhythm Engine which holds the basic song data
    /// </summary>
    [CreateAssetMenu(fileName = "Song", menuName = "RhythmEngine/Songs/Song")]
    public class Song : ScriptableObject
    {
        /// <summary>
        /// The audio clip to use for this song
        /// </summary>
        [Tooltip("The audio clip to use for this song.")]
        public AudioClip Clip;

        /// <summary>
        /// This offset should be used to delay the visual aspects of your game.
        /// As an example in a mania game, you would use this to delay the note press time (and not to add an offset to the input time)
        /// </summary>
        [Space]
        [Tooltip("The offset in milliseconds to the first beat of the song.")]
        public float FirstBeatOffset;

        /// <summary>
        /// Automatically loop the song after it's over. Setting this will automatically set the Loop property of the AudioSource to true
        /// </summary>
        [Tooltip("Automatically loop the song after it's over. Setting this will automatically set the Loop property of the AudioSource to true.")]
        public bool Looping;

        /// <summary>
        /// The BPM of the song. Used to calculate beat timing
        /// </summary>
        [Tooltip("The BPM of the song. Used to calculate beat timing.")]
        public float BaseBpm = 128;

        /// <summary>
        /// The BPM changes of the song. Used to calculate beat timing
        /// </summary>
        [Tooltip("The BPM changes of the song. Used to calculate beat timing.")]
        public List<BpmChange> BpmChanges = new();

        /// <summary>
        /// First beat offset in seconds
        /// </summary>
        public float FirstBeatOffsetInSec => FirstBeatOffset / 1000f;

        [Serializable]
        public class BpmChange
        {
            /// <summary>
            /// The time in seconds at which the BPM changes
            /// </summary>
            public float Time;

            /// <summary>
            /// The new BPM.
            /// </summary>
            public float Bpm;

            public BpmChange(float bpm, float time)
            {
                Bpm = bpm;
                Time = time;
            }

            public float TimeInMs => Time * 1000;
        }
    }
}
