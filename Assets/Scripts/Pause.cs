using UI.Menu;
using UnityEngine;

public class Pause : MonoBehaviour
{
    private bool isPaused = true;
    
    [SerializeField] private GameObject pauseMenu;
    
    public static Pause Instance;
    
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
