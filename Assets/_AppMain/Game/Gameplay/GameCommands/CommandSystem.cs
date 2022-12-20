using System.Collections;
using System.Collections.Generic;
using GameEvents;
using Gameplay.CardActions;
using UnityEngine;
using Gameplay.Commands;

namespace Gameplay
{
    [System.Serializable]
    public class CommandSystem
    {
       

        #region Game Specific Commands
        public void Ascend(Player player, Ascend.AscendArgs args)
        {

        }

        protected void Expend(Player player, params ElementCode[] toExpend)
        {
            if (toExpend == null || toExpend.Length == 0) { return; }
            for (int i = 0; i < toExpend.Length; i++)
            {

            }
        }
        #endregion
    }
}

