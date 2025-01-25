using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(TrailRenderer))]
public class TrailCollider : MonoBehaviour
{

    TrailRenderer trailRenderer;
    float detectionRange;

    public UnityEvent evenement;
    public Rigidbody otherRb;

    private void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        detectionRange = trailRenderer.startWidth;
    }

    private void FixedUpdate()
    {

 

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

            if (Physics.SphereCast(startPosition, width, direction, out hit, distance, LayerMask.GetMask("Curbs")))
            {
                evenement?.Invoke();
                otherRb = hit.rigidbody;
                return;
            }

        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, 1);
    }

}