using FMODUnity;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    enum CoinType  {
        COIN, BIG_COIN, BAD_COIN
    }
    
    private float speed = 5f;
    [SerializeField] int value;
    [SerializeField] private CoinType type;
    
    [SerializeField] private EventReference coinEvent;
    private FMOD.Studio.EventInstance coinEventInstance;
    
    void Start()
    {
        coinEventInstance = RuntimeManager.CreateInstance(coinEvent);
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
        PlayCoinSound();
        Destroy(gameObject);
    }

    void PlayCoinSound()
    {
        coinEventInstance.setParameterByName("coin-type", (int)type);
        coinEventInstance.start();
        coinEventInstance.release();
    }
}
