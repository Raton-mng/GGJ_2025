using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace UI.Menu
{
    public class ButtonCells : MonoBehaviour
    {
        private const int PlayCellID = 23;
        private const int OptionsCellID = 25;
        private const int QuitCellID = 27;

        [SerializeField] private UnityEvent OptionOpenEvent;
        [SerializeField] private UnityEvent OptionQuitEvent;

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

        public void Select(int cellID)
        {
            if (cellID == PlayCellID && CellManager.Instance.players.Count >= 1)
            {
                SceneManager.StartGame();
            }

            if (cellID == OptionsCellID)
            {
                OptionOpenEvent?.Invoke();
            }
        }

        public void CloseMenu()
        {
            OptionQuitEvent?.Invoke();
        }
    }
}
