using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;
using Databases;
using System.Globalization;
using TouchControls;
using GlobalUtilities;
using System;
using Gameplay;

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
    Gold = 6,
    GoldStellar = 7,
}

public enum ArtType
{
    Regular = 0,
    AltArt = 1,
    FullArt = 2,
    Stellar = 3,
}

[SortableObject]
public abstract class Card : iCard
{

   

    #region Operators and Convertions
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

    public static implicit operator Card(qUniqueCard dto)
    {
        if (dto.cardClass == (int)CardType.Elestral) { CardData data = new ElestralData(dto); Elestral e = new Elestral((ElestralData)data); return e; }
        if (dto.cardClass == (int)CardType.Rune) { CardData data = new RuneData(dto); Rune r = new Rune((RuneData)data); return r; }
        if (dto.cardClass == (int)CardType.Spirit) { CardData data = new CardData(dto); Spirit s = new Spirit(data); return s; }
        return null;
    }


    public static string CardLayer1 = "Card";
    public static string CardLayer2 = "CardL2";
    public static string CardLayer3 = "CardSpotlight";
    #endregion

    #region Properties

    #region iCard Interface
    [SortableValue(SortBy.CardType)] public CardType CardType { get { return GetCardType(); } }
    public iCardData cardData { get { return GetCardData(); } }

    [SortableValue(SortBy.Rarity)] public Rarity GetRarity { get => cardData.rarity; }
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
    public bool IsGold
    {
        get
        {
            bool gold = false;
            if (cardData.rarity == Rarity.Gold || cardData.rarity == Rarity.GoldStellar) { gold = true; }
            return gold;
        }
    }

    #region Compare
    public int Compare<T>(T x, T y)
    {
        ComparedTo compare = x.CompareTo(y);
        return (int)compare;
    }
    #endregion
    #endregion

    #region Card Effect
    protected CardEffect _effect = null;
    public CardEffect Effect
    {
        get
        {
            if (_effect == null)
            {
                _effect = CardEffect.GetEffect(cardData.baseKey);
            }
            return _effect;
        }
        set
        {
            _effect = value;
        }
    }
    #endregion

    #region Collection
    private List<string> _altArts = null;
    public List<string> AltArts
    {
        get
        {
            if (_altArts == null)
            {
                _altArts = new List<string>();

                string whereClause = $"baseKey = '{cardData.baseKey}' AND image <> '{cardData.image}'";
                List<qUniqueCard> sharedCards = CardService.GetAllWhere<qUniqueCard>(CardService.qUniqueCardView, whereClause);

                for (int i = 0; i < sharedCards.Count; i++)
                {
                    _altArts.Add(sharedCards[i].setKey);
                }
            }
            return _altArts;
           
        }
    }


    private List<string> _duplicates = null;
    public List<string> DuplicatePrints
    {
        get
        {
            if (_duplicates == null)
            {
                _duplicates = new List<string>();

                string whereClause = $"baseKey = '{cardData.baseKey}' AND image = '{cardData.image}'";
                List<qUniqueCard> sharedCards = CardService.GetAllWhere<qUniqueCard>(CardService.qUniqueCardView, whereClause);

                for (int i = 0; i < sharedCards.Count; i++)
                {
                    _duplicates.Add(sharedCards[i].setKey);
                }
            }
            return _duplicates;

        }
    }
    
    public int GetQuantityOwned()
    {
        return CardCollection.GetQuantity(this);
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

    #region Operators
    public static Card FromKey(string setKey)
    {
        qUniqueCard dto = CardService.ByKey<qUniqueCard>(CardService.qUniqueCardView, "setKey", setKey);
        Card c = dto;
        return c;
    }

    public static implicit operator Card(Decks.Decklist.DeckCard dc)
    {
        qUniqueCard dto = CardService.ByKey<qUniqueCard>(CardService.qUniqueCardView, "setKey", dc.key);
        Card c = dto;
        c._quantity = dc.copy;
        return c;
    }
    public static Card FromDeckCard(Decks.Decklist.DeckCard dc)
    {
        qUniqueCard dto = CardService.ByKey<qUniqueCard>(CardService.qUniqueCardView, "setKey", dc.key);
        Card c = dto;
        c._quantity = dc.copy;
        return c;
    }
    #endregion

    #endregion

   

    #region Specific Card Type Values
    [SortableValue(SortBy.Attack)]
    public virtual int Attack
    {
        get
        {
            if (CardType == CardType.Elestral)
            {
                ElestralData d = (ElestralData)cardData;
                return d.attack;
            }
            return 0;
        }
    }
    [SortableValue(SortBy.Defense)]
    public virtual int Defense
    {
        get
        {
            if (CardType == CardType.Elestral)
            {
                ElestralData d = (ElestralData)cardData;
                return d.defense;
            }
            return 0;
        }
    }
    [SortableValue(SortBy.CardElement)]
    public int GetElementValue
    {
        get
        {
            int value = 0;
            List<ElementCode> elements = new List<ElementCode>();

            for (int i = 0; i < SpiritsReq.Count; i++)
            {
                if (!elements.Contains(SpiritsReq[i].Code))
                {
                    elements.Add(SpiritsReq[i].Code);
                    value += (int)SpiritsReq[i].Code * 3;

                }
                else
                {
                    value += 1;
                }

            }
            return value;
        }
    }

    #endregion

    #region Sorting
    [SortableValue(SortBy.Name)] protected string Title { get { return cardData.cardName; } }
    [SortableValue(SortBy.Cost)] protected int Cost { get { return SpiritsReq.Count; } }
    [SortableValue(SortBy.CardSetName)] protected string SetName { get { return cardData.setCode; } }
    [SortableValue(SortBy.CardSetDate)] protected DateTime WhenReleased
    {
        get
        {
            var gameSet = CardLibrary.GetGameSet(cardData.setCode);
            if (gameSet != null) { return gameSet.releaseDate; }
            return DateTime.MaxValue;
        }
    }
    [SortableValue(SortBy.CardSetNumber)] protected int NumberInSet { get { return cardData.setNumber; } }

    public string DisplayName
    {
        get
        {
            string st = "";
           

            st = $"{cardData.cardName}";
            if (cardData.artType == ArtType.AltArt)
            {
                st += " - (Alternate Art)";
            }
            else if (cardData.artType == ArtType.FullArt)
            {
                st += " - (Full Art)";
            }
            if (cardData.artType == ArtType.Stellar || cardData.rarity == Rarity.Stellar || cardData.rarity == Rarity.GoldStellar)
            {
                st = $"Stellar {cardData.cardName}";
            }

            return st;
        }
    }

    private int _quantity = 1;
    [SortableValue(SortBy.Quantity)]
    public int quantity
    {
        get { return _quantity; }
    }
    #endregion




}









