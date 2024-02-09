using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmEngine
{
    public abstract class AudioBandListenerExtension : MonoBehaviour
    {
        [SerializeField] protected AudioBandListener AudioBandListener;
        [Tooltip("Depending on the AudioBandListener.BandCount, 0-7 or 0-63, represents part of the frequency spectrum, 0 being the lowest bass.")]
        [SerializeField] protected int Band;

        protected virtual void Start()
        {
            if (AudioBandListener == null)
            {
                Debug.LogError($"AudioBandListenerExtension: No AudioBandListener found. Please assign an AudioBandListener to {GetType()}.", gameObject);
            }

            var maxBand = AudioBandListener.BandCount == AudioBand.BandCount.Eight ? 7 : 63;
            if (Band < 0 || Band > maxBand)
            {
                Debug.LogError($"AudioBandListenerExtension: Band index out of range. Please assign a valid band index to {GetType()}.", gameObject);
            }
        }
    }
}
