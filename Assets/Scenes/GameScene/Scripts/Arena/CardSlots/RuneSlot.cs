using System.Collections;
using System.Collections.Generic;
using Gameplay.CardActions;
using Gameplay.Menus.Popup;
using Gameplay.Turns;
using UnityEngine;

namespace Gameplay
{


    public class RuneSlot : SingleSlot
    {

        public string EmpoweringSlot;
        protected override void SetSlot()
        {
            base.SetSlot();
            facing = CardFacing.Both;
            orientation = Orientation.Vertical;
            slotType = CardLocation.Rune;
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
            base.ClickCard(card);
            SetSelectedCard(card);
            
            if (GameManager.Instance.currentSelector == null)
            {
                OpenPopMenu();
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
                base.OpenPopMenu();
            }

        }

        #region Slot Commands
        protected override List<PopupCommand> GetSlotCommands()
        {
            List<PopupCommand> commands = new List<PopupCommand>();
          

            commands.Add(PopupCommand.Create("Inspect", () => InspectCommand()));
            if (IsYours)
            {
                commands.Add(PopupCommand.Create("Activate", () => ChangeModeCommand()));
                commands.Add(PopupCommand.Create("DisEnchant", () => EnchantCommand(false), 1, 0));
            }
          
            commands.Add(PopupCommand.Create("Close", () => CloseCommand()));



            return commands;
        }

        protected void InspectCommand()
        {

        }

        #region Change Card Mode
        protected void ChangeModeCommand()
        {
            CardMode current = SelectedCard.mode;
            CardMode newMode = CardMode.Defense;
            if (current == CardMode.Defense) { newMode = CardMode.Attack; }

            if (newMode == CardMode.Attack || current != CardMode.Attack)
            {
                EnchantCommand(true);
            }
            else
            {
                ClosePopMenu();
                Refresh();
            }
            

        }
        #endregion



        
        public void Empower(CardSlot toEmpower = null, bool sendToNetwork = true)
        {
            if (toEmpower == null) { EmpoweringSlot = ""; }
            else
            {
                if (toEmpower.slotType != CardLocation.Elestral) { return; }
                EmpoweringSlot = toEmpower.slotId;
            }

            if (sendToNetwork)
            {

            }
           
           
        }
        #region Enchant Command

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
                GameManager.Instance.DoEnchant(empower);
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

        protected void EnchantCommand(bool adding)
        {
            if (adding)
            {
                int enchantCount = SelectedCard.card.SpiritsReq.Count;
                List<GameCard> toShow = Owner.gameField.SpiritDeckSlot.cards;

                string title = $"Select {enchantCount} Spirits for Enchantment of {SelectedCard.name}";
                GameManager.Instance.browseMenu.LoadCards(toShow, title, true, enchantCount, enchantCount);
                GameManager.Instance.browseMenu.EnchantMode(SelectedCard);
                ClosePopMenu();
                GameManager.Instance.browseMenu.OnEnchantClose += AwaitEnchantClose;
            }



        }

       
        protected void AwaitEnchantClose(List<GameCard> selectedCards, CardMode cMode)
        {
            GameManager.Instance.browseMenu.OnEnchantClose -= AwaitEnchantClose;
            if (cMode == CardMode.None || cMode == CardMode.Defense) { return; }

            Field f = GameManager.Instance.arena.GetPlayerField(Owner);
            List<GameCard> cardsList = new List<GameCard>();
            for (int i = 0; i < selectedCards.Count; i++)
            {
                cardsList.Add(selectedCards[i]);
            }
            GameCard Selected = SelectedCard;
            CardSlot slot = SelectedCard.CurrentSlot;

            GameManager.Instance.FaceDownRuneEnchant(Owner, Selected, cardsList);
            Refresh();

        }

        #endregion

        protected void CloseCommand()
        {
            ClosePopMenu();
        }
        #endregion
    }
}
