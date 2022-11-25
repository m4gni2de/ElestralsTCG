using System.Collections;
using System.Collections.Generic;
using Databases;
using Gameplay.Menus;
using Gameplay.Menus.Popup;
using UnityEngine;
using static Gameplay.Menus.CardBrowseMenu;
using TMPro;

namespace Gameplay
{
    public class SpiritDeckSlot : DeckSlot
    {
        [SerializeField]
        private TMP_Text spiritCountText;
        #region Overrides
        protected override void WakeSlot()
        {
            base.WakeSlot();
            facing = CardFacing.FaceDown;
            orientation = Orientation.Vertical;
            slotType = CardLocation.SpiritDeck;
        }

       

        public override void AllocateTo(GameCard card, bool sendToServer = true)
        {
            card.RemoveFromSlot();
            cards.Insert(0, card);
            card.AllocateTo(this, sendToServer);

            DisplayCardObject(card);
            SetCommands(card);
            spiritCountText.text = cards.Count.ToString();


            if (!Owner.deck.SpiritDeck.Cards.Contains(card))
            {
                Owner.deck.SpiritDeck.AddCard(card);
            }


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
            Owner.deck.RemoveCard(card, Owner.deck.SpiritDeck);
            spiritCountText.text = cards.Count.ToString();
            Optimize();
        }
        public override bool ValidateCard(GameCard card)
        {
            if (card.CardType == CardType.Spirit) { return true; }
            return false;
        }

        protected override List<PopupCommand> GetSlotCommands()
        {
            List<PopupCommand> commands = new List<PopupCommand>();
            
            if (IsYours)
            {
                //commands.Add(PopupCommand.Create("Browse", () => BrowseCommand(), 0, 1));
                commands.Add(PopupCommand.Create("Manage", () => ManageCards(cards, $"{Owner.userId}'s Spirit Deck", true, 1, Owner.gameField.SpiritDeckSlot.cards.Count)));
                commands.Add(PopupCommand.Create("Expend", () => ExpendCommand(), 0, 1));
            }
            else
            {
                commands.Add(PopupCommand.Create("Browse", () => BrowseCards(cards, $"{Owner.userId}'s Spirit Deck", true, 0, 0)));
            }

            commands.Add(PopupCommand.Create("Close", () => CloseCommand(), 0, 5));
            return commands;
        }

        //protected override void BrowseCommand()
        //{
        //    GameManager.Instance.browseMenu.LoadCards(cards, "Spirit Deck", true, 1, Owner.gameField.SpiritDeckSlot.cards.Count);
        //    GameManager.Instance.browseMenu.OnClosed += AwaitManage;
        //    ClosePopMenu();
        //}

        protected override bool GetIsOpen()
        {
            return true;
        }
        #endregion

       

        protected override void AwaitManage(BrowseArgs args)
        {
            base.AwaitManage(args);
            if (!args.IsConfirm || args.Selections.Count <= 0) { return; }
            LocationMenu.Load(Owner, args.Selections);
        }

        protected void ExpendCommand()
        {
            GameManager.Instance.popupMenu.InputNumber("How many Spirits are you Expending?", StartExpend, 1, Owner.gameField.SpiritDeckSlot.cards.Count, 1);
        }

        protected void StartExpend(int count)
        {
            GameManager.Instance.browseMenu.LoadCards(cards, "Select Spirits to Expend", true, count, count);
            GameManager.Instance.browseMenu.OnClosed += AwaitExpend;
            ClosePopMenu();
        }

        protected void AwaitExpend(BrowseArgs args)
        {
            GameManager.Instance.browseMenu.OnClosed -= AwaitManage;
            if (!args.IsConfirm || args.Selections.Count <= 0) { return; }

            for (int i = 0; i < args.Selections.Count; i++)
            {
                GameManager.Instance.MoveCard(Owner, args.Selections[i], Owner.gameField.UnderworldSlot);
            }
        }
    }
}

