using UnityEngine;
using UnityEngine.SceneManagement;

namespace RhythmJam2024
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private GameObject _creditsPanel;

        public void PlayAlone()
        {
            StaticData.IsAgainstAI = true;
            SceneManager.LoadScene("LevelSelector");
        }

        public void Play2P()
        {
            StaticData.IsAgainstAI = false;
            SceneManager.LoadScene("LevelSelector");
        }

        public void OpenCredits()
        {
            _creditsPanel.SetActive(true);
        }

        public void CloseCredits()
        {
            _creditsPanel.SetActive(false);
        }

        public void Quit()
        {
            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                Application.Quit();
            }
        }
    }
}