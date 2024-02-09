using UnityEngine;

namespace RhythmEngine
{
    [AddComponentMenu("Rhythm Engine/Extensions/Audio Band Listener Extensions/Rotate To Band")]
    public class RotateToBand : AudioBandListenerExtension
    {
        [Space]
        [SerializeField] private Vector3 MinRotation = Vector3.zero;
        [SerializeField] private Vector3 MaxRotation = Vector3.zero;

        [Space]
        [Tooltip("Buffered values provide smoother falloff of values")]
        [SerializeField] private bool UseBufferedValues = true;
        [Tooltip("Additional smoothing")]
        [SerializeField] private float LerpSpeed = 8;

        private Vector3 _currentRotation;

        private void Update()
        {
            if (!AudioBandListener.IsPlaying) return;

            var band = AudioBandListener.Band(Band, UseBufferedValues);
            var rot = Vector3.Lerp(MinRotation, MaxRotation, band);

            _currentRotation = LerpSpeed == 0 ? rot : Vector3.Lerp(_currentRotation, rot, Time.deltaTime * LerpSpeed);

            transform.localRotation = Quaternion.Euler(_currentRotation);
        }
    }
}
