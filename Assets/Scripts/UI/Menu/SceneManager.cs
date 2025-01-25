using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.Menu
{
    public class SceneManager : MonoBehaviour
    {
        public static SceneManager Instance;

        private void Awake()
        {
            if (Instance)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        public void StartGame()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("_Main");
        }

        public void MainMenu()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("_Menu");
        }
    }
}
