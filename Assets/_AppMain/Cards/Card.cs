using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;

public enum CardType
{
    None = -1,
    Spirit = 0,
    Elestral = 1,
    Rune = 2
}

public enum Rarity
{
    Promo = -1,
    Common = 0,
    Uncommon = 1,
    Rare = 2,
    HoloRare = 3,
    SecretRare = 4,
    Stellar = 5,
}

public enum ArtType
{
    Regular = 0,
    AltArt = 1,
    FullArt = 2,
    Stellar = 3,
}

public abstract class Card : iCard
{

    #region Static Constructors
    public static Elestral ElestralCard(string key)
    {
        ElestralData data = CardService.FindElestralCard(key);
        return new Elestral(data);
    }

    public static Rune RuneCard(string key)
    {
        RuneData data = CardService.FindRuneCard(key);
        return new Rune(data);
    }

    public static Spirit SpiritCard(string key)
    {
        CardData data = CardService.FindSpiritCard(key);
        return new Spirit(data);
    }


    #endregion

    #region Operatoes and Convertions
    public static Card FromData(CardData data)
    {
        if (data.cardType == (int)CardType.Elestral)
        {
            ElestralData d = (ElestralData)data;
            return new Elestral(d);
        }
        if (data.cardType == (int)CardType.Rune)
        {
            RuneData d = (RuneData)data;
            return new Rune(d);
        }
        return new Spirit(data);
    }
    #endregion

    #region Properties

    #region iCard Interface
    public CardType CardType { get { return GetCardType(); } }
    public iCardData cardData { get { return GetCardData(); } }

    public Rarity GetRarity() { return cardData.rarity; }
    //private bool _isFullArt = false;
    public bool isFullArt { get { return cardData.artType == ArtType.FullArt; } }

    protected virtual iCardData GetCardData()
    {
        return null;
    }
    protected virtual CardType GetCardType()
    {
        return CardType.None;
    }
    #endregion

    #region Spirits and Elements
    private List<Element> _SpiritsReq = null;
    public List<Element> SpiritsReq
    {
        get
        {
            if (_SpiritsReq == null)
            {
                _SpiritsReq = GetElements();
            }
            return _SpiritsReq;
        }
        private set { _SpiritsReq = value; }
    }

    public List<Element> DifferentElements
    {
        get
        {
            List<Element> list = new List<Element>();
            List<ElementCode> codes = new List<ElementCode>();

            for (int i = 0; i < SpiritsReq.Count; i++)
            {
                if (!codes.Contains(SpiritsReq[i].Code))
                {
                    codes.Add(SpiritsReq[i].Code);
                    list.Add(SpiritsReq[i]);
                }
            }

            return list;
        }
    }

    public Element OfElement(int index)
    {
        index = Mathf.Clamp(index, 0, DifferentElements.Count - 1);
        return DifferentElements[index];
    }
    protected List<Element> GetElements()
    {
        List<Element> list = new List<Element>();
        if (cardData == null)
        {
            return list;
        }

        if (cardData.cost1 >= 0) { list.Add(new Element(cardData.cost1)); }
        if (cardData.cost2 >= 0) { list.Add(new Element(cardData.cost2)); }
        if (cardData.cost3 >= 0) { list.Add(new Element(cardData.cost3)); }

        return list;


    }
    #endregion

    #endregion
}

    

    

    



