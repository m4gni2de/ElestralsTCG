using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEvents;
using Gameplay;

public class CardEvent 
{
    private Dictionary<string, object> _data = null;
    public Dictionary<string, object> Data { get { _data ??= new Dictionary<string, object>(); return _data; } }

    public CardEvent(iGameEvent ev)
    {
        foreach (var para in ev.Parameters)
        {

        }
    }
}

