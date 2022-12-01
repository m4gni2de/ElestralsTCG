using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System;

namespace Gameplay.Menus
{
    public class MagicToggle : MonoBehaviour
    {
        #region Properties
        
        private Toggle m_toggle = null;
        private Toggle _toggle
        {
            get
            {
                if (m_toggle == null)
                {
                    if (GetComponent<Toggle>() == null)
                    {
                        m_toggle = gameObject.AddComponent<Toggle>();
                    }
                    else
                    {
                        m_toggle = GetComponent<Toggle>();
                    }
                }
                return m_toggle;
            }
        }


        [SerializeField]
        private TMP_Text label;
        public string valName;

        public event Action<MagicToggle> OnToggleChanged;
        public bool IsOn { get { return _toggle.isOn; } }
        private DateTime _whenToggled = DateTime.MinValue;
        public DateTime whenToggled { get { return _whenToggled; } }
        #endregion


        private void Awake()
        {
            if (IsOn)
            {
                _whenToggled = DateTime.Now;
            }
        }
        //set this the listener for the Toggle's OnValueChanged
        public void Toggle()
        {
            OnToggleChanged?.Invoke(this);
            if (IsOn)
            {
                _whenToggled = DateTime.Now;
            }
            else
            {
                _whenToggled = DateTime.MinValue;
            }
        }

        public void Toggle(bool isOn)
        {
            _toggle.isOn = isOn;
        }

        

        #region Visuals/UI
        public void Refresh()
        {
            
        }
        public void SetText(string txt)
        {
            label.text = txt;
        }
        public void Show()
        {
            gameObject.SetActive(true);
        }
        public void Hide()
        {
            gameObject.SetActive(false);
        }


        #endregion

        #region Controld
        public bool Interactable { get { return _toggle.interactable; } set { _toggle.interactable = value; } }
        #endregion
    }
}

