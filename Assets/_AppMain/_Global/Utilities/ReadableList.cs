using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ReadableList<T>
{
    public List<T> items;

    public ReadableList()
    {
        items = new List<T>();
    }
    public void Add(T obj)
    {
        items.Add(obj);
    }
    public void Remove(T obj)
    {
        items.Remove(obj);
    }

    public void AddRange(List<T> list)
    {
        items.AddRange(list);
    }
}

public static class ListExtensions
{
    public static string ToJson<T>(this List<T> list)
    {
        ReadableList<T> items = new ReadableList<T>();
        items.AddRange(list);
        return JsonUtility.ToJson(items);
    }
}
