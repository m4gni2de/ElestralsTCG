using System.Collections;
using System.Collections.Generic;
using Gameplay.Menus.Popup;
using Gameplay.Turns;
using UnityEngine;
using UnityEngine.Events;
using Gameplay.CardActions;
using System.IO;
using Gameplay.Menus;

namespace Gameplay
{
    public class ElestralSlot : SingleSlot, iCraftAction
    {
        
        protected override void SetSlot()
        {
            base.SetSlot();
            facing = CardFacing.FaceUp;
            orientation = Orientation.Both;
            slotType = CardLocation.Elestral;
        }

        

        protected override void ClickCard(GameCard card)
        {
            bool sameCard = GameManager.SelectedCard == card;
            base.ClickCard(card);
            SetSelectedCard(card);
            if (GameManager.Instance.currentSelector == null)
            {
                if (card.CardType == CardType.Elestral)
                {
                    GameManager.Instance.cardSlotMenu.LoadMenu(this);
                }
                if (GameManager.Instance.popupMenu.isOpen)
                {
                    if (sameCard)
                    {
                        GameManager.Instance.popupMenu.CloseMenu();
                    }
                    else
                    {
                        GameManager.Instance.popupMenu.LoadMenu(this);
                    }
                    
                }
                else
                {
                    GameManager.Instance.popupMenu.LoadMenu(this);
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

        public override bool ValidateCard(GameCard card)
        {
            if (card.CardType == CardType.Elestral && MainCard == null) { return true; }
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
                if (MainCard == SelectedCard)
                {
                    base.OpenPopMenu();

                }
                //SetSelectedCard(MainCard);
               
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
            //commands.Add(PopupCommand.Create("Move", () => MoveCommand()));
            commands.Add(PopupCommand.Create("Enchant", () => EnchantCommand(true)));
            commands.Add(PopupCommand.Create("Ascend", () => AscendCommand()));
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
                BrowseMenu.LoadCards(toShow, title, true, enchantCount, enchantCount);
                BrowseMenu.EnchantMode(SelectedCard);
                ClosePopMenu(true);
                BrowseMenu.OnEnchantClose += AwaitEnchantClose;
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
                BrowseMenu.LoadCards(toShow, title, true, minCount, maxEnchantCount);
                BrowseMenu.EnchantMode(SelectedCard, false);
                ClosePopMenu(true);
                BrowseMenu.OnMenuClose += AwaitDisEnchantClose;
            }
           
            

        }
        protected void AwaitEnchantClose(List<GameCard> selectedCards, CardMode cMode)
        {
            BrowseMenu.OnEnchantClose -= AwaitEnchantClose;
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
            BrowseMenu.OnMenuClose -= AwaitDisEnchantClose;
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
            BrowseMenu.LoadCards(cards, "Select Cards to Discard", true);
        }

        #region Ascend Command
        //do validation on clicking based on number of spirits the Ascending card has and the number of spirits the cards on the field have
        protected void AscendCommand()
        {
            CardActionData d = this.CraftAction(ActionCategory.Ascend, Owner);
            d.AddData(AscendAction.TributedCardKey, SelectedCard.cardId);
            d.AddData(AscendAction.SlotToKey, slotId);

            int cardCount = 1;
            List<GameCard> toShow = Owner.gameField.HandSlot.cards;
            string title = $"Select Elestral to Ascend from {SelectedCard.name}";
            BrowseMenu.LoadCards(toShow, title, true, cardCount, cardCount);
            ClosePopMenu(true);
            BrowseMenu.OnClosed += AwaitAscendingElestral;

            
        }
        protected void AwaitAscendingElestral(CardBrowseMenu.BrowseArgs args)
        {
            BrowseMenu.OnClosed -= AwaitAscendingElestral;
            if (!args.IsConfirm) { Refresh(); TurnManager.SetCrafingAction(); return; }
            this.AddActionData(CardActionData.SourceKey, args.Selections[0].cardId);
            GameCard sourceCard = this.GetCraftingAction().FindSourceCard();


            string title = $"Select Catalyst Spirit to Ascend from {SelectedCard.name} to {sourceCard.name}.";
            BrowseMenu.EnchantLoad(Owner.gameField.SpiritDeckSlot.cards, title, true, 1, 1, sourceCard, true);
            ClosePopMenu(true);
            BrowseMenu.OnClosed += DoAscend;
        }
        protected void DoAscend(CardBrowseMenu.BrowseArgs args)
        {
            BrowseMenu.OnClosed -= DoAscend;
            if (!args.IsConfirm)
            {
                Refresh();
                AscendCommand();
            }
            else
            {
                this.AddActionData("catalyst_spirit", args.Selections[0].cardId);
                this.AddActionData("card_mode", (int)args.EnchantMode);
                this.GetCraftingAction().SetSpiritList(SelectedCard.EnchantingSpirits);
                this.GetCraftingAction().SetResult(ActionResult.Succeed);
                AscendAction toSend = AscendAction.FromData(this.GetCraftingAction());
                GameManager.Instance.Ascend(toSend);
                TurnManager.SetCrafingAction();
                Refresh();
            }
        }
       
       
        #endregion

        #endregion
    }
}

