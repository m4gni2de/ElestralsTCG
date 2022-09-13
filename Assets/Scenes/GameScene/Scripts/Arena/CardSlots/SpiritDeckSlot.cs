using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class SpiritDeckSlot : DeckSlot
    {
        
        protected override void SetSlot()
        {
            facing = CardFacing.FaceDown;
            orientation = Orientation.Vertical;
            slotType = CardLocation.SpiritDeck;
            touch.OnClickEvent.AddListener(() => OpenPopMenu());
            touch.AddClickListener(() => OpenPopMenu());
        }

        public override void AllocateTo(GameCard card)
        {
            card.RemoveFromSlot();
            cards.Insert(0, card);
            card.AllocateTo(this);

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

