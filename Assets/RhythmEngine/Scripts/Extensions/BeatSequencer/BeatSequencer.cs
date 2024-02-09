using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmEngine
{
    /// <summary>
    /// A class that can be used to watch for a sequence of beats and trigger events when the sequence is played
    /// </summary>
    [AddComponentMenu("Rhythm Engine/Extensions/Beat Sequencer")]
    public class BeatSequencer : RhythmEngineExtension
    {
        /// <summary>
        /// The list of BeatSequences to play
        /// </summary>
        public BeatSequenceList BeatSequences = new();

        /// <summary>
        /// The list of sequence changes throughout the song
        /// </summary>
        public List<BeatSequenceChange> BeatSequenceChanges = new();

        private Queue<BeatSequenceChange> _sequenceChangesQueue = new();
        private double _stepTime;

        private BeatSequence _currentSequence;

        private int _stepCounter;
        private int _maxStep;
        private double _lastStepTime = 0;

        /// <summary>
        /// Called when the sequencer steps through the sequence.
        /// The parameter is the index of the instrument that was triggered
        /// </summary>
        public event Action<int> OnInstrumentStep;

        protected virtual void Start()
        {
            _sequenceChangesQueue = new Queue<BeatSequenceChange>(BeatSequenceChanges);
            if (BeatSequences[0] != null)
                ChangeActiveSequence(BeatSequences[0]);
        }

        private void ResetSequencer()
        {
            _stepCounter = 0;
            _maxStep = 0;
            _lastStepTime = 0;

            Start();
        }

        private void OnEnable()
        {
            RhythmEngine.OnBpmChange += UpdateStepTime;
            RhythmEngine.OnLoop += ResetSequencer;
        }

        private void OnDisable()
        {
            RhythmEngine.OnBpmChange -= UpdateStepTime;
            RhythmEngine.OnLoop -= ResetSequencer;
        }

        private void Update()
        {
            if (!RhythmEngine.HasStarted) return;

            var time = RhythmEngine.GetCurrentAudioTime() - RhythmEngine.Song.FirstBeatOffsetInSec;
            if (_sequenceChangesQueue.Count > 0 && time >= _sequenceChangesQueue.Peek().Time)
            {
                var sequenceChange = _sequenceChangesQueue.Dequeue();
                var sequenceIndex = sequenceChange.SequenceIndex;

                if (sequenceIndex == -1)
                {
                    _currentSequence = null;
                }
                else
                {
                    ChangeActiveSequence(sequenceChange);
                }
            }

            if (_currentSequence == null) return;

            var timePassedSinceLastStep = time - _lastStepTime;
            if (timePassedSinceLastStep >= _stepTime)
            {
                _lastStepTime = time;
                Step();
            }
        }

        private void Step()
        {
            for (var y = 0; y < _currentSequence.InstrumentCount; y++)
            {
                if (_currentSequence[_stepCounter, y])
                {
                    OnInstrumentStep?.Invoke(y);
                }
            }

            _stepCounter++;
            _stepCounter %= _maxStep;
        }

        private void ChangeActiveSequence(BeatSequence sequence)
        {
            _currentSequence = sequence;
            UpdateStepTime();
        }

        private void ChangeActiveSequence(BeatSequenceChange sequenceChange)
        {
            _currentSequence = BeatSequences[sequenceChange.SequenceIndex];
            UpdateStepTime(sequenceChange.Time);
        }

        private void UpdateStepTime() => UpdateStepTime(null);

        private void UpdateStepTime(double? sequenceChangeTime)
        {
            var beatsPerSequence = _currentSequence.SequenceLength * 4;
            _stepTime = 60f / RhythmEngine.CurrentBpm / beatsPerSequence;
            _lastStepTime = sequenceChangeTime ?? RhythmEngine.GetCurrentAudioTime();
            _stepCounter = 0;
            _maxStep = _currentSequence.StepCount;
        }

        /// <summary>
        /// Assign a callback to be called when the next step of an instrument is triggered.
        /// Can be useful for example to trigger a particle effect when a drum is hit.
        /// </summary>
        /// <param name="instrumentIndex">Instrument you want the callback to happen on</param>
        /// <param name="callback">The callback to be called</param>
        public void CallOnNextInstrumentStep(int instrumentIndex, Action callback) => StartCoroutine(CallOnNextInstrumentStepCoroutine(instrumentIndex, callback));

        private IEnumerator CallOnNextInstrumentStepCoroutine(int instrumentIndex, Action callback)
        {
            while (!RhythmEngine.HasStarted)
                yield return null;

            while (true)
            {
                if (_currentSequence[_stepCounter, instrumentIndex])
                {
                    callback?.Invoke();
                    yield break;
                }
                yield return null;
            }
        }

        /// <summary>
        /// Returns the time until the next step of the specified instrument. This is pretty expensive and should not be called every frame. Cache the results!
        /// </summary>
        /// <param name="instrumentIndex"></param>
        public double TimeToNextInstrumentStep(int instrumentIndex)
        {
            // Keep track of a temporary step counter
            var step = 0;

            // Get the time until the next step
            var audioTime = RhythmEngine.GetCurrentAudioTime() - RhythmEngine.Song.FirstBeatOffsetInSec;
            var timePassedSinceLastStep = audioTime - _lastStepTime;
            var timeToNextStep = _stepTime - timePassedSinceLastStep;

            // Keep track of the time we want to return.
            // At the very least, it's going to be the time until next step
            var time = timeToNextStep;

            // Store a temporary sequence
            // If the next instrumentation step is in a new sequence, we'll have to query that instead of the current one
            var sequence = _currentSequence;
            // Store a temporary queue
            // We don't want to be changing the main queue
            var tempQueue = new Queue<BeatSequenceChange>(_sequenceChangesQueue);

            // Grab the next item in the sequence, we'll need it later
            // Don't want to do this in a while loop
            var firstQueue = tempQueue.Count > 0 ? tempQueue.Dequeue() : new BeatSequenceChange();
            var nextSequenceIndex = tempQueue.Count > 0 ? firstQueue.SequenceIndex : 0;

            // While there's no enabled note at this step/instrument
            while (!sequence[_stepCounter + step, instrumentIndex])
            {
                step++;
                time += _stepTime;

                // If we've reached the end of the sequence
                if (_stepCounter + step >= sequence.StepCount)
                {
                    // Reset the temporary step counter
                    step -= sequence.StepCount;

                    if (nextSequenceIndex == -1)
                    {
                        time += firstQueue.Time;
                        // Immediately go to the next sequence
                        nextSequenceIndex = tempQueue.Dequeue().SequenceIndex;
                    }

                    // Get the next sequence
                    sequence = BeatSequences[nextSequenceIndex];

                    // If there's more in the queue
                    if (tempQueue.Count > 0)
                    {
                        // Store the next sequence index
                        nextSequenceIndex = tempQueue.Dequeue().SequenceIndex;
                        nextSequenceIndex %= BeatSequences.Count;
                    }
                    else
                    {
                        nextSequenceIndex = 0;
                    }
                }
            }

            return time;
        }
    }
}
