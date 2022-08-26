using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Databases;

namespace Elements
{


    [System.Serializable]
    public struct ElementData
    {
        public ElementCode Code;
        public string Keystroke;
        public string Name;
        public string Unicode;

        public ElementData(ElementDTO dto)
        {
            Code = (ElementCode)dto.typeKey;
            Keystroke = dto.keystrokeCode;
            Name = dto.typeName;
            Unicode = dto.unicode;

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
