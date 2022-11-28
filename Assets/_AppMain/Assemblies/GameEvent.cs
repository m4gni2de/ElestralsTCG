using System;
using System.Collections;
using System.Collections.Generic;

namespace GameEvents
{
    public class GameEvent 
    {
        #region Properties
        public string uniqueId { get; set; }
        public string name { get; set; }
        public Type[] paramTypes { get; set; }

        private List<Watcher> _watchers = null;
        public List<Watcher> Watchers
        {
            get
            {
                _watchers ??= new List<Watcher>();
                return _watchers;
            }
        }

        
        #endregion
    }
}
