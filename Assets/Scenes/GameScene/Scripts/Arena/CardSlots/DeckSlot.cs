using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameplay.Menus;
using UnityEngine.Events;
using Gameplay.Menus.Popup;
using static Gameplay.Menus.CardBrowseMenu;

namespace Gameplay
{


    public class DeckSlot : CardSlot, iSelectSlot
    {
        private TouchObject _touch = null;
        public TouchObject touch { get { _touch ??= GetComponent<TouchObject>(); return _touch; } }


        #region Interface
        public void Optimize()
        {
            for (int i = 0; i < cards.Count; i++)
            {
                if (i == 0)
                {
                    cards[i].cardObject.Show();
                }
                else
                {
                    cards[i].cardObject.Hide();
                }
            }
        }
        #endregion

        #region Overrides
        protected override bool GetIsOpen()
        {
            return true;
        }

        protected override void WakeSlot()
        {

            facing = CardFacing.FaceDown;
            orientation = Orientation.Vertical;
            slotType = CardLocation.Deck;
            touch.ClearAll();
            touch.AddClickListener(() => ClickSlot());
        }



        public override void AllocateTo(GameCard card, bool sendToServer = true)
        {
            
            card.RemoveFromSlot();
            cards.Insert(0, card);
            card.AllocateTo(this, sendToServer);
            DisplayCardObject(card);
            SetCommands(card);

            if (!Owner.deck.MainDeck.Cards.Contains(card))
            {
                Owner.deck.MainDeck.AddCard(card);
            }
            Optimize();

        }

        protected override void SetCommands(GameCard card)
        {
            TouchObject to = card.cardObject.touch;
            to.ClearClick();
            to.ClearHold();
        }

        public override void RemoveCard(GameCard card)
        {
            
            base.RemoveCard(card);
            Owner.deck.RemoveCard(card, Owner.deck.MainDeck);
            Optimize();
        }

        protected override void DisplayCardObject(GameCard card)
        {

            base.DisplayCardObject(card);
            Optimize();
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

            if (IsYours)
            {
                commands.Add(PopupCommand.Create("Draw", () => DrawCommand(), 0, 0));
                commands.Add(PopupCommand.Create("Browse", () => ManageCards(Owner.deck.MainDeck.InOrder.ReverseOf(), "Manage Deck", IsYours, 1, Owner.deck.MainDeck.InOrder.Count)));
                commands.Add(PopupCommand.Create("Mill", () => MillCommand(), 0, 2));
                commands.Add(PopupCommand.Create("Shuffle", () => ShuffleCommand(), 0, 2));
            }

            //commands.Add(PopupCommand.Create("Draw", () => DrawCommand(), 0, 0));
            //commands.Add(PopupCommand.Create("Browse", () => ManageCards(Owner.deck.MainDeck.InOrder.ReverseOf(), "Manage Deck", IsYours, 1, Owner.deck.MainDeck.InOrder.Count)));
            //commands.Add(PopupCommand.Create("Mill", () => MillCommand(), 0, 2));
            //commands.Add(PopupCommand.Create("Shuffle", () => ShuffleCommand(), 0, 2));

            commands.Add(PopupCommand.Create("Close", () => CloseCommand(), 0, 5));
            return commands;
        }

        #endregion

        #region Menu Commands

        protected void DrawCommand()
        {
            GameManager.Instance.popupMenu.InputNumber("How many cards do you want to Draw?", Owner.Draw, 0, Owner.deck.MainDeck.InOrder.Count, 1);
            
        }
        protected void MillCommand()
        {
            GameManager.Instance.popupMenu.InputNumber("How many cards do you want to Mill?", Owner.Mill, 0, Owner.deck.MainDeck.InOrder.Count, 1);
        }
       
        protected override void AwaitManage(BrowseArgs args)
        {
            base.AwaitManage(args);
            if (!args.IsConfirm || args.Selections.Count <= 0) { return; }
            LocationMenu.Load(Owner, args.Selections);
        }

        protected void ShuffleCommand()
        {
            GameManager.Instance.popupMenu.ConfirmAction("Do you want to Shuffle your deck?", ConfirmShuffle);
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
        
        public void LookCommand(int lookAmount, int takeAmount = 0)
        {
            List<GameCard> toLookAt = new List<GameCard>();

            for (int i = 0; i < lookAmount; i++)
            {
                toLookAt.Add(Owner.deck.MainDeck.InOrder[i]);
            }

            if (takeAmount <= 0)
            {
                GameManager.Instance.browseMenu.LoadCards(toLookAt, "Browse Deck", true);
            }
            else
            {
                GameManager.Instance.browseMenu.LoadCards(toLookAt, "Browse Deck", true, takeAmount, takeAmount);
                GameManager.Instance.browseMenu.OnClosed += AwaitManage;
                ClosePopMenu();
            }
            
        }

       
        #endregion


    }
}
