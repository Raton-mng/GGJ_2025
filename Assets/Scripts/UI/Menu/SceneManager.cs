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

        public static void StartGame()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("ValT");
        }

        public static void MainMenu()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("_Menu");
        }
    }
}
