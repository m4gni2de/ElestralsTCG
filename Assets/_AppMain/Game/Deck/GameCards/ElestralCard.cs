using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    [System.Serializable]
    public class ElestralCard : GameCard
    {
        #region Properties

        #endregion

        #region Functions
        public List<GameCard> EmpoweringRunes
        {
            get
            {
                List<GameCard> runes = new List<GameCard>();
                foreach (var item in GameManager.ActiveGame.EmpoweredRunes)
                {
                    if (item.Value == this) { runes.Add(item.Key); }
                }
                return runes;
            }
        }
        #endregion

        public ElestralCard(Elestral card, int copy) : base(card, copy)
        {
          
        }


        #region Overrides
        public override void AllocateTo(CardSlot slot, bool sendToServer = true)
        {
            base.AllocateTo(slot, sendToServer);
            if (!slot.IsInPlay)
            {
                Game.UnEmpowerFromElestral(this);
            }
        }
        protected override void SelectCard(bool toggle, Color color, bool sendToServer)
        {
            base.SelectCard(toggle, color, sendToServer);
            if (!toggle)
            {
                List<GameCard> empowering = EmpoweringRunes;
                for (int i = 0; i < empowering.Count; i++)
                {
                    empowering[i].cardObject.SelectCard(false, Color.black);
                }
            }
            else
            {
                for (int i = 0; i < EmpoweringRunes.Count; i++)
                {
                    EmpoweringRunes[i].cardObject.SelectCard(true, color);
                }
            }
            
        }
        #endregion
    }
}

