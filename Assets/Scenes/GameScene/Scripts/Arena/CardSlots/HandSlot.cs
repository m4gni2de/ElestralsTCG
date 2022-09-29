using System.Collections;
using System.Collections.Generic;
using Gameplay.Menus;
using Gameplay.Menus.Popup;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

        protected override void SetSlot()
        {
            orientation = Orientation.Vertical;
            slotType = CardLocation.Hand;

            
        }
        
        protected override void DisplayCardObject(GameCard card)
        {
            card.cardObject.SetAsChild(Content, CardScale, SortLayer, 0);
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
            
            base.ClickCard(card);
            if (App.WhoAmI == Owner.userId)
            {
                SetSelectedCard(card);
                OpenPopMenu();
                
            }
            

        }
        public override void OpenPopMenu()
        {
            base.OpenPopMenu();
            
        }

        public override bool ValidateCard(GameCard card)
        {
            if (card.CardType == CardType.Spirit) { return false; }
            return true;
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
            commands.Add(PopupCommand.Create("Enchant", () => EnchantCommand(), 0, 0));
            commands.Add(PopupCommand.Create("Discard", () => DiscardCommand(), 0, 1));
            commands.Add(PopupCommand.Create("Close", () => CloseCommand()));
            return commands;
        }
        
        #region Menu Commands
        protected void EnchantCommand()
        {
            int enchantCount = SelectedCard.card.SpiritsReq.Count;
            List<GameCard> toShow = Owner.gameField.SpiritDeckSlot.cards;

            string title = $"Select {enchantCount} Spirits for Enchantment of {SelectedCard.name}";
            GameManager.Instance.browseMenu.LoadCards(toShow, title, true, enchantCount, enchantCount);
            GameManager.Instance.browseMenu.EnchantMode(SelectedCard);
            ClosePopMenu(true);
            GameManager.Instance.browseMenu.OnEnchantClose += AwaitEnchantClose;

        }
        protected void AwaitEnchantClose(List<GameCard> selectedCards, CardMode cMode)
        {
            
            GameManager.Instance.browseMenu.OnEnchantClose -= AwaitEnchantClose;
            if (cMode == CardMode.None) { return; }

            Field f = GameManager.Instance.arena.GetPlayerField(Owner);
            List<GameCard> cardsList = new List<GameCard>();
            for (int i = 0; i < selectedCards.Count; i++)
            {
                cardsList.Add(selectedCards[i]);
            }
            GameCard Selected = SelectedCard;
            CardSlot slot = f.ElestralSlot(0, true);

            

            if (Selected.card.CardType == CardType.Rune)
            {
                slot = f.RuneSlot(0, true);
                if (cMode == CardMode.Defense)
                {
                    GameManager.Instance.SetEnchant(Owner, Selected, slot);
                    Refresh();
                    return;
                }
            }

            GameManager.Instance.NormalEnchant(Owner, Selected, cardsList, slot, cMode);
            Refresh();

        }
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

        protected void Refresh()
        {
            GameManager.Instance.browseMenu.SelectedCards.Clear();
            SelectedCard = null;
            GameManager.SelectedCard = null;
            
        }
        #endregion

    }
}
