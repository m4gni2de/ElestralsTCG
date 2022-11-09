using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalUtilities
{
    public class CustomGlyph : iCustomFont
    {
        #region Static Functions
        public static CustomGlyph Stellar
        {
            get
            {
                return FormattedText.CustomChars["<S>"];
            }
        }
        #endregion

        #region Properties
        public string PlainText ;
        private string _encodedString;

        public string title;
        private string _unicodeString;
        public string hexColor;
        #endregion



        public string EncodedText { get => _encodedString; }
        public string UnicodeString
        {
            get
            {
                return @"\"+_unicodeString;
            }
        }

        public string UnicodeWithColor
        {
            get
            {
                string hexString = $" <#{hexColor.ToUpper()}>";
                string closer = $"</color>";
                string fullString = $"{hexString}{UnicodeString}{closer} ";
                return fullString;
            }
        }


        public CustomGlyph(zCustomText dto)
        {
            PlainText = dto.charKey;
            _encodedString = dto.encodedVal;
            title = dto.title;
            _unicodeString = dto.unicode;
            hexColor = dto.colorHex;
        }
    }
}

