using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;
using Gameplay;
using Gameplay.CardActions;

public class Rune : Card
{
    public enum RuneType
    {
        none = 0,
        Invoke = 1,
        Counter = 2,
        Artifact = 3,
        Stadium = 4,
        Divine = 5
    }

    #region Properties
    private RuneData _data = null;
    public RuneData Data { get { return _data; } }

    public RuneType GetRuneType
    {
        get
        {
            return (RuneType)Data.runeType;
        }
    }
    #endregion

    #region Overrides
    protected override iCardData GetCardData() { return Data; }
    protected override CardType GetCardType() { return CardType.Rune; }
    #endregion

    public Rune(RuneData data)
    {
        _data = data;
    }


   //public static void EquipRune(Player p, GameCard source, CardSlot to, List<GameCard> spirits, GameCard equippedElestral, bool fromFaceDown)
   // {
   //     EnchantActionType en = EnchantActionType.Normal;
   //     if (fromFaceDown) { en = EnchantActionType.FromFaceDown; }

   //     EnchantAction ac = EnchantAction.EquipEnchant(p, source, to, spirits, equippedElestral, en);
   //     GameManager.Instance.DoEnchant(ac);
   // }


   


}
