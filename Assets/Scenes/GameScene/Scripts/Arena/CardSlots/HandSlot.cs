using System.Collections;
using System.Collections.Generic;
using Gameplay.Menus;
using Gameplay.Menus.Popup;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Gameplay.Menus.CardBrowseMenu;

namespace Gameplay
{


    public class HandSlot : CardSlot
    {
        public ScrollRect m_Scroll;
        protected RectTransform Content { get { return m_Scroll.content; } }


        protected override Vector2 GetSlotSize()
        {
            return Content.GetComponent<GridLayoutGroup>().cellSize;
        }

        [SerializeField]
        private SpriteDisplay sp;

        #region Overrides

        protected override bool GetIsOpen()
        {
            return true;
        }
        protected override void SetSlot()
        {
            orientation = Orientation.Vertical;
            slotType = CardLocation.Hand;

            
        }

        public override void SetPlayer(Player owner, int count)
        {
            base.SetPlayer(owner, count);
            if (!owner.IsLocal)
            {
                facing = CardFacing.FaceDown;
            }
            else
            {
                facing = CardFacing.FaceUp;
            }
           
        }
        protected override void DisplayCardObject(GameCard card)
        {
            
            card.cardObject.SetAsChild(Content, CardScale, SortLayer, Content.childCount);
            card.cardObject.SetSortingOrder(Content.childCount);

            card.cardObject.GetComponent<RectTransform>().ForceUpdateRectTransforms();
            Content.ForceUpdateRectTransforms();
            card.cardObject.Flip(facing == CardFacing.FaceDown);
        }
        //protected override void SetCommands(GameCard card)
        //{
        //    TouchObject to = card.cardObject.touch;
        //    to.AddClickListener(() => ClickCard(card));
        //    to.AddHoldListener(() => GameManager.Instance.DragCard(card, this));
        //}

        protected override void ClickCard(GameCard card)
        {

            GameManager.SelectedCard = card;
            if (App.WhoAmI == Owner.userId)
            {
                SetSelectedCard(card);
                OpenPopMenu();
                card.cardObject.SetSortingLayer(Card.CardLayer3);

            }
            

        }

        protected override void SetSelectedCard(GameCard view = null)
        {
            GameCard current = SelectedCard;
            if (current != null && view != null && current != view)
            {
                current.cardObject.SetSortingLayer(Card.CardLayer1);
            }

            base.SetSelectedCard(view);
            
        }
        public override void OpenPopMenu()
        {
            base.OpenPopMenu();
            
        }

        public override bool ValidateCard(GameCard card)
        {
            if (card.CardType == CardType.Spirit) { return false; }
            return card.Owner == GameManager.ActiveGame.You;
        }



        protected override void GetSpriteRenderer()
        {

        }

        public override void SetValidate(bool isValid)
        {
            if (isValid)
            {
                sp.SetColor(new Color(Color.green.r, Color.green.g, Color.green.g, .45f));
            }
            else
            {
                sp.SetColor(new Color(Color.red.r, Color.red.g, Color.red.g, .45f));
            }
        }
        public override void ClearValidation()
        {
            sp.SetColor(Color.clear);
        }

        protected override List<PopupCommand> GetSlotCommands()
        {
            List<PopupCommand> commands = new List<PopupCommand>();
            if (IsYours)
            {
                commands.Add(PopupCommand.Create("Enchant", () => BaseEnchantCommand(), 0, 0));
                commands.Add(PopupCommand.Create("Discard", () => DiscardCommand(), 0, 1));
            }
            
            commands.Add(PopupCommand.Create("Close", () => CloseCommand()));
            return commands;
        }

        #endregion

        #region Menu Commands
        //protected void EnchantCommand()
        //{
        //    if (SelectedCard.CardType == CardType.Rune)
        //    {
        //        Rune r = (Rune)SelectedCard.card;
        //        if (r.GetRuneType == Rune.RuneType.Artifact)
        //        {
        //            List<CardSlot> targets = new List<CardSlot>();
        //            targets.AddRange(Owner.gameField.ElestralSlots(false));
        //            targets.AddRange(Owner.Opponent.gameField.ElestralSlots(false));

        //            SlotSelector sourceSelect = SlotSelector.Create("Select Elestral to Empower", "Empowered Elestral", targets, 1);
        //            GameManager.Instance.SetSelector(sourceSelect);
        //            sourceSelect.OnSelectionHandled += AwaitEmpowerSource;
        //            ClosePopMenu(true);
        //        }
        //    }
        //    else
        //    {
        //        StartEnchantCommand();
        //    }

