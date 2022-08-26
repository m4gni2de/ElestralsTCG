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
            names.Add(array[0]);
        }
        return names;
        
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
