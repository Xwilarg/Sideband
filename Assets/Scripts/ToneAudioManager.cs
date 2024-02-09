using RhythmEngine;
using UnityEngine;

namespace RhythmJam2024
{
    public class ToneAudioManager : MonoBehaviour
    {
        public RhythmEngineCore Engine { private set; get; }

        private AudioSource _source;

        private void Awake()
        {
            Engine = GetComponent<RhythmEngineCore>();
            _source = GetComponentInChildren<AudioSource>();
        }

        public void SetVolume(float value)
        {
            _source.volume = MaxVolume * value;
        }

        private const float MaxVolume = .4f;
    }
}
