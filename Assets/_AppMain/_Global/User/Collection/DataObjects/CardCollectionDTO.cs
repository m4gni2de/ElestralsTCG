using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleSQL;
using System;

namespace Cards.Collection
{
    public class CardCollectionDTO
    {
        [PrimaryKey]
        public string setKey { get; set; }
        public int rarity { get; set; }
        public int qty { get; set; }
        public DateTime? colWhen { get; set; }
    }
}

