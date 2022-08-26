using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class GameLog
    {
        private List<string> _logs = null;
        public List<string> logs
        {
            get
            {
                _logs ??= new List<string>();
                return _logs;
            }
        }

        public GameLog(string gameId)
        {
            Add($"Game '{gameId}' has been started.");
        }




        public void Add(string msg)
        {
            logs.Add(msg);
        }
    }
}

