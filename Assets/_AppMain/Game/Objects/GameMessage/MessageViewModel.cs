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
        public bool isShowing = false;

        public void Show(GameMessage msg)
        {
            //if (isShowing)
            //{
            //    StopTimer(false);
            //}
           
            ActiveMessage = msg;
            messageText.text = msg.message;
            gameObject.SetActive(true);

            if (msg.GetMessageType() == GameMessage.MessageType.Action)
            {
                ActionMessage ac = (ActionMessage)msg;
               ac.cardAction.OnActionEnd.AddListener(() => Hide());
            }

            //if (msg.DisplayTime > 0f)
            //{
            //    StartCoroutine(TimedShow(msg.DisplayTime));
            //}

            
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

        protected void StopTimer(bool hide)
        {
            if (isShowing)
            {
                StopAllCoroutines();
            }

            if (ActiveMessage != null)
            {
                ActiveMessage.ForceClose();
            }

            if (hide)
            {
                Hide();
            }
            
        }
        private IEnumerator TimedShow(float maxTime)
        {
            isShowing = true;
            float acumTime = 0f;
            do
            {
                yield return new WaitForEndOfFrame();
                acumTime += Time.deltaTime;

            } while (true && acumTime <= maxTime);
            isShowing = false;
        }
       
    }
}


