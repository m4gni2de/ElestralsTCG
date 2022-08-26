using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using System;
using Databases;
using Elements;

public static class CustomFont 
{
    public static readonly string NullSymbol = "NULL SYMBOL";
    public static readonly string WindSymbol = @"\u0080";
    public static readonly string DarkSymbol = @"\u0081";
    public static readonly string EarthSymbol = @"\u0082";
    public static readonly string FireSymbol = @"\u0083";
    public static readonly string FrostSymbol = @"\u0084";
    public static readonly string LightSymbol = @"\u0085";
    public static readonly string ThunderSymbol = @"\u0086";
    public static readonly string WaterSymbol = @"\u0087";
    public static readonly string StellarSymbol = @"\u0088";

    private static Regex m_RegexExpression = new Regex(@"(?<!\\)(?:\\u[0-9a-fA-F]{4}|\\U[0-9a-fA-F]{8})");

    public static void Format(string unicode, TMP_Text textLabel)
    {
        string plainText = unicode;

        unicode = m_RegexExpression.Replace(unicode,
            match =>
            {
                if (match.Value.StartsWith("\\U"))
                {
                    string st = char.ConvertFromUtf32(int.Parse(match.Value.Replace("\\U", ""), NumberStyles.HexNumber));
                    return st;
                }

                string stl = char.ConvertFromUtf32(int.Parse(match.Value.Replace("\\u", ""), NumberStyles.HexNumber));
                return stl;
            });

        textLabel.text = unicode;
    }

    public static void FormatEffect(string effect, TMP_Text textLabel)
    {
        string newEffect = effect;

        List<ElementData> elements = ElementService.ElementsList;

        for (int i = 0; i < elements.Count; i++)
        {
            string keystroke = $"<{elements[i].Keystroke}>";


            if (newEffect.Contains(keystroke))
            {
                string unicode = GetUnicode(keystroke);
                string newUnicode = $" {unicode} ";
                newEffect = newEffect.Replace(keystroke, newUnicode);
            }
        }

        Format(newEffect, textLabel);
    }

    public static string GetUnicode(string keyStroke)
    {
        switch (keyStroke)
        {
            case "<T>":
            return ThunderSymbol;
            case "<Wi>":
                return WindSymbol;
            case "<E>":
                return EarthSymbol;
            case "<Wa>":
                return WaterSymbol;
            case "<Fi>":
                return FireSymbol;
            case "<Fr>":
                return FrostSymbol;
            case "<D>":
                return DarkSymbol;
            case "<Stellar>":
                return StellarSymbol;
            default:
                return keyStroke;
        }
    }
    
    
}
