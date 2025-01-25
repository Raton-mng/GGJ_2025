using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public float cursorFadeDuration = 0.3f; // Durée de l'animation

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

    [Header("Multiplayer Settings")]
    public Sprite[] cursorSprites; // Sprites pour différencier les curseurs
    private GameObject[] cursors;  // Tableau pour stocker les curseurs
    private int[] selectedIndices; // Indices sélectionnés pour chaque joueur

    [Header("Shop Signal")]
    public GameObject shopSignal; // Associez un objet (par exemple une image) dans l'éditeur pour le signal clignotant
    public float blinkDuration = 2f; // Durée totale du clignotement
    public int blinkCount = 5; // Nombre de fois que le signal clignote

    private readonly List<SingleContract> contracts = new();
    private bool contractsReady = false;
    private readonly List<ContractEffect> assignedEffects = new();
    private Coroutine autoHideCoroutine; // Référence à la coroutine de disparition automatique
    private int playerCount = 2; // Nombre de joueurs (variable static du script PlayerController)
    private int votedPlayer = 0; // Joueur qui a voté
    private bool shopActive = false; // Indique si le shop est actif


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            TriggerContractAppearance();
        }
        if (contractsReady)
        {
            if (Input.GetKeyDown(KeyCode.Z)) MoveCursor(0, false);  // Exemple pour le joueur 1, monter
            if (Input.GetKeyDown(KeyCode.S)) MoveCursor(0, true); // Exemple pour le joueur 1, descendre
            if (Input.GetKeyDown(KeyCode.UpArrow)) MoveCursor(1, false);  // Exemple pour le joueur 2, monter
            if (Input.GetKeyDown(KeyCode.DownArrow)) MoveCursor(1, true); // Exemple pour le joueur 2, descendre
        }

        if (contractsReady)
        {
            // Exemple pour le joueur 1
            if (Input.GetKeyDown(KeyCode.A)) ActivateCardEffect(0);

            // Ajoute d'autres contrôles pour d'autres joueurs
            if (Input.GetKeyDown(KeyCode.K)) ActivateCardEffect(1);
        }
    }

    public void MoveCursor(int playerIndex, bool moveUp)
    {
        if (playerIndex < 0 || playerIndex >= cursors.Length || cursors[playerIndex] == null) return; // Validation de l'index et vérification du curseur

        // Calcul du nouvel indice sélectionné
        int direction = moveUp ? -1 : 1; // Haut (-1) ou bas (+1)
        int newIndex = (selectedIndices[playerIndex] + direction + numberOfContracts) % numberOfContracts;

        // Vérifier si le contrat à l'index nouvellement calculé est détruit
        while (contracts[newIndex] == null)
        {
            newIndex = (newIndex + direction + numberOfContracts) % numberOfContracts; // Passer au prochain contrat
        }

        selectedIndices[playerIndex] = newIndex; // Mettre à jour l'indice sélectionné

        // Mettre à jour la position du curseur
        cursors[playerIndex].GetComponent<RectTransform>().position = contracts[selectedIndices[playerIndex]].GetComponent<RectTransform>().position;
    }

    public void ActivateCardEffect(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= cursors.Length || cursors[playerIndex] == null) return; // Validation de l'index et vérification du curseur

        int selectedIndex = selectedIndices[playerIndex];
        if (selectedIndex < 0 || selectedIndex >= contracts.Count || contracts[selectedIndex] == null) return; // Vérification du contrat

        SingleContract selectedContract = contracts[selectedIndex];

        if (selectedContract != null && !selectedContract.GetIsSelected())
        {
            selectedContract.SelectContract(playerIndex);

            // Rétrécir et désactiver le curseur
            StartCoroutine(FadeOutCursor(playerIndex));

            // Empêcher les autres joueurs de sélectionner des cartes
            contractsReady = false;

            // Réorganiser les curseurs si nécessaire
            for (int i = 0; i < cursors.Length; i++)
            {
                if (i != playerIndex && selectedIndices[i] == selectedIndex)
                {
                    MoveCursor(i, true); // Déplace le curseur vers le haut
                }
            }

            // Vérifier si tous les joueurs ont sélectionné une carte
            votedPlayer++;
            if (votedPlayer>=playerCount)
            {
                StartCoroutine(HideContractsAndCursors());
            }
        }
    }


    // Fonction pour déclencher l'animation
    public void TriggerContractAppearance()
    {
        if (shopActive)
        {
            return;
        }
        shopActive = true;

        // Réinitialiser
        foreach (var contract in contracts)
        {
            Destroy(contract.gameObject);
        }
        contracts.Clear();
        assignedEffects.Clear();

        contractsReady = false;

        // Lancer l'animation de clignotement avant de faire apparaître le shop
        StartCoroutine(BlinkSignal(() =>
        {
            // Une fois que le shop doit apparaître (avant la fin totale du clignotement), le shop commence à se générer
            StartShopAppearance();
        }));
    }

    private IEnumerator BlinkSignal(System.Action onComplete)
    {
        float blinkInterval = blinkDuration / (blinkCount * 2); // Temps entre les clignotements (allumé/éteint)
        bool isActive = false;
        float shopStartTime = blinkDuration * 0.75f; // Le shop apparaît à 75% de l'animation de clignotement
        bool shopStarted = false;

        for (int i = 0; i < blinkCount * 2; i++) // Chaque cycle compte comme un allumage + extinction
        {
            isActive = !isActive;
            shopSignal.SetActive(isActive); // Activer/désactiver le signal

            if (!shopStarted && i * blinkInterval >= shopStartTime)
            {
                shopStarted = true;
                onComplete?.Invoke(); // Appeler la fonction de rappel pour commencer à générer le shop
            }

            yield return new WaitForSeconds(blinkInterval);
        }

        shopSignal.SetActive(false); // Assurez-vous que le signal est éteint à la fin
    }

    private void StartShopAppearance()
    {
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

        // Créer les curseurs
        playerCount = 2;
        votedPlayer = 0; // Réinitialiser le joueur qui a voté
        cursors = new GameObject[playerCount];
        selectedIndices = new int[playerCount];

        for (int i = 0; i < playerCount; i++)
        {
            GameObject playerCursor = Instantiate(cursorPrefab, startTransform.position, Quaternion.identity, transform);
            playerCursor.GetComponent<Image>().sprite = cursorSprites[i]; // Associer un sprite différent
            cursors[i] = playerCursor;
            selectedIndices[i] = 0; // Initialiser à la première carte
        }

        // Lancer l'animation d'apparition
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
        StartCoroutine(HideContractsAndCursors());
    }

    private IEnumerator SlideContracts()
    {
        // Récupérer le `RectTransform` du Canvas
        RectTransform canvasRect = GetComponent<RectTransform>();
        Vector3 startPosition = startTransform.position;
        Vector3 endPosition = endTransform.position;

        // Calculer la direction de glissement
        Vector3 slideDirection = (endPosition - startPosition).normalized;

        // Calculer un vecteur perpendiculaire à la direction de glissement pour l'alignement
        Vector3 perpendicularDirection = Vector3.Cross(slideDirection, Vector3.forward).normalized;

        // Récupérer les dimensions du Canvas en unités locales
        float canvasWidth = 960; // Largeur du Canvas
        float canvasHeight = 540; // Hauteur du Canvas

        // Calculer les limites de l'écran en unités locales
        Vector2 screenTopLeft = new Vector2(-canvasWidth, canvasHeight);
        Vector2 screenBottomRight = new Vector2(canvasWidth, -canvasHeight);

        // Calculer la largeur disponible pour positionner les contrats
        float availableSpace = Vector2.Distance(screenTopLeft, screenBottomRight);

        // Calculer l'espacement dynamique
        float dynamicSpacing = numberOfContracts > 1 ? availableSpace / (numberOfContracts + 1) : 0f;

        // Calculer les positions finales des contrats
        Vector3[] targetPositions = new Vector3[numberOfContracts];

        for (int i = 0; i < numberOfContracts; i++)
        {
            // Calculer la position locale dans le Canvas
            Vector3 localPosition = endTransform.localPosition + (Vector3)perpendicularDirection * (dynamicSpacing * (i + 1) - availableSpace / 2);

            // Convertir en position monde pour les `RectTransform`
            targetPositions[i] = canvasRect.TransformPoint(localPosition);
        }

        // Appliquer les positions aux contrats
        for (int i = 0; i < numberOfContracts; i++)
        {
            if (contracts[i] != null)
            {
                contracts[i].GetComponent<RectTransform>().position = targetPositions[i];
            }
        }

        float elapsedTime = 0f;

        // Sauvegarder les positions de départ pour chaque contrat
        Vector3[] initialPositions = new Vector3[numberOfContracts];
        for (int i = 0; i < numberOfContracts; i++)
        {
            initialPositions[i] = startPosition;
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

            for (int i = 0; i < cursors.Length; i++)
            {
                cursors[i].GetComponent<RectTransform>().position = Vector3.Lerp(startPosition, targetPositions[selectedIndices[i]], t);
            }

            yield return null;
        }

        // Assurer que les contrats atteignent exactement leur position finale
        for (int i = 0; i < numberOfContracts; i++)
        {
            contracts[i].GetComponent<RectTransform>().position = targetPositions[i];
        }

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

    private IEnumerator FadeOutCursor(int playerIndex)
    {
        GameObject cursor = cursors[playerIndex];
        Vector3 initialScale = cursor.transform.localScale;

        float elapsedTime = 0f;

        while (elapsedTime < cursorFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / cursorFadeDuration;

            // Rétrécir le curseur
            cursor.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t);

            yield return null;
        }
        contractsReady = true; // Permettre aux autres joueurs de sélectionner des cartes
        cursor.SetActive(false); // Désactiver le curseur
    }

    private IEnumerator HideContractsAndCursors()
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

        for(int i = 0; i < cursors.Length; i++)
        {
            if (cursors[i] != null)
            {
                StartCoroutine(FadeOutCursor(i));
            }
        }

        while (elapsedTime < slideDuration)
        {
            elapsedTime += Time.deltaTime;
            float tContracts = elapsedTime / slideDuration;

            for (int i = 0; i < numberOfContracts; i++)
            {
                if (contracts[i] != null && !contracts[i].GetIsSelected())
                {
                    contracts[i].GetComponent<RectTransform>().position = Vector3.Lerp(initialPositions[i], startPosition, tContracts);
                }
            }

            yield return null;
        }

        // Détruire les contrats restants
        foreach (var contract in contracts)
        {
            if(contract != null) Destroy(contract.gameObject);
        }

        contracts.Clear();

        // Détruire tous les curseurs
        foreach (var cursor in cursors)
        {
            if (cursor != null) Destroy(cursor);
        }

        contractsReady = false;
        shopActive = false;
    }



}
