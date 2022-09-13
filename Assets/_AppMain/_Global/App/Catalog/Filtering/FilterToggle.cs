using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace CardsUI.Filtering
{
    public class FilterToggle : MonoBehaviour
    {
        public Toggle toggle;
        [SerializeField]
        private TMP_Text txt;
        [SerializeField]
        private ToggleGroup _group;
        public string valName;

        [SerializeField]
        private bool DefaultChecked = true;
        public bool IsChecked { get { return toggle.isOn; } }

        //protected UnityEvent<bool> _CheckedChanged;
        //public UnityEvent<bool> CheckChanged { get { _CheckedChanged ??= new UnityEvent<bool>(); return _CheckedChanged; } }

        private void Awake()
        {
            
        }

        public void SetChecked(bool isChecked)
        {
            toggle.isOn = isChecked;
        }
        public void DoToggle()
        {
            toggle.isOn = !toggle.isOn;
        }

        protected void SetDefault()
        {
            toggle.isOn = DefaultChecked;
        }
        
        public void SetGroup(ToggleGroup g)
        {
            _group = g;
        }
        public void Refresh()
        {
            toggle.onValueChanged.RemoveAllListeners();
            SetDefault();
        }

        public void SetText(string text)
        {
            txt.text = text;
        }

        public void Hide()
        {
            SetChecked(false);
            gameObject.SetActive(false);
        }
        public void Show()
        {
            gameObject.SetActive(true);
        }
    }
}

