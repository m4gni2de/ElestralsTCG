using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardsUI;
using System.Threading.Tasks;
using Gameplay;
using Elements;

public static class CardUI 
{
    #region Static Sprites
    public static readonly string ElestralFrame = "stoneElestral";
    public static readonly string faElestralFrame = "fa_ElestralStone";
    public static readonly string RuneFrame = "stoneRune";
    public static readonly string faRuneFrame = "fa_RuneStone";
    public static readonly string SpiritFrame = "stoneSpirit";
    public static readonly string faSpiritFrame = "stoneSpiritFA";


    public static readonly string SwordSprite = "swordSp";
    public static readonly string ShieldSprite = "shieldSp";
    public static readonly string InvokeSprite = "invokeSp";
    public static readonly string CounterSprite = "counterSp";
    public static readonly string ArtifactSprite = "artifactSp";
    public static readonly string DivineSprite = "divineSp";
    public static readonly string StadiumSprite = "stadiumSp";



    public static readonly string CommonSprite = "commonSp";
    public static readonly string UncommonSprite = "uncommonSp";
    public static readonly string RareSprite = "rareSp";
    public static readonly string StellarRareSprite = "stellarRareSp";
    public static readonly string LegendaryRareSprite = "legendaryRareSp";
    public static readonly string ExtraRareSprite = "extraRareSp";

    public static readonly string BorderSprite = "borderSpWhite";
    public static readonly string BlankSprite = "blankCardSp";
    #endregion


    public static readonly string SpiritGlowLeft = "glowSpiritL";
    public static readonly string SpiritGlowRight = "glowSpiritR";

    #region Conversions
    public static string SpriteString(this Rarity rare)
    {
        switch (rare)
        {
            case Rarity.Common:
                return CommonSprite;
            case Rarity.Uncommon:
                return UncommonSprite;
            case Rarity.Rare:
                return RareSprite;
            case Rarity.HoloRare:
                return RareSprite;
            case Rarity.Stellar:
                return StellarRareSprite;
            case Rarity.SecretRare:
                return LegendaryRareSprite;
            default:
                return LegendaryRareSprite;
        }
    }
    #endregion

   
    #region Rune Sprites
    public static Sprite RuneTypeSprite(this Rune.RuneType subClass)
    {
        string st = RuneSpriteCode(subClass);
        return AssetPipeline.ByKey<Sprite>(st);
    }
    public static string RuneSpriteCode(this Rune.RuneType rType)
    {
        switch (rType)
        {
            case Rune.RuneType.Invoke:
                return InvokeSprite;
            case Rune.RuneType.Artifact:
                return ArtifactSprite;
            case Rune.RuneType.Counter:
                return CounterSprite;
            case Rune.RuneType.Stadium:
                return StadiumSprite;
            case Rune.RuneType.Divine:
                return DivineSprite;
            default:
                return "";
        }
    }
    #endregion

    public static Sprite ElestralSubClassSprite(this Elestral.SubClass subClass)
    {
        string st = subClass.ToString().ToLower() + "Sp";
        return AssetPipeline.ByKey<Sprite>(st);
    }
    

    private static Sprite GetSprite(string key)
    {
        return AssetPipeline.ByKey<Sprite>(key);
    }

    public static string SpiritUnicodeString(List<GameCard> spirits)
    {
        string s = "";

        for (int i = 0; i < spirits.Count; i++)
        {
            if (spirits[i].CardType != CardType.Spirit) { App.LogError($"Cannot parse string for cards that are not Spirits."); }

            string code = spirits[i].card.OfElement(0).BaseData.UnicodeString;
            s += code; 
        }
        return s;
    }

    public static Color TextColor(this ElementCode code)
    {
        ElementData fromCode = code;
        return FromHex(fromCode.ColorCode);
       
    }

    public static Color FromHex(string hex)
    {
        hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
        hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
        byte a = 255;//assume fully visible unless specified in hex
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        //Only use alpha if the string has enough characters
        if (hex.Length == 8)
        {
            a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        }
        return new Color32(r, g, b, a);
    }



    #region Extensions
    public static string SpiritUnicode(this List<GameCard> spirits)
    {
        string s = "";

        for (int i = 0; i < spirits.Count; i++)
        {
            if (spirits[i].CardType != CardType.Spirit) { App.LogError($"Cannot parse string for cards that are not Spirits."); }

            string code = spirits[i].card.OfElement(0).BaseData.UnicodeString; ;
            s += code;
        }
        return $" {s} ";
    }

    public static string AnySpiritUnicode(int count)
    {
        string s = "";

        iCustomFont anyType = ElementData.ElementGlyph(ElementCode.Any);
        string keystroke = anyType.UnicodeString;

        for (int i = 0; i < count; i++)
        {
            s += $" {keystroke} ";
        }
        return s;

    }
    #endregion

}
