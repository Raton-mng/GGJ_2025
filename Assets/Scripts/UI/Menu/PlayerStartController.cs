using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace UI.Menu
{
    public class PlayerStartController : MonoBehaviour
    {
        private Vector2 _moveDirection;
        private CellManager _cellManager;
        public int currentCellID = 0;
        
        [SerializeField] private float moveTimer;
        private bool _moveDelayed;

        private ButtonCells _buttonCell;

        private void Start()
        {
            _cellManager = CellManager.Instance;
            currentCellID = 0;
            _cellManager.UpdateCellOutlinePosition(currentCellID, currentCellID);
            _cellManager.players.Add(this);
            _buttonCell = FindObjectsByType<ButtonCells>(FindObjectsSortMode.None)[0];
        }

        private void Update()
        {
            if (_moveDelayed) return;
            
            if (_moveDirection.x > 0.5)
            {
                if (currentCellID + _cellManager.cellRowCount < _cellManager.cellList.Count)
                {
                    currentCellID += _cellManager.cellRowCount;
                    _cellManager.UpdateCellOutlinePosition(currentCellID - _cellManager.cellRowCount, currentCellID);
                    StartCoroutine(DontMoveTooFast());
                }
            }

            if (_moveDirection.x < -0.5)
            {
                if (currentCellID - _cellManager.cellRowCount >= 0)
                {
                    currentCellID -= _cellManager.cellRowCount;
                    _cellManager.UpdateCellOutlinePosition(currentCellID + _cellManager.cellRowCount, currentCellID);
                    StartCoroutine(DontMoveTooFast());
                } 
            }
            
            if (_moveDirection.y > 0.5)
            {
                if (currentCellID % _cellManager.cellRowCount > 0)
                {
                    currentCellID -= 1;
                    _cellManager.UpdateCellOutlinePosition(currentCellID + 1, currentCellID);
                    StartCoroutine(DontMoveTooFast());
                } 
            }
            
            if (_moveDirection.y < -0.5)
            {
                if (currentCellID % _cellManager.cellRowCount < _cellManager.cellRowCount - 1 && currentCellID + 1 < _cellManager.cellList.Count)
                {
                    currentCellID += 1;
                    _cellManager.UpdateCellOutlinePosition(currentCellID - 1, currentCellID);
                    StartCoroutine(DontMoveTooFast());
                } 
            }
        }

        public void OnMenuMove(InputAction.CallbackContext context)
        {
            //Vector2 oldDirection = _moveDirection;
            _moveDirection = context.action.ReadValue<Vector2>();
            //if ((oldDirection.x - _moveDirection.x > 0.1) &&  (oldDirection.y - _moveDirection.y > 0.1)) _moveDelayed = false;
            //_moveDelayed = false;
        }

        public void OnSelect(InputAction.CallbackContext context)
        {
            if (context.action.triggered)
            {
                _buttonCell.Select(currentCellID);
            }
        }

        private IEnumerator DontMoveTooFast()
        {
            _moveDelayed = true;
            yield return new WaitForSeconds(moveTimer);
            _moveDelayed = false;
        }
    }
}
