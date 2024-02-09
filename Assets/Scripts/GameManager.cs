using RhythmEngine;
using RhythmJam2024.SO;
using UnityEngine;

namespace RhythmJam2024
{
    public class GameManager : MonoBehaviour
    {   
        [SerializeField]
        private ToneAudioManager _goodEngine, _badEngine;

        [SerializeField]
        private TwoToneSong _song;

        private void Awake()
        {
            _goodEngine.Engine.SetSong(new Song()
            {
                Clip = _song.GoodClip,
                BaseBpm = _song.NoteData.BaseBpm
            });
            _badEngine.Engine.SetSong(new Song()
            {
                Clip = _song.BadClip,
                BaseBpm = _song.NoteData.BaseBpm
            });

            _goodEngine.Engine.InitTime();
            _badEngine.Engine.InitTime();

            _goodEngine.Engine.Play();
            _badEngine.Engine.Play();

            _goodEngine.SetVolume(.5f);
            _badEngine.SetVolume(.5f);
        }
    }
}
