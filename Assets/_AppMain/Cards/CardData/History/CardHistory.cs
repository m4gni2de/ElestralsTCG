using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Decks
{
    [System.Serializable]
    public class CardHistory : iArchive
    {
        public string cardKey { get; set; }
        public int oldQty { get; set; }
        public int qty { get; set; }

        public string Print()
        {
            return $"Card '{cardKey}' had Quantity changed from {oldQty} to {qty}.";
        }
        
        
        public CardHistory(string card, int oldQty, int newQty)
        {
            cardKey = card;
            this.oldQty = oldQty;
            qty = newQty;
        }
    }
}

