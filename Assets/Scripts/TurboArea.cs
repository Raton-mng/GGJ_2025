using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;


[RequireComponent(typeof(TrailCollider))]
public class TurboArea : MonoBehaviour
{
    private float turboFillDuration = 5f;

    [SerializeField] private float baseDuration = 1f;
    [SerializeField] private float amplitude = 1f;
    [SerializeField] private float tau = 30f;

    [SerializeField] private float frequencyAcceleration = 1f;

    TrailCollider coll;

    [SerializeField] private float cooldown = 0.1f;


    private void Start()
    {
        coll = GetComponent<TrailCollider>();
        coll.evenement.AddListener(OnTrailCollision);
    }

    // Update is called once per frame
    void Update()
    {
        turboFillDuration = baseDuration + amplitude * (1 - Mathf.Exp(-Time.time / tau));

    }

    private void OnTrailCollision()
    {
        coll.otherRb.GetComponent<TurboManager>().GainTurbo( Time.deltaTime / turboFillDuration );
    }
}
