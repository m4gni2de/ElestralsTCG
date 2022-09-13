using System.Collections;
using System.Collections.Generic;
using Gameplay.CardActions;
using UnityEngine;

namespace Gameplay.Turns
{
    public class EndPhase : GamePhase
    {

        #region Properties


        #endregion

        #region Overrides

       
        protected override int GetTurnIndex()
        {
            return 3;
        }
        #endregion
        public EndPhase(Player p) : base(p)
        {
            AutoEnd = true;
        }


    }
}


