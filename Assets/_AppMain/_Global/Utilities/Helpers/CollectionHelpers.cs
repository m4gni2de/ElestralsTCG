using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CollectionHelpers 
{
    /// <summary>
    /// Use this to make a list from the objects using in a params T[]
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="args"></param>
    /// <returns></returns>
    public static List<T> ListFrom<T>(this T[] args)
    {
        List<T> list = new List<T>();
        for (int i = 0; i < args.Length; i++)
        {
            list.Add(args[i]);
        }
        return list;
    }

    /// <summary>
    /// Use this to make a list of objects as parameters
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="args"></param>
    /// <returns></returns>
    public static List<T> ListWith<T>(params T[] args)
    {
        List<T> list = new List<T>();
        for (int i = 0; i < args.Length; i++)
        {
            list.Add(args[i]);
        }
        return list;
    }

    public static void AddUnique<T>(this List<T> list, List<T> toAddFrom)
    {
        for (int i = 0; i < toAddFrom.Count; i++)
        {
            if (!list.Contains(toAddFrom[i]))
            {
                list.Add(toAddFrom[i]);
            }
        }
    }


    public static string ToJson<T>(this List<T> list)
    {
        ReadableList<T> items = new ReadableList<T>();
        items.AddRange(list);
        return JsonUtility.ToJson(items);
    }
    public static T Last<T>(this List<T> list)
    {
        if (list.Count == 0) { App.LogFatal("This list does not have any items in it."); return default(T); }
        return list[list.Count - 1];
    }

    public static List<T> ReverseOf<T>(this List<T> original)
    {
        List<T> list = new List<T>();
        for (int i = original.Count -1; i > -1; i--)
        {
            list.Add(original[i]);
        }
        return list;
    }
}
