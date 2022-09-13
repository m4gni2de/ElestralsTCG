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

        public TMP_Text txtButton;
        public string CommandName { get { return txtButton.text; } }


        public UnityEvent OnClickEvent { get { return button.onClick; } }

        public int Page { get; set; }
        #endregion

        public void LoadCommand(PopupCommand cmd)
        {
            OnClickEvent.RemoveAllListeners();
            OnClickEvent.AddListener(cmd.action);
            Page = cmd.level;
            SetText(cmd.name);
        }
        public void Clear()
        {
            txtButton.text = "";
            OnClickEvent.RemoveAllListeners();
            gameObject.SetActive(false);
            Page = -1;
        }
        public void Show()
        {
            gameObject.SetActive(true);
        }
        public void SetText(string msg)
        {
            txtButton.text = msg;
        }
        
    }
}

