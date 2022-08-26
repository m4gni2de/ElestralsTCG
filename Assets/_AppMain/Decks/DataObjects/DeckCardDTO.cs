using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleSQL;

namespace Databases
{
    [System.Serializable]
    public class DeckCardDTO
    {
       
        public string deckKey { get; set; }
        public string cardKey { get; set; }
        public int qty { get; set; }
    }

    [System.Serializable]
    public class qDeckList
    {

        public string deckKey { get; set; }
        public string cardKey { get; set; }
        public int qty { get; set; }
        public int cardClass { get; set; }
    }
}

