using System.Collections;
using System.Collections.Generic;
using Gameplay.Menus.Popup;
using UnityEngine;

namespace Gameplay
{


    public class RuneSlot : CardSlot, iMainCard
    {
        #region Interface
        public void AddMainCard(GameCard card)
        {
            MainCard = card;
        }
        protected override void SetMainCard(GameCard card)
        {
            if (card.CardType == CardType.Rune)
            {
                Rune rune = (Rune)card.card;
                if (rune.GetRuneType == Rune.RuneType.Stadium)
                {
                    AddMainCard(card);
                }
            }
            
        }
        #endregion
        protected override void SetSlot()
        {
            facing = CardFacing.Both;
            orientation = Orientation.Vertical;
            slotType = CardLocation.Rune;
        }

       

        public override void AllocateTo(GameCard card)
        {
            card.RemoveFromSlot();
            cards.Add(card);
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


            if (card.CardType != CardType.Spirit)
            {
                sortLayer = "CardL2";
                SetCommands(card);
                offsetHeight *= -1;

            }
            else
            {
                offsetHeight *= (1 + cards.Count);
            }
           

            c.SetAsChild(transform, CardScale, sortLayer);
            c.transform.localPosition = new Vector2(0f, offsetHeight);

            card.rect.sizeDelta = rect.sizeDelta;


            c.Flip(card.mode == CardMode.Defense);
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


        protected void Refresh()
        {
            GameManager.Instance.browseMenu.SelectedCards.Clear();
            SelectedCard = null;
        }

        #region Slot Commands
        protected override List<PopupCommand> GetSlotCommands()
        {
            List<PopupCommand> commands = new List<PopupCommand>();
            commands.Add(PopupCommand.Create("Inspect", () => InspectCommand()));
            if (Owner.userId != App.WhoAmI) { commands.Add(PopupCommand.Create("Close", () => CloseCommand())); return commands; }

            commands.Add(PopupCommand.Create("Activate", () => ChangeModeCommand()));
            //commands.Add(PopupCommand.Create("Move", () => MoveCommand()));
            commands.Add(PopupCommand.Create("DisEnchant", () => EnchantCommand(false), 1, 0));
            //commands.Add(PopupCommand.Create("Nexus", () => NexusCommand(), 1, 1));
            //commands.Add(PopupCommand.Create("Attack", () => AttackCommand()));
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
