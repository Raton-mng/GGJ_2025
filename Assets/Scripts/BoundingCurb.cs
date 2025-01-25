using UnityEngine;

public class BoundingCurb : MonoBehaviour
{
    [SerializeField] private float translationMinAmplitude = 5;
    [SerializeField] private float translationMaxAmplitude = 15;

    [SerializeField] private float verticalSpeed = 1f;

    [SerializeField] private Rigidbody[] curbs = new Rigidbody[2];

    //chaque curbs a en child la death curb et la turbo curb

    // Update is called once per frame
    void Update()
    {
        float r1 = Random.value -0.5f; float r2 = Random.value -0.5f;
        float d = curbs[0].position.y - curbs[1].position.y;
        // Les deux randoms permettent de garder la même taille, ou alors on a 
        if (d >= translationMaxAmplitude)
        {
            if (r1 < 0) curbs[0].linearVelocity += r1 * verticalSpeed * Vector3.up * Time.deltaTime; 
            if (r2 > 0) curbs[1].linearVelocity += r2 * verticalSpeed * Vector3.up * Time.deltaTime;
        }
        else if (d <= translationMinAmplitude)
        {
            if (r1 > 0) curbs[0].linearVelocity += r1 * verticalSpeed * Vector3.up * Time.deltaTime;
            if (r2 < 0) curbs[1].linearVelocity += r2 * verticalSpeed * Vector3.up * Time.deltaTime;
        }
        else
        {
            curbs[0].linearVelocity += r1 * verticalSpeed * Vector3.up * Time.deltaTime;
            curbs[1].linearVelocity += r2 * verticalSpeed * Vector3.up * Time.deltaTime;
        }
    }
}
