using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using PopupBox;


namespace PopupBox
{
    public class DropdownPopup : BasePopup
    {
        public TMP_Text txtMessage;
        public TMP_Dropdown ddOptions;
        protected int OptionSelection { get; set; }
        protected List<string> _optionDisplays = null;
        public List<string> OptionDisplays
        {
            get
            {
                _optionDisplays ??= new List<string>();
                return _optionDisplays;
            }
        }

        public Type optionType;
        public IList Options { get; set; }
       
        protected List<TMP_Dropdown.OptionData> DropDownOptions<T>(List<T> objects, string propName)
        {
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            for (int i = 0; i < objects.Count; i++)
            {
                var props = objects[i].GetType().GetProperties();
                foreach (var prop in props)
                {
                    if (prop.Name.ToLower() == propName.ToLower())
                    {
                        object propVal = prop.GetValue(objects[i]);
                        string strVal = (string)propVal;
                        TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(strVal);
                        options.Add(optionData);
                    }
                }

            }
            return options;
        }

        protected List<string> GetBaseOptions<T>(List<T> objects, string propName)
        {
            List<string> options = new List<string>();
            for (int i = 0; i < objects.Count; i++)
            {
                var props = objects[i].GetType().GetProperties();
                foreach (var prop in props)
                {
                    if (prop.Name.ToLower() == propName.ToLower())
                    {
                        object propVal = prop.GetValue(objects[i]);
                        string strVal = (string)propVal;
                        options.Add(strVal);
                    }
                }

            }
            return options;
        }


        public override void Refresh()
        {
            txtMessage.text = "";
            ConfirmButton.onClick.RemoveAllListeners();
            CancelButton.onClick.RemoveAllListeners();
            ddOptions.onValueChanged.RemoveAllListeners();
            ddOptions.ClearOptions();
            OptionDisplays.Clear();
            if (Options == null)
            {
                Options = new List<object>();
            }
            Options.Clear();
            optionType = null;
            ToggleHandled(false);
        }

        public void ShowStrings(string msg, List<string> options, Action<string> callback)
        {
            Refresh();
            gameObject.SetActive(true);
            optionType = typeof(string);
            Options = new List<string>();
            for (int i = 0; i < options.Count; i++)
            {
                Options.Add(options[i]);
            }
            OptionDisplays.AddRange(options);
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

            
            ddOptions.AddOptions(OptionDisplays);
            OptionSelection = 0;
        }

        public void Show<T>(string msg, List<T> options, Action<T> callback, string propName)
        {

            Refresh();
            gameObject.SetActive(true);
            optionType = typeof(T);
            Options = new List<T>();
            for (int i = 0; i < options.Count; i++)
            {
                Options.Add(options[i]);
            }

            OptionDisplays.AddRange(GetBaseOptions(options, propName));
            
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

            ddOptions.AddOptions(OptionDisplays);
            OptionSelection = 0;
        }



        public static event Action<int> OnNewDropdownValue;
        protected void DropdownChanged(int newVal)
        {
            OnNewDropdownValue?.Invoke(newVal);
        }
        public override void Confirm()
        {
            OptionSelection = ddOptions.value;
            object selected = Options[OptionSelection];

            SendResult(selected);
        }
       
        public override void Cancel()
        {
            SendCancel();
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

