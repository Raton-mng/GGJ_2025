using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu
{
    public class Cell : MonoBehaviour
    {
        
        [SerializeField] private GameObject outline;

        [SerializeField] private List<Sprite> outlinesPlayerList;

        private TextMeshProUGUI inputField;
        private Image oultineImage;
    
        private void Awake()
        {
            SetOutlineActive(false);
            inputField = GetComponentInChildren<TextMeshProUGUI>();
            oultineImage = outline.GetComponentInChildren<Image>();
        }

        public void SetOutlineActive(bool active)
        {
            outline.SetActive(active);
        }

        public void SetText(string text)
        {
            inputField.text = text;
        }

        public void SetOutlineColor(int playerID)
        {
            if (playerID < outlinesPlayerList.Count)
                oultineImage.sprite = outlinesPlayerList[playerID];
        }
    }
}
