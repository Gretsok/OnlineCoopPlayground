using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Booting
{
    public class BootLoader : MonoBehaviour
    {
        [SerializeField]
        private string m_sceneNameToLoad = "MainMenu";
    
        private void Start()
        {
            SceneManager.LoadSceneAsync(m_sceneNameToLoad);
        }
    }
}
