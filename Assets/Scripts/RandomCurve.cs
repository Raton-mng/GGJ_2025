using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class RandomCurves : MonoBehaviour
{
    public static RandomCurves Instance;

    [Header("Curve Settings")]
    public float segmentWidth = 2f; // Largeur de chaque segment
    public float maxSlope = 2f; // Pente maximale (valeurs positives ou négatives)
    public float initialHeightMin = 1f; // Hauteur minimale initiale
    public float initialHeightMax = 5f; // Hauteur maximale initiale
    public float minCurveSpacing = 2f; // Espacement minimal entre les deux courbes
    public float maxCurveSeparation = 20; // Écartement maximal autorisé entre les deux courbes
    public float removalOffset = 10f; // Distance supplémentaire pour supprimer les segments derrière la caméra
    public float apparitionOffset = 10f; // Distance supplémentaire pour supprimer les segments derrière la caméra

    [Header("Line Renderer")]
    public LineRenderer upperCurveRenderer; // Renderer pour la courbe supérieure
    public LineRenderer lowerCurveRenderer; // Renderer pour la courbe inférieure
    public LineRenderer redZoneRenderer;
    public MeshFilter redZoneMeshFilter;
    public MeshRenderer redZoneMeshRenderer;

    [Header("Dynamic Curve Settings")]
    public float movementSpeed = 5f; // Vitesse à laquelle les courbes reculent (unités par seconde)
    public float generationInterval = 0.1f; // Temps entre chaque génération de segment

    private List<Vector3> upperCurvePoints; // Points de la courbe supérieure
    private List<Vector3> lowerCurvePoints; // Points de la courbe inférieure

    private float currentX; // Position actuelle pour les nouveaux segments
    private float upperCurrentY; // Position Y actuelle de la courbe supérieure
    private float lowerCurrentY; // Position Y actuelle de la courbe inférieure

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

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
        GenerateInitialSegments(10);

        // Configurer les LineRenderers
        ConfigureLineRenderer(upperCurveRenderer, Color.red, 0.1f);
        ConfigureLineRenderer(lowerCurveRenderer, Color.blue, 0.1f);

    }

    void Update()
    {
        // Faire reculer les courbes
        MoveCurvesBackward();

        // Supprimer les points hors champ
        RemoveOutOfBoundsSegments();

        // Vérifier si un nouveau segment doit être généré
        if (upperCurvePoints.Count > 0 && upperCurvePoints[upperCurvePoints.Count - 1].x < Camera.main.transform.position.x + apparitionOffset)
        {
            GenerateNextSegment();
        }

        // Mettre à jour les LineRenderers
        RenderCurves();
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

        // Assurer l'espacement maximal
        if (upperNewY - lowerNewY > maxCurveSeparation)
        {
            float adjustment = (upperNewY - lowerNewY - maxCurveSeparation) / 2f;
            upperNewY -= adjustment;
            lowerNewY += adjustment;
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

    void MoveCurvesBackward()
    {
        // Décaler tous les points vers la gauche (axe X)
        for (int i = 0; i < upperCurvePoints.Count; i++)
        {
            upperCurvePoints[i] = new Vector3(
                upperCurvePoints[i].x - movementSpeed * Time.deltaTime,
                upperCurvePoints[i].y,
                upperCurvePoints[i].z
            );

            lowerCurvePoints[i] = new Vector3(
                lowerCurvePoints[i].x - movementSpeed * Time.deltaTime,
                lowerCurvePoints[i].y,
                lowerCurvePoints[i].z
            );
        }

        // Synchroniser currentX avec le dernier segment
        if (upperCurvePoints.Count > 0)
        {
            currentX = upperCurvePoints[upperCurvePoints.Count - 1].x + segmentWidth;
        }
    }


    void RemoveOutOfBoundsSegments()
    {
        // Supprimer les points qui sont hors du champ de jeu (trop à gauche)
        while (upperCurvePoints.Count > 0 && upperCurvePoints[0].x < -removalOffset)
        {
            upperCurvePoints.RemoveAt(0);
            lowerCurvePoints.RemoveAt(0);
        }
    }

    void RenderCurves()
    {
        // Appliquer les points aux LineRenderers
        upperCurveRenderer.positionCount = upperCurvePoints.Count;
        upperCurveRenderer.SetPositions(upperCurvePoints.ToArray());

        lowerCurveRenderer.positionCount = lowerCurvePoints.Count;
        lowerCurveRenderer.SetPositions(lowerCurvePoints.ToArray());

        // Dessiner la zone rouge
        DrawRedZone();
    }

    void DrawRedZone()
    {
        Mesh mesh = new();
        List<Vector3> vertices = new();
        List<int> triangles = new();

        for (int i = 0; i < upperCurvePoints.Count; i++)
        {
            float upperY = upperCurvePoints[i].y;
            float lowerY = lowerCurvePoints[i].y;
            float redZoneY = lowerY + (upperY - lowerY) * 0.8f; // Calculer les 20% en haut

            vertices.Add(new Vector3(upperCurvePoints[i].x, redZoneY, 0f));
            vertices.Add(new Vector3(upperCurvePoints[i].x, upperY, 0f));

            if (i < upperCurvePoints.Count - 1)
            {
                int startIndex = i * 2;
                triangles.Add(startIndex);
                triangles.Add(startIndex + 1);
                triangles.Add(startIndex + 2);

                triangles.Add(startIndex + 1);
                triangles.Add(startIndex + 3);
                triangles.Add(startIndex + 2);
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        redZoneMeshFilter.mesh = mesh;
    }

    public bool IsPlayerAboveLowerCurve(Vector3 playerPosition)
    {
        float x = playerPosition.x;

        for (int i = 0; i < lowerCurvePoints.Count - 1; i++)
        {
            if (x >= lowerCurvePoints[i].x && x <= lowerCurvePoints[i + 1].x)
            {
                float t = (x - lowerCurvePoints[i].x) / (lowerCurvePoints[i + 1].x - lowerCurvePoints[i].x);
                float lowerY = Mathf.Lerp(lowerCurvePoints[i].y, lowerCurvePoints[i + 1].y, t);

                return playerPosition.y < lowerY;
            }
        }

        return true;
    }

    public bool IsPlayerBelowUpperCurve(Vector3 playerPosition)
    {
        float x = playerPosition.x;

        for (int i = 0; i < upperCurvePoints.Count - 1; i++)
        {
            if (x >= upperCurvePoints[i].x && x <= upperCurvePoints[i + 1].x)
            {
                float t = (x - upperCurvePoints[i].x) / (upperCurvePoints[i + 1].x - upperCurvePoints[i].x);
                float upperY = Mathf.Lerp(upperCurvePoints[i].y, upperCurvePoints[i + 1].y, t);

                return playerPosition.y > upperY;
            }
        }

        return true;
    }

    public bool IsPlayerInRedZone(Vector3 playerPosition)
    {
        float x = playerPosition.x;

        for (int i = 0; i < upperCurvePoints.Count - 1; i++)
        {
            if (x >= upperCurvePoints[i].x && x <= upperCurvePoints[i + 1].x)
            {
                float t = (x - upperCurvePoints[i].x) / (upperCurvePoints[i + 1].x - upperCurvePoints[i].x);
                float upperY = Mathf.Lerp(upperCurvePoints[i].y, upperCurvePoints[i + 1].y, t);
                float lowerY = Mathf.Lerp(lowerCurvePoints[i].y, lowerCurvePoints[i + 1].y, t);
                float redZoneY = lowerY + (upperY - lowerY) * 0.8f;

                return playerPosition.y > redZoneY && playerPosition.y < upperY;
            }
        }

        return false;
    }

    public float GetHighestPoint()
    {
        // Récupérer le Y maximal parmi les points de la courbe supérieure
        float highestY = float.MinValue;
        foreach (var point in upperCurvePoints)
        {
            if (point.y > highestY)
            {
                highestY = point.y;
            }
        }
        return highestY;
    }

    public float GetLowestPoint()
    {
        // Récupérer le Y minimal parmi les points de la courbe inférieure
        float lowestY = float.MaxValue;
        foreach (var point in lowerCurvePoints)
        {
            if (point.y < lowestY)
            {
                lowestY = point.y;
            }
        }
        return lowestY;
    }
}
