using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class ElestralSlot : CardSlot
    {
        
        protected override void SetSlot()
        {
            facing = CardFacing.FaceUp;
            orientation = Orientation.Both;
            slotType = CardLocation.Elestral;
        }

        public override void AllocateTo(GameCard card)
        {

            card.RemoveFromSlot();
            cards.Add(card);
            card.SetSlot(index);
            card.AllocateTo(slotType);
            TouchObject to = card.cardObject.touch;
            to.ClearClick();
            to.ClearHold();

            if (card.card.CardType == CardType.Spirit)
            {
                DisplayCardObject(card, Orientation.Horizontal);
            }
            else
            {
                DisplayCardObject(card, Orientation.Vertical);
                SetCommands(card);
            }

        }

        

        protected virtual void DisplayCardObject(GameCard card, Orientation orient = Orientation.Vertical)
        {
            CardObject c = card.cardObject;
            c.transform.SetParent(transform);
            card.rect.sizeDelta = rect.sizeDelta;
            if (!atSlot.Contains(card.cardObject))
            {
                atSlot.Add(c);
            }
            c.transform.localPosition = new Vector2(0f, 0f);
            c.SetScale(CardScale);


            card.cardObject.Rotate(orient == Orientation.Horizontal);

        }

        protected override void SetCommands(GameCard card)
        {
            base.SetCommands(card);
        }
        protected override void ClickCard(GameCard card)
        {
            base.ClickCard(card);
        }

        public override bool ValidateCard(GameCard card)
        {
            if (card.card.CardType == CardType.Elestral) { return true; }
            if (card.card.CardType == CardType.Spirit)
            {
                if (cards.Count > 0 && cards[0].card.CardType == CardType.Elestral) { return true; }
            }
            return false;
        }
    }
}

