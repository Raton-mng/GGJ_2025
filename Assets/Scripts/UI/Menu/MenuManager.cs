using UnityEngine;

public class MenuManager : MonoBehaviour
{

    public Pause MenuPause;
    [SerializeField] private GameObject finMenu;


    public static MenuManager Instance;
    
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

    public void OnGameEnd()
    {
        Instantiate(finMenu);

    }

}
