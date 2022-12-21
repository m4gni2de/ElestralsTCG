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
        private GameCommand _activeCommand = null;
        protected GameCommand ActiveCommand
        {
            get
            {
                return _activeCommand;
            }
            set
            {
                if (value == _activeCommand) { return; }
                _activeCommand = value;

                //do some waiting here if you are not the one doing the command
                if (!_activeCommand.CanActivate)
                {
                    _activeCommand.CompleteCommand();
                }
            }
        }

        #region Game Specific Commands
        public void DoAscend(Ascend ascend)
        {
            ActiveCommand = ascend;
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

