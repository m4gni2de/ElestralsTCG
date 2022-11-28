using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace PopupBox
{
    public enum PopupResponse
    {
        Cancel = 0,
        Yes = 1,
        No = 2,
    }
    public class BasePopup : MonoBehaviour
    {
        public Button ConfirmButton, DenyButton;
        protected bool _Handled = false;
        public bool Handled { get { return _Handled; } }

        public Delegate OnHandled;
        protected void SendResult(params object[] args)
        {
            OnHandled?.DynamicInvoke(args);
            ToggleHandled(true);
            OnHandled = null;
            Close();
        }
        protected void SendDeny()
        {
            StopAllCoroutines();
            ToggleHandled(true);
            OnHandled = null;
            Close();
        }

        protected void ToggleHandled(bool isHandled)
        {
            _Handled = isHandled;
            if (!isHandled)
            {
                ConfirmButton.onClick.RemoveAllListeners();
                DenyButton.onClick.RemoveAllListeners();
            }
        }
        public void ForceClose()
        {
            ToggleHandled(false);
            
        }

        public virtual void Refresh()
        {
            
        }
        public virtual void Confirm()
        {

        }
        public virtual void Deny()
        {

        }
        public virtual void Cancel()
        {

        }

        public virtual void Close()
        {
            gameObject.SetActive(false);
        }

    }
}

