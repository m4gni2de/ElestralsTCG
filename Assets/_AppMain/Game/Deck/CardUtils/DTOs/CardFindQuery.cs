using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleSQL;

namespace Databases
{
    [System.Serializable]
    public class CardFindDTO
    {
        [PrimaryKey]
        public string queryKey { get; set; }
        public int playerScope { get; set; }
        public string locations { get; set; }
        public string cardTypes { get; set; }
        public string cardElements { get; set; }
        public string enchantedElements { get; set; }
        public string costs { get; set; }
        public string withName { get; set; }

    }


    [System.Serializable]
    public class CardFindQuery
    {
        public static readonly string TableName = "CardFind";
        public static readonly string PkColumn = "queryKey";

        public string queryKey;
        public int playerScope;
        public List<int> locations;
        public List<int> cardTypes;
        public List<int> cardElements;
        public List<int> enchantedElements;
        public List<int> costs;
        public string withName;

        public CardFindQuery(CardFindDTO dto)
        {
            queryKey = dto.queryKey;
            playerScope = dto.playerScope;

            if (!dto.locations.IsEmpty()) { locations = dto.locations.AsList(",").StringToInt(); } else { locations = new List<int>(); }
            if (!dto.cardTypes.IsEmpty()) { cardTypes = dto.cardTypes.AsList(",").StringToInt(); } else { cardTypes = new List<int>(); }
            if (!dto.cardElements.IsEmpty()) { cardElements = dto.cardElements.AsList(",").StringToInt(); } else { cardElements = new List<int>(); }
            if (!dto.enchantedElements.IsEmpty()) { enchantedElements = dto.enchantedElements.AsList(",").StringToInt(); } else { enchantedElements = new List<int>(); }
            if (!dto.costs.IsEmpty()) { costs = dto.costs.AsList(",").StringToInt(); } else { costs = new List<int>(); }
            withName = dto.withName;

        }

        public static CardFindQuery All
        {
            get
            {
                CardFindDTO dto = new CardFindDTO();
                dto.queryKey = "";
                dto.playerScope = 100;
                dto.locations = "";
                dto.cardTypes = "";
                dto.cardElements = "";
                dto.enchantedElements = "";
                dto.costs = "";
                dto.withName = "";
                return new CardFindQuery(dto);
            }
        }

        public static CardFindQuery Lookup(string key)
        {
            CardFindDTO dto = DataService.ByPk<CardFindDTO>(TableName, key);
            if (dto != null)
            {
                return new CardFindQuery(dto);
                
            }
            return null;
        }
    }

   
}

