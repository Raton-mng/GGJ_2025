using System.Collections.Generic;
using UnityEngine;

public class CameraFollowCurb : MonoBehaviour
{
    public List<Transform> playerTransforms = new List<Transform>();
    [SerializeField] Camera Camera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        List<GameObject> players = PlayerManager.Instance.GetPlayers(); // permet de récupérer toute la liste des joueurs
        foreach (GameObject player in players)
        {
            playerTransforms.Add(player.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("count : " + playerTransforms.Count);

        if (playerTransforms.Count == 0) return;
        if (playerTransforms.Count == 1)
        {
            transform.position = playerTransforms[0].position;
            Camera.orthographicSize = 1f;
        }

        float y = 0f;
        float minY = float.MaxValue;
        float maxY = float.MinValue;
        foreach (Transform player in playerTransforms)
        {
            y += player.position.y;
            if(y < minY) minY = player.position.y;
            if(y > maxY) maxY = player.position.y;
        }
        transform.position += (y / playerTransforms.Count - transform.position.y) * Vector3.up;
        Camera.orthographicSize = -(5/3) * (maxY - minY);
    }
}
