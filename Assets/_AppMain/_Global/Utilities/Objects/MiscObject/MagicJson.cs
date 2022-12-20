using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicJson 
{
    private Dictionary<string, object> _data = null;
    public Dictionary<string, object> Data { get { _data ??= new Dictionary<string, object>(); return _data; } }


    public MagicJson(string id)
    {
        AddData("id", id);
    }

    public object this[string key]
    {
        get
        {
            if (Data.ContainsKey(key)) return Data[key];
            return null;
        }
    }

    public void AddKey(string key)
    {
        if (!Data.ContainsKey(key))
        {
            Data.Add(key, null);
        }
    }
    public void AddData<T>(string key, T value)
    {
        if (!Data.ContainsKey(key))
        {
            Data.Add(key, value);
        }
        else
        {
            SetData(key, value);
        }
    }

    private void SetData<T>(string key, T value)
    {
        if (Data.ContainsKey(key))
        {
            Data[key] = value;
        }
    }
}
