using System.Collections;
using System.Collections.Generic;
using Gameplay.Menus.Popup;
using UnityEngine;

namespace Gameplay
{
    public class StadiumSlot : RuneSlot
    {
        #region Interface
        
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

        protected override bool GetIsOpen()
        {
            return true;
        }

        protected override void WakeSlot()
        {
            facing = CardFacing.Both;
            orientation = Orientation.Vertical;
            slotType = CardLocation.Stadium;
        }


        public override bool ValidateCard(GameCard card)
        {
            if (card.CardType == CardType.Rune)
            {
                Rune rune = (Rune)card.card;
                if (rune.GetRuneType == Rune.RuneType.Stadium)
                {
                    return true;
                }
                return false;
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
        }


        #region Slot Commands
        protected override List<PopupCommand> GetSlotCommands()
        {
            List<PopupCommand> commands = new List<PopupCommand>();


            commands.Add(PopupCommand.Create("Inspect", () => InspectCommand()));
            if (IsYours)
            {
                if (!SelectedCard.IsFaceUp)
                {
                    commands.Add(PopupCommand.Create("Enchant", () => ChangeModeCommand(), 1, 0));
                }
                else
                {
                    commands.Add(PopupCommand.Create("Cast", () => CastToSlotCommand(SelectedCard, this)));
                    commands.Add(PopupCommand.Create("Enchant", () => EnchantCommand(1)));
                    commands.Add(PopupCommand.Create("DisEnchant", () => DisEnchantCommand(), 1, 0));
                    
                }
                
            }

            commands.Add(PopupCommand.Create("Close", () => CloseCommand()));



            return commands;
        }

   

        #endregion
    }
}

