using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using Gameplay.Messaging;

namespace Gameplay
{
    public class MessageController : MonoBehaviour
    {
        #region Properties
        public TouchObject touch;
        private List<GameMessage> _messages = null;
        public List<GameMessage> Messages
        {
            get
            {
                _messages ??= new List<GameMessage>();
                return _messages;
            }
        }
        public TMP_Text messageText;

        public GameMessage ActiveMessage { get; set; }
        public GameObject messageObject;
        public MessageViewModel vmMessage;

        #region Hide/Show Properties
        protected float timeOn = 0f;
        protected float maxTimeOn = 0f;
        #endregion
        #endregion

       
       
        public void ShowMessage(string msg)
        {
            GameMessage message = GameMessage.JustMessage(msg);
            ShowMessage(message);
        }

        public void ShowMessage(GameMessage msg)
        {
            //if (ActiveMessage != null)
            //{
            //    GameMessage a = ActiveMessage;
            //    CloseMessage(a);
            //}


            Messages.Add(msg);
            vmMessage.ShowMessage(msg);





            //ActiveMessage = msg;
            //messageText.text = msg.message;
            //messageObject.SetActive(true);

            if (msg != null && msg.DisplayTime > 0f)
            {
                //StartCoroutine(DoDisplay(msg.DisplayTime));
                //StartDisplayTimer(msg.DisplayTime);
            }
        }

       
       
        #region Touch Events
        //public void EndOnTouch()
        //{
        //    if (ActiveMessage != null)
        //    {
        //        GameMessage msg = ActiveMessage;
        //        if (msg.CloseOnTouch)
        //        {
        //            ForceClose(msg);
        //        }
        //    }
        //}
        #endregion


       
        #region DisplayTimer
       
        protected void EndDisplayTimer()
        {
            timeOn = 0f;
            maxTimeOn = 0f;
            if (messageObject.activeSelf == true)
            {
                messageObject.SetActive(false);
            }
            
        }

        protected IEnumerator DoDisplay(float maxTime)
        {
            timeOn = 0f;
            maxTimeOn = maxTime;
            do
            {
                yield return new WaitForEndOfFrame();
                timeOn += Time.deltaTime;

            } while (true && messageObject.activeSelf == true && timeOn <= maxTimeOn);

            timeOn = 0f;
            maxTimeOn = 0f;
            EndDisplayTimer();
        }

        private void Update()
        {
            
        }
        #endregion


    }
}

