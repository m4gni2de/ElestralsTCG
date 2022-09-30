using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using TMPro.EditorUtilities;

namespace PopupBox
{
    public class DropdownPopup : BasePopup
    {
        public TMP_Text txtMessage;
        public TMP_Dropdown ddOptions;
        protected int OptionSelection { get; private set; }
        private List<string> _baseOptions = null;
        public List<string> BaseOptions
        {
            get
            {
                _baseOptions ??= new List<string>();
                return _baseOptions;
            }
        }


        protected List<TMP_Dropdown.OptionData> DropDownoptions
        {
            get
            {
                List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
                for (int i = 0; i < BaseOptions.Count; i++)
                {
                    TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(BaseOptions[i]);
                    options.Add(optionData);
                }
                
                
               
                return options;
            }
        }


        public override void Refresh()
        {
           
            ConfirmButton.onClick.RemoveAllListeners();
            CancelButton.onClick.RemoveAllListeners();
            ddOptions.onValueChanged.RemoveAllListeners();
            ddOptions.ClearOptions();
            BaseOptions.Clear();
            ToggleHandled(false);
        }

        public void Show(string msg, List<string> options, Action<string> callback)
        {
            Refresh();
            gameObject.SetActive(true);
            BaseOptions.AddRange(options);
            ddOptions.onValueChanged.AddListener(DropdownChanged);
            txtMessage.text = msg;
            ConfirmButton.gameObject.SetActive(true);
            CancelButton.gameObject.SetActive(false);
            if (callback != null)
            {
                OnHandled = callback;

            }
            ConfirmButton.onClick.AddListener(() => Confirm());
            CancelButton.onClick.AddListener(() => Cancel());

            ddOptions.AddOptions(BaseOptions);
            OptionSelection = 0;



        }

        public static event Action<int> OnNewDropdownValue;
        private void DropdownChanged(int newVal)
        {
            OnNewDropdownValue?.Invoke(newVal);
        }
        public override void Confirm()
        {
            OptionSelection = ddOptions.value;
            string selected = BaseOptions[OptionSelection];

            SendResult(selected);
        }
        public override void Cancel()
        {
            SendResult("");
        }
        public override void Close()
        {
            ConfirmButton.onClick.RemoveAllListeners();
            ddOptions.onValueChanged.RemoveAllListeners();
            gameObject.SetActive(false);
            base.Close();
        }

    }
}
