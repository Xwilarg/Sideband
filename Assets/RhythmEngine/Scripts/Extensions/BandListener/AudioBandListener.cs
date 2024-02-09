using UnityEngine;

namespace RhythmEngine
{
    /// <summary>
    /// Class that hold an instance of an AudioBand and provides a simple interface to get the values of the bands
    /// </summary>
    [AddComponentMenu("Rhythm Engine/Extensions/Audio Band Listener")]
    public class AudioBandListener : RhythmEngineExtension
    {
        [Tooltip("If true, the AudioBandListener will not be updated by the RhythmEngine, and will instead be updated by an AudioSource.")]
        [SerializeField] private bool StandaloneMode;
        [SerializeField] private AudioSource AudioSource;

        [Space]
        [Tooltip("The number of bands to use. 8 is recommended for most cases, but 64 could be used if you need more precision.")]
        public AudioBand.BandCount BandCount = AudioBand.BandCount.Eight;

        public bool IsPlaying => _musicSource.isPlaying;

        private AudioSource _musicSource;
        private AudioBand _audioBand;

        /// <summary>
        /// Get specified band's frequency
        /// </summary>
        /// <param name="band">Index of a band you want to get the value of</param>
        /// <param name="buffered">Provides smoother falloff of values</param>
        /// <returns></returns>
        public float Frequency(int band, bool buffered = false) => _audioBand.GetFrequencyBand(band, buffered);

        /// <summary>
        /// Get specified band's normallized frequency (0 to 1)
        /// </summary>
        /// <param name="band">Index of a band you want to get the value of</param>
        /// <param name="buffered">Provides smoother falloff of values</param>
        /// <returns></returns>
        public float Band(int band, bool buffered = false) => _audioBand.GetAudioBand(band, buffered);

        /// <summary>
        /// Get the total amplitude of the audio source
        /// </summary>
        /// <param name="buffered">Provides smoother falloff of values</param>
        /// <returns></returns>
        public float Amplitude(bool buffered = false) => _audioBand.GetAmplitude(buffered);

        private void Start()
        {
            _musicSource = StandaloneMode ? AudioSource : RhythmEngine.MusicSource;

            if (_musicSource == null)
            {
                Debug.LogError("AudioBandListener: No AudioSource found. Please assign an AudioSource to the AudioBandListener or use the RhythmEngine.");
            }

            _audioBand = new AudioBand(BandCount);
        }

        private void Update()
        {
            if (!StandaloneMode && !RhythmEngine.IsSourcePlaying) return;

            _audioBand.Update(sample => _musicSource.GetSpectrumData(sample, 0, FFTWindow.Blackman));
        }
    }
}
