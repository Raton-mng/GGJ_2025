using UnityEngine;

public class CoinController : MonoBehaviour
{
    private float speed = 5f;
    [SerializeField] int value;
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

    void OnTriggerEnter(Collider col){
        PlayerController player = col.GetComponent<PlayerController>();
        CoinManager wallet = player.coinManager;
        wallet.EarnCoins(value);
        Debug.Log("nique ta mere");
        Destroy(gameObject);
    }
}
