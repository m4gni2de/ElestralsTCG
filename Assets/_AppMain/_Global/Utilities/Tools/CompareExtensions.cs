using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalUtilities;
using System;

public enum ComparedTo
{
    EqualTo = 0,
    GreaterThan = 1,
    LessThan = -1,
    NoComparison = -2,
}
public static class CompareExtensions
{


    public static ComparedTo CompareTo<T>(this T obj1, T obj2)
    {
        if (obj1.GetType().IsNumeric())
        {
            GenericNumber<T> a = new GenericNumber<T>(obj1);
            GenericNumber<T> b = new GenericNumber<T>(obj2);

            if (a > b) { return ComparedTo.GreaterThan; }
            if (a < b) { return ComparedTo.LessThan; }
            return ComparedTo.EqualTo;

        }

        if (obj1.GetType().IsBoolean())
        {
            bool a = obj1.BoolValueGeneric();
            bool b = obj2.BoolValueGeneric();

            if (a == b) { return ComparedTo.EqualTo; } else { return ComparedTo.NoComparison; }
        }

        if (obj1.GetType() == typeof(string))
        {
            string a1 = obj1.ToString();
            string b1 = obj2.ToString();

            if (a1.ToLower() == b1.ToLower()) { return ComparedTo.EqualTo; } else { return ComparedTo.NoComparison; }
        }

        if (obj1.GetType() == typeof(DateTime))
        {
            DateTime a = DateTime.Parse(obj1.ToString());
            DateTime b = DateTime.Parse(obj2.ToString());

            if (a > b) { return ComparedTo.GreaterThan; }
            if (a < b) { return ComparedTo.LessThan; }
            return ComparedTo.EqualTo;
        }

        return ComparedTo.NoComparison;
    }

    private static bool BoolValueGeneric(this object x)
    {
        if (!x.IsBoolean()) { throw new Exception("Generic value not derived from boolean value."); }
        bool objBool = (bool)x;
        return objBool;
    }



}
