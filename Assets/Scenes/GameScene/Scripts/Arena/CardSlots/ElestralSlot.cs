using System.Collections;
using System.Collections.Generic;
using Gameplay.Menus.Popup;
using Gameplay.Turns;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay
{
    public class ElestralSlot : SingleSlot
    {
        
        protected override void SetSlot()
        {
            base.SetSlot();
            facing = CardFacing.FaceUp;
            orientation = Orientation.Both;
            slotType = CardLocation.Elestral;
        }

        protected override void SetOrientation(GameCard card)
        {
            card.cardObject.Flip();
        }
        protected override void SetRotation(GameCard card)
        {
            if (card.mode == CardMode.Defense)
            {
                card.cardObject.Rotate(true);
            }
        }

        protected override void ClickCard(GameCard card)
        {
            SetSelectedCard(card);
            if (GameManager.Instance.currentSelector == null)
            {
                GameManager.Instance.cardSlotMenu.LoadMenu(this);
                GameManager.Instance.popupMenu.LoadMenu(this);
            }
            else
            {
                if (GameManager.Instance.currentSelector.TargetSlots.Contains(this))
                {
                    GameManager.Instance.currentSelector.SelectSlot(this);
                }
            }
            
        }

        public override bool ValidateCard(GameCard card)
        {
            if (card.cardStats.cardType == CardType.Elestral && MainCard == null) { return true; }
            if (card.cardStats.cardType == CardType.Spirit)
            {
                if (MainCard != null) { return true; }
            }
            return false;
        }

        public override void OpenPopMenu()
        {
            if (MainCard != null)
            {
                SetSelectedCard(MainCard);
                base.OpenPopMenu();
            }
            
        }
        #region Slot Menu

        protected override bool GetClickValidation()
        {
            
            bool isYours = Owner.userId == App.WhoAmI;
            bool validate = isYours;



            return validate;
        }

        protected override List<PopupCommand> GetSlotCommands()
        {
            List<PopupCommand> commands = new List<PopupCommand>();
            //commands.Add(PopupCommand.Create("Inspect", () => InspectCommand()));
            if (Owner.userId != App.WhoAmI) { commands.Add(PopupCommand.Create("Close", () => CloseCommand())); return commands; }

            commands.Add(PopupCommand.Create("Change Mode", () => ChangeModeCommand()));
            commands.Add(PopupCommand.Create("Move", () => MoveCommand()));
            commands.Add(PopupCommand.Create("Enchant", () => EnchantCommand(true)));
            commands.Add(PopupCommand.Create("DisEnchant", () => EnchantCommand(false), 1, 0));
            commands.Add(PopupCommand.Create("Nexus", () => NexusCommand(), 1, 1));
            commands.Add(PopupCommand.Create("Attack", () => AttackCommand()));
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

            GameManager.Instance.ChangeCardMode(Owner, SelectedCard, newMode);

            ClosePopMenu();
            Refresh();

        }
        
        #endregion
        protected void MoveCommand()
        {

        }


        #region Enchant Command
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
            else
            {
                List<GameCard> toShow = new List<GameCard>();
                for (int i = 0; i < cards.Count; i++)
                {
                    if (cards[i].CardType == CardType.Spirit)
                    {
                        toShow.Add(cards[i]);
                    }
                }
                int maxEnchantCount = toShow.Count;
                int minCount = 1;
                

                string title = $"Select {minCount} Spirits to DisEnchant from {SelectedCard.name}";
                GameManager.Instance.browseMenu.LoadCards(toShow, title, true, maxEnchantCount, minCount);
                GameManager.Instance.browseMenu.EnchantMode(SelectedCard, false);
                ClosePopMenu();
                GameManager.Instance.browseMenu.OnMenuClose += AwaitDisEnchantClose;
            }
           
            

        }
        protected void AwaitEnchantClose(List<GameCard> selectedCards, CardMode cMode)
        {
            GameManager.Instance.browseMenu.OnEnchantClose -= AwaitEnchantClose;
            if (cMode == CardMode.None) { return; }

            List<GameCard> cardsList = new List<GameCard>();
            for (int i = 0; i < selectedCards.Count; i++)
            {
                cardsList.Add(selectedCards[i]);
            }
            GameManager.Instance.ReEnchant(Owner, MainCard, cardsList);
            Refresh();

        }
        protected void AwaitDisEnchantClose(List<GameCard> selectedCards)
        {
            GameManager.Instance.browseMenu.OnMenuClose -= AwaitDisEnchantClose;
            List<GameCard> cardsList = new List<GameCard>();
            for (int i = 0; i < selectedCards.Count; i++)
            {
                cardsList.Add(selectedCards[i]);
            }
            GameManager.Instance.DisEnchant(Owner, MainCard, cardsList, Owner.gameField.UnderworldSlot);
            Refresh();

        }

        #endregion

        

        #region Attack Command
        protected void AttackCommand()
        {
            if (TurnManager.ValidateStartBattle())
            {
                if (TurnManager.IsBattlePhase)
                {
                    AttackTargetSelect(true);
                }
                else
                {
                    ClosePopMenu();
                    App.AskYesNo("Do you wish to end the Main Phase and start the Battle Phase?", AttackTargetSelect);
                }
            }
            
        }

        protected void AttackTargetSelect(bool confirm)
        {
            if (confirm)
            {
                SlotSelector attackSelector = SlotSelector.AttackSlots(Owner.Opponent, 1);
                GameManager.Instance.SetSelector(attackSelector);
                GameManager.Instance.currentSelector.OnSelectionHandled += AwaitAttackSelection;
                ClosePopMenu();
            }
           
        }
        protected void AwaitAttackSelection(bool isSuccess, SlotSelector selector)
        {
            if (!isSuccess) { return; }
            List<CardSlot> attackTargets = selector.SelectedSlots;
            for (int i = 0; i < attackTargets.Count; i++)
            {
                GameManager.Instance.ElestralAttack(MainCard, attackTargets[i]);
                GameManager.Instance.SetSelector();
            }
        }
    
        #endregion

        protected void DiscardCommand()
        {
            GameManager.Instance.browseMenu.LoadCards(cards, "Select Cards to Discard", true);
        }
        protected void CloseCommand()
        {
            ClosePopMenu();
        }

       
        #endregion
    }
}

