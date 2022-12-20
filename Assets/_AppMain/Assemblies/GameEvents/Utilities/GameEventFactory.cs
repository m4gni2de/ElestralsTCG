using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace GameEvents
{
    public class GameEventFactory
    {
        public static Dictionary<int, string> IndexedEvents = new Dictionary<int, string>();

        public static GameEvent CreateGameEvent(string eventKey, params object[] source)
        {
            int count = source.Length;

            Type eventType;
            switch (count)
            {
                case 0:
                    return new GameEvent(eventKey);
                case 1:
                    eventType = typeof(GameEvent<>).MakeGenericType(source[0].GetType());
                    return (GameEvent)Activator.CreateInstance(eventType, eventKey);
                case 2:
                    eventType = typeof(GameEvent<,>).MakeGenericType(source[0].GetType(), source[1].GetType());
                    return (GameEvent)Activator.CreateInstance(eventType, eventKey);
                case 3:
                    eventType = typeof(GameEvent<,,>).MakeGenericType(source[0].GetType(), source[1].GetType(), source[2].GetType());
                    return (GameEvent)Activator.CreateInstance(eventType, eventKey);
            }

            return null;
        }




        
    }
}
