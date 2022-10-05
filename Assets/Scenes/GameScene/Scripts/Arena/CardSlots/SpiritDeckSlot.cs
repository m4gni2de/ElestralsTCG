using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class SpiritDeckSlot : DeckSlot
    {
        
        protected override void SetSlot()
        {
            base.SetSlot();
            facing = CardFacing.FaceDown;
            orientation = Orientation.Vertical;
            slotType = CardLocation.SpiritDeck;
        }

       

        public override void AllocateTo(GameCard card, bool sendToServer = true)
        {
            card.RemoveFromSlot();
            cards.Insert(0, card);
            card.AllocateTo(this, sendToServer);

            DisplayCardObject(card);
            SetCommands(card);
        }

        protected override void SetCommands(GameCard card)
        {
            TouchObject to = card.cardObject.touch;
            to.ClearClick();
            to.ClearHold();
        }

        public override bool ValidateCard(GameCard card)
        {
            if (card.CardType == CardType.Spirit) { return true; }
            return false;
        }
    }
}

