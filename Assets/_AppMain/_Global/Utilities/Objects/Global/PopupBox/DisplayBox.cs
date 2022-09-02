using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace PopupBox
{
    public class DisplayBox : BasePopup
    {
        public TMP_Text txtMessage;


        public void Show(string msg, Action callback)
        {
            ToggleHandled(false);
            txtMessage.text = msg;
            ConfirmButton.gameObject.SetActive(true);
            CancelButton.gameObject.SetActive(false);
            if (callback != null)
            {
                OnHandled = callback;
                
            }
            ConfirmButton.onClick.AddListener(() => Confirm());



        }

        public override void Confirm()
        {
            SendResult();
        }
    }
}
