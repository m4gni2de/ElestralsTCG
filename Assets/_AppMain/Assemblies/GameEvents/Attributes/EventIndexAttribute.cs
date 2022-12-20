using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using GameEvents;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace GameEvents
{

    /// <summary>
    /// Attribute that gives a GameEvent a unique index so that the Event can be referenced by the index. 
    /// If set to local, then the event will also be registered on the local GameEventSystem where it exists, as well as the global list.
    /// If local is FALSE, then only 1 instance of the Event can exist/be called upon.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EventIndexAttribute : Attribute
    {

        public int index { get; set; }
        public bool isLocal { get; set; }
        public EventIndexAttribute(int eventIndex, bool localEvent)
        {
            index = eventIndex;
            isLocal = localEvent;
        }


      
    }
}
