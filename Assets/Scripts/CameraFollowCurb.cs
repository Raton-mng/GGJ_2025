using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class CameraFollowCurb : MonoBehaviour
{
    [SerializeField] List<Transform> playerTransforms = new List<Transform>();
    [SerializeField] Camera Camera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        List<PlayerController> players = PlayerManager.Instance.GetOtherPlayers(-1); // permet de récupérer toute la liste des joueurs
        foreach (PlayerController player in players)
        {
            playerTransforms.Add(player.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float y = 0f;
        float minY = float.MaxValue;
        float maxY = float.MinValue;
        foreach (Transform player in playerTransforms)
        {
            y += player.position.y;
            if(y < minY) minY = y;
            if(y > maxY) maxY = y;
        }
        transform.position += (y / playerTransforms.Count - transform.position.y) * Vector3.up;
        Debug.Log("min :" + minY + " max :" + maxY + " | dist = " + (maxY - minY));
        Camera.orthographicSize = (5/3) * (maxY - minY);
    }
}
