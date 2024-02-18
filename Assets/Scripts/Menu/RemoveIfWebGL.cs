using UnityEngine;

namespace RhythmEngine.Menu
{
    public class RemoveIfWebGL : MonoBehaviour
    {
        private void Awake()
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                Destroy(gameObject);
            }
        }
    }
}
