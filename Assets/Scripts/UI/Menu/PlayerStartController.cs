using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace UI.Menu
{
    public class PlayerStartController : MonoBehaviour
    {
        private Vector2 _moveDirection;
        public int currentCellID = 0;
        
        [SerializeField] private float moveTimer;
        private bool _moveDelayed;

        private ButtonCells _buttonCell;

        private int playerID;

        private void Awake()
        {
            CellManager.Instance.players.Add(this);
            _buttonCell = FindObjectsByType<ButtonCells>(FindObjectsSortMode.None)[0];
        }

        private void Start()
        {
            CellManager.Instance.UpdateCellOutlinePosition(currentCellID, currentCellID, playerID);
            _moveDelayed = false;
        }

        private void OnEnable()
        {
            if (playerID == 0)
            {
                currentCellID = 0;
            }
            else if (playerID == 1)
            {
                currentCellID = CellManager.Instance.cellRowCount - 1;
            }
            else if (playerID == 2)
            {
                currentCellID = CellManager.Instance.cellList.Count - CellManager.Instance.cellRowCount;
            }
            else if (playerID == 3)
            {
                currentCellID = CellManager.Instance.cellList.Count - 1;
            }
        }

        public int GetID()
        {
            return playerID;
        }

        public void SetID(int id)
        {
            playerID = id;

            if (id == 0)
            {
                currentCellID = 0;
            }
            else if (id == 1)
            {
                currentCellID = CellManager.Instance.cellRowCount - 1;
            }
            else if (id == 2)
            {
                currentCellID = CellManager.Instance.cellList.Count - CellManager.Instance.cellRowCount;
            }
            else if (id == 3)
            {
                currentCellID = CellManager.Instance.cellList.Count - 1;
            }
        }

        public void OnMenuMove(InputAction.CallbackContext context)
        {
            _moveDirection = context.action.ReadValue<Vector2>();

            if (context.phase == InputActionPhase.Performed)
            {
                if (_moveDirection.x > 0.5)
                {
                    if (currentCellID + CellManager.Instance.cellRowCount < CellManager.Instance.cellList.Count)
                    {
                        currentCellID += CellManager.Instance.cellRowCount;
                        CellManager.Instance.UpdateCellOutlinePosition(currentCellID - CellManager.Instance.cellRowCount, currentCellID, playerID);
                    }
                }

                if (_moveDirection.x < -0.5)
                {
                    if (currentCellID - CellManager.Instance.cellRowCount >= 0)
                    {
                        currentCellID -= CellManager.Instance.cellRowCount;
                        CellManager.Instance.UpdateCellOutlinePosition(currentCellID + CellManager.Instance.cellRowCount, currentCellID, playerID);
                    }
                }

                if (_moveDirection.y > 0.5)
                {
                    if (currentCellID % CellManager.Instance.cellRowCount > 0)
                    {
                        currentCellID -= 1;
                        CellManager.Instance.UpdateCellOutlinePosition(currentCellID + 1, currentCellID, playerID);
                    }
                }

                if (_moveDirection.y < -0.5)
                {
                    if (currentCellID % CellManager.Instance.cellRowCount < CellManager.Instance.cellRowCount - 1 && currentCellID + 1 < CellManager.Instance.cellList.Count)
                    {
                        currentCellID += 1;
                        CellManager.Instance.UpdateCellOutlinePosition(currentCellID - 1, currentCellID, playerID);
                    }
                }
            }

        }

        public void OnSelect(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _buttonCell.Select(currentCellID);
            }
        }
        
        public void OnEscapeMenu(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _buttonCell.CloseMenu();
            }
        }
    }
}
