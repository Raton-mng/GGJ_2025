using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCurb : MonoBehaviour
{
    [SerializeField] private float speed = 0.5f;
    [SerializeField] private LineRenderer lineRenderer;
    private List<Vector3> curvePoints = new(); // Points de la courbe supérieure



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        curvePoints.Add(transform.position);

        MoveCurvesBackward();
        // Mettre à jour les LineRenderers
        RenderCurves();
    }

    void MoveCurvesBackward()
    {
        // Décaler tous les points vers la gauche (axe X)
        for (int i = 0; i < curvePoints.Count; i++)
        {
            curvePoints[i] = new Vector3(
                curvePoints[i].x - speed * Time.deltaTime,
                curvePoints[i].y,
                curvePoints[i].z
            );
        }

        // Synchroniser currentX avec le dernier segment
        /*if (curvePoints.Count > 0)
        {
            currentX = curvePoints[curvePoints.Count - 1].x + segmentWidth;
        }*/
    }
    void RenderCurves()
    {
        // Appliquer les points aux LineRenderers
        lineRenderer.positionCount = curvePoints.Count;
        lineRenderer.SetPositions(curvePoints.ToArray());

    }
}
