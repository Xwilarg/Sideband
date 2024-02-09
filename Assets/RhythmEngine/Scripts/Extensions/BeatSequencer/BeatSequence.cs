using System;
using UnityEngine;

namespace RhythmEngine
{
    /// <summary>
    /// A 2d array of booleans that represents a sequence of beats
    /// </summary>
    [CreateAssetMenu(fileName = "BeatSequence", menuName = "RhythmEngine/BeatSequence")]
    public class BeatSequence : ScriptableObject
    {
        [SerializeField] private bool[] Steps = new bool[128]; // Flattened 2d array (32x4)

        /// <summary>
        /// The number of steps in a sequence
        /// </summary>
        [Tooltip("The number of steps in a sequence.")]
        public int StepCount = 16;

        /// <summary>
        /// The number of instruments in a sequence
        /// </summary>
        [Tooltip("The number of instruments in a sequence.")]
        public int InstrumentCount = 4;

        /// <summary>
        /// The length of a sequence in bars
        /// </summary>
        [Tooltip("The length of a sequence in bars.")]
        public float SequenceLength = 1;

        public bool this[int x, int y]
        {
            get => Steps[x + y * StepCount];
            set => Steps[x + y * StepCount] = value;
        }

        /// <summary>
        /// Get either the step count or the instrument count
        /// </summary>
        /// <param name="dimension">0 or 1, Step Count or Instrument Count</param>
        /// <returns>Step Count or Instrument Count</returns>
        public int GetLength(int dimension)
        {
            return dimension switch
            {
                0 => StepCount,
                1 => InstrumentCount,
                _ => throw new IndexOutOfRangeException()
            };
        }

        /// <summary>
        /// Get the length of a sequence in bars
        /// </summary>
        /// <returns>Length of the sequence in bars</returns>
        public float GetSequenceLength() => SequenceLength;

        /// <summary>
        /// Get the length of a sequence in beats
        /// </summary>
        /// <returns>Length of a sequence in beats</returns>
        public int Length() => Steps.Length;

        /// <summary>
        /// Change the length of a sequence in beats
        /// </summary>
        /// <param name="newLength">New length in beats</param>
        public void ChangeStepsLength(int newLength)
        {
            var newBeats = new bool[newLength];
            Array.Copy(Steps, newBeats, Math.Min(newLength, Steps.Length));
            Steps = newBeats;
        }
    }
}
