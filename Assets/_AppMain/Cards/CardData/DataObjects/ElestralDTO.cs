using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleSQL;

namespace Databases
{
    [System.Serializable]
    public class ElestralDTO
    {
        [PrimaryKey]
        public string eleKey { get; set; }
        public string species { get; set; }
        public int attack { get; set; }
        public int defense { get; set; }
        public int subClass1 { get; set; }
        public int subClass2 { get; set; }
    }

    [System.Serializable]
    public class qElestralsBase
    {
        public string baseKey { get; set; }
        public string species { get; set; }
        public int cost1 { get; set; }
        public int? cost2 { get; set; }
        public int? cost3 { get; set; }
        public int attack { get; set; }
        public int defense { get; set; }
        public int sClass1 { get; set; }
        public int sClass2 { get; set; }
        public string effect { get; set; }
        public string setName { get; set; }
        public int artType { get; set; }
        public string imageFile { get; set; }
        public int rarity { get; set; }
    }

    [System.Serializable]
    public class qAltArts
    {
        public string baseKey { get; set; }
        public string altKey { get; set; }
        public string title { get; set; }
        public int rarity { get; set; }
        public int artType { get; set; }
        public string altImageFile { get; set; }
        
    }

    [System.Serializable]
    public class qStellarRares
    {
        public string baseKey { get; set; }
        public string stellarKey { get; set; }
        public string species { get; set; }
        public string setName { get; set; }
        public int artType { get; set; }
        public string image { get; set; }

    }
}

