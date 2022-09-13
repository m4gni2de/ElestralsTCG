using System.Collections;
using System.Collections.Generic;
using Gameplay.CardActions;
using UnityEngine;

namespace Gameplay.Turns
{
    public class DrawPhase : GamePhase
    {

        #region Properties


        #endregion

        #region Overrides

        protected override int GetTurnIndex()
        {
            return 0;
        }
        #endregion
        public DrawPhase(Player p, bool isFirstTurn = false) : base(p)
        {
            AutoEnd = true;
            if (!isFirstTurn)
            {
                DrawAction ac = DrawAction.TurnStart(p);
                AddAction(ac, 0);
            }
        }

       
    }
}


