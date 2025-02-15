using System.Collections.Generic;
using UI.Menu;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    private PlayerInputManager playerInputManager;

    public static int playerCount = 0;

    private List<GameObject> _players;

    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _players = new List<GameObject>();
        
            playerInputManager.playerLeftEvent.AddListener(OnRemovePlayer);

        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void SetupPlayerForGame()
    {
        playerInputManager.DisableJoining();
        for (int i = 0; i < _players.Count; i++)
        {
            GameObject player = _players[i];
            player.GetComponent<PlayerInput>().actions.FindActionMap("Player").Enable();
            player.GetComponent<PlayerInput>().actions.FindActionMap("PlayerUI").Disable();
            player.transform.GetChild(0).gameObject.SetActive(false);
            player.transform.GetChild(1).gameObject.SetActive(true);
            player.GetComponentInChildren<PlayerController>().myID = i;
            player.transform.GetChild(1).transform.position = new Vector3(0, i * 4, 0);

        }
    }

    public void SetupPlayerForUI()
    {
        playerInputManager.EnableJoining();
        foreach (GameObject player in _players)
        {
            player.GetComponent<PlayerInput>().actions.FindActionMap("Player").Disable();
            player.GetComponent<PlayerInput>().actions.FindActionMap("PlayerUI").Enable();
            player.transform.GetChild(0).gameObject.SetActive(true);
            player.transform.GetChild(1).gameObject.SetActive(false);

            PlayerStartController playerStartController = player.transform.GetChild(0).GetComponent<PlayerStartController>();
            CellManager.Instance.players.Add(playerStartController);
        }

    }

    public void OnAddPlayer(PlayerInput newInput)
    {
        newInput.actions.FindActionMap("Player").Disable();
        
        GameObject player = newInput.gameObject;
        
        DontDestroyOnLoad(player);
        //player.transform.SetParent(transform);
        
       _players.Add(player);
        player.GetComponentInChildren<PlayerStartController>().SetID(playerCount);
        playerCount++;

    }

    public void OnRemovePlayer(PlayerInput removedInput)
    {
        _players.Remove(removedInput.gameObject);
        Destroy(removedInput.gameObject);
        playerCount--;
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
        playerCount--;
        if (playerCount <= 1)
        {
            AudioManager.Instance.EndMusic();
            MenuManager.Instance.OnGameEnd();
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
