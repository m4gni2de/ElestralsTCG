using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class StadiumSlot : RuneSlot
    {
        
        protected override void SetSlot()
        {
            facing = CardFacing.Both;
            orientation = Orientation.Vertical;
            slotType = CardLocation.Stadium;
        }


        public override bool ValidateCard(GameCard card)
        {
            if (card.card.CardType == CardType.Rune)
            {
                Rune rune = (Rune)card.card;
                if (rune.GetRuneType == Rune.RuneType.Stadium)
                {
                    return true;
                }
                return false;
            }
            if (card.card.CardType == CardType.Spirit)
            {
                if (cards.Count > 0 && cards[0].card.CardType == CardType.Rune) { return true; }
            }
            return false;
        }


        protected override void ClickCard(GameCard card)
        {
            base.ClickCard(card);
        }
    }
}

