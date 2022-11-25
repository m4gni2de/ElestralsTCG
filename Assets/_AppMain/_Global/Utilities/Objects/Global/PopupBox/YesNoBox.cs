using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace PopupBox
{
    public class YesNoBox : BasePopup
    {
        public TMP_Text txtMessage;
        /// <summary>
        /// If true, then the user has the option to cancel the input without doing either the Confirm or Deny actions. If CanCancel, send a PopupResponse instead of a boolean to the Handled Event
        /// </summary>
        protected bool CanCancel = false;

        /// <summary>
        /// Cancel button differs from the Deny Button, because sometimes the user will be prompted to do something like "Save before quitting?", in which case they might not want to quit at all, so they
        /// should be able to cancel the action altogether, instead of just saying "No".
        /// </summary>
        [SerializeField]
        private Button CancelButton;

        

        public void Show(string msg, Action<bool> callback, bool canCancel = false)
        {
            Refresh();
            txtMessage.text = msg;
            OnHandled = callback;
            ConfirmButton.gameObject.SetActive(true);
            DenyButton.gameObject.SetActive(true);
            ConfirmButton.onClick.AddListener(() => Confirm());
            DenyButton.onClick.AddListener(() => Deny());
            
            

            this.CanCancel = canCancel;
            CancelButton.gameObject.SetActive(this.CanCancel);
            if (this.CanCancel)
            {
                CancelButton.onClick.AddListener(() => Cancel());
            }
        }

        public void ShowWithCancel(string msg, Action<PopupResposne> callback)
        {
            Refresh();
            txtMessage.text = msg;
            OnHandled = callback;
            ConfirmButton.gameObject.SetActive(true);
            DenyButton.gameObject.SetActive(true);
            ConfirmButton.onClick.AddListener(() => Confirm());
            DenyButton.onClick.AddListener(() => Deny());



            this.CanCancel = true;
            CancelButton.gameObject.SetActive(true);
            CancelButton.onClick.AddListener(() => Cancel());
        }

        public override void Refresh()
        {
            ToggleHandled(false);
            gameObject.SetActive(true);
            txtMessage.text = "";
            ConfirmButton.onClick.RemoveAllListeners();
            DenyButton.onClick.RemoveAllListeners();
            CancelButton.onClick.RemoveAllListeners();
        }
        public override void Confirm()
        {
            if (!CanCancel) { SendResult(true); } else { SendResult(PopupResposne.Yes); }
            
        }
        public override void Deny()
        {
            if (!CanCancel) { SendResult(false); } else { SendResult(PopupResposne.No); }
        }
        public override void Cancel()
        {
            if (CanCancel)
            {
                SendResult(PopupResposne.Cancel);
            }
            
        }
    }
}

