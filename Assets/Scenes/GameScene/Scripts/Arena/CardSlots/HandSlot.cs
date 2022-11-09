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

        [SerializeField]
        private CanvasGroup m_Canvas;
        protected void ToggleCanvasGroup(bool turnOn)
        {
            m_Canvas.alpha = turnOn ? 1 : 0;
            m_Canvas.interactable = turnOn;
        }

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
        protected override void StartSlot()
        {
            orientation = Orientation.Vertical;
            slotType = CardLocation.Hand;
            //GameManager.Instance.browseMenu.OnMenuToggled += ToggleCanvasGroup;
            
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
            card.cardObject.SetSortingOrder(Content.childCount * 20);

            card.cardObject.GetComponent<RectTransform>().ForceUpdateRectTransforms();
            Content.ForceUpdateRectTransforms();
            card.cardObject.Flip(facing == CardFacing.FaceDown);
        }
        protected override bool GetClickValidation()
        {
            if (App.WhoAmI == Owner.userId)
            {
                return true;
            }
            return false;
        }
        //protected override void SetCommands(GameCard card)
        //{
        //    TouchObject to = card.cardObject.touch;
        //    to.AddClickListener(() => ClickCard(card));
        //    to.AddHoldListener(() => GameManager.Instance.DragCard(card, this));
        //}

        protected override void ClickCard(GameCard card)
        {
            bool sameCard = GameManager.SelectedCard == card;
            base.ClickCard(card);
            SetSelectedCard(card);

            if (GameManager.Instance.currentSelector == null)
            {
                if (GameManager.Instance.popupMenu.isOpen)
                {
                    if (sameCard) { ClosePopMenu(); } else { OpenPopMenu(); }

                }
                else
                {
                    OpenPopMenu();
                }

            }
            else
            {
                if (GameManager.Instance.currentSelector.TargetSlots.Contains(this))
                {
                    GameManager.Instance.currentSelector.SelectSlot(this);
                }
            }
           


            
        }

        protected override void SetSelectedCard(GameCard view = null)
        {
            GameCard current = SelectedCard;
            if (current != null && view != null && current != view)
            {
                current.cardObject.SetSortingLayer(Card.CardLayer1);
                if (view != null)
                {
                    view.cardObject.SetSortingLayer(Card.CardLayer3);
                }
            }
            base.SetSelectedCard(view);
            
        }

        public override void OpenPopMenu()
        {
            GameManager.Instance.popupMenu.LoadMenu(this, Validate);
            
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
                commands.Add(PopupCommand.Create("Cast", () => BaseCastCommand(), 0, 0));
                commands.Add(PopupCommand.Create("Discard", () => DiscardCommand(), 0, 1));
                commands.Add(PopupCommand.Create("Close", () => CloseCommand()));
            }

            return commands;
        }

        #endregion

        #region Menu Commands
        
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


        private void OnDestroy()
        {
            //GameManager.Instance.browseMenu.OnMenuToggled -= ToggleCanvasGroup;
            
        }
        private void OnApplicationQuit()
        {
            //GameManager.Instance.browseMenu.OnMenuToggled -= ToggleCanvasGroup;
            
        }

    }
}
