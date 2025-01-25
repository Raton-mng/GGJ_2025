using System.Collections.Generic;
using UI.Menu;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    private List<GameObject> _players;

    private void Awake()
    {
        PlayerInputManager playerInputManager = GetComponent<PlayerInputManager>();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _players = new List<GameObject>();
        
            playerInputManager.playerJoinedEvent.AddListener(OnAddPlayer);
            playerInputManager.playerLeftEvent.AddListener(OnRemovePlayer);

            SceneManager.sceneLoaded += OnLoad;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnLoad(Scene scene, LoadSceneMode lsm)
    {
        Debug.Log(_players.Count);
        for (int i = 0; i < _players.Count; i++)
        {
            Debug.Log(_players.Count + "; i = " + i);
            GameObject player = _players[i];
            Transform playerChild = player.transform.GetChild(0);
            if (playerChild.TryGetComponent(out PlayerStartController psc))
            {
                //Debug.Log(player);
                //Debug.Log(playerChild);
                playerChild.gameObject.SetActive(false);
                //Debug.Log(playerChild);
                //Debug.Log(transform.GetChild(1).gameObject);
                player.transform.GetChild(1).gameObject.SetActive(true);
                PlayerController pc = player.transform.GetChild(1).GetComponent<PlayerController>();
                //Debug.Log(pc);
                pc.myID = _players.Count;
            }
            else
            {
                playerChild.gameObject.SetActive(true);
                PlayerController pc = playerChild.GetComponent<PlayerController>();
                pc.myID = _players.Count;
                player.transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }

    public void OnAddPlayer(PlayerInput newInput)
    {
        GameObject player = newInput.transform.parent.gameObject;
        
        DontDestroyOnLoad(player);
        //player.transform.SetParent(transform);
        
       _players.Add(player);
    }

    public void OnRemovePlayer(PlayerInput removedInput)
    {
        _players.Remove(removedInput.transform.parent.gameObject);
    }

    public PlayerController GetPlayer(int playerID)
    {
        foreach (GameObject playerGo in _players)
        {
            PlayerController player = playerGo.GetComponentInChildren<PlayerController>();
            if (player.myID == playerID) return player;
        }
        return null;
    }

    public List<PlayerController> GetOtherPlayers(int playerID)
    {
        List<PlayerController> otherPlayers = new List<PlayerController>();
        foreach (GameObject playerGo in _players)
        {
            PlayerController player = playerGo.GetComponentInChildren<PlayerController>();
            if (player.myID != playerID) otherPlayers.Add(player);
        }
        return otherPlayers;
    }

    public int GetPlayerCount()
    {
        return _players.Count;
    }
}
