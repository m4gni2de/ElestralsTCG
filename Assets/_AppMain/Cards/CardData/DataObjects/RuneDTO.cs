using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleSQL;

namespace Databases
{

    [System.Serializable]
    public class RuneDTO
    {
        [PrimaryKey]
        public string runeKey { get; set; }
        public string runeTitle { get; set; }
        public int runeType { get; set; }

    }

    [System.Serializable]
    public class vRunes
    {
        [PrimaryKey]
        public string runeKey { get; set; }
        public string rune{ get; set; }
        public int cost1 { get; set; }
        public int? cost2 { get; set; }
        public int? cost3 { get; set; }
        public string effect { get; set; }
        public int runeType { get; set; }
        public int rarity { get; set; }
        public string setName { get; set; }
        public int setNumber { get; set; }
        public string artist { get; set; }
    }
}

