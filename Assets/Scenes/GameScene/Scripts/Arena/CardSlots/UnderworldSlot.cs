using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class UnderworldSlot : CardSlot, iSelectSlot
    {
        private TouchObject _touch = null;
        public TouchObject touch { get { _touch ??= GetComponent<TouchObject>(); return _touch; } }

        protected override void SetSlot()
        {
            
            facing = CardFacing.FaceUp;
            orientation = Orientation.Vertical;
            slotType = CardLocation.Underworld;
            touch.ClearAll();
            touch.AddClickListener(() => ClickSlot());
        }

        protected override void SetCommands(GameCard card)
        {
            TouchObject to = card.cardObject.touch;
            to.ClearClick();
            to.ClearHold();
        }
        public override bool ValidateCard(GameCard card)
        {
            return true;
        }



        public void BrowseCards()
        {

            GameManager.Instance.browseMenu.LoadCards(cards, "Your Underworld", true);
        }
    }
}

