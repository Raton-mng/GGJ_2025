using System.Collections;
using UnityEngine;

public class PlayerCurb : MonoBehaviour
{
    [SerializeField] private float speed = 0.5f;
    [SerializeField] private float xPlane = 3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Update());

        IEnumerator Update()
        {
            while(transform.position.x < xPlane)
            {
                transform.position += (Vector3)Vector2.right * Time.deltaTime * speed;
                yield return 0;
            }
            CurbManager.m_Instance.enabled = true;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButton(0)) transform.position += (Vector3)Vector2.up * Time.deltaTime;
        if (Input.GetMouseButton(1)) transform.position += (Vector3)Vector2.down * Time.deltaTime;
    }

}
