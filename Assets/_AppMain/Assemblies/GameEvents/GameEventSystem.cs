using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace GameEvents
{
    public class GameEventSystem 
    {
       
        public GameEventSystem(bool doLocal)
        {
            if (doLocal) { ValidateLocalEvents(); } 
        }
        public static void RegisterIndexedEvents()
        {
            foreach (var ev in IndexedEvents)
            {
                string key = ev.Value;
                GameEvent iEvent = Find(key);

                if (!RegisteredEvents.ContainsKey(key))
                {
                    RegisteredEvents.Add(key, iEvent);
                }
            }
           
        }
        #region Event Registering/Finding
        private static Dictionary<string, GameEvent> _registeredEvents = null;
        public static Dictionary<string, GameEvent> RegisteredEvents { get { _registeredEvents ??= new Dictionary<string, GameEvent>(); return _registeredEvents; } }

        public static void Register(string eventKey, GameEvent ev)
        {
            if (!RegisteredEvents.ContainsKey(eventKey))
            {
                RegisteredEvents.Add(eventKey, ev);
            }
        }

        public static void UnRegister(string eventKey, GameEvent ev)
        {
            if (RegisteredEvents.ContainsKey(eventKey))
            {
                RegisteredEvents.Remove(eventKey);
            }
        }


        public static GameEvent Find(int index)
        {
            if (IndexedEvents.ContainsKey(index))
            {
                string evName = IndexedEvents[index];
                return Find(evName);
            }
            return null;
        }
        public GameEvent FindLocal(int index)
        {
            if (LocalEvents.ContainsKey(index))
            {
                return LocalEvents[index];
            }
            return null;
        }
        public static GameEvent Find(string key)
        {
            if (RegisteredEvents.ContainsKey(key)) { return RegisteredEvents[key]; }
            return null;
        }
        #endregion

        #region Event Indexing
        public static Dictionary<int, string> IndexedEvents = new Dictionary<int, string>();
        public static void ValidateEvents(Type t)
        {
            IndexedEvents.Clear();

            foreach (var prop in t.GetProperties())
            {
                foreach (var att in prop.GetCustomAttributes(typeof(EventIndexAttribute), true))
                {
                    if (att is EventIndexAttribute)
                    {
                        int i = prop.GetCustomAttribute<EventIndexAttribute>().index;
                        bool local = prop.GetCustomAttribute<EventIndexAttribute>().isLocal;
                        if (IndexedEvents.ContainsKey(i))
                        {
                            string existingVal = IndexedEvents[i];
                            string msg = $"DUPLICATE ENTRY ({prop.Name}). Event Index '{i}' already belongs to Event '{existingVal}' in Class {t.FullName}.";
                            throw new Exception(msg);
                        }
                        else
                        {
                            IndexedEvents.Add(i, prop.Name);
                          
                        }
                    }
                }
            }
        }

        #region Local Indexed Events
        private Dictionary<int, GameEvent> _localEvents = null;
        public Dictionary<int, GameEvent> LocalEvents { get { _localEvents ??= new Dictionary<int, GameEvent>(); return _localEvents; } }
        #endregion

        public virtual void ValidateLocalEvents()
        {
            
            foreach (var prop in this.GetType().GetProperties())
            {
                foreach (var att in prop.GetCustomAttributes(typeof(EventIndexAttribute), true))
                {
                    if (att is EventIndexAttribute)
                    {
                        int i = prop.GetCustomAttribute<EventIndexAttribute>().index;
                        bool local = prop.GetCustomAttribute<EventIndexAttribute>().isLocal;
                        if (local)
                        {
                            if (LocalEvents.ContainsKey(i))
                            {
                                string existingVal = LocalEvents[i].key;
                                string msg = $"DUPLICATE ENTRY ({prop.Name}). Event Index '{i}' already belongs locally to Event '{existingVal}'.";
                                throw new Exception(msg);
                            }
                            else
                            {
                                LocalEvents.Add(i, (GameEvent)prop.GetValue(this));
                            }


                        }
                    }
                }
            }
        }
        #endregion
    }
}
