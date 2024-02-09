using System;
using UnityEngine;

namespace RhythmEngine
{
    /// <summary>
    /// Simple struct storing a time and a sequence index.
    /// </summary>
    [Serializable]
    public struct BeatSequenceChange
    {
        /// <summary>
        /// The time in seconds at which to change to the sequence
        /// </summary>
        [Tooltip("The time in seconds at which to change to the sequence.")]
        public double Time;

        /// <summary>
        /// The index of the sequence to change to
        /// </summary>
        [Tooltip("The index of the sequence to change to.")]
        public int SequenceIndex;
    }
}
