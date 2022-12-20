using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

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

   
    /// <summary>
    /// Gets all Values of an Enum, with an optional parameter to exlude any(by their int value) from the returned list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="except"></param>
    /// <returns></returns>
    public static List<T> GetAll<T>(params int[] except) 
    {
        List<T> list = new List<T>();

        
        foreach (T item in Enum.GetValues(typeof(T)))
        {
            int eVal = (int)Enum.ToObject(typeof(T), item);
            if (except != null && except.Length > 0)
            {
                if (!except.Contains<int>(eVal))
                {
                    list.Add(item);
                }
            }
        }
        return list;
    }

    public static List<T> IntToEnum<T>(List<int> values)
    {
        List<T> list = new List<T>();

        for (int i = 0; i < values.Count; i++)
        {
            T obj = (T)Enum.ToObject(typeof(T), values[i]);
            list.Add(obj);
        }
        return list;
    }

}
