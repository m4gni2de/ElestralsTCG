using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class ObjectExtensions 
{
    public static T Clone<T>(this T source)
    {
        var ser = JsonUtility.ToJson(source);
        return JsonUtility.FromJson<T>(ser);
    }


    public static object GetPropertyOrFieldValue(this object obj, string propName)
    {
        if (obj == null) { return null; }

        propName = propName.ToLower();
        PropertyInfo[] props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Default | BindingFlags.Instance | BindingFlags.FlattenHierarchy);


        foreach (var prop in props)
        {
            if (prop.Name.ToLower() == propName)
            {
                object propVal = prop.GetValue(obj);
                if (propVal.GetType().IsEnum) { return (int)propVal; }
                return propVal;
            }
        }

        FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Default | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        foreach (var field in fields)
        {
            if (field.Name.ToLower() == propName)
            {
                object fieldVal = field.GetValue(obj);
                if (fieldVal.GetType().IsEnum) { return (int)fieldVal; }
                return fieldVal;
            }
        }

        return null;
    }


    public static object GetNestedValue(this object obj, List<string> propKeys, string propName)
    {
        if (obj == null) { return null; }

        object nested = obj;

        for (int i = 0; i < propKeys.Count; i++)
        {
            string key = propKeys[i].ToLower();

            if (nested.GetPropertyOrFieldValue(key) != null)
            {
                nested = nested.GetPropertyOrFieldValue(key);
            }
        }


        propName = propName.ToLower();
        PropertyInfo[] props = nested.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Default | BindingFlags.Instance | BindingFlags.FlattenHierarchy);


        foreach (var prop in props)
        {
            if (prop.Name.ToLower() == propName)
            {
                object propVal = prop.GetValue(nested);
                if (propVal.GetType().IsEnum) { return (int)propVal; }
                return propVal;
            }
        }

        FieldInfo[] fields = nested.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Default | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        foreach (var field in fields)
        {
            if (field.Name.ToLower() == propName)
            {
                object fieldVal = field.GetValue(nested);
                if (fieldVal.GetType().IsEnum) { return (int)fieldVal; }
                return fieldVal;
            }
        }

        return null;
    }

   
}
