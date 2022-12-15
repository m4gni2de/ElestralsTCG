using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEvents
{
    public class GameEventFactory
    {
       
        public static GameEvent CreateGameEvent(params object[] source)
        {
            int count = source.Length;

            Type eventType;
            switch (count)
            {
                case 0:
                    return new GameEvent();
                case 1:
                    eventType = typeof(GameEvent<>).MakeGenericType(source[0].GetType());
                    return (GameEvent)Activator.CreateInstance(eventType);
                case 2:
                    eventType = typeof(GameEvent<,>).MakeGenericType(source[0].GetType(), source[1].GetType());
                    return (GameEvent)Activator.CreateInstance(eventType);
                case 3:
                    eventType = typeof(GameEvent<,,>).MakeGenericType(source[0].GetType(), source[1].GetType(), source[2].GetType());
                    return (GameEvent)Activator.CreateInstance(eventType);
            }

            return null;
        }
    }
}
