using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{


    public class DeckSlot : CardSlot
    {

        public TouchObject touch;
        protected override void SetSlot()
        {
            facing = CardFacing.FaceDown;
            orientation = Orientation.Vertical;
            slotType = CardLocation.Deck;
            //touch.OnClickEvent.AddListener(() => DrawCard());
            touch.OnClickEvent.AddListener(() => BrowseCards());
        }

       

        public override void AllocateTo(GameCard card)
        {
            card.RemoveFromSlot();
            cards.Add(card);
            TouchObject to = card.cardObject.touch;
            to.ClearClick();
            to.ClearHold();
            card.SetSlot(index);
            card.AllocateTo(slotType);


            DisplayCardObject(card);
            SetCommands(card);

        }

        protected override void SetCommands(GameCard card)
        {
            TouchObject to = card.cardObject.touch;
            to.ClearClick();
            to.ClearHold();
        }

        protected override void DisplayCardObject(GameCard card)
        {
            
            base.DisplayCardObject(card);
        }

        public override bool ValidateCard(GameCard card)
        {
            if (card.card.CardType == CardType.Spirit) { return false; }
            return true;
        }



        public void DrawCard()
        {
            GameManager.ActiveGame.You.Draw(1);
        }

        public void BrowseCards()
        {
           if (!ValidatePlayer()) { return; }

            GameManager.Instance.browseMenu.LoadCards(cards, true);
        }

        
    }
}
