using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SingleContract;

public class ContractSlideIn : MonoBehaviour
{
    [Header("Transforms for Movement")]
    public RectTransform startTransform; // Point de départ hors champ
    public RectTransform endTransform;   // Point d'arrivée visible

    [Header("Animation Settings")]
    public float slideDuration; // Temps pour parcourir la distance
    public float contractSpacing; // Espacement entre les contrats (en pixels)

    [Header("Contract Prefab")]
    public GameObject contractPrefab; // Prefab à utiliser pour les contrats (UI Image)
    public GameObject cursorPrefab; // Prefab pour le curseur

    [Header("Contracts Parameters")]
    public int numberOfContracts; // Nombre de contrats à afficher
    public float autoHideDelay = 5f; // Temps d'attente avant de cacher automatiquement le shop

    [Header("Contract Effects")]
    public Sprite[] effectSprites; // Tableau de sprites correspondant aux effets
    public float[] effectWeights; // Pondérations pour les probabilités des effets
    public bool allowDuplicateEffects; // Permettre des effets dupliqués

    private readonly List<SingleContract> contracts = new();
    private GameObject cursor;
    private int selectedIndex = 0;
    private bool contractsReady = false;
    private readonly List<ContractEffect> assignedEffects = new();
    private Coroutine autoHideCoroutine; // Référence à la coroutine de disparition automatique

    private void Start()
    {
        TriggerContractAppearance();
    }

    private void Update()
    {
        if (contractsReady && Input.GetKeyDown(KeyCode.Z))
        {
            // Déplacer le curseur vers le contrat suivant
            selectedIndex = (selectedIndex + 1) % numberOfContracts;
            cursor.GetComponent<RectTransform>().position = contracts[selectedIndex].GetComponent<RectTransform>().position;
        }

        if (contractsReady && Input.GetKeyDown(KeyCode.A))
        {
            SingleContract selectedContract = contracts[selectedIndex].GetComponent<SingleContract>();

            if (!selectedContract.GetIsSelected())
            {
                selectedContract.SelectContract();
                StartCoroutine(HideContractsAndCursor());
            }
        }
    }

    // Fonction pour déclencher l'animation
    public void TriggerContractAppearance()
    {
        // Réinitialiser
        foreach (var contract in contracts)
        {
            Destroy(contract);
        }
        contracts.Clear();
        assignedEffects.Clear();

        if (cursor != null)
        {
            Destroy(cursor);
        }

        contractsReady = false;
        selectedIndex = 0;

        // Créer les contrats
        for (int i = 0; i < numberOfContracts; i++)
        {
            GameObject contract = Instantiate(contractPrefab, startTransform.position, Quaternion.identity, transform);
            SingleContract contractScript = contract.AddComponent<SingleContract>();
            contracts.Add(contractScript);

            // Assigner un effet aléatoire avec pondération
            ContractEffect effect = GetRandomEffect();
            assignedEffects.Add(effect);

            // Configurer le contrat
            contractScript.SetEffect(effect, effectSprites[(int)effect]);
        }

        // Créer le curseur
        cursor = Instantiate(cursorPrefab, startTransform.position, Quaternion.identity, transform);
        cursor.transform.SetAsFirstSibling(); // S'assurer que le curseur est derrière les contrats

        // Lancer l'animation
        StartCoroutine(SlideContracts());

        // Lancer la coroutine d'auto-hide
        if (autoHideCoroutine != null)
        {
            StopCoroutine(autoHideCoroutine);
        }
        autoHideCoroutine = StartCoroutine(AutoHideShop());
    }

