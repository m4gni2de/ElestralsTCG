using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEvents
{
    public interface iGameEvent
    {
        string key { get; set; }
        string id { get; set; }
        List<iParameter> Parameters { get; }
        List<Watcher> Watchers { get; }
        void Invoke(params object[] args);
    }
}
