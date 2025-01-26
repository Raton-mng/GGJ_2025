using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class PlayerCurb : MonoBehaviour
{
    [SerializeField] private float speed = 0.5f;
    [SerializeField] private LineRenderer lineRenderer;
    private List<Vector3> curvePoints = new(); // Points de la courbe supérieure

    private void Start()
    {
        Material lineMaterial = new Material(Shader.Find("Unlit/Color"));
        int playerId = GetComponent<PlayerController>().myID;
        
        switch (playerId) // Ajout des parenthèses autour de l'expression
        {
            case 1:
                lineMaterial.color = UnityEngine.Color.blue;
                break;
            case 2:
                lineMaterial.color = UnityEngine.Color.green;
                break;
            case 3:
                lineMaterial.color = UnityEngine.Color.magenta;
                break;
            case 4:
                lineMaterial.color = UnityEngine.Color.red;
                break;
            default:
                lineMaterial.color = UnityEngine.Color.white; // Couleur par défaut
                break;
        }
        lineRenderer.material = lineMaterial;

        lineRenderer.startWidth = 0.5f;
        lineRenderer.endWidth = 0.5f;
    }

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