    private IEnumerator AutoHideShop()
    {
        float elapsedTime = 0f;
        while (elapsedTime < autoHideDelay)
        {

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Si le délai est écoulé sans sélection, cacher les contrats et le curseur
        StartCoroutine(HideContractsAndCursor());
    }

    private IEnumerator SlideContracts()
    {
        Vector3 startPosition = startTransform.position;
        Vector3 endPosition = endTransform.position;

        // Calculer la direction de glissement
        Vector3 slideDirection = (endPosition - startPosition).normalized;

        // Calculer un vecteur perpendiculaire à la direction de glissement pour l'alignement
        Vector3 perpendicularDirection = Vector3.Cross(slideDirection, Vector3.forward).normalized;

        // Calculer les positions finales des contrats avec espacement
        Vector3[] targetPositions = new Vector3[numberOfContracts];
        float startOffset = -(numberOfContracts - 1) / 2.0f * contractSpacing;

        for (int i = 0; i < numberOfContracts; i++)
        {
            targetPositions[i] = endPosition + perpendicularDirection * (startOffset + i * contractSpacing);
        }

        float elapsedTime = 0f;

        // Sauvegarder les positions de départ pour chaque contrat
        Vector3[] initialPositions = new Vector3[numberOfContracts];
        for (int i = 0; i < numberOfContracts; i++)
        {
            initialPositions[i] = contracts[i].GetComponent<RectTransform>().position;
        }

        // Animer les contrats
        while (elapsedTime < slideDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / slideDuration;

            for (int i = 0; i < numberOfContracts; i++)
            {
                contracts[i].GetComponent<RectTransform>().position = Vector3.Lerp(initialPositions[i], targetPositions[i], t);
            }

            cursor.GetComponent<RectTransform>().position = Vector3.Lerp(startPosition, targetPositions[selectedIndex], t);

            yield return null;
        }

        // Assurer que les contrats atteignent exactement leur position finale
        for (int i = 0; i < numberOfContracts; i++)
        {
            contracts[i].GetComponent<RectTransform>().position = targetPositions[i];
        }

        cursor.GetComponent<RectTransform>().position = targetPositions[selectedIndex];
        contractsReady = true;
    }

    private ContractEffect GetRandomEffect()
    {
        List<ContractEffect> availableEffects = new();

        if (allowDuplicateEffects)
        {
            // Si les doublons sont autorisés, utiliser toutes les pondérations
            float totalWeight = 0f;
            foreach (float weight in effectWeights)
            {
                totalWeight += weight;
            }

            float randomValue = Random.Range(0, totalWeight);
            float cumulativeWeight = 0f;

            for (int i = 0; i < effectWeights.Length; i++)
            {
                cumulativeWeight += effectWeights[i];
                if (randomValue <= cumulativeWeight)
                {
                    return (ContractEffect)i;
                }
            }
        }
        else
        {
            // Si les doublons ne sont pas autorisés, filtrer les effets déjà attribués
            for (int i = 0; i < effectWeights.Length; i++)
            {
                if (!assignedEffects.Contains((ContractEffect)i) || assignedEffects.Count >= effectWeights.Length)
                {
                    availableEffects.Add((ContractEffect)i);
                }
            }

            if (availableEffects.Count > 0)
            {
                int randomIndex = Random.Range(0, availableEffects.Count);
                return availableEffects[randomIndex];
            }
        }

        return ContractEffect.None; // Valeur par défaut au cas où
    }

    private IEnumerator HideContractsAndCursor()
    {
        contractsReady = false;
        float elapsedTime = 0f;
        Vector3[] initialPositions = new Vector3[numberOfContracts];
        Vector3 startPosition = startTransform.position;

        for (int i = 0; i < numberOfContracts; i++)
        {
            if (contracts[i] != null)
            {
                initialPositions[i] = contracts[i].GetComponent<RectTransform>().position;
            }
        }

        Vector3 initialCursorScale = cursor.transform.localScale;

        float cursorHideDuration = slideDuration / 8f; // Vitesseplus rapide pour le curseur

        while (elapsedTime < slideDuration)
        {
            elapsedTime += Time.deltaTime;
            float tContracts = elapsedTime / slideDuration;
            float tCursor = Mathf.Clamp01(elapsedTime / cursorHideDuration); // Limiter la valeur de t pour le curseur

            for (int i = 0; i < numberOfContracts; i++)
            {
                print(contracts[i].GetIsSelected());
                if (contracts[i] != null && !contracts[i].GetIsSelected())
                {
                    contracts[i].GetComponent<RectTransform>().position = Vector3.Lerp(initialPositions[i], startPosition, tContracts);
                }
            }

            if (cursor != null)
            {
                // Le curseur reste immobile et rétrécit
                cursor.transform.localScale = Vector3.Lerp(initialCursorScale, Vector3.zero, tCursor);
            }

            yield return null;
        }

        // Détruire les contrats restants et le curseur
        foreach (var contract in contracts)
        {
            if (contract != null) Destroy(contract);
        }

        contracts.Clear();
        if (cursor != null) Destroy(cursor);
        contractsReady = false;
    }


}
