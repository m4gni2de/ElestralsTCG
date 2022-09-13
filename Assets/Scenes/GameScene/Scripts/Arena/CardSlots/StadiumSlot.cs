using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class StadiumSlot : RuneSlot
    {
        #region Interface
        
        protected override void SetMainCard(GameCard card)
        {
            if (card.CardType == CardType.Rune)
            {
                Rune rune = (Rune)card.card;
                if (rune.GetRuneType == Rune.RuneType.Stadium)
                {
                    AddMainCard(card);
                }
            }
        }
        #endregion
        protected override void SetSlot()
        {
            facing = CardFacing.Both;
            orientation = Orientation.Vertical;
            slotType = CardLocation.Stadium;
        }


        public override bool ValidateCard(GameCard card)
        {
            if (card.CardType == CardType.Rune)
            {
                Rune rune = (Rune)card.card;
                if (rune.GetRuneType == Rune.RuneType.Stadium)
                {
                    return true;
                }
                return false;
            }
            if (card.CardType == CardType.Spirit)
            {
                if (MainCard != null) { return true; }
            }
            return false;
        }


        protected override void ClickCard(GameCard card)
        {
            base.ClickCard(card);
        }
    }
}

