using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.GameCommands
{
    public class MoveCommand : GameCommand
    {
        [System.Serializable]
        public class MoveCommandArgs : CommandArgs
        {
            public Player player { get; set; }
            public GameCard card { get; set; }
            public GameCard.VisualInfo lastInfo { get; set; }
            public GameCard.VisualInfo newInfo { get; set; }
            public CardSlot currentSlot { get; set; }
            public CardSlot newSlot { get; set; }
            public CommandStatus MoveStatus { get; set; }

            public MoveCommandArgs()
            {
                commandType = CommandType.Move;
            }

            public void Update(CommandStatus status)
            {
                newInfo = card.m_VisualInfo;
                newSlot = card.CurrentSlot;
                MoveStatus = status;
            }

            

        }

        #region Properties
        protected override CommandType GetCommandType { get { return CommandType.Move; } }
        private MoveCommandArgs _moveArgs = null;
        public MoveCommandArgs moveArgs { get { return _moveArgs; } }

        protected override CommandArgs GetArgs()
        {
            return moveArgs;
        }
        #endregion


        #region Creation
        MoveCommand(MoveCommandArgs args)
        {
            _moveArgs = args;
        }
        public static MoveCommand StartMove(GameCard card)
        {
            MoveCommandArgs args = new MoveCommandArgs();
            args.player = card.Owner;
            args.card = card;
            args.lastInfo = card.m_VisualInfo;
            args.currentSlot = card.CurrentSlot;
            args.MoveStatus = CommandStatus.Pending;

            return new MoveCommand(args);
        }
        #endregion

        public override void Complete(CommandStatus status)
        {
            moveArgs.Update(status);
        }


    }
}

