using System;
using UnityEngine;

namespace RhythmEngine
{
    // Original concept of this class by Peer Play, https://www.youtube.com/@PeerPlay, modified by me.

    /// <summary>
    /// A class that can be used to get the frequency bands of an audio source.
    /// </summary>
    public class AudioBand
    {
        public enum BandCount
        {
            Eight = 8,
            SixtyFour = 64
        }

        private readonly int _bandCount;

        private readonly float[] _originalSamplesLeft;
        private readonly float[] _bandFrequency;
        private readonly float[] _bandFrequencyNormalized;
        private readonly float[] _bandFrequencyHighest;
        private readonly float[] _bandFrequencyBuffer;
        private readonly float[] _bufferDecrease;
        private readonly float[] _bandFrequencyBufferNormalized;

        private float _amplitudeHighest = 5;
        private float _amplitudeBuffer;
        private float _amplitude;

        private const int SamplesCount = 512;
        private const float BufferDecreaseMin = 0.005f;
        private const float BufferDecreaseMultiplier = 1.2f;

        /// <summary>
        /// Create a new instance of AudioBand with the specified band count
        /// </summary>
        /// <param name="bandCount"></param>
        public AudioBand(BandCount bandCount)
        {
            _bandCount = (int)bandCount;
            _originalSamplesLeft = new float[SamplesCount];
            _bandFrequency = new float[_bandCount];
            _bandFrequencyNormalized = new float[_bandCount];
            _bandFrequencyHighest = new float[_bandCount];
            _bandFrequencyBuffer = new float[_bandCount];
            _bufferDecrease = new float[_bandCount];
            _bandFrequencyBufferNormalized = new float[_bandCount];
            AudioProfile();
        }

        /// <summary>
        /// Get specified band's frequency.
        /// </summary>
        /// <param name="bandIndex">Index of a band you want to get the value of.</param>
        /// <param name="buffered">Provides smoother falloff of values.</param>
        /// <returns></returns>
        public float GetFrequencyBand(int bandIndex, bool buffered = false)
        {
            if (bandIndex < 0 || bandIndex >= _bandCount) throw new IndexOutOfRangeException("Band index is out of range, use a value between 0 and " + (_bandCount - 1) + ".");

            return buffered ? _bandFrequencyBuffer[bandIndex] : _bandFrequency[bandIndex];
        }

        /// <summary>
        /// Get specified band's normallized frequency (0 to 1).
        /// </summary>
        /// <param name="bandIndex">Index of a band you want to get the value of.</param>
        /// <param name="buffered">Provides smoother falloff of values.</param>
        /// <returns></returns>
        public float GetAudioBand(int bandIndex, bool buffered = false)
        {
            if (bandIndex < 0 || bandIndex >= _bandCount) throw new IndexOutOfRangeException("Band index is out of range, use a value between 0 and " + (_bandCount - 1) + ".");

            var band = buffered ? _bandFrequencyBufferNormalized[bandIndex] : _bandFrequencyNormalized[bandIndex];
            return Mathf.Clamp01(band);
        }

        /// <summary>
        /// Get the total amplitude of the audio source.
        /// </summary>
        /// <param name="buffered">Provides smoother falloff of values.</param>
        /// <returns></returns>
        public float GetAmplitude(bool buffered = false)
        {
            return buffered ? _amplitudeBuffer : _amplitude;
        }

        /// <summary>
        /// Update the band values.
        /// </summary>
        /// <param name="getSamples">A delegate that assigns samples from AudioSource.GetSpectrumData</param>
        public void Update(Action<float[]> getSamples)
        {
            getSamples(_originalSamplesLeft);
            UpdateFrequencyBands();
            UpdateBuffer();
            UpdateFrequencyBandsNormilized();
            UpdateAmplitude();
        }

        private void UpdateFrequencyBands()
        {
            if (_bandCount == 8) UpdateFrequencyBands8();
            else if (_bandCount == 64) UpdateFrequencyBands64();
        }

        private void UpdateFrequencyBands8()
        {
            int count = 0;

            for (int i = 0; i < _bandCount; i++)
            {
                float average = 0;
                int sampleCount = (int)Math.Pow(2, i) * 2;

                if (i == _bandCount - 1) sampleCount += 2;

                for (int j = 0; j < sampleCount; j++)
                {
                    average += _originalSamplesLeft[count] * (count + 1);
                    count++;
                }

                average /= count;
                _bandFrequency[i] = average * 10;
            }
        }

        private void UpdateFrequencyBands64()
        {
            int count = 0;
            int sampleCount = 1;
            int power = 0;

            for (int i = 0; i < _bandCount; i++)
            {
                float average = 0;

                if (i is 16 or 32 or 40 or 48 or 56)
                {
                    power++;
                    sampleCount = (int)Math.Pow(2, power);

                    if (power == 3) sampleCount -= 2;
                }

                for (int j = 0; j < sampleCount; j++)
                {
                    average += _originalSamplesLeft[count] * (count + 1);
                    count++;
                }

                average /= count;
                _bandFrequency[i] = average * 80;
            }
        }

        private void UpdateFrequencyBandsNormilized()
        {
            for (int i = 0; i < _bandCount; i++)
            {
                if (_bandFrequency[i] > _bandFrequencyHighest[i]) _bandFrequencyHighest[i] = _bandFrequency[i];

                _bandFrequencyNormalized[i] = _bandFrequency[i] / _bandFrequencyHighest[i];
                _bandFrequencyBufferNormalized[i] = _bandFrequencyBuffer[i] / _bandFrequencyHighest[i];
            }
        }

        private void AudioProfile()
        {
            for (int i = 0; i < _bandCount; i++) _bandFrequencyHighest[i] = 1;
        }

        private void UpdateAmplitude()
        {
            float currAmplitude = 0;
            float currAmplitudeBuffer = 0;

            for (int i = 0; i < _bandCount; i++)
            {
                currAmplitude += _bandFrequencyNormalized[i];
                currAmplitudeBuffer += _bandFrequencyBufferNormalized[i];
            }

            if (currAmplitude > _amplitudeHighest) _amplitudeHighest = currAmplitude;

            _amplitude = currAmplitude / _amplitudeHighest;
            _amplitudeBuffer = currAmplitudeBuffer / _amplitudeHighest;
        }

        private void UpdateBuffer()
        {
            for (int i = 0; i < _bandCount; i++)
            {
                if (_bandFrequency[i] > _bandFrequencyBuffer[i])
                {
                    _bandFrequencyBuffer[i] = _bandFrequency[i];
                    _bufferDecrease[i] = BufferDecreaseMin;
                }

                if (_bandFrequency[i] < _bandFrequencyBuffer[i])
                {
                    _bandFrequencyBuffer[i] -= _bufferDecrease[i];
                    _bufferDecrease[i] *= BufferDecreaseMultiplier;
                }
            }
        }
    }
}
