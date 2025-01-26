using System.Diagnostics.Contracts;
using UnityEngine;

public class InvestmentManager : MonoBehaviour
{
    [SerializeField] private GameObject selectPlayerPrefab; // Nombre de contrats

    [Header("InvestmentVariable")]
    [SerializeField] private float investTimer = 2f; // Temps pour investir
    [SerializeField] private int investAmount = 10; // Montant à investir²

    private GameObject[] cursors;  // Tableau pour stocker les curseurs
    private GameObject[] hudList;  // Tableau pour stocker les curseurs
    private int[] selectedIndices; // Indices sélectionnés pour chaque joueur
    private int[] moneyInvestByPLayer; // Indices sélectionnés pour chaque joueur
    private int playerCount;       // Nombre de joueurs

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
            cursor.transform.SetParent(hudList[i].transform);
        }
        moneyInvestByPLayer = new int[playerCount];

    }

    private void Update()
    {
        if (!ShopManager.ShopActive)
        {
            if (Input.GetKeyDown(KeyCode.Z)) MoveCursor(0, false);  // Exemple pour le joueur 1, monter
            if (Input.GetKeyDown(KeyCode.S)) MoveCursor(0, true); // Exemple pour le joueur 1, descendre
            if (Input.GetKeyDown(KeyCode.UpArrow)) MoveCursor(1, false);  // Exemple pour le joueur 2, monter
            if (Input.GetKeyDown(KeyCode.DownArrow)) MoveCursor(1, true); // Exemple pour le joueur 2, descendre
        }

        if (!ShopManager.ShopActive)
        {
            // Exemple pour le joueur 1
            if (Input.GetKeyDown(KeyCode.A)) InvestInPLayer(0);

            // Ajoute d'autres contrôles pour d'autres joueurs
            if (Input.GetKeyDown(KeyCode.K)) InvestInPLayer(1);
        }
    }

    public void MoveCursor(int playerIndex, bool moveUp)
    {
        if (playerIndex < 0 || playerIndex >= cursors.Length || cursors[playerIndex] == null) return; // Validation de l'index et vérification du curseur

        // Calcul du nouvel indice sélectionné
        int direction = moveUp ? -1 : 1; // Haut (-1) ou bas (+1)
        int newIndex = (selectedIndices[playerIndex] + direction + playerCount) % playerCount;

        while (PlayerManager.Instance.GetPlayer(newIndex).IsDead())
        {
            newIndex = (newIndex + direction + playerCount) % playerCount; // Passer au prochain contrat
        }

        selectedIndices[playerIndex] = newIndex; // Mettre à jour l'indice sélectionné

        // Mettre à jour la position du curseur
        cursors[playerIndex].transform.parent = hudList[newIndex].transform;
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
                MoveCursor(i, true);
            }
        }
    }

}
