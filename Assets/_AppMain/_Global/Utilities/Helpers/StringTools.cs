using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public static class StringTools 
{
    public static string[] Array(params string[] args)
    {
        string[] array = new string[args.Length];
        for (int i = 0; i < args.Length; i++)
        {
            array[i] = args[i];
        }
        return array;
    }

    private static Regex m_RegexExpression = new Regex(@"(?<!\\)(?:\\u[0-9a-fA-F]{4}|\\U[0-9a-fA-F]{8})");
    public static void UnicodeString(this string stringWithUnicodeChars, out string convertedString)
    {
        string plainText = stringWithUnicodeChars;

        convertedString = m_RegexExpression.Replace(stringWithUnicodeChars,
            match =>
            {
                if (match.Value.StartsWith("\\U"))
                {
                    string st = char.ConvertFromUtf32(int.Parse(match.Value.Replace("\\U", " "), NumberStyles.HexNumber));
                    return st;
                }

                string stl = char.ConvertFromUtf32(int.Parse(match.Value.Replace("\\u", " "), NumberStyles.HexNumber));
                return stl;
            });

    }
}
