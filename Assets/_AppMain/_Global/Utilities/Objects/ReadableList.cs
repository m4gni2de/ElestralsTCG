using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
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

[System.Serializable]
public class DataList
{
    public List<string> items { get; set; }
}


[System.Serializable]
public class KeyValueItem
{
    public string Key { get; set; }
    public string Value { get; set; }
}


