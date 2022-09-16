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

        public ActionMessage(string msg, CardAction ac, bool CloseOnTouch, float displayTime) : base(msg, CloseOnTouch, displayTime)
        {
            cardAction = ac;
        }

        public override void CloseMessage()
        {
            base.CloseMessage();
            if (cardAction != null)
            {
                cardAction.actionResult = ActionResult.Succeed;
                cardAction.ForceCompleteAction();
            }
        }
    }
}

