using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameplay.Menus.Popup;

namespace Gameplay.Menus
{
    public class MenuPage : MonoBehaviour
    {
        public int Index { get; set; }
        private List<PopupButton> _buttons = null;
        public List<PopupButton> Buttons
        {
            get
            {
                _buttons ??= new List<PopupButton>();
                return _buttons;
            }
        }

        public bool IsTemp = false;


        public void SetPage(int index, bool isTemp = false)
        {
            this.Index = index;
            this.IsTemp = isTemp;
            name = $"ButtonPage{index + 1}";
            Hide();
        }
        public void LoadButtons(List<PopupButton> buttonsList)
        {
            for (int i = 0; i < buttonsList.Count; i++)
            {
                Buttons.Add(buttonsList[i]);
                buttonsList[i].transform.SetParent(transform);
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
