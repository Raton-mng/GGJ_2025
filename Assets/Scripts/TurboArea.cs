using UnityEngine;

public class TurboArea : MonoBehaviour
{
    private float turboFillDuration = 5f;

    [SerializeField] private float baseDuration = 1f;
    [SerializeField] private float amplitude = 1f;
    [SerializeField] private float tau = 30f;

    [SerializeField] private float frequencyAcceleration = 1f;


    // Update is called once per frame
    void Update()
    {
        turboFillDuration = baseDuration + amplitude * (1-Mathf.Exp(-Time.time / tau));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*PlayerStats stats;
        if (collision.TryGetComponent(out stats))
        {
            stats.power += turboFillDuration * Time.deltaTime;
        }*/
    }
}
