using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Elements;
using Databases;

public class Element
{
    #region Operators
    public static bool operator ==(Element a, Element b)
    {
        return a.Code == b.Code;
    }
    public static bool operator !=(Element a, Element b)
    {
        return a.Code != b.Code;
    }

    public override bool Equals(object obj)
    {
        if (obj is Element || obj is ElementCode || obj is ElementData)
        {
            if (obj is Element) { return this == (Element)obj; }
            if (obj is ElementCode)
            {
                ElementCode code = (ElementCode)obj;
                return Code == code;
            }
            if (obj is ElementData)
            {
                ElementData data = (ElementData)obj;
                return Code == data.Code;
            }
        }
        return false;

    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return Name;
    }
    #endregion

    #region Static Functions
    public static Color FireColor { get => CardUI.FromHex(HexCodeFire); }
    public static Color WaterColor { get => CardUI.FromHex(HexCodeWater);}
    public static Color WindColor { get => CardUI.FromHex(HexCodeWind); }
    public static Color EarthColor { get => CardUI.FromHex(HexCodeEarth); }
    public static Color ThunderColor { get => CardUI.FromHex(HexCodeThunder); }

    public static string HexCodeFire { get => "#F15B38"; }
    public static string HexCodeWater { get => "#00AEFF"; }
    public static string HexCodeWind { get => "#E1F2F9"; }
    public static string HexCodeEarth { get => "#CDFF00"; }
    public static string HexCodeThunder { get => "#FFC16D"; }

    
    #endregion

    #region Properties & Functions
    private ElementData _baseData;
    public ElementData BaseData
    {
        get
        {
            return _baseData;
        }
    }

    public ElementCode Code { get { return BaseData.Code; } }
    public string Name { get { return BaseData.Name; } }
    
   

    public string SpriteName
    {
        get
        {
            string st = $"{BaseData.Name}Symbol";
            return st;
        }
    }

    
    #endregion

    public Element(int code)
    {
        _baseData = ElementData.GetData(code);
    }

    public static int ElementInt(string elementName)
    {
        List<string> names = Enums.GetNames(typeof(ElementCode));

        for (int i = 0; i < names.Count; i++)
        {
            if (elementName.ToLower() == names[i].ToLower())
            {
                ElementCode c = (ElementCode)System.Enum.Parse(typeof(ElementCode), elementName, true);
                return (int)c;
            }
        }
        return -1;
    }
   


    public Color ElementColor()
    {
        return CardUI.FromHex(BaseData.ColorCode);
    }
    public static Color TextColor(ElementCode el)
    {
        ElementData data = el;
        return CardUI.FromHex(data.ColorCode);
    }
    public Material FontMaterial(string suffix)
    {
        return CardFactory.CardFontMaterial(BaseData.Code, suffix);
    }

}
