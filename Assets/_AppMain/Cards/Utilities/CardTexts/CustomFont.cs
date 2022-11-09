using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using System;
using Databases;
using Elements;
using Gameplay.Data;
using GlobalUtilities;

public static class CustomFont 
{

    private static Dictionary<string, CustomGlyph> _customChars = null;
    public static Dictionary<string, CustomGlyph> CustomChars
    {
        get
        {
            if (_customChars == null)
            {
                _customChars = new Dictionary<string, CustomGlyph>();
                List<zCustomText> dtos = DataService.GetAll<zCustomText>("zCustomText");

                for (int i = 0; i < dtos.Count; i++)
                {
                    CustomGlyph c = new CustomGlyph(dtos[i]);
                    _customChars.Add(c.EncodedText, c);
                }
            }
            return _customChars;
        }
    }

    private static Regex m_RegexExpression = new Regex(@"(?<!\\)(?:\\u[0-9a-fA-F]{4}|\\U[0-9a-fA-F]{8})");

    public static void FormatTextBox(this TMP_Text textLabel, string stringWithUnicodeChars)
    {
        string plainText = stringWithUnicodeChars;

        stringWithUnicodeChars = m_RegexExpression.Replace(stringWithUnicodeChars,
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

        textLabel.text = stringWithUnicodeChars;

    }


    public static readonly string NullSymbol = "NULL";
    public static readonly string WindSymbol = @"\u0080";
    public static readonly string DarkSymbol = @"\u0081";
    public static readonly string EarthSymbol = @"\u0082";
    public static readonly string FireSymbol = @"\u0083";
    public static readonly string FrostSymbol = @"\u0084";
    public static readonly string LightSymbol = @"\u0085";
    public static readonly string ThunderSymbol = @"\u0086";
    public static readonly string WaterSymbol = @"\u0087";
    public static readonly string StellarSymbol = @"\u0088";
    public static readonly string AnySymbol = @"\u0089";

    

    //private static List<string> GetUnicodeKeys()
    //{
    //    List<string> list = new List<string>();

    //    List<ElementData> elements = ElementService.ElementsList;
    //    for (int i = 0; i < elements.Count; i++)
    //    {
    //        string keystroke = $"<{elements[i].Keystroke}>";
    //        list.Add(keystroke);
    //    }
    //    list.Add("<S>");
    //    list.Add("<X>");
    //    return list;

    //}

   
    //private static Dictionary<string, string> _unicodes = null;
    //private static Dictionary<string, string> Unicodes
    //{
    //    get
    //    {
    //        if (_unicodes == null)
    //        {
    //            _unicodes = new Dictionary<string, string>();

    //            List<string> codes = GetUnicodeKeys();
               
    //            for (int i = 0; i < codes.Count; i++)
    //            {
    //                if (!_unicodes.ContainsKey(codes[i]))
    //                {
                       
    //                    string symbol = GetUnicode(codes[i]);

                       
    //                    _unicodes.Add(codes[i], symbol);
    //                }
                    
    //            }
    //        }
    //        return _unicodes;
    //    }
    //}

    //public static void FormatTextBox(string stringWithUnicodeChars, TMP_Text textLabel)
    //{
    //    string plainText = stringWithUnicodeChars;

    //    stringWithUnicodeChars = m_RegexExpression.Replace(stringWithUnicodeChars,
    //        match =>
    //        {
    //            if (match.Value.StartsWith("\\U"))
    //            {
    //                string st = char.ConvertFromUtf32(int.Parse(match.Value.Replace("\\U", " "), NumberStyles.HexNumber));
    //                return st;
    //            }

    //            string stl = char.ConvertFromUtf32(int.Parse(match.Value.Replace("\\u", " "), NumberStyles.HexNumber));
    //            return stl;
    //        });

    //    textLabel.text = stringWithUnicodeChars;
        
    //}

   

   
    //public static void FormatEffect(string effect, TMP_Text textLabel, bool colorSpirits)
    //{
    //    string newEffect = effect;

    //    foreach (var item in Unicodes)
    //    {

    //        if (newEffect.Contains(item.Key))
    //        {
    //            string newUnicode = $" {item.Value} ";
    //            Debug.Log(effect);

    //            ElementData d = item.Key;
                
    //            string st = d.FormattedUnicode;
    //            if (colorSpirits)
    //            {
    //                st = d.FormattedUnicodeColor;
    //            }
    //            newEffect = newEffect.Replace(item.Key, st);


    //        }
    //    }

    //    FormatTextBox(newEffect, textLabel);
    //}



    //private static string GetUnicode(string keyStroke)
    //{
    //    switch (keyStroke)
    //    {
    //        case "<T>":
    //        return ThunderSymbol;
    //        case "<Wi>":
    //            return WindSymbol;
    //        case "<E>":
    //            return EarthSymbol;
    //        case "<Wa>":
    //            return WaterSymbol;
    //        case "<Fi>":
    //            return FireSymbol;
    //        case "<Fr>":
    //            return FrostSymbol;
    //        case "<D>":
    //            return DarkSymbol;
    //        case "<S>":
    //            return StellarSymbol;
    //        case "<X>":
    //            return AnySymbol;
    //        default:
    //            return AnySymbol;
    //    }
    //}
    
   
    //public static string UnicodeElement(ElementData data)
    //{
    //    string code = GetUnicode($"<{data.Keystroke}>");
    //    return $" {code} ";
    //}

    
}
