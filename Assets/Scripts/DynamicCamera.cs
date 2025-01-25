using UnityEngine;

public class DynamicCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera mainCamera; // La caméra principale
    public float padding = 2f; // Espace supplémentaire autour des courbes
    public float zoomSpeed = 2f; // Vitesse du zoom/dézoom de la caméra
    public float positionOffset = 100f; // Décalage horizontal de la caméra

    [SerializeField] private RandomCurves randomCurves;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main; // Utiliser la caméra principale si aucune n'est assignée
        }

        // Récupérer la référence au script RandomCurves
        if (randomCurves == null)
        {
            Debug.LogError("Aucun script RandomCurves trouvé dans la scène !");
        }
    }

    void LateUpdate()
    {
        if (randomCurves == null) return;

        // Récupérer l'étendue verticale des courbes
        float highestPoint = randomCurves.GetHighestPoint();
        float lowestPoint = randomCurves.GetLowestPoint();

        // Calculer la taille et le centre nécessaires pour cadrer les courbes
        float verticalExtent = (highestPoint - lowestPoint) / 2f + padding;
        float verticalCenter = (highestPoint + lowestPoint) / 2f;

        // Ajuster la caméra (position et zoom)
        Vector3 targetPosition = new Vector3(mainCamera.transform.position.x, verticalCenter, mainCamera.transform.position.z);
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, Time.deltaTime * zoomSpeed);
        mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, mainCamera.transform.position.z);
        float targetSize = Mathf.Lerp(mainCamera.orthographicSize, verticalExtent, Time.deltaTime * zoomSpeed);
        mainCamera.orthographicSize = Mathf.Clamp(targetSize, 5f, 50f); // Limiter le zoom pour éviter des tailles extrêmes
    }
}