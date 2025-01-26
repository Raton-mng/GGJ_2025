using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowCurb : MonoBehaviour
{
    [SerializeField] private Transform topCurb;
    [SerializeField] private Transform botCurb;
    WaitForSeconds delay;
    [SerializeField] Camera Camera;
    [SerializeField] private float scale = 1.6f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Start()
    {
        delay = new WaitForSeconds(0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(MyCoroutine());
    }
    IEnumerator MyCoroutine()
    {
        float minY = botCurb.position.y;
        float maxY = topCurb.position.y;
        float dy = maxY - minY;
        Debug.Log("m " + minY + " M " + maxY);

        yield return delay;
        transform.position += ((maxY + minY) / 2 - transform.position.y) * Vector3.up;
        Camera.orthographicSize = scale * dy;
    }
}
