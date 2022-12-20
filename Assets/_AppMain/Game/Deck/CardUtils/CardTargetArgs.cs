using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay
{
    public enum TargetCriteria
    {
        None = 0,
        CardID = 1,
        Player = 2,
        IsElement = 3,
        IsElementEnchanted = 4,
        CardType = 50,
        CardLocation = 60,
        RuneType = 70,
        SubClass = 80,
        WithName = 90,

    }

    public class CardTargetArgs
    {

        private List<TargetCriteria> _criteria = null;
        public List<TargetCriteria> Criteria { get { _criteria ??= new List<TargetCriteria>(); return _criteria; } private set { _criteria = value; } }
        #region Parameter Lists
        protected List<string> _players = null;
        public List<string> players { get { _players ??= new List<string>(); return _players; } set { _players = value; } }

        protected List<string> _cardId = null;
        public List<string> cardId { get { _cardId ??= new List<string>(); return _cardId; } }

        protected List<CardType> _cardType = null;
        public List<CardType> cardType { get { _cardType ??= new List<CardType>(); return _cardType; } set { _cardType = value; } }

        protected List<CardLocation> _locations = null;
        public List<CardLocation> locations { get { _locations ??= new List<CardLocation>(); return _locations; } set { _locations = value; } }

        protected List<ElementCode> _elements = null;
        public List<ElementCode> elements { get { _elements ??= new List<ElementCode>(); return _elements; } set { _elements = value; } }

        protected List<Rune.RuneType> _runeTypes = null;
        public List<Rune.RuneType> runeTypes { get { _runeTypes ??= new List<Rune.RuneType>(); return _runeTypes; } set { _runeTypes = value; } }

        protected List<Elestral.SubClass> _subClass = null;
        public List<Elestral.SubClass> subClass { get { _subClass ??= new List<Elestral.SubClass>(); return _subClass; } set { _subClass = value; } }
        #endregion

        #region Non-List Filters
        protected string _withName = "";
        public string withName { get { return _withName; } set { _withName = value; } }

        protected int _minQty = 0;
        public int minQty { get { return _minQty; } set { _minQty = value; } }

        protected int _maxQty = 0;
        public int maxQty { get { return _maxQty; } set { _maxQty = value; } }
        #endregion

        public bool MeetsTargetCriteria(GameCard card)
        {
           
            if (CardMatch(card)) { return true; }


            if (!PlayerMatch(card)) { return false; }
            if (!CardTypeMatch(card)) { return false; }
            if (!LocationMatch(card)) { return false; }
            if (!ElementMatch(card)) { return false; }
            if (!RuneTypeMatch(card)) { return false; }
            if (!SubClassMatch(card)) { return false; }

            return true;
        }

        #region Validate

        /// <summary>
        /// Card Match overrides all over criteria. If the specific card ID matches, then the rest of the Arguments don't matter
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        protected bool CardMatch(GameCard card)
        {
            return cardId.Contains(card.cardId);
        }
        protected bool PlayerMatch(GameCard card)
        {
            if (players.Count == 0) { return true; }
            return players.Contains(card.Owner.userId);
        }
        protected bool CardTypeMatch(GameCard card)
        {
            if (cardType.Count == 0) { return true; }
            return cardType.Contains(card.CardType);
        }
        protected bool LocationMatch(GameCard card)
        {
            if (locations.Count == 0) { return true; }
            if (card.CurrentSlot != null)
            {
                return locations.Contains(card.CurrentSlot.slotType);
            }
            return false;
            
        }
        protected bool ElementMatch(GameCard card)
        {
            if (elements.Count == 0) { return true; }

            for (int i = 0; i < card.EnchantingSpiritTypes.Count; i++)
            {
                if (elements.Contains(card.EnchantingSpiritTypes[i])) { return true; }
            }
            return false;
        }
        protected bool RuneTypeMatch(GameCard card)
        {
            if (runeTypes.Count == 0) { return true; }
            if (card.CardType != CardType.Rune) { return false; }
            Rune r = (Rune)card.card;
            return runeTypes.Contains(r.GetRuneType);
        }
        protected bool SubClassMatch(GameCard card)
        {
            if (subClass.Count == 0) { return true; }
            if (card.CardType != CardType.Elestral) { return false; }
            Elestral r = (Elestral)card.card;
            if (subClass.Contains(r.Data.subType1)) { return true; }
            if (r.Data.subType2 != Elestral.SubClass.None)
            {
                return (subClass.Contains(r.Data.subType2));
            }
            return false;
        }

        #endregion

        #region Parsing 
        public List<Player> PlayersList
        {
            get
            {
                List<Player> list = new List<Player>();
                for (int i = 0; i < players.Count; i++)
                {
                    Player p = Game.FindPlayer(players[i]);
                    if (p != null) { list.Add(p); }
                }
                return list;
            }
        }
        #endregion
    }


   
}

