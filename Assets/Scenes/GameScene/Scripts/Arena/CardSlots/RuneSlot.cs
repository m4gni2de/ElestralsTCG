using System.Collections.Generic;
using Gameplay.CardActions;
using Gameplay.Menus.Popup;
using Gameplay.Turns;

namespace Gameplay
{


    public class RuneSlot : SingleSlot
    {

        //public string EmpoweringSlot;

        public bool IsEmpowering
        {
            get
            {
                if (MainCard == null) { return false; }

                return GameManager.ActiveGame.EmpoweredRunes.ContainsKey(MainCard);
            }
        }
        protected override void WakeSlot()
        {
            base.WakeSlot();
            facing = CardFacing.Both;
            orientation = Orientation.Vertical;
            slotType = CardLocation.Rune;
        }

        protected override bool GetClickValidation()
        {
            return IsYours;
        }

        protected override void SetOrientation(GameCard card)
        {
            card.cardObject.Flip(card.mode == CardMode.Defense);
        }
        
        public override bool ValidateCard(GameCard card)
        {
            if (card.CardType == CardType.Rune)
            {
                Rune rune = (Rune)card.card;
                if (rune.GetRuneType == Rune.RuneType.Stadium)
                {
                    return false;
                }
                return true;
            }
            if (card.CardType == CardType.Spirit)
            {
                if (MainCard != null) { return true; }
            }
            return false;
        }
        

        protected override void ClickCard(GameCard card)
        {
            bool sameCard = GameManager.SelectedCard == card;
            base.ClickCard(card);
            SetSelectedCard(card);

            if (GameManager.Instance.currentSelector == null)
            {
                if (card.CardType == CardType.Rune)
                {
                    if (card.IsFaceUp)
                    {
                        GameManager.Instance.cardSlotMenu.LoadMenu(this);
                    }
                }
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

        public override void OpenPopMenu()
        {
            if (MainCard != null)
            {
                SetSelectedCard(MainCard);
                bool canClick = Validate;

                if (!IsYours) { canClick = MainCard.IsFaceUp; }
                GameManager.Instance.popupMenu.LoadMenu(this, canClick);
            }

        }

        #region Slot Commands
        protected override List<PopupCommand> GetSlotCommands()
        {
            List<PopupCommand> commands = new List<PopupCommand>();
          

            
            if (IsYours)
            {
                commands.Add(PopupCommand.Create("Inspect", () => InspectCommand()));
                if (!SelectedCard.IsFaceUp)
                {
                    commands.Add(PopupCommand.Create("Cast", () => ChangeModeCommand()));
                }
                else
                {
                    commands.Add(PopupCommand.Create("Cast", () => CastToSlotCommand(SelectedCard, this)));
                    commands.Add(PopupCommand.Create("Enchant", () => EnchantCommand(1)));
                    commands.Add(PopupCommand.Create("DisEnchant", () => DisEnchantCommand(), 1, 0));
                    commands.Add(PopupCommand.Create("Empower", () => EmpowerCommand(), 1, 0));
                    commands.Add(PopupCommand.Create("Destroy", () => DestroyCommand(), 1, 0));
                }

                commands.Add(PopupCommand.Create("Close", () => CloseCommand()));
            }
          
            



            return commands;
        }

        protected void InspectCommand()
        {

        }

        #region Change Card Mode
        protected override void ChangeModeCommand()
        {
            CardMode current = SelectedCard.mode;
            CardMode newMode = CardMode.Defense;
            if (current == CardMode.Defense) { newMode = CardMode.Attack; }

            if (newMode == CardMode.Attack || current != CardMode.Attack)
            {
                int castCount = SelectedCard.card.SpiritsReq.Count;
                List<GameCard> toShow = Owner.gameField.SpiritDeckSlot.cards;

                string title = $"Select {CardUI.AnySpiritUnicode(castCount)} for Cast of {SelectedCard.name}";
                GameManager.Instance.browseMenu.LoadCards(toShow, title, true, castCount, castCount);
                GameManager.Instance.browseMenu.CastMode(SelectedCard);
                ClosePopMenu();
                GameManager.Instance.browseMenu.OnCastClose += AwaitFlipCast;
            }
            else
            {
                ClosePopMenu();
                Refresh();
            }
            

        }
        #endregion

        #region Enchant Command
        public override void CastToSlotCommand(GameCard card, CardSlot from)
        {
            int castCount = card.card.SpiritsReq.Count;
            List<GameCard> toShow = Owner.gameField.SpiritDeckSlot.cards;

            string title = $"Select {CardUI.AnySpiritUnicode(castCount)} Spirits for Cast of {card.card.cardData.cardName}";
            GameManager.Instance.browseMenu.LoadCards(toShow, title, true, castCount, castCount);
            GameManager.Instance.browseMenu.CastMode(card, this);
            ClosePopMenu(true);
            BrowseMenu.OnClosed += CastToClose;
        }

        protected void EmpowerCommand()
        {
            ChooseEmpoweredElestral();
        }
        protected override void AwaitEmpowerSource(bool isConfirm, SlotSelector sel)
        {
            sel.OnSelectionHandled -= AwaitEmpowerSource;

            if (isConfirm)
            {

                EmpowerAction empower = EmpowerAction.EmpowerElestral(Owner, SelectedCard, this, SelectedCard.EnchantingSpirits, sel.SelectedSlots[0].MainCard);
                GameManager.Instance.DoCast(empower);
                sel.SelectedSlots[0].MainCard.SelectCard(false);
                TurnManager.SetCrafingAction();
                GameManager.Instance.SetSelector();
                Refresh();
            }
            else
            {
                GameCard source = TurnManager.Instance.CraftingAction.FindSourceCard();
                GameManager.Instance.SetSelector();
                TurnManager.SetCrafingAction();
                Refresh();

            }


        }

        protected void AwaitFlipCast(List<GameCard> selectedCards, CardMode cMode)
        {
            GameManager.Instance.browseMenu.OnCastClose -= AwaitFlipCast;
            if (cMode == CardMode.None || cMode == CardMode.Defense) { return; }

            //Field f = GameManager.Instance.arena.GetPlayerField(Owner);
            List<GameCard> cardsList = new List<GameCard>();
            for (int i = 0; i < selectedCards.Count; i++)
            {
                cardsList.Add(selectedCards[i]);
            }
            GameCard Selected = SelectedCard;
            CardSlot slot = SelectedCard.CurrentSlot;

            GameManager.Instance.FlipUpCast(Owner, Selected, cardsList);
            Refresh();

        }

        #endregion


        protected override void DestroyCommand()
        {
            if (MainCard != null)
            {
                App.AskYesNo($"Send {MainCard.cardStats.title} and all of its Enchanting Spirits to the Underworld?", DestroyResponse);
                ClosePopMenu(false);
            }
            
        }
        protected override void DestroyResponse(bool confirm)
        {
            if (confirm)
            {
                for (int i = 0; i < cards.Count; i++)
                {
                    GameManager.Instance.MoveCard(Owner, cards[i], cards[i].Owner.gameField.UnderworldSlot);
                }
                GameManager.Instance.cardSlotMenu.Close();
                
            }
            Refresh();
        }

        protected void CloseCommand()
        {
            ClosePopMenu();
        }
        #endregion
    }
}
