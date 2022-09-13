using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalUtilities;
using System;

public class ItemFilter<T> : iItemFilter
{
    public T _FilterType { get { return default(T); } }
    
    

    public static ItemFilter<T> Create(params T[] values)
    {
        if (values.Length == 1)
        {
            return new ValueFilter<T>(values[0]);
        }
        return new MultiFilter<T>(values);
    }
}
public class ValueFilter<T> : ItemFilter<T>
{
    public T Value { get; private set; }
    public ValueFilter(T val)
    {
        Value = val;
    }
}
    
public class MultiFilter<T> : ItemFilter<T>
{
    public List<T> Values { get; private set; }
    public MultiFilter(params T[] vals)
    {
        Values = vals.ListFrom();
    }
}

