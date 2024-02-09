using UnityEngine;

namespace RhythmEngine
{
    [AddComponentMenu("Rhythm Engine/Extensions/Audio Band Listener Extensions/Position To Band")]
    public class PositionToBand : AudioBandListenerExtension
    {
        [Space]
        [SerializeField] private Vector3 MinPosition = Vector3.zero;
        [SerializeField] private Vector3 MaxPosition = Vector3.zero;

        [Space]
        [Tooltip("Buffered values provide smoother falloff of values")]
        [SerializeField] private bool UseBufferedValues = true;
        [Tooltip("Additional smoothing")]
        [SerializeField] private float LerpSpeed = 8;

        private Vector3 _currentPos;

        private void Update()
        {
            if (!AudioBandListener.IsPlaying) return;

            var band = AudioBandListener.Band(Band, UseBufferedValues);
            var pos = Vector3.Lerp(MinPosition, MaxPosition, band);

            _currentPos = LerpSpeed == 0 ? pos : Vector3.Lerp(_currentPos, pos, Time.deltaTime * LerpSpeed);

            transform.localPosition = _currentPos;
        }
    }
}
