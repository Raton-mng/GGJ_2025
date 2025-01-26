using UnityEngine;

public class MenuManager : MonoBehaviour
{

    public Pause MenuPause;
    public Fin MenuFin;
    
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
    
}
