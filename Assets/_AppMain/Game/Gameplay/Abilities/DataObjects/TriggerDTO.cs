using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleSQL;

namespace Database
{
    [System.Serializable]
    public class TriggerDTO
    {
        public string triKey { get; set; }
        public int activation { get; set; }
        public int result { get; set; }
        public int timing { get; set; }
        public int isLocal { get; set; }
        public string triggerArgs { get; set; }
    }
}

