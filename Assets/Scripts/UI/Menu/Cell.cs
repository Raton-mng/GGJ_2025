using TMPro;
using UnityEngine;

namespace UI.Menu
{
    public class Cell : MonoBehaviour
    {

        [SerializeField] private GameObject outline;

        private TextMeshProUGUI inputField; 
    
        private void Awake()
        {
            SetOutlineActive(false);
            inputField = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void SetOutlineActive(bool active)
        {
            outline.SetActive(active);
        }

        public void SetText(string text)
        {
            inputField.text = text;
        }
    }
}
