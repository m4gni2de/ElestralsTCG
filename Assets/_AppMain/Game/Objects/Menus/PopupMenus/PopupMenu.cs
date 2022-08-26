using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gameplay.Menus.Popup;

namespace Gameplay.Menus
{
    public class PopupMenu : MonoBehaviour
    {
        [SerializeField]
        private PopupButton _templateButton;

        private List<PopupButton> _buttons = null;
        public List<PopupButton> buttons { get { _buttons ??= new List<PopupButton>(); return _buttons; } }




        public void InspectCard(GameCard card)
        {

        }
        
    }
}

