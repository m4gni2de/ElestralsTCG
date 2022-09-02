using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameplay.Menus;
using UnityEngine.Events;

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
            touch.OnClickEvent.AddListener(() => OpenSlotMenu());
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



        

        public override void OpenSlotMenu()
        {
            base.OpenSlotMenu();
            
        }
        protected override Dictionary<string, UnityAction> GetButtonCommands()
        {
            Dictionary<string, UnityAction> commands = new Dictionary<string, UnityAction>();
            commands.Add("Draw", () => DrawCommand());
            commands.Add("Browse", () => BrowseCommand());
            commands.Add("Mill", () => MillCommand());
            commands.Add("Close", () => CloseCommand());
            return commands;
            
        }
        
        #region Menu Commands
       
        protected void DrawCommand()
        {
            GameManager.Instance.popupMenu.InputNumber("How many cards do you want to Draw?", Owner.Draw);
        }
        protected void MillCommand()
        {
            GameManager.Instance.popupMenu.InputNumber("How many cards do you want to Mill?", Owner.Mill);
        }
        protected void BrowseCommand()
        {
            GameManager.Instance.browseMenu.LoadCards(cards, true);
        }
        protected void CloseCommand()
        {
            CloseSlotMenu();
        }
        #endregion


    }
}
