using System.Collections.Generic;
using UnityEngine;

namespace UI.Menu
{
    public class CellManager : MonoBehaviour
    {
        [SerializeField] private int cellNumber;
        public int cellRowCount;
        [SerializeField] private int cellColumnCount;
        [SerializeField] private GameObject cellPrefab;

        private GameObject cellOutline;
        public List<PlayerStartController> players;

        public static CellManager Instance;


        public List<Cell> cellList = new ();
        
        private void Awake()
        {
            if (Instance == null) Instance = this;
            
            for(int i = 0; i < cellNumber; i++)
            {
                cellList.Add(Instantiate(cellPrefab, transform).GetComponent<Cell>());
            }

            players = new List<PlayerStartController>();
        }

        public void UpdateCellOutlinePosition(int previousCellIndex, int currentCellID)
        {
            if (currentCellID > cellList.Count)
                return;
            if (!IsCellActive(previousCellIndex)) cellList[previousCellIndex].SetOutlineActive(false);
            cellList[currentCellID].SetOutlineActive(true);
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
