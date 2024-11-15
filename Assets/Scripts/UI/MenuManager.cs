using UnityEngine;
using UnityEngine.SceneManagement;

namespace Oathstring
{
    public class MenuManager : MonoBehaviour
    {
        private bool m_paused;

        [SerializeField] MenuType type;
        [Space]
        [SerializeField] GameObject menu;
        [SerializeField] GameObject options;

        public static MenuManager Instance;

        public bool Paused
        {
            get { return m_paused; }
        }

        // Awake is called when the script object is initialised
        private void Awake()
        {
            if (!Instance) Instance = this;
        }

        private void Start()
        {
            if (type == MenuType.Main)
            {
                //Active Menu first, and Deactive all ui
                menu.SetActive(true);
                options.SetActive(false);
            }

            else
            {
                //Deactive all ui
                menu.SetActive(false);
                options.SetActive(false);
            }
        }

        // Update is called once per frame
        private void Update()
        {
            // for main only
            m_paused = (type != MenuType.Main || !m_paused) && m_paused;

            // for pause only
            if(type == MenuType.Pause)
            {
                if (m_paused)
                {
                    Time.timeScale = 0;
                }

                else
                {
                    Time.timeScale = 1;

                    //Deactive all ui
                    menu.SetActive(false);
                    options.SetActive(false);
                }

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (menu.activeSelf) //currently active menu ui, press escape again to resume the game
                    {
                        m_paused = !m_paused;
                    }

                    else if (m_paused) // currently active options ui, press escape back to menu
                    {
                        BackBtn();
                    }

                    else // do.... pause
                    {
                        m_paused = true;

                        menu.SetActive(true);
                        options.SetActive(false);
                    }
                }
            }
        }

        // for main only
        public void StartGameBtn()
        {
            //do.... Start The Game
            //scene manager....
            SceneManager.LoadScene("Game");
        }

        public void QuitBtn()
        {
            //do.... quit the application / game
            Application.Quit();
        }

        // for pause only
        public void ResumeBtn()
        {
            m_paused = false;
        }

        public void RTMBtn()
        {
            //do.... Back Again to Menu
            //scene manager....
            SceneManager.LoadScene("Menu");
        }

        // for universal
        public void OptionsBtn()
        {
            //do.... Deactive Menu, Active Options
            menu.SetActive(false);
            options.SetActive(true);
        }

        public void BackBtn()
        {
            //do.... Deactive Current UI Opened, Active Menu
            menu.SetActive(true);
            options.SetActive(false);
        }
    }

    public enum MenuType
    {
        Main, Pause
    }
}
