using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;

namespace Gameplay
{
    [System.Serializable]
    public class SpiritCard : GameCard
    {
        public ElementCode originalType { get; set; }
        public List<ElementCode> CurrentTypes { get; set; }
        public SpiritCard(Spirit card, int copy) : base(card, copy)
        {
            CurrentTypes = new List<ElementCode>();
            originalType = (ElementCode)card.cardData.cost1;
            CurrentTypes.Add(originalType);
        }
    }
}

