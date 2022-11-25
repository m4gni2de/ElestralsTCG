using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleSQL;

namespace Databases
{
    [System.Serializable]
    public class CardDTO
    {
        [PrimaryKey]
        public string cardKey { get; set; }
        public string title { get; set; }
        public int cardClass { get; set; }
        public int cost1 { get; set; }
        public int? cost2 { get; set; }
        public int? cost3 { get; set; }
        public string effect { get; set; }
        public int? attack { get; set; }
        public int? defense { get; set; }
        public string artist { get; set; }
        public int? subType1 { get; set; }
        public int? subType2 { get; set; }
        public string image { get; set; }
    }

    public class qBaseCard
    {
        [PrimaryKey]
        public string cardKey { get; set; }
        public string title { get; set; }
        public int cardClass { get; set; }
        public int cost1 { get; set; }
        public int? cost2 { get; set; }
        public int? cost3 { get; set; }
        public string effect { get; set; }
        public int? attack { get; set; }
        public int? defense { get; set; }
        public string artist { get; set; }
        public int? subType1 { get; set; }
        public int? subType2 { get; set; }
        public string image { get; set; }
        public string setKey { get; set; }
        public string setName { get; set; }
        public int setNumber { get; set; }
        public int rarity { get; set; }
        public int artType { get; set; }


    }

    public class CardBySet
    {
        [PrimaryKey]
        public string setKey { get; set; }
        public string cardKey { get; set; }
        public string setName { get; set; }
        public int setNumber { get; set; }
        public int rarity { get; set; }
        public int artType { get; set; }
        public string image { get; set; }
    }

    public class qUniqueCard
    {
        [PrimaryKey]
        public string setKey { get; set; }
        public string title { get; set; }
        public int cardClass { get; set; }
        public int cost1 { get; set; }
        public int? cost2 { get; set; }
        public int? cost3 { get; set; }
        public string effect { get; set; }
        public int? attack { get; set; }
        public int? defense { get; set; }
        public string artist { get; set; }
        public int? subType1 { get; set; }
        public int? subType2 { get; set; }
        public string image { get; set; }
        public string setName { get; set; }
        public int setNumber { get; set; }
        public int rarity { get; set; }
        public int artType { get; set; }
        public string baseKey { get; set; }


    }

    [System.Serializable]

    public class qCardArt
    {
        [PrimaryKey]
        public string cardKey { get; set; }
        public string image { get; set; }
    }

   
}


