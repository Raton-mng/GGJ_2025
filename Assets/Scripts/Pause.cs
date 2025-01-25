using UI.Menu;
using UnityEngine;

public class Pause : MonoBehaviour
{
    private bool isPaused;
    
    [SerializeField] private GameObject pauseMenu;
    
    
    // Executé quand le binding de pause est trigger
    public void OnPauseButton()
    {
        if (!isPaused)
        {
            isPaused = true;
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
            
        }
        else
        {
            isPaused = false;
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
        }
    }

    // Executer quand le boutton Continuer est trigger
    public void OnContinueButton()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
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
