using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleSQL;

namespace Databases
{
   
    [System.Serializable]
    public class CardActionDTO
    {
        public string actionKey { get; set; }
        public string actionValue { get; set; }
    }
}