        //}

        //protected void StartEnchantCommand()
        //{
        //    int enchantCount = SelectedCard.card.SpiritsReq.Count;
        //    List<GameCard> toShow = Owner.gameField.SpiritDeckSlot.cards;

        //    string title = $"Select {enchantCount} Spirits for Enchantment of {SelectedCard.name}";
        //    GameManager.Instance.browseMenu.LoadCards(toShow, title, true, enchantCount, enchantCount);
        //    GameManager.Instance.browseMenu.EnchantMode(SelectedCard);
        //    ClosePopMenu(true);
        //    GameManager.Instance.browseMenu.OnEnchantClose += AwaitEnchantClose;
        //}
        //protected void AwaitEmpowerSource(bool isConfirm, SlotSelector sel)
        //{
        //    sel.OnSelectionHandled -= AwaitEmpowerSource;
        //    if (isConfirm)
        //    {
        //        int enchantCount = SelectedCard.card.SpiritsReq.Count;
        //        List<GameCard> toShow = Owner.gameField.SpiritDeckSlot.cards;

        //        string title = $"Select {enchantCount} Spirits for Enchantment of {SelectedCard.name}";
        //        GameManager.Instance.browseMenu.LoadCards(toShow, title, true, enchantCount, enchantCount);
        //        GameManager.Instance.browseMenu.EnchantMode(SelectedCard);
        //        ClosePopMenu(true);
        //        GameManager.Instance.browseMenu.OnClosed += AwaitEmpowerClose;
        //    }
        //    else
        //    {
        //        GameManager.Instance.SetSelector();
        //        StartEnchantCommand();
        //    }
        //}
        //protected void AwaitEnchantClose(List<GameCard> selectedCards, CardMode cMode)
        //{
            
        //    GameManager.Instance.browseMenu.OnEnchantClose -= AwaitEnchantClose;
        //    if (cMode == CardMode.None) { return; }

        //    Field f = GameManager.Instance.arena.GetPlayerField(Owner);
        //    List<GameCard> cardsList = new List<GameCard>();
        //    for (int i = 0; i < selectedCards.Count; i++)
        //    {
        //        cardsList.Add(selectedCards[i]);
        //    }
        //    GameCard Selected = SelectedCard;
        //    CardSlot slot = f.ElestralSlot(0, true);

            

        //    if (Selected.card.CardType == CardType.Rune)
        //    {
        //        slot = f.RuneSlot(0, true);
        //        if (cMode == CardMode.Defense)
        //        {
        //            GameManager.Instance.SetEnchant(Owner, Selected, slot);
        //            Refresh();
        //            return;
        //        }
        //    }

        //    GameManager.Instance.NormalEnchant(Owner, Selected, cardsList, slot, cMode);
        //    Refresh();

        //}

        //protected void AwaitEmpowerClose(BrowseArgs args)
        //{
        //    GameManager.Instance.browseMenu.OnClosed -= AwaitEmpowerClose;
        //    SlotSelector selector = GameManager.Instance.currentSelector;
        //    GameCard targetElestral = selector.SelectedSlots[0].MainCard;

        //}
        protected void DiscardCommand()
        {
            GameManager.Instance.browseMenu.LoadCards(cards, "Select Cards to Discard", true, 1, cards.Count);
            ClosePopMenu(true);
            GameManager.Instance.browseMenu.OnClosed += AwaitDiscardClose;
        }
        protected void AwaitDiscardClose(CardBrowseMenu.BrowseArgs args)
        {
            GameManager.Instance.browseMenu.OnClosed -= AwaitDiscardClose;
            if (!args.IsConfirm) { Refresh(); return; }
            for (int i = 0; i < args.Selections.Count; i++)
            {
                GameCard toMove = args.Selections[i];
                MoveAction ac = new MoveAction(Owner, toMove, Owner.gameField.UnderworldSlot);
                GameManager.Instance.MoveCard(ac);
            }
            
        }
        protected void CloseCommand()
        {
            ClosePopMenu();
        }
        protected override void ClosePopMenu(bool keepSelected = false)
        {
            GameManager.Instance.popupMenu.CloseMenu();

            
            if (!keepSelected)
            {
                Refresh();
            }

        }

        protected override void Refresh()
        {
            base.Refresh();
            GameManager.Instance.browseMenu.SelectedCards.Clear();

            if (SelectedCard != null)
            {
                SelectedCard.cardObject.SetSortingLayer(Card.CardLayer1);
            }
            SelectedCard = null;
            GameManager.SelectedCard = null;
            
        }
        #endregion

    }
}
