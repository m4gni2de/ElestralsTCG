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
    /// Use this to make a list from the objects using items in a params T[]
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

    /// <summary>
    /// Create a list from characters/segments of a string, broken up by given intervals. 
    /// </summary>
    /// <param name="fullString"></param>
    /// <param name="interval"></param>
    /// <returns></returns>
    public static List<string> AsList(this string fullString, int interval)
    {
        List<string> items = new List<string>();
        int count = 1;
        string code = "";


        for (int i = 0; i < fullString.Length; i++)
        {
            string c = fullString[i].ToString();

            code += c;
            if (count >= interval)
            {
                items.Add(code);
                count = 0;
                code = "";
            }

            count += 1;
        }

        return items;
    }

    /// <summary>
    /// Create a list from characters/segments from a given string that are identified by the breakChar(commonly as a comma, or '|')
    /// </summary>
    /// <param name="fullString"></param>
    /// <param name="breakChar"></param>
    /// <returns></returns>
    public static List<string> AsList(this string fullString, string breakChar)
    {
        List<string> items = new List<string>();
        string code = "";


        for (int i = 0; i < fullString.Length; i++)
        {
            string c = fullString[i].ToString();

            if (c != breakChar)
            {
                code += c;
            }
            else 
            {
                items.Add(code);
                code = "";
            }

            if (i >= fullString.Length - 1)
            {
                items.Add(code);
            }
        }

        return items;
    }

    

    /// <summary>
    /// Add a range of items to a list by setting them as an array of parameters, separated by a comma
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="args"></param>
    public static void AddRange<T>(this List<T> list, params T[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            list.Add(args[i]);
        }
    }

    /// <summary>
    /// Returns true only if ALL of the items in the parameter args exist in the list being searched.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="toCheck"></param>
    /// <returns></returns>
    public static bool ContainsAll<T>(this List<T> list, params T[] args)
    {
        if (args == null || args.Length == 0) { return false; }
        for (int i = 0; i < args.Length; i++)
        {
            if (!list.Contains(args[i])) { return false; }
        }
        return true;
    }

    /// <summary>
    /// Returns true only if ALL of the items in the parameter args exist in the list being searched.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="toCheck"></param>
    /// <returns></returns>
    public static bool ContainsAll<T>(this List<T> list, List<T> toCheck)
    {
        for (int i = 0; i < toCheck.Count; i++)
        {
            if (!list.Contains(toCheck[i])) { return false; }
        }
        return true;
    }

    /// <summary>
    /// Returns true if any of the items in the parameter list exist in the list being searched.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="toCheck"></param>
    /// <returns></returns>
    public static bool ContainsAny<T>(this List<T> list, params T[] args)
    {
        if (args == null || args.Length == 0) { return false; }
        for (int i = 0; i < args.Length; i++)
        {
            if (list.Contains(args[i])) { return true; }
        }
        return false;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="toCheck"></param>
    /// <returns></returns>
    public static bool ContainsAny<T>(this List<T> list, List<T> toCheck)
    {
        
        for (int i = 0; i < toCheck.Count; i++)
        {
            if (list.Contains(toCheck[i])) { return true; }
        }
        return false;
    }

    /// <summary>
    /// Convert a list of strings in to a list of integers
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public static List<int> StringToInt(this List<string> list)
    {
        List<int> results = new List<int>();
        for (int i = 0; i < list.Count; i++)
        {
            results.Add(int.Parse(list[i]));
        }
        return results;
    }
}
