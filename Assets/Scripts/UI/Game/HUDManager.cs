using UnityEngine;

public class HUDManager : MonoBehaviour
{
    static public HUDManager Instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        HUDManager instance = this;
        enabled = true;
    }

    private void OnEnable()
    {
        InitCanvas();
        Debug.Log(gameObject);
    }
    void InitCanvas()
    {
        foreach (PlayerController player in PlayerManager.Instance.GetPlayerControllers())
        {
            Debug.Log("HUD " + player.GetHUD());
            GameObject playerHUD = Instantiate(player.GetHUD(), transform);
            int myID = player.myID;
            if (player.myID == 0)
            {
                playerHUD.transform.position = new Vector3(283, 895, 0);
            }
            else if (myID == 1)
            {
                playerHUD.transform.position = new Vector3(1618, 914, 0);
            }
            else if (myID == 2)
            {
                playerHUD.transform.position = new Vector3(283, 145, 0);
            }
            else if (myID == 3)
            {
                playerHUD.transform.position = new Vector3(1618, 145, 0);
            }
        }
    }
}
