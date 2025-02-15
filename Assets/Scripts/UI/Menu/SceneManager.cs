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
            PlayerManager.Instance.SetupPlayerForGame();
            UnityEngine.SceneManagement.SceneManager.LoadScene("_Main");
        }

        public static void MainMenu()
        {
            PlayerManager.Instance.SetupPlayerForUI();
            UnityEngine.SceneManagement.SceneManager.LoadScene("_Menu");
        }
    }
}
