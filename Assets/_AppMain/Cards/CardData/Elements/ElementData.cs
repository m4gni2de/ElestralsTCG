using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Databases;
using Gameplay.Turns;
using GlobalUtilities;

namespace Elements
{


    [System.Serializable]
    public struct ElementData : iCustomFont
    {

        #region Operators
        public static implicit operator ElementData(string keyCode)
        {
            List<ElementData> elements = ElementService.ElementsList;
            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].Keystroke == keyCode)
                {
                    return elements[i];
                }

                string formatted = $"<{elements[i].Keystroke}>";
                if (formatted == keyCode.Trim())
                {
                    return elements[i];
                }
            }
            return Empty;
        }
        public static implicit operator ElementData(ElementCode code)
        {
            List<ElementData> elements = ElementService.ElementsList;
            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].Code == code)
                {
                    return elements[i];
                }
            }
            return Empty;
        }
        #endregion

        #region Interface
        public string EncodedText { get => Keystroke; }

        public string UnicodeString { get => @"\" + Unicode; }

        public string UnicodeWithColor
        {
            get
            {
                
                string hexString = $" <#{ColorCode.ToUpper()}>";
                string closer = $"</color>";
                string fullString = $"{hexString}{UnicodeString}{closer} ";
                return fullString;
            }
        }
        #endregion

        public ElementCode Code;
        public string Keystroke;
        public string Name;
        public string Unicode;
        public string ColorCode;

        public ElementData(ElementDTO dto)
        {
            Code = (ElementCode)dto.typeKey;
            Keystroke = dto.keystrokeCode;
            Name = dto.typeName;
            Unicode = dto.unicode;
            ColorCode = dto.colorCode;

        }

        public static ElementData Empty
        {
            get
            {
                ElementDTO dto = ElementService.GetElement(-1);
                return new ElementData(dto);
            }

        }

        public bool IsNull
        {
            get
            {
                bool isNull = string.IsNullOrEmpty(Keystroke) || string.IsNullOrEmpty(Name);
                return isNull;
            }
        }

        public static ElementData GetData(int code)
        {
            if (code < 0)
            {
                code = -1;
            }

            for (int i = 0; i < ElementService.ElementsList.Count; i++)
            {
                ElementData data = ElementService.ElementsList[i];
                if (data.Code == (ElementCode)code)
                {
                    return data;
                }
            }

            return Empty;
        }

        public static iCustomFont ElementGlyph(ElementCode code)
        {
            ElementData data = GetData((int)code);
            return data;
            
        }

        //public string FormattedUnicodeColor
        //{
        //    get
        //    {
        //        string unicode = CustomFont.UnicodeElement(this);
        //        string hexColor = $" <#{ColorCode.ToUpper()}>";
        //        string closer = $"</color>";
        //        string fullString = $"{hexColor}{unicode}{closer} ";
        //        return fullString;
        //    }
        //}
        //public string FormattedUnicode
        //{
        //    get
        //    {

        //         return CustomFont.UnicodeElement(this);

        //    }
        //}


    }

   
}

public enum ElementCode
{
    None = -1,
    Any = 0,
    Wind = 1,
    Dark = 2,
    Earth = 3,
    Fire = 4,
    Frost = 5,
    Light = 6,
    Thunder = 7,
    Water = 8
}
