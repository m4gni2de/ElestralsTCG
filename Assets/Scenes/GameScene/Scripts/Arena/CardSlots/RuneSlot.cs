using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Gameplay
{


    public class RuneSlot : CardSlot
    {
        
        protected override void SetSlot()
        {
            facing = CardFacing.Both;
            orientation = Orientation.Vertical;
            slotType = CardLocation.Rune;
        }

        public override void AllocateTo(GameCard card)
        {
            card.RemoveFromSlot();
            cards.Add(card);
            card.SetSlot(index);
            card.AllocateTo(slotType);

            if (card.location != CardLocation.Rune)
            {
                TouchObject to = card.cardObject.touch;
                to.OnClickEvent.AddListener(() => ClickCard(card));
                to.OnHoldEvent.AddListener(() => GameManager.Instance.DragCard(card, this));
            }

            if (card.card.CardType == CardType.Spirit)
            {
                DisplayCardObject(card, CardFacing.FaceUp, Orientation.Horizontal);
            }
            else
            {
                DisplayCardObject(card, CardFacing.FaceUp, Orientation.Vertical);
                SetCommands(card);
            }
        }



        protected virtual void DisplayCardObject(GameCard card, CardFacing cardFacing, Orientation orient)
        {
            base.DisplayCardObject(card);
            card.cardObject.Flip(facing == CardFacing.FaceDown);
            card.cardObject.Rotate(orient == Orientation.Horizontal);

        }

        public override bool ValidateCard(GameCard card)
        {
            if (card.card.CardType == CardType.Rune)
            {
                Rune rune = (Rune)card.card;
                if (rune.GetRuneType == Rune.RuneType.Stadium)
                {
                    return false;
                }
                return true;
            }
            if (card.card.CardType == CardType.Spirit)
            {
                if (cards.Count > 0 && cards[0].card.CardType == CardType.Rune) { return true; }
            }
            return false;
        }

        protected override void ClickCard(GameCard card)
        {

        }
    }
}
