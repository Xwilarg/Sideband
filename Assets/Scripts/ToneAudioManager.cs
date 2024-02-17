using UnityEngine;

namespace RhythmJam2024
{
    public class ToneAudioManager : MonoBehaviour
    {
        private AudioSource _source;

        private float _delayBeforeSong;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
        }

        public void SetVolume(float value)
        {
            _source.volume = MaxVolume * value;
        }

        public void SetClip(AudioClip cllp)
        {
            _source.clip = cllp;
        }

        public float GetTime()
        {
            return _source.time - _delayBeforeSong;
        }

        public void Play(float timer)
        {
            _delayBeforeSong = timer;
        }

        public void Stop()
        {
            _source.Stop();
        }

        private void Update()
        {
            if (_delayBeforeSong > 0f)
            {
                _delayBeforeSong -= Time.deltaTime;
                if (_delayBeforeSong <= 0f)
                {
                    _source.Play();
                    _source.time = -_delayBeforeSong;
                }
            }
        }

        private const float MaxVolume = .4f;
    }
}
