using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OperatorConvertExtensions 
{
    public static bool IntToBool(this int a)
    {
        if (a == 0) { return false; }
        return true;
    }
    public static int BoolToInt(this bool a)
    {
        if (a == false) { return 0; }
        return 1;
    }

    public static bool IsEvenNumber(this int a)
    {
        return a % 2 == 0;
    }

    public static bool IsEmpty(this string a)
    {
        return string.IsNullOrWhiteSpace(a);
    }
}
