using UI.Menu;
using UnityEngine;

public class Fin : MonoBehaviour
{
    public static Fin Instance;

    [SerializeField] private GameObject finMenu;

    public void OnGameEnd()
    {
        finMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void OnMainMenuButton()
    {
        SceneManager.MainMenu();
    }
}
