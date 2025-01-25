using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    private HashSet<PlayerController> _players;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        _players = new HashSet<PlayerController>();
        PlayerInputManager playerInputManager = GetComponent<PlayerInputManager>();
        playerInputManager.playerJoinedEvent.AddListener(OnAddPlayer);
        playerInputManager.playerLeftEvent.AddListener(OnRemovePlayer);
    }

    public void OnAddPlayer(PlayerInput newInput)
    {
        PlayerController player = newInput.GetComponent<PlayerController>();
        Debug.Log("joined : " + player);
        player.myID = _players.Count;
        _players.Add(player);
    }

    public void OnRemovePlayer(PlayerInput removedInput)
    {
        PlayerController removedPlayer = removedInput.GetComponent<PlayerController>();
        Debug.Log("left : " + removedPlayer);
        int oldID = removedPlayer.myID;
        _players.Remove(removedPlayer);
        foreach (PlayerController player in _players)
        {
            if (player.myID > oldID) player.myID -= 1;
        }
    }

    public PlayerController GetPlayer(int playerID)
    {
        foreach (PlayerController player in _players)
        {
            if (player.myID == playerID) return player;
        }
        return null;
    }

    public List<PlayerController> GetOtherPlayers(int playerID)
    {
        List<PlayerController> otherPlayers = new List<PlayerController>();
        foreach (PlayerController player in _players)
        {
            if (player.myID != playerID) otherPlayers.Add(player);
        }
        return otherPlayers;
    }

    public int GetPlayerCount()
    {
        return _players.Count;
    }
}
