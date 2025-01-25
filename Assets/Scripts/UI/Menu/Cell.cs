using TMPro;
using UnityEngine;

namespace UI.Menu
{
    [RequireComponent(typeof(TMP_InputField))]
    public class Cell : MonoBehaviour
    {

        [SerializeField] private GameObject outline;

        private TMP_InputField inputField; 
    
        private void Awake()
        {
            SetOutlineActive(false);
            inputField = GetComponent<TMP_InputField>();
            inputField.interactable = false;
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
