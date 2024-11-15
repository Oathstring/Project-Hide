using UnityEngine;
using UnityEngine.SceneManagement;

namespace Oathstring
{
    public class QuitShortcut : MonoBehaviour
    {
        // Awake is called when the script object is initialised
        private void Awake()
        {
        
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        public void RestartButton()
        {
            SceneManager.LoadScene(0);
        }
    }
}
