using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardsUI;


public class StoneVariant : MonoBehaviour
{
    #region Static Variants
   

    #endregion

    #region Sprite Strings
    protected string FrameString(Card card)
    {
        CardType cType = card.CardType;
        bool isFA = card.isFullArt;

        switch (cType)
        {
            case CardType.Spirit:
                if (isFA)
                {
                    return CardUI.faSpiritFrame;
                }
                return CardUI.SpiritFrame;
            case CardType.Elestral:
                if (isFA)
                {
                    return CardUI.faElestralFrame;
                }
                return CardUI.ElestralFrame;
            case CardType.Rune:
                if (isFA)
                {
                    return CardUI.faRuneFrame;
                }
                return CardUI.RuneFrame;
            default:
                return CardUI.RuneFrame;
        }
    }

    protected string LeftString(Card card)
    {
        CardType cType = card.CardType;
        bool isFA = card.isFullArt;

        switch (cType)
        {
            case CardType.Spirit:
                return "";
            case CardType.Elestral:
                return CardUI.SwordSprite;
            case CardType.Rune:
                return RuneSpriteCode((Rune)card);
            default:
                return CardUI.RuneFrame;
        }
    }

    protected string RightString(Card card)
    {
        CardType cType = card.CardType;
        bool isFA = card.isFullArt;

        switch (cType)
        {
            case CardType.Spirit:
                return "";
            case CardType.Elestral:
                return CardUI.ShieldSprite;
            case CardType.Rune:
                return RuneSpriteCode((Rune)card);
            default:
                return CardUI.RuneFrame;
        }
    }



    protected string RuneSpriteCode(Rune card)
    {
        Rune.RuneType rType = card.GetRuneType;
        switch (rType)
        {
            case Rune.RuneType.Invoke:
                return CardUI.InvokeSprite;
            case Rune.RuneType.Artifact:
                return CardUI.ArtifactSprite;
            case Rune.RuneType.Counter:
                return CardUI.CounterSprite;
            case Rune.RuneType.Stadium:
                return CardUI.StadiumSprite;
            case Rune.RuneType.Divine:
                return CardUI.DivineSprite;
            default:
                return "";
        }
    }
    #endregion

    public string Key;
    public SpriteRenderer FrameSp;
    public SpriteRenderer LeftSp;
    public SpriteRenderer RightSp;

    public TypeStones Stones;

    private bool _isFullArt = false;
    public bool isFullArt
    {
        get
        {
            return _isFullArt;
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public virtual void Set(Card card)
    {
        Show();
        _isFullArt = card.isFullArt;
        SetSprite(card);
        SetStones(card);
    }

    public void SetBlank()
    {
        FrameSp.sprite = null;
        LeftSp.sprite = null;
        RightSp.sprite = null;
        Stones.SetBlank();
    }

    protected virtual void SetStones(Card card)
    {
        Stones.SetStones(card);
    }

    protected virtual void SetSprite(Card card)
    {
        string frameString = FrameString(card);
        FrameSp.sprite = AssetPipeline.ByKey<Sprite>(frameString);

        string leftString = LeftString(card);
        LeftSp.sprite = AssetPipeline.ByKey<Sprite>(leftString);

        string rightString = RightString(card);
        RightSp.sprite = AssetPipeline.ByKey<Sprite>(rightString);
    }

   
    
}
