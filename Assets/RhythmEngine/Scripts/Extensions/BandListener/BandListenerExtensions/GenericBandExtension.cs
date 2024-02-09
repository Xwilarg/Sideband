using UnityEngine;
using UnityEngine.Events;

namespace RhythmEngine
{
    public abstract class GenericBandExtension<T> : AudioBandListenerExtension
    {
        [Space]
        [SerializeField] private T MinValue;
        [SerializeField] private T MaxValue;
        [SerializeField] private UnityEvent<T> OnValueChanged;

        [Space]
        [Tooltip("Buffered values provide smoother falloff of values")]
        [SerializeField] private bool UseBufferedValues = true;
        [Tooltip("Additional smoothing")]
        [SerializeField] private float LerpSpeed = 8;

        private T _currentValue;

        private void Update()
        {
            if (!AudioBandListener.IsPlaying) return;

            var band = AudioBandListener.Band(Band, UseBufferedValues);
            var value = Lerp(MinValue, MaxValue, band);

            _currentValue = LerpSpeed == 0 ? value : Lerp(_currentValue, value, Time.deltaTime * LerpSpeed);

            OnValueChanged.Invoke(_currentValue);
        }

        protected abstract T Lerp(T a, T b, float t);
    }
}
