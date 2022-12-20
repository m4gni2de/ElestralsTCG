using System.Collections;
using System.Collections.Generic;
using Gameplay.Menus.Popup;
using Gameplay.Turns;
using UnityEngine;
using UnityEngine.Events;
using Gameplay.CardActions;
using System.IO;
using Gameplay.Menus;
using Databases;

namespace Gameplay
{
    public class ElestralSlot : SingleSlot, iCraftAction
    {

        public List<GameCard> EmpoweringRunes
        {
            get
            {

                List<GameCard> runes = new List<GameCard>();
                if (MainCard == null) { return runes; }
                ElestralCard es = (ElestralCard)MainCard;
                return es.EmpoweringRunes;
            }
        }
        protected override void WakeSlot()
        {
            base.WakeSlot();
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
                    if (sameCard) { ClosePopMenu(); } else { OpenPopMenu(); }
                    
                }
                else
                {
                    //GameManager.Instance.popupMenu.LoadMenu(this);
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


        public override bool ValidateCard(GameCard card)
        {
            if (card.CardType == CardType.Elestral && MainCard == null) { return true; }
            if (card.cardStats.cardType == CardType.Spirit)
            {
                if (MainCard != null) { return true; }
            }
            if (card.CardType == CardType.Rune && MainCard != null)
            {
                if (card.cardStats.Tags.Contains(CardTag.Artifact)) { return true; }
            }
            return false;
        }
        public override void RemoveCard(GameCard card)
        {
            base.RemoveCard(card);
            card.EmpoweredChanged -= EmpowerElestral;
        }

        public override void AllocateTo(GameCard card, bool sendToServer = true)
        {
            base.AllocateTo(card, sendToServer);
            card.EmpoweredChanged += EmpowerElestral;

        }
        #region Slot Menu

        protected override bool GetClickValidation()
        {

            return IsYours;
        }

        protected override List<PopupCommand> GetSlotCommands()
        {
            List<PopupCommand> commands = new List<PopupCommand>();
            //commands.Add(PopupCommand.Create("Inspect", () => InspectCommand()));
            //if (Owner.userId != App.WhoAmI) { commands.Add(PopupCommand.Create("Close", () => CloseCommand())); return commands; }

            if (IsYours)
            {
              
                if (SelectedCard.CardType == CardType.Elestral)
                {
                    commands.Add(PopupCommand.Create("Change Mode", () => ChangeModeCommand()));
                    //this Command is for when 'Real Rules' are not in place and players are allowed to free drag.
                    commands.Add(PopupCommand.Create("Cast", () => CastToSlotCommand(SelectedCard, this)));
                    commands.Add(PopupCommand.Create("Enchant", () => EnchantCommand(1)));

                    if (!SelectedCard.Effect.IsEmpty)
                    {
                        if (SelectedCard.Effect.EffectsList[0].trigger.whenActivate == Abilities.ActivationEvent.YouCan)
                        {
                            if (SelectedCard.Effect.EffectsList[0].ability.CanActivate())
                            {
                                commands.Add(PopupCommand.Create("Activate Effect", () => SelectedCard.Effect.EffectsList[0].ability.Do(SelectedCard)));
                            }
                        }
                    }

                    commands.Add(PopupCommand.Create("Ascend", () => AscendCommand()));
                   
                    //commands.Add(PopupCommand.Create("Nexus", () => NexusCommand(), 1, 1));
                    commands.Add(PopupCommand.Create("Attack", () => AttackCommand()));
                    commands.Add(PopupCommand.Create("DisEnchant", () => DisEnchantCommand(), 1, 0));
                    commands.Add(PopupCommand.Create("Destroy", () => DestroyCommand(), 1, 0));
                }
                else
                {
                   
                    commands.Add(PopupCommand.Create("Move", () => ManageCards(cards, "Select Cards to Move", true, 1, cards.Count)));
                }
                
               
                //commands.Add(PopupCommand.Create("Move", () => MoveCommand()));
               
            }

           
            commands.Add(PopupCommand.Create("Close", () => CloseCommand()));



            return commands;
        }
       
        protected void InspectCommand()
        {

        }

        #region Change Card Mode
        protected override void ChangeModeCommand()
        {
            CardMode current = MainCard.mode;
            CardMode newMode = CardMode.Defense;
            if (current == CardMode.Defense) { newMode = CardMode.Attack; }

            GameManager.Instance.ChangeCardMode(Owner, MainCard, newMode);

            ClosePopMenu();
            Refresh();

        }
        protected override void ClosePopMenu(bool keepSelected = false)
        {
            base.ClosePopMenu(keepSelected);
        }

        #endregion

        #region Enchant Command

        // right now, this command works for dragging an elestral on to the slot and then doing the Cast from there, or doing an Enchant from an Elestral that already exists in the slot
        //currently app just assumes there will be clicking and dragging, so you might be technically Casting a card that you drag to the field first

        public override void CastToSlotCommand(GameCard card, CardSlot from)
        {
            int enchantCount = card.card.SpiritsReq.Count;
            List<GameCard> toShow = Owner.gameField.SpiritDeckSlot.cards;

            string title = $"Select {CardUI.AnySpiritUnicode(enchantCount)} for Cast of {card.name}";
            GameManager.Instance.browseMenu.LoadCards(toShow, title, true, enchantCount, enchantCount);
            GameManager.Instance.browseMenu.CastMode(card, this);
            ClosePopMenu(true);
            BrowseMenu.OnClosed += CastToClose;

            //do something if you drag an Artifact card on to an Elestral slot with an Elestral on it
            if (card.CardType == CardType.Rune && card.cardStats.Tags.Contains(CardTag.Artifact))
            {
               
               
            }
            else
            {
               
            }
            
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
            GameManager.Instance.currentSelector.OnSelectionHandled -= AwaitAttackSelection;
            if (!isSuccess) { return; }
            List<CardSlot> attackTargets = selector.SelectedSlots;
            for (int i = 0; i < attackTargets.Count; i++)
            {
                GameManager.Instance.ElestralAttack(MainCard, attackTargets[i]);
                
            }
            GameManager.Instance.SetSelector();
            Refresh();
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
        public void AscendCommand(List<GameCard> toShow)
        {
            CardActionData d = this.CraftAction(ActionCategory.Ascend, Owner);
            d.AddData(AscendAction.TributedCardKey, SelectedCard.cardId);
            d.AddData(AscendAction.SlotToKey, slotId);

            int cardCount = 1;
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
            BrowseMenu.CastLoad(Owner.gameField.SpiritDeckSlot.cards, title, true, 1, 1, sourceCard, true);
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
                this.AddActionData("card_mode", (int)args.CastMode);
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

        
        protected override void DestroyResponse(bool confirm)
        {
            if (confirm)
            {
                List<GameCard> toMove = new List<GameCard>();
                toMove.AddRange(cards);
                List<GameCard> runes = EmpoweringRunes;
               
                for (int i = 0; i < runes.Count; i++)
                {
                    GameCard rune = runes[i];
                    toMove.Add(rune);
                    toMove.AddRange(rune.EnchantingSpirits);
                }
                for (int i = 0; i < toMove.Count; i++)
                {
                    GameManager.Instance.MoveCard(Owner, toMove[i], toMove[i].Owner.gameField.UnderworldSlot);
                }
                GameManager.Instance.cardSlotMenu.Close();
            }
            Refresh();
        }

        #region Rune Empowering

        protected void EmpowerElestral(GameCard card)
        {
            //if (card == null) { return; }
            //if (card.CurrentSlot != null && card.CurrentSlot != this) { return; }
            //_empoweringRunes = GameManager.ActiveGame.EmpoweringRunes(card);
        }
        #endregion
    }
}

