using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleSQL;

namespace Databases
{
    [System.Serializable]
    public class EffectDTO
    {
        public string cardKey { get; set; }
        public string abiKey { get; set; }
        public string castCost { get; set; }
        public int effOrder { get; set; }
        public string triggerKey { get; set; }
          
        public int autoUse { get; set; }

    }
}

