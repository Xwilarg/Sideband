using System;
using System.Collections;
using System.Collections.Generic;

namespace RhythmEngine
{
    /// <summary>
    /// Helper class for storing a list of BeatSequences.
    /// This is mainly used to have a nice property drawer in the inspector.
    /// </summary>
    [Serializable]
    public class BeatSequenceList : IEnumerable<BeatSequence>
    {
        /// <summary>
        /// The list of BeatSequences
        /// </summary>
        public List<BeatSequence> BeatSequences;

        //Below are all the useful methods that List<BeatSequence> has.

        public BeatSequence this[int index]
        {
            get => BeatSequences[index];
            set => BeatSequences[index] = value;
        }

        public int Count => BeatSequences.Count;

        public void Add(BeatSequence sequence) => BeatSequences.Add(sequence);

        public void Remove(BeatSequence sequence) => BeatSequences.Remove(sequence);

        public void RemoveAt(int index) => BeatSequences.RemoveAt(index);

        public void Clear() => BeatSequences.Clear();

        public bool Contains(BeatSequence sequence) => BeatSequences.Contains(sequence);

        public int IndexOf(BeatSequence sequence) => BeatSequences.IndexOf(sequence);

        public void Insert(int index, BeatSequence sequence) => BeatSequences.Insert(index, sequence);

        public void CopyTo(BeatSequence[] array, int arrayIndex) => BeatSequences.CopyTo(array, arrayIndex);

        public void Sort() => BeatSequences.Sort();

        public void Sort(Comparison<BeatSequence> comparison) => BeatSequences.Sort(comparison);

        public void Sort(IComparer<BeatSequence> comparer) => BeatSequences.Sort(comparer);

        public BeatSequence[] ToArray() => BeatSequences.ToArray();
        public IEnumerator<BeatSequence> GetEnumerator() => BeatSequences.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString() => BeatSequences.ToString();

        public BeatSequenceList() => BeatSequences = new List<BeatSequence>();
    }
}
