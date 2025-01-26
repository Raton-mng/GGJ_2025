using UI.Menu;
using UnityEngine;

public class Pause : MonoBehaviour
{
    private bool isPaused = true;

    public static Pause Instance;
    
    [SerializeField] private GameObject pauseMenu;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Executé quand le binding de pause est trigger
    public void OnPauseButton()
    {
        Debug.Log("OnPauseButton");
        if (!isPaused)
        {
            isPaused = true;
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
            AudioManager.Instance.PauseMenuOpened();
        }
        else
        {
            isPaused = false;
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
            AudioManager.Instance.PauseMenuClosed();
        }
    }

    // Executer quand le boutton Continuer est trigger
    public void OnContinueButton()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        AudioManager.Instance.PauseMenuClosed();
    }

    // Executer quand le boutton Main menu est trigger
    public void OnMainMenuButton()
    {
        SceneManager.MainMenu();
    }

    void Start()
    {
        OnPauseButton();
    }
}
