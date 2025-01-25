using System;
using UnityEngine;

namespace UI.Menu
{
    public class ButtonCells : MonoBehaviour
    {
        private const int PlayCellID = 23;
        private const int OptionsCellID = 25;
        private const int QuitCellID = 27;

        private CellManager cellManager;
        
        private void Awake()
        {
            cellManager = GetComponent<CellManager>();
        }

        private void Start()
        {
            cellManager.SetCellText(PlayCellID, "Play");
            cellManager.SetCellText(OptionsCellID, "Options");
            cellManager.SetCellText(QuitCellID, "Quit");
        }

        public static void Select(int cellID)
        {
            if (cellID == PlayCellID && CellManager.Instance.players.Count >= 2)
            {
                SceneManager.StartGame();
            }
        }
    }
}
