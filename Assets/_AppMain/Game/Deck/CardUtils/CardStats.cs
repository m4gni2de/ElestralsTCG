using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    [System.Serializable]
    public class CardStats
    {
        public string name;
        public string title;
        public int attack;
        protected int _attackBase;
        public int defense;
        protected int _defenseBase;
        public CardType cardType;
        public CardMode cardMode;
        public string effect;
        public CardTags Tags = null;
        public List<ElementCode> CardElements = null;

        #region Initialization
        public CardStats(GameCard card)
        {
            CardElements = new List<ElementCode>();

            name = card.name;
            title = card.card.cardData.cardName;
            if (card.DefaultCardType == CardType.Elestral)
            {
                SetElestral((Elestral)card.card);
            }
            else if (card.DefaultCardType == CardType.Rune)
            {
                SetRune((Rune)card.card);
            }
            else if (card.DefaultCardType == CardType.Spirit)
            {
                SetSpirit((Spirit)card.card);
            }

            SetElements(card.card);
        }

        protected void SetElestral(Elestral card)
        {
            attack = card.Data.attack;
            defense = card.Data.defense;
            cardType = CardType.Elestral;
            Tags = CardTags.OfElestral(card);
            effect = card.Data.effect;
            
        }
        protected void SetRune(Rune card)
        {
            attack = 0;
            defense = 0;
            cardType = CardType.Rune;
            Tags = CardTags.OfRune(card);
            effect = card.cardData.effect;
        }
        protected void SetSpirit(Spirit card)
        {
            attack = 0;
            defense = 0;
            cardType = CardType.Spirit;
            effect = "";
        }

        protected void SetElements(Card card)
        {
            for (int i = 0; i < card.SpiritsReq.Count; i++)
            {
                if (!CardElements.Contains(card.SpiritsReq[i].Code))
                {
                    CardElements.Add(card.SpiritsReq[i].Code);
                }
            }
        }
        #endregion

        #region Updating
        
        #endregion
    }
}

