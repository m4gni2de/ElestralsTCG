using System.Collections;
using System.Collections.Generic;
using Gameplay.Menus.Popup;
using Gameplay.Turns;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay
{
    public class ElestralSlot : CardSlot, iMainCard
    {
        #region Interface
        public void AddMainCard(GameCard card)
        {
            MainCard = card;
        }
        protected override void SetMainCard(GameCard card)
        {
            if (card.CardType == CardType.Elestral)
            {
                AddMainCard(card);
            }
            
        }
        #endregion

        protected override void SetSlot()
        {
            facing = CardFacing.FaceUp;
            orientation = Orientation.Both;
            slotType = CardLocation.Elestral;
        }

       
        public override void AllocateTo(GameCard card)
        {

            card.RemoveFromSlot();
            cards.Add(card);
            if (card.CardType == CardType.Elestral)
            {
                SetMainCard(card);
            }
            card.AllocateTo(this);
            DisplayCardObject(card);

        }


        protected override void DisplayCardObject(GameCard card)
        {
            CardView c = card.cardObject;
            string sortLayer = SortLayer;
            float height = rect.sizeDelta.y;
            float offsetVal = .05f;
            float offsetHeight = height * offsetVal;
            Vector2 basePos = new Vector2(0f, 0f);


            if (card.cardStats.cardType != CardType.Spirit)
            {
                sortLayer = "CardL2";
                SetCommands(card);
                offsetHeight *= -1;
                if (card.mode == CardMode.Defense)
                {
                    card.cardObject.Rotate(true);
                }

            }
            else
            {
                offsetHeight *= (1 + cards.Count);
                c.SetSortingOrder(-cards.Count);
                
            }


            c.SetAsChild(transform, CardScale, sortLayer);
            c.transform.localPosition = new Vector2(0f, offsetHeight);

            card.rect.sizeDelta = rect.sizeDelta;
            
            c.Flip();
        }


        

        protected override void SetCommands(GameCard card)
        {
            base.SetCommands(card);
        }
        protected override void ClickCard(GameCard card)
        {
            //OpenSlotMenu();
            
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

        #region Slot Menu
        public override void OpenPopMenu()
        {
            base.OpenPopMenu();
        }
        protected override bool GetClickValidation()
        {
            
            bool isYours = Owner.userId == App.WhoAmI;
            bool validate = isYours;



            return validate;
        }

        protected override List<PopupCommand> GetSlotCommands()
        {
            List<PopupCommand> commands = new List<PopupCommand>();
            commands.Add(PopupCommand.Create("Inspect", () => InspectCommand()));
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
                GameManager.Instance.browseMenu.OnMenuClose += AwaitEnchantClose;
            }
           
            

        }
        protected void AwaitEnchantClose(List<GameCard> selectedCards, CardMode cMode)
        {
            GameManager.Instance.browseMenu.OnMenuClose -= AwaitEnchantClose;
            if (cMode == CardMode.None) { return; }

            Field f = GameManager.Instance.arena.GetPlayerField(Owner);
            List<GameCard> cardsList = new List<GameCard>();
            for (int i = 0; i < selectedCards.Count; i++)
            {
                cardsList.Add(selectedCards[i]);
            }
            GameCard Selected = SelectedCard;
            CardSlot slot = f.ElestralSlot(0, true);

            GameManager.Instance.NormalEnchant(Owner, Selected, cardsList, slot, cMode);
            Refresh();

        }
       
        #endregion
        protected void NexusCommand()
        {

        }

        #region Attack Command
        protected void AttackCommand()
        {
            if (TurnManager.ValidateStartBattle())
            {
                if (TurnManager.IsBattlePhase)
                {
                    SlotSelector attackSelector = SlotSelector.AttackSlots(Owner.Opponent, 1);
                    GameManager.Instance.SetSelector(attackSelector);
                    GameManager.Instance.currentSelector.OnSelectionHandled += AwaitAttackSelection;
                    ClosePopMenu();
                }
            }
            
        }
        protected void AwaitAttackSelection(bool isSuccess, List<CardSlot> attackTargets)
        {
            if (!isSuccess) { return; }
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

        protected void Refresh()
        {
            GameManager.Instance.browseMenu.SelectedCards.Clear();
            SelectedCard = null;
        }
        #endregion
    }
}

