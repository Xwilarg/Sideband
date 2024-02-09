// Copyright 2021 Matthew Sitton

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// This license just applies to this file, this is seperate from the original codebase licsense

// To add to this, this script was originally made during by Matthew Rhythm Jam 2021, and was extended over time by me.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace RhythmEngine
{
    /// <summary>
    /// The main class that handles the playback and timing of a song.
    /// This script requires an empty audiosource to be attached to the same gameobject.
    /// This ISN'T the audio source that will be used for playback, this is just used to get the audio thread time
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    [AddComponentMenu("Rhythm Engine/Rhythm Engine")]
    public class RhythmEngineCore : MonoBehaviour
    {
        /// <summary>
        /// Audio source that the music is played from
        /// </summary>
        [Tooltip("The audio source that the music will be played from.")]
        public AudioSource MusicSource;
        [Tooltip("The offset in ms that the song will be played at, this should be set per user, since it's different for everyone. Current audio time will be offset by this value.")]
        [SerializeField] private int OffsetInMs;
        [Tooltip("The time in seconds before the song starts playing, this is mainly used for gameplay purposes, like mania notes falling before the song starts playing.")]
        [SerializeField] private float PreStartTime = 2f;
        [Tooltip("The song that will be played")]
        [SerializeField] private Song SongToPlay;

        /// <summary>
        /// If true, the song won't start playing automatically, you'll have to call SetSong()/SetClip(), InitTime() and Play() manually
        /// </summary>
        [HideInInspector] public bool ManualMode;

        private bool _timeInitialized;

        private double _audioStartTime;
        private double _sourceStartTime;
        private double _currentTime;
        private double _lastTime;
        private double _currentPreStartTime;

        private double _systemUnityTimeOffset;
        private double _lastFrameTime;
        private double _currentFrameTime;

        private bool _gameThreadStall;
        private double _syncDelta;
        private double _syncSpeedupRate;

        private double _lastDspTime;
        private double _dspUpdatePeriod;
        private double _lastDspUpdatePeriod;

        private bool _gameThreadLatencyAck;
        private double _audioThreadTimeLatencyAck;

        private bool _audioHasBeenScheduled;
        private double _audioDspScheduledTime;
        private bool _isPlaying;

        private float _timeSincePause;

        private double _previousTimestamp;
        private double _loopTime = 999d;

        private Queue<Song.BpmChange> _bpmChangesQueue = new();
        private double OffsetInSec => OffsetInMs * 0.001f;

        /// <summary>
        /// True if the music source is playing
        /// </summary>
        public bool IsSourcePlaying => MusicSource.isPlaying;

        /// <summary>
        /// True if the current song time is greater than 0
        /// </summary>
        public bool HasStarted => _isPlaying ? GetCurrentAudioTime() >= 0 : _sourceStartTime >= 0;

        /// <summary>
        /// True if the song is not playing
        /// </summary>
        public bool IsPaused => !_isPlaying;

        /// <summary>
        /// Current BPM of the song
        /// </summary>
        public float CurrentBpm { get; private set; }

        /// <summary>
        /// More accurate version of Time.DeltaTime, it should NOT be used for gameplay purposes, since it's not affected by the audio sync
        /// </summary>
        public double DeltaTime { get; private set; }

        /// <summary>
        /// More accurate version of Time.DeltaTime, it can be used for gameplay purposes, since it's affected by the audio sync.
        /// However, I would still recommend using stuff like Lerp() with GetCurrentAudioTime() instead of this, since it can skip frames
        /// </summary>
        public double AudioDelta { get; private set; }

        /// <summary>
        /// (Read only) Time in seconds of the moment that the game will unpause on.
        /// If you want to set this time, use SetStartTime()
        /// </summary>
        public double SourceStartTime => _sourceStartTime;

        /// <summary>
        /// Music source pitch
        /// </summary>
        public float SongPitch => MusicSource.pitch;

        /// <summary>
        /// Currently playing song
        /// </summary>
        public Song Song => SongToPlay;

        /// <summary>
        /// Called alongside Play(), it's not exactly the same time the audio starts playing, it's just the time that the audio has been scheduled for playback
        /// </summary>
        public event Action OnStartPlaying;

        /// <summary>
        /// Called on Pause() and Stop()
        /// </summary>
        public event Action OnStopPlaying;

        /// <summary>
        /// Called on bpm change, this is called AFTER the CurrentBpm value has been changed
        /// </summary>
        public event Action OnBpmChange;

        /// <summary>
        /// Called when the song loops
        /// </summary>
        public event Action OnLoop;

        private void Awake()
        {
            _currentPreStartTime = PreStartTime;

            if (ManualMode) return;

            SetClip();
            InitTime();
            Play();
        }

        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();

            if (ManualMode && MusicSource.clip == null)
            {
                Debug.LogError("You should call SetSong/SetClip() and InitTime() in Awake() first!");
                yield break;
            }

            _loopTime = MusicSource.clip.length;
        }

        /// <summary>
        /// Initializes the time of the song, this should be called before playing the song
        /// </summary>
        /// <param name="time">Time in seconds</param>
        public void InitTime(double time = 0)
        {
            _timeInitialized = true;

            _currentTime = time - _currentPreStartTime;
            _currentPreStartTime = 0;
            _previousTimestamp = _currentTime;
            _sourceStartTime = time;
            _currentFrameTime = _lastFrameTime = Time.timeAsDouble;
            InitBpm();
            ProcessAudioTime();
        }

        private void InitBpm()
        {
            CurrentBpm = SongToPlay.BaseBpm;
            _bpmChangesQueue = new Queue<Song.BpmChange>();
            foreach (var bpmChange in SongToPlay.BpmChanges)
            {
                _bpmChangesQueue.Enqueue(bpmChange);
            }
        }

        private void OnEnable() => _currentTime = -PreStartTime;

        private void SetClip()
        {
            if (Song.Clip == null)
            {
                Debug.LogError("No clip set for the song, set it in the song's inspector", gameObject);
                return;
            }

            SetClip(Song.Clip);
            MusicSource.loop = Song.Looping;
        }

        /// <summary>
        /// Set the song that'll be playing, if you'd rather set the clip directly, use SetClip().
        /// </summary>
        /// <param name="song">Song that'll be playing</param>
        /// <param name="setClip">If true, also set the clip that'll be playing from the song</param>
        public void SetSong(Song song, bool setClip = true)
        {
            SongToPlay = song;
            if (setClip) SetClip();
        }

        /// <summary>
        /// Set the clip of the music source. Can be used instead of SetSong() if you want to create your own gameplay back-end logic for songs.
        /// </summary>
        /// <param name="clip">Audio clip of the music that'll be playing</param>
        public void SetClip(AudioClip clip) => MusicSource.clip = clip;

        /// <summary>
        /// Set the offset of the song, this should be set per user, since it's different for everyone.
        /// This also should be preferably set before InitTime() and Play() is called.
        /// </summary>
        /// <param name="offsetInMs">Offset in milliseconds</param>
        public void SetOffset(int offsetInMs) => OffsetInMs = offsetInMs;

        private void Update()
        {
            ProcessLooping();
            ProcessAudioTime();

            // The following is the playback scheduling system, this Schedules the audio to start at the begining of the next
            // Audio thread invoke time so we know exactly when the audio thread should begin playing the audio for latency calulation
            // Make sure that the dspUpdatePeriod caculation has been found before scheduling playback
            if (_isPlaying && Math.Abs(_dspUpdatePeriod) > 0.01f && Math.Abs(_lastDspUpdatePeriod - _dspUpdatePeriod) < 0.01f && !_audioHasBeenScheduled)
            {
                // Play 2 update periods in the future
                double playOffset = (int)(PreStartTime / _dspUpdatePeriod) * _dspUpdatePeriod;

                _currentTime = -playOffset;
                double playTime = AudioSettings.dspTime + playOffset;
                _audioDspScheduledTime = playTime;
                MusicSource.time = (float)_sourceStartTime * SongPitch;
                if (PreStartTime <= 0.01f) _currentTime = _sourceStartTime;
                MusicSource.PlayScheduled(playTime);
                _audioHasBeenScheduled = true;
            }

            // In case there is a need for a delta time that is not affected by the audio thread latency
            double currentTimestamp = GetCurrentAudioTime();
            AudioDelta = currentTimestamp - _previousTimestamp;
            _previousTimestamp = currentTimestamp;

            ProcessBpmChanges(currentTimestamp);
        }

        private bool ProcessLooping()
        {
            if (!MusicSource.loop) return false;

            if (_currentTime >= _loopTime)
            {
                _currentTime = 0;
                _currentFrameTime = _lastFrameTime = Time.timeAsDouble;
                _audioStartTime = GetTime();
                _sourceStartTime = 0;

                OnLoop?.Invoke();

                return true;
            }

            return false;
        }

        private void ProcessBpmChanges(double currentTime)
        {
            if (_bpmChangesQueue.Count == 0) return;

            var nextBpmChange = _bpmChangesQueue.Peek();

            if (currentTime >= nextBpmChange.Time)
            {
                CurrentBpm = nextBpmChange.Bpm;
                _bpmChangesQueue.Dequeue();
                OnBpmChange?.Invoke();
            }
        }

        private void ProcessAudioTime()
        {
            // Measure the time offset between unity realtimeSinceStartup and System.Diags stopwatch time
            // This is used for offsetting when checking time from the audio thread to match unity time
            double systemTime = GetTimeImpl();
            double unityTime = Time.realtimeSinceStartupAsDouble;
            _systemUnityTimeOffset = systemTime - unityTime;

            // Using Time.timeAsDouble to calculate Time.deltaTime as a double since unity doesn't have an api for this
            _lastFrameTime = _currentFrameTime;
            _currentFrameTime = Time.timeAsDouble;
            DeltaTime = _currentFrameTime - _lastFrameTime;

            // You could consider this the audio thread "pinging" the game thread.
            // This calculates the latency between when the audio thread was ran and when the game thread runs
            // and if the game thread is greater than dspUpdatePeriod (the update period between calls of the audio thread)
            // then it will consider this a game thread stall and activate the re-syncronization code
            if (!_gameThreadStall && _gameThreadLatencyAck)
            {
                _syncDelta = Time.realtimeSinceStartupAsDouble - _audioThreadTimeLatencyAck;
                _gameThreadLatencyAck = false;

                if (_syncDelta > _dspUpdatePeriod)
                {
                    // Calculate a more accurate sync delta from the realtime value
                    double syncDeltaAccurate = unityTime - _audioStartTime + _sourceStartTime - _currentTime;

                    // If syncDeltaAccurate is more than 100ms off use the original value.
                    // This likely means the editor was paused and resumed, in this case check time source and sync to that
                    if (syncDeltaAccurate - _syncDelta < 0.1)
                    {
                        double sourceDelta = MusicSource.time - _currentTime;
                        _syncDelta = sourceDelta;
                    }

                    _gameThreadStall = true;
                }
            }

            if (_gameThreadStall)
            {
                // Doubles the speed of time until we catch up
                if (Math.Abs(_syncSpeedupRate) < 0.01f) _syncSpeedupRate = 2.0;
                DeltaTime *= _syncSpeedupRate;

                if (DeltaTime > _syncDelta) DeltaTime = _syncDelta;
                _syncDelta -= DeltaTime;
            }

            if (_syncDelta <= 0)
            {
                _syncSpeedupRate = 0.0;
                _gameThreadStall = false;
            }

            // currentTime += doubleDelta;

            if (_audioStartTime > 0)
            {
                // This is for measuring the time offset after the song start has been scheduled and getting the exact latency offset since the start of audio playback
                if (_currentTime >= 0) _currentTime = unityTime - _audioStartTime + _sourceStartTime;
                else _currentTime += DeltaTime;
            }
            else if (_currentTime < 0)
            {
                _currentTime += DeltaTime;
            }
        }

        private static double GetTimeImpl() => Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency;

        // Using this because it's threadsafe and unity's Time api is not
        // This is being translated into the same starting position as Time.realtimeSinceStartupAsDouble
        // And the offset is measured at the start of each frame to compensate for any drift
        private double GetTime() => GetTimeImpl() - _systemUnityTimeOffset;

        // This is used to schedule the audio playback and get the exact start time of audio to calculate latency, runs from the audio thread
        private void OnAudioFilterRead(float[] data, int channels)
        {
            // Calculate the update period of the audio thread, basically how much time between calls
            // lastDspUpdatePeriod is used to determine if the update period is stable
            _lastDspUpdatePeriod = _dspUpdatePeriod;
            _dspUpdatePeriod = AudioSettings.dspTime - _lastDspTime;

            // DSP time isn't updated until after OnAudioFilterRead runs from what i can tell.
            // This typically gives an exact estimation of the next dspTime
            double nextDspTime = AudioSettings.dspTime + _dspUpdatePeriod;

            if (_audioDspScheduledTime > 0.0 && _audioDspScheduledTime <= nextDspTime && Math.Abs(_audioStartTime) < 0.01f) _audioStartTime = GetTime();
            _lastDspTime = AudioSettings.dspTime;

            // Trigger audio -> game thread latency check, if the game thread detects a latency larger than the dspUpdatePeriod
            // Then it will trigger the audio time sync code
            if (!_gameThreadLatencyAck && _audioDspScheduledTime > 0.0 && _audioDspScheduledTime <= nextDspTime)
            {
                _gameThreadLatencyAck = true;
                _audioThreadTimeLatencyAck = GetTime();
            }
        }

        /// <summary>
        /// Return current time of the audio playback, this is the most accurate time you can get from the audio thread, and it should be used for syncing gameplay
        /// </summary>
        /// <returns>(double) Time</returns>
        public double GetCurrentAudioTime()
        {
            if (IsPaused) return _sourceStartTime * SongPitch;

            return _currentTime * SongPitch - OffsetInSec;
        }

        /// <summary>
        /// If the time has been initialized, schedules the audio playback.
        /// The audio won't start on the same frame as this is called, but the next frame that the audio thread runs
        /// </summary>
        public void Play()
        {
            if (!_timeInitialized)
            {
                Debug.LogError("Time has not been initialized, please call InitTime() before playing audio", gameObject);
                return;
            }

            _isPlaying = true;
            OnStartPlaying?.Invoke();
        }

        /// <summary>
        /// Unpause the audio playback. Just calls Play()
        /// </summary>
        public void Unpause() => Play();

        /// <summary>
        /// Pause the audio playback
        /// </summary>
        public void Pause()
        {
            if (MusicSource.time > 0) PreStartTime = 0;

            _sourceStartTime = GetCurrentAudioTime() / SongPitch;
            MusicSource.Stop();
            _isPlaying = false;
            _audioStartTime = 0;
            _currentTime = _sourceStartTime;
            _audioHasBeenScheduled = false;
            _audioDspScheduledTime = 0.0;
            OnStopPlaying?.Invoke();
        }

        /// <summary>
        /// Stop the audio playback and reset the time to 0
        /// </summary>
        public void Stop()
        {
            Pause();
            _currentTime = 0f;
        }

        /// <summary>
        /// Sets the time that the audio will start playing at.
        /// Useful for song editors or gameplay that requires you to start at a specific time in the song
        /// </summary>
        /// <param name="time">Time in seconds</param>
        public void SetStartTime(double time)
        {
            if (!_timeInitialized)
            {
                Debug.LogError("Time has not been initialized, please call InitTime() before setting audio start time", gameObject);
                return;
            }

            _sourceStartTime = time;
        }
    }
}
