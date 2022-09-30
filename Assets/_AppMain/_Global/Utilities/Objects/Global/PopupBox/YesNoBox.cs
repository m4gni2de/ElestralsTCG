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

        public void Show(string msg, Action<bool> callback)
        {
            Refresh();
            txtMessage.text = msg;
            OnHandled = callback;
            ConfirmButton.gameObject.SetActive(true);
            CancelButton.gameObject.SetActive(true);
            ConfirmButton.onClick.AddListener(() => Confirm());
            CancelButton.onClick.AddListener(() => Cancel());

        }

        public override void Refresh()
        {
            ToggleHandled(false);
            gameObject.SetActive(true);
            ConfirmButton.onClick.RemoveAllListeners();
            CancelButton.onClick.RemoveAllListeners();
        }
        public override void Confirm()
        {
            SendResult(true);
        }
        public override void Cancel()
        {
            SendResult(false);
        }
    }
}

