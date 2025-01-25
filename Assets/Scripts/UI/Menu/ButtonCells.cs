using System;
using UnityEngine;

namespace UI.Menu
{
    public class ButtonCells : MonoBehaviour
    {
        [SerializeField] private int playCellID;
        [SerializeField] private int optionsCellID;
        [SerializeField] private int quitCellID;

        private CellManager cellManager;
        
        private void Awake()
        {
            cellManager = GetComponent<CellManager>();
        }

        private void Start()
        {
            cellManager.SetCellText(playCellID, "Play");
            cellManager.SetCellText(optionsCellID, "Options");
            cellManager.SetCellText(quitCellID, "Quit");
        }
    }
}
