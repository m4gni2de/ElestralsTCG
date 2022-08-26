using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Log 
{
    public static void Show(string msg)
    {
        Debug.Log(msg);
    }

    public static bool Warn(string msg)
    {
        Debug.Log(msg);
        return false;
    }

    public static void Stop(string msg)
    {
        Debug.Log(msg);
        throw new System.Exception(msg);
    }

    
}
