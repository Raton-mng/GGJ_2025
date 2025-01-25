using System.Collections;
using UnityEngine;

public class PlayerCurb : MonoBehaviour
{
    [SerializeField] private float speed = 0.5f;
    [SerializeField] private float xPlane = 3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int id = GetComponent<PlayerController>().myID;
        transform.parent = CurbManager.m_Instance.transform;
        transform.localPosition = -5f * Vector3.forward + id * Vector3.up;

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

    /* Debug Tools : --------------------- 
        private void Update()
        {
            if (Input.GetMouseButton(0)) transform.position += (Vector3)Vector2.up * Time.deltaTime;
            if (Input.GetMouseButton(1)) transform.position += (Vector3)Vector2.down * Time.deltaTime;
        }
     ----------------------------------- */
}
