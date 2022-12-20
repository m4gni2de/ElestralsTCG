using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Commands
{
    public class CommandArgs 
    {
        public string id { get; protected set; }
        public string key { get; protected set; }
        public GameCommand.Result result { get; protected set; }

        public CommandArgs(GameCommand comm)
        {
            SetArgs(comm);
        }
        protected virtual void SetArgs(GameCommand comm)
        {

            id = comm.id;
            key = comm.key;
            result = comm.result;
        }
    }
}

