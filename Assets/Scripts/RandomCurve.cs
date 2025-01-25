using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class RandomCurves : MonoBehaviour
{
    [Header("Curve Settings")]
    public float segmentWidth = 2f; // Largeur de chaque segment
    public float maxSlope = 2f; // Pente maximale (valeurs positives ou négatives)
    public float initialHeightMin = 1f; // Hauteur minimale initiale
    public float initialHeightMax = 5f; // Hauteur maximale initiale
    public float minCurveSpacing = 2f; // Espacement minimal entre les deux courbes

    [Header("Line Renderer")]
    public LineRenderer upperCurveRenderer; // Renderer pour la courbe supérieure
    public LineRenderer lowerCurveRenderer; // Renderer pour la courbe inférieure

    private List<Vector3> upperCurvePoints; // Points de la courbe supérieure
    private List<Vector3> lowerCurvePoints; // Points de la courbe inférieure

    [Header("Dynamic Curve Extension")]
    public float generationInterval = 1f; // Temps entre chaque génération de segment
    private float currentX; // Position actuelle sur l'axe X
    private float upperCurrentY; // Position Y actuelle de la courbe supérieure
    private float lowerCurrentY; // Position Y actuelle de la courbe inférieure

    void Start()
    {
        // Initialisation des courbes
        upperCurvePoints = new List<Vector3>();
        lowerCurvePoints = new List<Vector3>();

        // Position de départ
        currentX = 0f;
        upperCurrentY = Random.Range(initialHeightMin, initialHeightMax);
        lowerCurrentY = Random.Range(initialHeightMin, initialHeightMax);

        // Générer les premiers segments
        GenerateInitialSegments(10); // Par exemple, démarrer avec 10 segments

        // Configurer les LineRenderers
        ConfigureLineRenderer(upperCurveRenderer, Color.red, 0.1f);
        ConfigureLineRenderer(lowerCurveRenderer, Color.blue, 0.1f);

        // Commencer la génération continue
        StartCoroutine(GenerateSegmentsOverTime());
    }

    void ConfigureLineRenderer(LineRenderer lineRenderer, Color color, float width)
    {
        Material lineMaterial = new Material(Shader.Find("Unlit/Color"));
        lineMaterial.color = color;
        lineRenderer.material = lineMaterial;

        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineRenderer.useWorldSpace = true;
    }

    void GenerateInitialSegments(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GenerateNextSegment();
        }

        RenderCurves();
    }

    void GenerateNextSegment()
    {
        // Générer des pentes aléatoires
        float upperSlope = Random.Range(-maxSlope, maxSlope);
        float lowerSlope = Random.Range(-maxSlope, maxSlope);

        // Calculer les nouvelles hauteurs
        float upperNewY = upperCurrentY + upperSlope;
        float lowerNewY = lowerCurrentY + lowerSlope;

        // Assurer l'espacement minimum
        if (upperNewY - lowerNewY < minCurveSpacing)
        {
            float adjustment = (minCurveSpacing - (upperNewY - lowerNewY)) / 2f;
            upperNewY += adjustment;
            lowerNewY -= adjustment;
        }

        // Ajouter les points
        upperCurvePoints.Add(new Vector3(currentX, upperNewY, 0f));
        lowerCurvePoints.Add(new Vector3(currentX, lowerNewY, 0f));

        // Avancer sur l'axe X
        currentX += segmentWidth;

        // Mettre à jour les positions actuelles
        upperCurrentY = upperNewY;
        lowerCurrentY = lowerNewY;
    }

    IEnumerator GenerateSegmentsOverTime()
    {
        while (true)
        {
            // Générer un nouveau segment
            GenerateNextSegment();

            // Limiter le nombre de segments pour éviter une surcharge
            if (upperCurvePoints.Count > 50)
            {
                upperCurvePoints.RemoveAt(0);
                lowerCurvePoints.RemoveAt(0);
            }

            // Rendre les courbes
            RenderCurves();

            // Attendre avant de générer le prochain segment
            yield return new WaitForSeconds(generationInterval);
        }
    }

    void RenderCurves()
    {
        // Appliquer les points aux LineRenderers
        upperCurveRenderer.positionCount = upperCurvePoints.Count;
        upperCurveRenderer.SetPositions(upperCurvePoints.ToArray());

        lowerCurveRenderer.positionCount = lowerCurvePoints.Count;
        lowerCurveRenderer.SetPositions(lowerCurvePoints.ToArray());
    }

    public bool IsPlayerOutOfBounds(Vector3 playerPosition)
    {
        float x = playerPosition.x;

        for (int i = 0; i < upperCurvePoints.Count - 1; i++)
        {
            if (x >= upperCurvePoints[i].x && x <= upperCurvePoints[i + 1].x)
            {
                float t = (x - upperCurvePoints[i].x) / (upperCurvePoints[i + 1].x - upperCurvePoints[i].x);
                float upperY = Mathf.Lerp(upperCurvePoints[i].y, upperCurvePoints[i + 1].y, t);
                float lowerY = Mathf.Lerp(lowerCurvePoints[i].y, lowerCurvePoints[i + 1].y, t);

                return playerPosition.y > upperY || playerPosition.y < lowerY;
            }
        }

        return true;
    }
}
