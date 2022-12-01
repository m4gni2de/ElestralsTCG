using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleSQL;
using System;

namespace Databases
{
    [System.Serializable]
    public class DeckDTO
    {
        [PrimaryKey]
        public string deckKey { get; set; }
        public string title { get; set; }
        public string owner { get; set; }
        public DateTime whenCreated { get; set; }
        public string uploadCode { get; set; }
        public string sDeckKey { get; set; }
    }
}


