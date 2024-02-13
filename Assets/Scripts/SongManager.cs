using RhythmJam2024.SO;
using UnityEngine;

namespace RhythmJam2024
{
    public class SongManager : MonoBehaviour
    {
        public static SongManager Instance { private set; get; }

        [SerializeField]
        private TwoToneSong[] _songs;

        public TwoToneSong[] Songs => _songs;

        private void Awake()
        {
            Instance = this;
        }
    }
}
