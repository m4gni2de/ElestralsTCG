using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Databases;

namespace Databases
{
    [System.Serializable]
    public class KeyTypeDTO
    {
        public string uniqueKey { get; set; }
        public string type { get; set; }
        public string value { get; set; }
    }
}

