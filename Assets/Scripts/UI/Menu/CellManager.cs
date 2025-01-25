using System.Collections.Generic;
using UnityEngine;

namespace UI.Menu
{
    public class CellManager : MonoBehaviour
    {
        [SerializeField] private int cellNumber;
        [SerializeField] private int cellRowCount;
        [SerializeField] private int cellColumnCount;
        [SerializeField] private GameObject cellPrefab;

        private GameObject cellOutline;


        private List<Cell> cellList = new ();
        private int currentCellID = 0;
        
        private void Awake()
        {
            for(int i = 0; i < cellNumber; i++)
            {
                cellList.Add(Instantiate(cellPrefab, transform).GetComponent<Cell>());
            }
        }

        private void Start()
        {
            currentCellID = 0;
            UpdateCellOutlinePosition(currentCellID);
        }

        private void UpdateCellOutlinePosition(int previousCellIndex)
        {
            if (currentCellID > cellList.Count)
                return;
            cellList[previousCellIndex].SetOutlineActive(false);
            cellList[currentCellID].SetOutlineActive(true);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (currentCellID + cellRowCount < cellList.Count)
                {
                    currentCellID += cellRowCount;
                    UpdateCellOutlinePosition(currentCellID - cellRowCount);
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (currentCellID - cellRowCount >= 0)
                {
                    currentCellID -= cellRowCount;
                    UpdateCellOutlinePosition(currentCellID + cellRowCount);
                } 
            }
            
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (currentCellID % cellRowCount > 0)
                {
                    currentCellID -= 1;
                    UpdateCellOutlinePosition(currentCellID + 1);
                } 
            }
            
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (currentCellID % cellRowCount < cellRowCount - 1 && currentCellID + 1 < cellList.Count)
                {
                    currentCellID += 1;
                    UpdateCellOutlinePosition(currentCellID - 1);
                } 
            }
            
        }

        public void SetCellText(int cellID, string text)
        {
            cellList[cellID].SetText(text);
        }
        
    }
}
