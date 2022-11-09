using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleSQL;

namespace Databases
{
    [System.Serializable]
    public class ElementDTO 
    {
        [PrimaryKey]
        public int typeKey { get; set; }
        public string typeName { get; set; }
        public string keystrokeCode { get; set; }
        public string unicode { get; set; }
        public string colorCode { get; set; }

    }
}

