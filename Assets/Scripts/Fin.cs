using UI.Menu;
using UnityEngine;

public class Fin : MonoBehaviour
{

    public void Start()
    {
        Time.timeScale = 0;
    }

    public void OnMainMenuButton()
    {
        SceneManager.MainMenu();
    }
}
