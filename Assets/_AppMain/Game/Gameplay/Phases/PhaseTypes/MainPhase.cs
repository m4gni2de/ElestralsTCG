using System.Collections;
using System.Collections.Generic;
using Gameplay.CardActions;
using UnityEngine;

namespace Gameplay.Turns
{
    public class MainPhase : GamePhase
    {
        #region Properties


        #endregion

        #region Overrides
       
        protected override int GetTurnIndex()
        {
            return 1;
        }
        #endregion
        public MainPhase(Player p) : base(p)
        {

        }
    }
}
