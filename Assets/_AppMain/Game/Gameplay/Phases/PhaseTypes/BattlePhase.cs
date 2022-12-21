using System.Collections;
using System.Collections.Generic;
using Gameplay.CardActions;
using UnityEngine;

namespace Gameplay.Turns
{
    public class BattlePhase : GamePhase
    {

        #region Properties


        #endregion

        #region Overrides

        
        protected override int GetTurnIndex()
        {
            return 2;
        }
        #endregion
        public BattlePhase(Player p) 
        {

        }


    }
}


