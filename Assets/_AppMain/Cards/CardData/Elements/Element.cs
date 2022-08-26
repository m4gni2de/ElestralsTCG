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

   
}
