using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RhythmEngine.Menu
{
    public class LevelSelector : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text[] _before, _after;

        private int _currIndex;

        private void Awake()
        {
            SceneManager.LoadScene("SongData", LoadSceneMode.Additive);
        }
    }
}
