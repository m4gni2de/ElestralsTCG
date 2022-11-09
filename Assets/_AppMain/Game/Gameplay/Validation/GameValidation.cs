using System.Collections;
using System.Collections.Generic;
using System.Data;
using Gameplay;
using Gameplay.CardActions;
using UnityEngine;

public static class GameValidation 
{
    #region Rules/Validations
    public static int ElestralSlotCount = 4;
    #endregion
    #region Errors
    private static List<string> _ErrorList = null;
    public static List<string> ErrorList { get { _ErrorList ??= new List<string>(); return _ErrorList; } }

    private static void AddError(string msg)
    {
        ErrorList.Add(msg);
    }
    #endregion

    public static bool CanEnchant(Player player, GameCard toEnchant, CastActionType enchantType)
    {
        ErrorList.Clear();
        
        switch (enchantType)
        {
            case CastActionType.Set:
                if (!player.CanEnchantCard(toEnchant)) { AddError("You do not have enough Spirits to pay for cost of card."); }
                SetRune(player, toEnchant);
                break;
            case CastActionType.Cast:
                if (!player.CanEnchantCard(toEnchant)) { AddError("You do not have enough Spirits to pay for cost of card."); }
                NormalEnchant(player, toEnchant);
                break;
            case CastActionType.Enchant:
                if (!player.CanEnchantCard(toEnchant, 1)) { AddError("You do not have enough Spirits to pay for this Re-Enchantment."); }
                break;
            case CastActionType.DisEnchant:
                break;
            case CastActionType.FromFaceDown:
                if (!player.CanEnchantCard(toEnchant)) { AddError("You do not have enough Spirits to pay for cost of card."); }
                break;
            default:
                break;
        }

        return ErrorList.Count > 0;
    }

    private static void SetRune(Player player, GameCard toEnchant)
    {
        
        if (player.gameField.OpenRuneSlots() == 0) { AddError("No Available Slots for Rune."); }
        
    }

    private static void NormalEnchant(Player player, GameCard toEnchant)
    {
        if (toEnchant.card.CardType == CardType.Rune)
        {
            if (toEnchant.cardStats.Tags.Contains(CardTag.Counter))
            {
                AddError("Counter Runes must be played Face-Down for 1 Turn before activation.");
            }
            if (toEnchant.cardStats.Tags.Contains(CardTag.Artifact))
            {
                int yourElCount = player.gameField.OpenElestralSlots();
                int oppElCount = player.Opponent.gameField.OpenElestralSlots();
                if (yourElCount + oppElCount == ElestralSlotCount * 2) { AddError("There are no Elestrals to Empower with this Artifact."); }

            }
        }
        else
        {
            if (player.gameField.OpenElestralSlots() == 0) { AddError("No Available Slots for Elestral."); }
        }
    }


    private static void TryEnchantRune(Player player, GameCard rune)
    {
        
    }
}
