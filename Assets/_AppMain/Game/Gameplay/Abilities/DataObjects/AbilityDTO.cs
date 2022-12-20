using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleSQL;

namespace Databases
{
    [System.Serializable]
    public class AbilityDTO
    {
        [PrimaryKey]
        public string abiKey { get; set; }
        public string abiName { get; set; }
        public int abiType { get; set; }
        public string abiArgs { get; set; }
    }
}

