using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(TrailRenderer))]
public class TrailCollider : MonoBehaviour
{

    TrailRenderer trailRenderer;
    public float detectionRange = 1.0f;

    public UnityEvent evenement;
    public Rigidbody otherRb;

    private void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
    }

    private void FixedUpdate()
    {

   /*   //I dont want to do this raycasting all the time, so I am going to check if the player
        //is close enough, if not we can ignore trying to detect collisions
        if (!Physics.CheckSphere(transform.position, detectionRange, LayerMask.GetMask("Curbs")))
        {

            return;
        }
   */
        Debug.Log("Continue !");

        for (int i = 0; i < trailRenderer.positionCount; i++)
        {
            if (i == trailRenderer.positionCount - 1)
                continue;

            float t = i / (float)trailRenderer.positionCount;

            //get the approximate width of the line segment
            float width = trailRenderer.widthCurve.Evaluate(t);

            Vector3 startPosition = trailRenderer.GetPosition(i);
            Vector3 endPosition = trailRenderer.GetPosition(i + 1);
            Vector3 direction = endPosition - startPosition;
            float distance = Vector3.Distance(endPosition, startPosition);

            RaycastHit hit;

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
            if (Physics.SphereCast(startPosition, width, direction, out hit, distance, LayerMask.GetMask("Curbs")))
            {
                evenement.Invoke();
                otherRb = hit.rigidbody;
                return;
            }

        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

}