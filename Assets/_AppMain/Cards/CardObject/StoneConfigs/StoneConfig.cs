using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public struct StoneConfig
{
    #region Enums
    public enum TopIcon
    {
        None = 0,
        Sword = 1,
        Shield = 2,
        Invoke = 3,
        Artifact = 4,
        Counter = 5,
        Stadium = 6,
        Divine = 7,
        SpiritL = 90,
        SpiritR = 91
    }

    public enum FrameType
    {
        None = 0,
        Spirit = 1,
        SpiritFullArt = 2,
        Elestral = 3,
        ElestralFullArt = 4,
        Rune = 5,
        RuneFullArt = 6
    }

    public enum IconWayPoints
    {
        None = 0,
        Elestral = 1,
        Rune = 2,
        ElestralFA = 3,
        RuneFA = 4
    }

    #endregion


    #region Properties

    public IconWayPoints WayPoints;
    public FrameType Frame;
    public TopIcon LeftIcon;
    public TopIcon RightIcon;
    #endregion

    
    private static IconWayPoints GetWaypoints(CardType cl, bool isFullArt)
    {
        IconWayPoints wp = IconWayPoints.None;

        switch (cl)
        {
            case CardType.Spirit:
                return wp;
            case CardType.Elestral:
                if (isFullArt) { wp = IconWayPoints.ElestralFA; } else { wp = IconWayPoints.Elestral; }
                return wp;
            case CardType.Rune:
                if (isFullArt) { wp = IconWayPoints.RuneFA; } else { wp = IconWayPoints.Rune; }
                return wp;
            default:
                return wp;
        }
    }
    private static FrameType GetFrameType(CardType cl, bool isFullArt)
    {
        FrameType fr = FrameType.None;
        switch (cl)
        {
            case CardType.Spirit:
                if (isFullArt) { fr = FrameType.SpiritFullArt; } else { fr = FrameType.Spirit; }
                return fr;
            case CardType.Elestral:
                if (isFullArt) { fr = FrameType.ElestralFullArt; } else { fr = FrameType.Elestral; }
                return fr;
            case CardType.Rune:
                if (isFullArt) { fr = FrameType.RuneFullArt; } else { fr = FrameType.Rune; }
                return fr;
            default:
                return fr;
        }
    }

    private static TopIcon GetRuneIcon(Rune.RuneType rType)
    {
        switch (rType)
        {
            case Rune.RuneType.Invoke:
                return TopIcon.Invoke;
            case Rune.RuneType.Counter:
                return TopIcon.Counter;
            case Rune.RuneType.Artifact:
                return TopIcon.Artifact;
            case Rune.RuneType.Stadium:
                return TopIcon.Stadium;
            case Rune.RuneType.Divine:
                return TopIcon.Divine;
            default:
                return TopIcon.None;
        }
    }


    #region Configs
    public static StoneConfig ElestralConfig(bool isFullArt = false)
    {
        StoneConfig config = new StoneConfig
        {
            LeftIcon = TopIcon.Sword,
            RightIcon = TopIcon.Shield,
            Frame = GetFrameType(CardType.Elestral, isFullArt),
            WayPoints = GetWaypoints(CardType.Elestral, isFullArt),
        };

        return config;
    }

    public static StoneConfig RuneConfig(Rune.RuneType rType, bool isFullArt = false)
    {
        TopIcon runeIcon = GetRuneIcon(rType);
        StoneConfig config = new StoneConfig
        {
            LeftIcon = runeIcon,
            RightIcon = runeIcon,
            Frame = GetFrameType(CardType.Rune, isFullArt),
            WayPoints = GetWaypoints(CardType.Rune, isFullArt),
        };

        return config;
    }

    public static StoneConfig SpiritConfig(bool isFullArt = false)
    {
        
        StoneConfig config = new StoneConfig
        {
            LeftIcon = TopIcon.None,
            RightIcon = TopIcon.None,
            Frame = GetFrameType(CardType.Spirit, isFullArt),
            WayPoints = GetWaypoints(CardType.Spirit, isFullArt),
        };

        return config;
    }

    public static StoneConfig Empty()
    {
        StoneConfig config = new StoneConfig
        {
            LeftIcon = TopIcon.None,
            RightIcon = TopIcon.None,
            Frame = FrameType.None,
            WayPoints = IconWayPoints.None

        };
        return config;
    }
    #endregion

    #region Enum To Sprite
    private static Sprite GetSprite(string key)
    {
        return AssetPipeline.ByKey<Sprite>(key);
    }

    public static async Task<Sprite> IconToSprite(TopIcon icon)
    {
        string assetString = "";

        switch (icon)
        {
            case TopIcon.Sword:
                assetString = CardUI.SwordSprite;
                break;
            case TopIcon.Shield:
                assetString = CardUI.ShieldSprite;
                break;
            case TopIcon.Invoke:
                assetString = CardUI.InvokeSprite;
                break;
            case TopIcon.Artifact:
                assetString = CardUI.ArtifactSprite;
                break;
            case TopIcon.Counter:
                assetString = CardUI.CounterSprite;
                break;
            case TopIcon.Stadium:
                assetString = CardUI.StadiumSprite;
                break;
            case TopIcon.Divine:
                assetString = CardUI.DivineSprite;
                break;
            case TopIcon.SpiritR:
                assetString = CardUI.SpiritGlowRight;
                break;
            case TopIcon.SpiritL:
                assetString = CardUI.SpiritGlowLeft;
                break;
            default:
                break;
        }

        return GetSprite(assetString);
    }

    
    #endregion
}

