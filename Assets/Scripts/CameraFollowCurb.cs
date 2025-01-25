using System.Collections.Generic;
using UnityEngine;

public class CameraFollowCurb : MonoBehaviour
{
    [SerializeField] private Transform topCurb;
    [SerializeField] private Transform botCurb;

    [SerializeField] Camera Camera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    void Update()
    {

        float minY = botCurb.position.y;
        float maxY = topCurb.position.y;
        float dy = maxY - minY;
        Debug.Log("m " + minY + " M " + maxY);
        transform.position += ((maxY + minY)/2 - transform.position.y) * Vector3.up;
        Camera.orthographicSize = (5/3) * dy;
    }
}
