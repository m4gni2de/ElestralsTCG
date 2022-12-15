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

        #region Interface
        public void Optimize()
        {
            for (int i = 0; i < cards.Count; i++)
            {
                if (i < cards.Count - 1)
                {
                    cards[i].cardObject.Hide();
                }
                else
                {
                    cards[i].cardObject.Show();
                }
            }
        }
        #endregion

        protected override bool GetIsOpen()
        {
            return true;
        }
        protected override void WakeSlot()
        {
            
            facing = CardFacing.FaceUp;
            orientation = Orientation.Vertical;
            slotType = CardLocation.Underworld;
            touch.ClearAll();
            touch.AddClickListener(() => ClickSlot());
        }


        public override void AllocateTo(GameCard card, bool sendToServer = true)
        {
            card.cardObject.Show();
            card.RemoveFromSlot();
            cards.Add(card);
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

        protected override void DisplayCardObject(GameCard card)
        {
            CardView c = card.cardObject;
            c.SetAsChild(transform, CardScale, SortLayer);
            card.rect.sizeDelta = rect.sizeDelta;
            c.transform.localPosition = new Vector2(0f, 0f);
            c.Flip();
            c.Rotate(false);
            card.SetCardMode(CardMode.None);

            Optimize();

        }
        public override bool ValidateCard(GameCard card)
        {
            return true;
        }

        #region Slot Commands

        protected override List<PopupCommand> GetSlotCommands()
        {
            List<PopupCommand> commands = new List<PopupCommand>();

            if (cards.Count > 0)
            {
                commands.Add(PopupCommand.Create("Browse", () => BrowseCards(cards, $"{Owner.userId}'s Underworld", IsYours, 0, 0)));
                if (IsYours)
                {
                    commands.Add(PopupCommand.Create("Manage", () => ManageCards(cards, "Select Cards to Move", true, 1, cards.Count)));
                }


                commands.Add(PopupCommand.Create("Close", () => CloseCommand()));



            }

            return commands;
        }
        //public void BrowseCommand()
        //{
        //    BrowseCards(cards, $"{Owner.userId}'s Underworld", IsYours, 0, 0);
        //}

        //public void ManageCommand()
        //{
        //    ManageCards(cards, "Select Cards to Move", true, 1, Owner.gameField.UnderworldSlot.cards.Count);
        //}
        protected override void AwaitManage(BrowseArgs args)
        {
            base.AwaitManage(args);
            if (!args.IsConfirm || args.Selections.Count <= 0) { return; }
            LocationMenu.Load(Owner, args.Selections);
        }

        
        #endregion
    }
}

