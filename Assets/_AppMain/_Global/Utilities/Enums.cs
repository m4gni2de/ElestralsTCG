using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Enums 
{
    
    public static List<string> GetNames(Type ty)
    {
        List<string> names = new List<string>();

        string[] array = Enum.GetNames(ty);

        for (int i = 0; i < array.Length; i++)
        {
            names.Add(array[i]);
        }
        return names;
        
    }
    
    public static T ConvertTo<T>(string val)
    {
        foreach (T item in Enum.GetValues(typeof(T)))
        {
            if (item.ToString().ToLower() == val.ToLower())
            {
                return item;
            }
        }

        App.LogFatal($"Enum of type {typeof(T)} does not contain Name '{val}'");
        return default(T);
        
    }

    public static string NameOf(Type ty, string text)
    {
        List<string> names = GetNames(ty);
        for (int i = 0; i < names.Count; i++)
        {
            if (names[i].ToLower() == text.ToLower())
            {
                return names[i];
            }
        }
        return "";
    }

   

}
