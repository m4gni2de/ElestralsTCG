using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FreezeExtensions
{
    public static void Freeze(this iFreeze obj)
    {
        AppManager.Freeze(obj);
    }
    public static void Thaw(this iFreeze obj)
    {
        AppManager.Thaw(obj);
    }
}
