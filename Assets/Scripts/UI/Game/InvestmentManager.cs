
using UnityEngine;

public class InvestmentManager : MonoBehaviour
{
    public static InvestmentManager Instance;

    [SerializeField] private GameObject selectPlayerPrefab; // Nombre de contrats

    [SerializeField] private RuntimeAnimatorController[] selectorSprites;

    [Header("InvestmentVariable")]
    [SerializeField] private float investTimer = 2f; // Temps pour investir
    [SerializeField] private int investAmount = 10; // Montant à investir²

    private GameObject[] cursors;  // Tableau pour stocker les curseurs
    private GameObject[] hudList;  // Tableau pour stocker les curseurs
    private int[] selectedIndices; // Indices sélectionnés pour chaque joueur
    private int[] moneyInvestByPLayer; // Indices sélectionnés pour chaque joueur
    private int playerCount;       // Nombre de joueurs

    private Vector3[] cursorPositions; // Positions des curseurs

    private void Awake()
    {
        cursorPositions = new Vector3[] { new Vector3(1625, 940, 0), new Vector3(280, 940, 0), new Vector3(728, 940, 0), new Vector3(1176, 940, 0) };
        cursors = new GameObject[4];
        hudList = new GameObject[4];
        selectedIndices = new int[4];
        moneyInvestByPLayer = new int[4];

        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitiateInvestment();

    }

    public int GetInvestAmountByPlayer(int playerId)
    {
        return moneyInvestByPLayer[playerId];
    }

    public void InitiateInvestment()
    {
        // Initialisation des curseurs
        playerCount = PlayerManager.Instance.GetPlayerCount();
        for (int i = 0; i < playerCount; i++)
        {
            hudList[i] = PlayerManager.Instance.GetPlayer(i).GetHUD();
            selectedIndices[i] = i;
            GameObject cursor = Instantiate(selectPlayerPrefab);
            cursors[i] = cursor;
            cursor.transform.parent = hudList[i].transform;
            cursor.GetComponent<RectTransform>().anchoredPosition = cursorPositions[i];
            cursor.GetComponent<Animator>().runtimeAnimatorController = selectorSprites[i];
        }
        moneyInvestByPLayer = new int[playerCount];

    }

    public void MoveCursor(int playerIndex, float moveUp)
    {
        if (playerIndex < 0 || playerIndex >= cursors.Length || cursors[playerIndex] == null) return; // Validation de l'index et vérification du curseur

        // Calcul du nouvel indice sélectionné
        int newIndex = (selectedIndices[playerIndex] + (int)moveUp + playerCount) % playerCount;

        while (PlayerManager.Instance.GetPlayer(newIndex).IsDead())
        {
            newIndex = (newIndex + (int)moveUp + playerCount) % playerCount; // Passer au prochain contrat
        }

        selectedIndices[playerIndex] = newIndex; // Mettre à jour l'indice sélectionné

        // Mettre à jour la position du curseur
        cursors[playerIndex].GetComponent<RectTransform>().anchoredPosition = cursorPositions[newIndex];
    }

    public void InvestInPLayer(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= cursors.Length || cursors[playerIndex] == null) return; // Validation de l'index et vérification du curseur

        int selectedIndex = selectedIndices[playerIndex];

        int coinToInvest = Mathf.Min(PlayerManager.Instance.GetPlayer(selectedIndex).coinManager.GetCoins(), investAmount);

        if(coinToInvest > 0)
        {
            PlayerManager.Instance.GetPlayer(playerIndex).coinManager.UseCoins(coinToInvest);
            moneyInvestByPLayer[selectedIndex] += coinToInvest;
            //Update la sensibilité du playerController
        }
    }

    //Fonction à appeler quand le joueur meurt
    //Supprimer le curseur du joueur mort, et empêcher les autres de sélectionner le joueur mort
    public void PlayerDied(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= cursors.Length || cursors[playerIndex] == null) return; // Validation de l'index et vérification du curseur

        Destroy(cursors[playerIndex]);
        cursors[playerIndex] = null;

        for (int i = 0; i < playerCount; i++)
        {
            if (selectedIndices[i] == playerIndex)
            {
                MoveCursor(i, 1);
            }
        }
    }

}
