using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;

namespace Gameplay
{
    [System.Serializable]
    public class RuneCard : GameCard
    {
        #region Functions
        public GameCard EmpoweredElestral
        {
            get
            {

                if (CurrentSlot == null || CurrentSlot.GetType() != typeof(RuneSlot)) { return null; }
                RuneSlot slot = (RuneSlot)CurrentSlot;
                return slot.EmpoweredElestral;
            }
        }
        #endregion

        public RuneCard(Rune card, int copy) : base(card, copy)
        {

        }

        #region Overrides
        public override void AllocateTo(CardSlot slot, bool sendToServer = true)
        {
            base.AllocateTo(slot, sendToServer);
            if (!slot.IsInPlay)
            {
                Game.UnEmpower(this);
            }
        }
        protected override void SelectCard(bool toggle, Color color, bool sendToServer)
        {
            base.SelectCard(toggle, color, sendToServer);
            if (!toggle)
            {
                if (EmpoweredElestral != null)
                {
                    EmpoweredElestral.cardObject.SelectCard(false, Color.black);
                }
            }
            else
            {
                if (EmpoweredElestral != null)
                {
                    EmpoweredElestral.cardObject.SelectCard(true, color);
                }
            }
        }
        #endregion
    }



}

