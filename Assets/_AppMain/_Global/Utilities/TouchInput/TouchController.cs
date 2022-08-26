using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class TouchController 
{
    public static DateTime LastTouch { get; private set; }

    private static Dictionary<int, DateTime> _touches = null;
    public static Dictionary<int, DateTime> Touches
    {
        get
        {
            _touches ??= new Dictionary<int, DateTime>();
            return _touches;
        }
    }

    private static void AddTouch(int touchIndex)
    {
        if (!Touches.ContainsKey(touchIndex))
        {
            Touches.Add(touchIndex, DateTime.Now);
        }
    }

    public static void RegisterTouch(int touchIndex)
    {
        AddTouch(touchIndex);
    }
    public static float TouchRelease(int touchIndex)
    {
        TimeSpan interval = DateTime.Now - Touches[touchIndex];
        return (float)interval.TotalSeconds;
    }
    
}
