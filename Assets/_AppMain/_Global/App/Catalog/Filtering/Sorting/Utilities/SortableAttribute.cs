using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using GlobalUtilities;
using UnityEngine;


/// <summary>
/// Objects marked with this attribute can use the SortableValue and SortableProxy attributes
/// </summary>

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class SortableObjectAttribute : Attribute
{
    //public Type objectType { get; set; }

    //public SortableObjectAttribute(Type type)
    //{
    //    objectType = type;
    //}

    
}

/// <summary>
/// Place this attribute over a Property to allow it to be identified as the value to be used when it is being sorted, and the sorter is sorting by the set SortBy
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class SortableValueAttribute : Attribute
{
    public SortBy Sorter { get; set; }
    public SortableValueAttribute(SortBy sortBy)
    {
        Sorter = sortBy;
    }

   
}

/// <summary>
/// Use this Attribute to name a variable Proxy object that contains SortableAttributes for Sorting
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class SortableProxyAttribute : Attribute
{
    public Type proxyType { get; set; }

    private List<SortBy> _exclusions = null;
    public List<SortBy> Exclusions
    {
        get
        {
            _exclusions ??= new List<SortBy>();
            return _exclusions;
        }
    }
    public SortableProxyAttribute(Type type, params SortBy[] toExclude)
    {
        proxyType = type;
        if (toExclude == null || toExclude.Length == 0) { return; }
        for (int i = 0; i < toExclude.Length; i++)
        {
            if (!Exclusions.Contains(toExclude[i]))
            {
                Exclusions.Add(toExclude[i]);
            }
        }
    }

    public Dictionary<SortBy, object> GetProxyValues(object obj)
    {
        Dictionary<SortBy, object> values = new Dictionary<SortBy, object>();
        PropertyInfo[] props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Default | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        foreach (var prop in props)
        {
            foreach (var att in prop.GetCustomAttributes(typeof(SortableValueAttribute), true))
            {
                if (att is SortableValueAttribute)
                {
                    SortableValueAttribute s = att as SortableValueAttribute;
                    var val = prop.GetValue(obj);
                    if (!Exclusions.Contains(s.Sorter) || !values.ContainsKey(s.Sorter))
                    {
                        values.Add(s.Sorter, val);
                    }
                }
            }
        }

        return values;
    }
}
