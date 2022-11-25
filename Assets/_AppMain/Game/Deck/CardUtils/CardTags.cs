using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public enum CardTag
    {
        Empty = 0,
        Spirit = 1,
        Rune = 2,
        Elestral = 3,
        Artifact = 4,
        Invoke = 5,
        Stadium = 6,
        Counter = 7,
        Divine = 8,
        FullArt = 9,
        AltArt = 10,
        AttackMode = 11,
        DefenseMode = 12,
        Aquatic = 13,
        Archaic = 14,
        Avian = 15,
        Behemoth = 16,
        Brute = 17,
        Dragon = 18,
        Dryad = 19,
        Eldritch = 20,
        Ethereal = 21,
        Golem = 22,
        Insectoid = 23,
        Oceanic = 24,
        MultiColor = 25,
        Empowered = 26,
        Empowering = 27,

    }
    public class CardTags 
    {
        #region Properties
        private List<CardTag> _Tags = null;
        public List<CardTag> Tags
        {
            get
            {
                _Tags ??= new List<CardTag>();
                return _Tags;
            }
        }

        #endregion


        #region Initilization
        public static CardTags Create(Card card)
        {
            if (card.CardType == CardType.Elestral)
            {
                return OfElestral((Elestral)card);
            }
            if (card.CardType == CardType.Rune)
            {
                return OfRune((Rune)card);
            }
            return new CardTags(SpiritTags(card));
        }
        public static CardTags OfElestral(Elestral card)
        {
            List<CardTag> tags = ElestralTags(card);
            return new CardTags(tags);
        }
        public static CardTags OfRune(Rune card)
        {
            List<CardTag> tags = RuneTags(card);
            return new CardTags(tags);
        }


        CardTags(List<CardTag> tags)
        {
            Add(tags);
        }
        #endregion



        public bool Contains(CardTag tag)
        {
            return Tags.Contains(tag);
        }
        public void Add(List<CardTag> tags)
        {
            for (int i = 0; i < tags.Count; i++)
            {
                Add(tags[i]);
            }
        }
        public void Add(CardTag tag)
        {
            if (!Tags.Contains(tag))
            {
                Tags.Add(tag);
            }
        }
        public void Remove(CardTag tag)
        {
            if (Tags.Contains(tag))
            {
                Tags.Remove(tag);
            }
        }



        #region Base Tag Generating
        public static List<CardTag> ElestralTags(Elestral card)
        {
            List<CardTag> tags = new List<CardTag>();

            tags.Add(CardTag.Elestral);
            if (card.Data.subType1 != Elestral.SubClass.None) { tags.Add(Enums.ConvertTo<CardTag>(card.Data.subType1.ToString())); }
            if (card.Data.subType2 != Elestral.SubClass.None) { tags.Add(Enums.ConvertTo<CardTag>(card.Data.subType2.ToString())); }


            return tags;
        }

        public static List<CardTag> RuneTags(Rune card)
        {
            List<CardTag> tags = new List<CardTag>();

            tags.Add(CardTag.Rune);
            tags.Add(Enums.ConvertTo<CardTag>(card.GetRuneType.ToString()));

            bool isMulti = false;
            for (int i = 0; i < card.DifferentElements.Count; i++)
            {
                if (card.DifferentElements[i].Code == ElementCode.Any) { isMulti = true;}
            }
            if (isMulti) { tags.Add(CardTag.MultiColor); }
           
            return tags;
        }

        public static List<CardTag> SpiritTags(Card card)
        {
            List<CardTag> tags = new List<CardTag>();
            tags.Add(CardTag.Spirit);
            return tags;
        }

        #endregion


        #region Tag Management
        public void SetMode(CardMode mode)
        {
           
            if (mode == CardMode.Defense)
            {
                if (Contains(CardTag.AttackMode)) { Tags.Remove(CardTag.AttackMode); }
                Add(CardTag.DefenseMode);
            }
            else
            {
                if (Contains(CardTag.DefenseMode)) { Tags.Remove(CardTag.DefenseMode); }
                Add(CardTag.AttackMode);
            }

            
        }
        #endregion
    }
}

