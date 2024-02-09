using System;
using UnityEngine;

namespace RhythmEngine.Examples
{
    /// <summary>
    /// Simple struct containing the data needed to play a mania note.
    /// </summary>
    [Serializable]
    public struct SimpleManiaNote
    {
        [Tooltip("The time in seconds at which to play the note.")]
        public double Time;

        [Tooltip("Lane to play the note in.")]
        public int Lane;

        public bool Equals(SimpleManiaNote other)
        {
            // We use a small epsilon to account for floating point errors.
            return Math.Abs(Time - other.Time) < 0.0001d && Lane == other.Lane;
        }
    }
}
