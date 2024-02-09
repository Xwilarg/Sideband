using UnityEngine;

namespace RhythmEngine
{
    [AddComponentMenu("Rhythm Engine/Extensions/Audio Band Listener Extensions/Scale To Band")]
    public class ScaleToBand : AudioBandListenerExtension
    {
        [Space]
        [SerializeField] private Vector3 MinScale = Vector3.one;
        [SerializeField] private Vector3 MaxScale = Vector3.one * 1.1f;

        [Space]
        [Tooltip("Buffered values provide smoother falloff of values")]
        [SerializeField] private bool UseBufferedValues = true;
        [Tooltip("Additional smoothing")]
        [SerializeField] private float LerpSpeed = 8;

        private Vector3 _currentScale;

        private void Update()
        {
            if (!AudioBandListener.IsPlaying) return;

            var band = AudioBandListener.Band(Band, UseBufferedValues);
            var scale = Vector3.Lerp(MinScale, MaxScale, band);

            _currentScale = LerpSpeed == 0 ? scale : Vector3.Lerp(_currentScale, scale, Time.deltaTime * LerpSpeed);

            transform.localScale = _currentScale;
        }
    }
}
