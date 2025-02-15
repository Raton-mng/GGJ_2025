using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI.Menu
{
    public class CellManager : MonoBehaviour
    {
        [SerializeField] private int cellNumber;
        public int cellRowCount;
        [SerializeField] private int cellColumnCount;
        [SerializeField] private GameObject cellPrefab;

        public List<PlayerStartController> players;

        public static CellManager Instance;

        public List<Cell> cellList = new ();
        
        private void Awake()
        {
            Instance = this;
            
            cellList.Clear();

            for(int i = 0; i < cellNumber; i++)
            {
                cellList.Add(Instantiate(cellPrefab, transform).GetComponent<Cell>());
            }
            
        }

        private void Start()
        {
            foreach (GameObject player in PlayerManager.Instance.GetPlayers())
            {
                PlayerStartController playerStartController = player.transform.GetChild(0).GetComponent<PlayerStartController>();
                players.Add(playerStartController);
                UpdateCellOutlinePosition(playerStartController.currentCellID, playerStartController.currentCellID, playerStartController.GetID());

            }
        }

        public void UpdateCellOutlinePosition(int previousCellIndex, int currentCellID, int playerID)
        {
            if (currentCellID > cellList.Count)
                return;

            if (!IsCellActive(previousCellIndex))
            {
                cellList[previousCellIndex].SetOutlineActive(false);
            }
            else
            {
                cellList[previousCellIndex].SetOutlineColor(GetPlayerIDInCell(previousCellIndex));
            }
            cellList[currentCellID].SetOutlineColor(playerID);
            cellList[currentCellID].SetOutlineActive(true);
        }

        private int GetPlayerIDInCell(int cellID)
        {
            foreach (PlayerStartController player in players)
            {
                if (player.currentCellID == cellID)
                {
                    return player.GetID();
                }
            }

            return 0;
        }

        public bool IsCellActive(int cellIndex)
        {
            foreach (PlayerStartController player in players)
            {
                if (player.currentCellID == cellIndex)
                {
                    return true;
                }
            }

            return false;
        }

        public void SetCellText(int cellID, string text)
        {
            cellList[cellID].SetText(text);
        }
        
    }
}
