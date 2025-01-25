using UnityEngine;

public class CoinController : MonoBehaviour
{
    private float speed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 behind = new Vector3(-1f,0f,0f);
        transform.Translate(behind * Time.deltaTime * speed);

        if (transform.position.x < -30){
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D col){
        Debug.Log("nique ta mere");
        Destroy(gameObject);
    }
}
