using System.Collections.Generic;
using UI.Menu;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public Transform canvas; 

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
        //Debug.Log(_players.Count);
        for (int i = 0; i < _players.Count; i++)
        {
            GameObject player = _players[i];
            //Debug.Log("_players : " + _players.Count);
            //Debug.Log("playersCopy : " + playersCopy.Count);
            Transform playerChild = player.transform.GetChild(0);
            if (playerChild.TryGetComponent(out PlayerStartController psc))
            {
                playerChild.gameObject.SetActive(false);
                player.transform.GetChild(1).gameObject.SetActive(true);
                PlayerController pc = player.transform.GetChild(1).GetComponent<PlayerController>();
                pc.myID = i;
            }
            else //ne se passe pas normalement
            {
                playerChild.gameObject.SetActive(true);
                PlayerController pc = playerChild.GetComponent<PlayerController>();
                pc.myID = i;
                player.transform.GetChild(1).gameObject.SetActive(false);
            }
            player.GetComponent<PlayerInput>().actions.FindActionMap("Player").Enable();
            player.GetComponent<PlayerInput>().actions.FindActionMap("PlayerUI").Disable();
        }
        //Debug.Log("_players : " + _players.Count);
        //Debug.Log("playersCopy : " + playersCopy.Count);
    }

    public void OnAddPlayer(PlayerInput newInput)
    {
        newInput.actions.FindActionMap("Player").Disable();
        
        GameObject player = newInput.gameObject;
        
        DontDestroyOnLoad(player);
        //player.transform.SetParent(transform);
        
       _players.Add(player);
        //Debug.Log("PrTOUTE " + _players.Count);

    }

    public void OnRemovePlayer(PlayerInput removedInput)
    {
        _players.Remove(removedInput.gameObject);
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

    public PlayerController GetRandomPlayer(int playerID)
    {
        int otherID;
        do
        {
            otherID = Random.Range(1, _players.Count + 1);
        } while (otherID == playerID);

        return GetPlayer(otherID);
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

    public List<PlayerController> GetPlayerControllers()
    {
        List<PlayerController> otherPlayers = new List<PlayerController>();
        foreach (GameObject playerGo in _players)
        {
            PlayerController player = playerGo.GetComponentInChildren<PlayerController>();
            otherPlayers.Add(player);
        }
        return otherPlayers;
    }

    public void SomeoneDied(GameObject died)
    {
        _players.Remove(died);
        if (_players.Count <= 1)
        {
            MenuManager.Instance.MenuFin.OnGameEnd();
        }
    }

    public List<GameObject> GetPlayers()
    {
        return _players;
    }

    public int GetPlayerCount()
    {
        return _players.Count;
    }
}
