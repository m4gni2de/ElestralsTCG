using System.Collections;
using System.Collections.Generic;
using Gameplay.Turns;
using UnityEngine;

namespace Gameplay.CardActions
{
    public static class CardActionHelpers
    {
        public static CardActionData GetCraftingAction(this iCraftAction obj)
        {
            return TurnManager.Instance.CraftingAction;
        }
        public static CardActionData CraftAction(this iCraftAction obj, ActionCategory cat, Player p)
        {
            return TurnManager.NewAction(ActionCategory.Ascend, p);
        }
        public static void AddActionData(this iCraftAction obj, string valueKey, object value)
        {
            obj.GetCraftingAction().AddData(valueKey, value);
            string a = $"{obj.GetCraftingAction().GetCategory()} Action: {obj.GetCraftingAction().actionKey}";
            string msg = $"Data with Key '{valueKey}' has been set with a Value of '{value}'";

            string[] lines = StringTools.Array(a, msg);
            App.Log(lines);
        }
    }
}

