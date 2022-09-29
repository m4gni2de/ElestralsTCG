using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectExtensions 
{
    public static T Clone<T>(this T source)
    {
        var ser = JsonUtility.ToJson(source);
        return JsonUtility.FromJson<T>(ser);
    }
}
