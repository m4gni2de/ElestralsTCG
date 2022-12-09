using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GlobalUtilities;
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
        for (int i = original.Count - 1; i > -1; i--)
        {
            list.Add(original[i]);
        }
        return list;
    }

    public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, SortDirection direction)
    {
        if (direction == SortDirection.ASC) { return source.OrderBy(keySelector); } else { return source.OrderByDescending(keySelector); }
    }

    public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, SortDirection direction)
    {
        bool isDescending = direction == SortDirection.DESC;
        return source.CreateOrderedEnumerable(keySelector, null, descending: isDescending);
    }


}
