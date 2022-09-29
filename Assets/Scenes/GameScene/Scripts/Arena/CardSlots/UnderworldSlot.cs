using System.Collections;
using System.Collections.Generic;
using Gameplay.Menus;
using Gameplay.Menus.Popup;
using UnityEngine;
using static Gameplay.Menus.CardBrowseMenu;

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

        protected override void DisplayCardObject(GameCard card)
        {
            CardView c = card.cardObject;
            c.SetAsChild(transform, CardScale, SortLayer);
            card.rect.sizeDelta = rect.sizeDelta;
            c.transform.localPosition = new Vector2(0f, 0f);
            c.SetSortingOrder(cards.Count);
            c.Flip();
            c.Rotate(false);
            card.SetCardMode(CardMode.None);
        }
        public override bool ValidateCard(GameCard card)
        {
            return true;
        }

        #region Slot Commands

        protected override List<PopupCommand> GetSlotCommands()
        {
            List<PopupCommand> commands = new List<PopupCommand>();
            commands.Add(PopupCommand.Create("Browse", () => BrowseCommand()));
            commands.Add(PopupCommand.Create("Manage", () => ManageCommand()));
            if (Owner.userId != App.WhoAmI) { commands.Add(PopupCommand.Create("Close", () => CloseCommand())); return commands; }

            commands.Add(PopupCommand.Create("Close", () => CloseCommand()));



            return commands;
        }
        public void BrowseCommand()
        {
            GameManager.Instance.browseMenu.LoadCards(cards, "Your Underworld", true);
            ClosePopMenu();
        }

        public void ManageCommand()
        {
            GameManager.Instance.browseMenu.LoadCards(cards, "Select Cards to Move", true, 1, Owner.gameField.UnderworldSlot.cards.Count);
            GameManager.Instance.browseMenu.OnClosed += AwaitManage;
            ClosePopMenu();
        }
        protected void AwaitManage(BrowseArgs args)
        {
            GameManager.Instance.browseMenu.OnClosed -= AwaitManage;
            if (!args.IsConfirm || args.Selections.Count <= 0) { return; }
            LocationMenu.Load(Owner, args.Selections);
        }

        
        #endregion
    }
}

