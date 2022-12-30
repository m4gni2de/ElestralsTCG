using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    
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
        public string slotLocation;

      
        #region Change History
        private List<string> _history = null;
        public List<string> History { get { _history ??= new List<string>(); return _history; } }
        #endregion

        #region Initialization
        public CardStats(GameCard card)
        {
            CardElements = new List<ElementCode>();

            name = card.cardName;
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

            if (card.CurrentSlot != null)
            {
                slotLocation = card.CurrentSlot.slotId;
            }
            else
            {
                slotLocation = null;
            }

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

       
        protected void LogChange<T>(string propName, T newVal)
        {

        }
        public void UpdateCardType(CardType type)
        {
            cardType = type;
        }
        public void UpdateAttackDefense(int newAtk, int newDef)
        {

        }
        #endregion
    }
}

