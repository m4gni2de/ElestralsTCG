using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameplay;
using UnityEngine.Events;

namespace Gameplay.Messaging
{
    public class ActionMessage : GameMessage
    {
        public CardAction cardAction;

        private UnityEvent _OnTouchAction = null;
        public UnityEvent OnTouchAction
        {
            get
            {
                _OnTouchAction ??= new UnityEvent();
                return _OnTouchAction;
            }
        }
        public override MessageType GetMessageType()
        {
            return MessageType.Action;
        }

        protected override bool CloseEvents()
        {
            return true;
        }

        public ActionMessage(string msg, CardAction ac, bool CloseOnTouch, float displayTime) : base(msg, CloseOnTouch, displayTime)
        {
            cardAction = ac;
            
        }
       

        public override void CloseMessage()
        {
            RemoveWatchers();
        }

        protected void RemoveWatchers()
        {
            cardAction.OnActionEnd.RemoveAllListeners();
        }
        public override void ForceClose()
        {
            RemoveWatchers();
            if (cardAction != null)
            {
                cardAction.actionResult = ActionResult.Succeed;
                cardAction.ForceCompleteAction();
            }
        }

    }
}

