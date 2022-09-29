using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameplay.Menus;
using UnityEngine.Events;
using Gameplay.Menus.Popup;

namespace Gameplay
{


    public class DeckSlot : CardSlot, iSelectSlot
    {
        private TouchObject _touch = null;
        public TouchObject touch { get { _touch ??= GetComponent<TouchObject>(); return _touch; } }
        protected override void SetSlot()
        {
            
            facing = CardFacing.FaceDown;
            orientation = Orientation.Vertical;
            slotType = CardLocation.Deck;
            touch.ClearAll();
            touch.AddClickListener(() => ClickSlot());
        }

       

        public override void AllocateTo(GameCard card)
        {
            base.AllocateTo(card);
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
            if (card.CardType == CardType.Spirit) { return false; }
            return true;
        }


       


        public override void OpenPopMenu()
        {
            base.OpenPopMenu();
            
        }
        protected override List<PopupCommand> GetSlotCommands()
        {
            List<PopupCommand> commands = new List<PopupCommand>();
            commands.Add(PopupCommand.Create("Draw", () => DrawCommand(), 0, 0));
            commands.Add(PopupCommand.Create("Browse", () => BrowseCommand(), 0, 1));
            commands.Add(PopupCommand.Create("Mill", () => MillCommand(), 0, 2));
            commands.Add(PopupCommand.Create("Shuffle", () => ShuffleCommand(), 0, 2));
            commands.Add(PopupCommand.Create("Close", () => CloseCommand(), 0, 5));
            return commands;
        }
        
        #region Menu Commands
       
        protected void DrawCommand()
        {
            GameManager.Instance.popupMenu.InputNumber("How many cards do you want to Draw?", Owner.Draw, 0, Owner.deck.MainDeck.InOrder.Count);
        }
        protected void MillCommand()
        {
            GameManager.Instance.popupMenu.InputNumber("How many cards do you want to Mill?", Owner.Mill, 0, Owner.deck.MainDeck.InOrder.Count);
        }
        protected void BrowseCommand()
        {
            GameManager.Instance.browseMenu.LoadCards(cards, "Browse Deck", true);
        }
        protected void ShuffleCommand()
        {
            GameManager.Instance.popupMenu.ConfirmAction("Do you want to start the Battle Phase?", ConfirmShuffle);
        }

        protected void ConfirmShuffle(bool doShuffle)
        {
            if (doShuffle)
            {
                Owner.Shuffle();
            }
            else
            {
                GameManager.Instance.popupMenu.ShowMenu();
            }
        }
        
        #endregion


    }
}
