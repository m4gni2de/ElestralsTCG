using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class UnderworldSlot : CardSlot
    {
        private TouchObject _touch = null;
        public TouchObject touch
        {
            get
            {
                if (_touch == null)
                {
                    if (GetComponent<TouchObject>() != null)
                    {
                        _touch = GetComponent<TouchObject>();
                    }
                    else
                    {
                        _touch = gameObject.AddComponent<TouchObject>();   
                    }
                }
                return _touch;
            }
        }
        protected override void SetSlot()
        {
            facing = CardFacing.FaceUp;
            orientation = Orientation.Vertical;
            slotType = CardLocation.Underworld;
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

            GameManager.Instance.browseMenu.LoadCards(cards, true);
        }
    }
}

