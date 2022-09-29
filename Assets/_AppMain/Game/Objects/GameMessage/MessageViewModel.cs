using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameplay;
using TMPro;

namespace Gameplay.Messaging
{
    public class MessageViewModel : MonoBehaviour
    {
        public GameMessage ActiveMessage { get; set; }
        public TMP_Text messageText;
        public TouchObject touch;

        public void Show(GameMessage msg)
        {
            ActiveMessage = msg;
            messageText.text = msg.message;
            gameObject.SetActive(true);

            if (msg.GetMessageType() == GameMessage.MessageType.Action)
            {
                ActionMessage ac = (ActionMessage)msg;
               ac.cardAction.OnActionEnd.AddListener(() => Hide());
            }
        }
        public void Hide()
        {
            gameObject.SetActive(false);
            messageText.text = "";
            ActiveMessage = null;
        }

        protected void Close(GameMessage msg)
        {
            msg.CloseMessage();
            Hide();
        }
        protected void ForceClose(GameMessage msg)
        {
            msg.ForceClose();
            if (ActiveMessage != null && ActiveMessage == msg)
            {
                Hide();
            }
        }

        public void ShowMessage(GameMessage msg)
        {
            if (ActiveMessage != null)
            {
                ActiveMessage.CloseMessage();
            }

            Show(msg);
            
        }



        #region Touch Events
        public void EndOnTouch()
        {
            if (ActiveMessage != null)
            {
                GameMessage msg = ActiveMessage;
                if (msg.CloseOnTouch)
                {
                    ForceClose(msg);
                }
            }
        }
        #endregion
    }
}


