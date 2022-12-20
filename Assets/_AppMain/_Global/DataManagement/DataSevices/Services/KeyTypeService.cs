using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Databases
{
    public class KeyTypeService : DataService
    {
        public static readonly string TableName = "KeyTypeObject";
        public static readonly string PkColumn = "uniqueKey";


        public static CardFindQuery Find(string key)
        {
            KeyTypeDTO dto = ByKey<KeyTypeDTO>(TableName, PkColumn, key);
            if (dto != null)
            {
                CardFindQuery q = JsonUtility.FromJson<CardFindQuery>(dto.value);
                return q;
            }
            return null;
        }
    }
}

