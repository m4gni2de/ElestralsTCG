using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Gameplay.GameCommands
{
    public enum CommandType
    {
        Generic = 0,
        Move = 1,
    }

    public enum CommandStatus
    {
        Success = 0,
        Fail = 1,
        Cancel = 2,
        Pending = 3
    }
    public class GameCommand 
    {
        public event Action<GameCommand> OnEventComplete;

        public CommandType m_commandType { get { return GetCommandType; } }
        protected virtual CommandType GetCommandType { get { return CommandType.Generic; } }    

        public CommandArgs Args { get { return GetArgs(); } }

        protected virtual CommandArgs GetArgs()
        {
            return null;
        }

        public virtual void Complete(CommandStatus status)
        {
            OnEventComplete?.Invoke(this);
        }

        public virtual void End()
        {

        }
    }
}

