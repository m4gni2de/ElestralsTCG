using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Gameplay.Messaging;

namespace Gameplay
{
    public class GameMessage 
    {

        #region Properties
        public string message;
        protected bool _closeOnTouch = false;
        public bool CloseOnTouch { get { return _closeOnTouch; } }
        protected float _displayTime = 1f;
        public float DisplayTime
        {
            get
            {
                return _displayTime;
            }
        }
        #endregion


        protected GameMessage(string message, bool TouchToClose = false, float displayTime = 1f)
        {
            this.message = message;
            _closeOnTouch = TouchToClose;
            _displayTime = displayTime;
            
        }

        public static GameMessage JustMessage(string msg)
        {
            return new GameMessage(msg);
        }
        public static GameMessage FromAction(string msg, CardAction ac, bool closeOnTouch, float displayTime)
        {
            return new ActionMessage(msg, ac, closeOnTouch, displayTime);
        }

        public virtual void CloseMessage()
        {

        }
  
    }
}

