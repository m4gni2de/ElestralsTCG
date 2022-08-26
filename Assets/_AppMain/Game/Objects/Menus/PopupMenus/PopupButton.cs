using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace Gameplay.Menus.Popup
{
    public class PopupButton : MonoBehaviour
    {
        #region Properties
        private Button _button = null;
        public Button button
        {
            get
            {
                _button ??= GetComponent<Button>();
                return _button;
            }
        }

        private TMP_Text txtButton { get { return button.GetComponentInChildren<TMP_Text>(); } }

        private UnityEvent OnClickEvent { get { return button.onClick; } }
        #endregion
        public void SetText(string msg)
        {
            txtButton.text = msg;
        }
        
    }
}

