using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringTools 
{
    public static string[] Array(params string[] args)
    {
        string[] array = new string[args.Length];
        for (int i = 0; i < args.Length; i++)
        {
            array[i] = args[i];
        }
        return array;
    }
}
