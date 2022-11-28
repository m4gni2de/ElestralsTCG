using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;
using Databases;
using System.Globalization;

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

    private List<string> _altArts = null;
    public List<string> AltArts
    {
        get
        {
            if (_altArts == null)
            {
                _altArts = new List<string>();

                string whereClause = $"baseKey = '{cardData.cardKey}' AND image <> '{cardData.image}'";
                List<qUniqueCard> sharedCards = CardService.GetAllWhere<qUniqueCard>(CardService.qUniqueCardView, whereClause);

                for (int i = 0; i < sharedCards.Count; i++)
                {
                    _altArts.Add(sharedCards[i].setKey);
                }
            }
            return _altArts;
           
        }
    }


    private List<string> reprints = null;
    public List<string> Reprints
    {
        get
        {
            if (reprints == null)
            {
                reprints = new List<string>();

                string whereClause = $"baseKey = '{cardData.cardKey}' AND image = '{cardData.image}'";
                List<qUniqueCard> sharedCards = CardService.GetAllWhere<qUniqueCard>(CardService.qUniqueCardView, whereClause);

                for (int i = 0; i < sharedCards.Count; i++)
                {
                    reprints.Add(sharedCards[i].setKey);
                }
            }
            return reprints;

        }
    }


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









