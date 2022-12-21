using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Turns
{
    public class StandbyPhase : GamePhase
    {
        #region Properties


        #endregion

        #region Overrides


        protected override int GetTurnIndex()
        {
            return -1;
        }
        #endregion

        public StandbyPhase() 
        {

        }
    }
}
