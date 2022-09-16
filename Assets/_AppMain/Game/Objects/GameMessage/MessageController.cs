using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static UnityEngine.Rendering.DebugUI;

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

        private GameMessage _activeMessage = null;
        public GameMessage ActiveMessage
        {
            get
            {
                return _activeMessage;
            }
            set
            {
                _activeMessage = value;
            }
            
        }
        public GameObject messageObject;

        #region Hide/Show Properties
        protected float timeOn = 0f;
        protected float maxTimeOn = 0f;
        #endregion
        #endregion

        #region Events
        public event Action<GameMessage> OnMessageClose;
        protected void CloseMessage(GameMessage msg)
        {
            msg.CloseMessage();
            messageObject.SetActive(false);
            messageText.text = "";
            _activeMessage = null;


        }
        #endregion

        public void ShowMessage(string msg)
        {
            GameMessage message = GameMessage.JustMessage(msg);
            ShowMessage(message);
        }
        public void ShowMessage(GameMessage msg)
        {
            Messages.Add(msg);
            messageText.text = msg.message;
            messageObject.SetActive(true);
            ActiveMessage = msg;

            if (msg != null && msg.DisplayTime > 0f)
            {
                StartDisplayTimer(msg.DisplayTime);
            }
        }

       
       
        #region Touch Events
        public void EndOnTouch()
        {
            if (ActiveMessage != null)
            {
                GameMessage msg = ActiveMessage;
                if (msg.CloseOnTouch)
                {
                    CloseMessage(msg);
                }
            }
        }
        #endregion


       
        #region DisplayTimer
        protected void StartDisplayTimer(float maxTime)
        {
            timeOn = 0f;
            maxTimeOn = maxTime;
        }
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

