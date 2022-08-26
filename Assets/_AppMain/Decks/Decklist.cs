using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Databases;
using Cards;
using System;

namespace Decks
{
    [System.Serializable]
    public class Decklist 
    {
        public class CardSlot
        {
            public string key;
            public CardType cardType;
            public int copy;
        }

        private List<CardSlot> _cards = null;
        public List<CardSlot> Cards { get { _cards ??= new List<CardSlot>(); return _cards; } }
        #region Properties
        private string _deckName;
        public string Name { get { return _deckName; } }

        private string _key;
        public string DeckKey { get { return _key; } }

        private DateTime _whenCreated = DateTime.MinValue;
        public DateTime WhenCreated { get {return _whenCreated; } }

        private string _owner;
        public string Owner { get { return _owner; } }

        private string _uploadCode = string.Empty;
        public string UploadCode { get { return _uploadCode; } }

        private List<string> _spiritList = null;
        public List<string> SpiritList { get { _spiritList ??= new List<string>(); return _spiritList; } }

        private List<string> _cardList = null;
        public List<string> CardList
        {
            get
            {
                _cardList ??= new List<string>();
                return _cardList;
            }
        }

        


        #endregion

        #region Static Functions
        public static Decklist Load(string deckKey)
        {
            DeckDTO dto = DeckService.LoadDeck(deckKey);
            List<qDeckList> cards = DeckService.LoadCards(deckKey);
            return new Decklist(dto, cards);

        }
        #endregion

        Decklist(DeckDTO deck, List<qDeckList> cards)
        {
            _deckName = deck.title;
            _key = deck.deckKey;
            _owner = deck.owner;
            _whenCreated = deck.whenCreated;
            _uploadCode = deck.uploadCode;

            for (int i = 0; i < cards.Count; i++)
            {
                for (int j = 0; j < cards[i].qty; j++)
                {
                    CardSlot c = NewCard(cards[i].cardKey, cards[i].cardClass, j);
                    AddCard(c);
                }
            }
        }



        #region Adding Cards
        protected static CardSlot NewCard(string key, int cardType, int indexOfCopy)
        {
            return new CardSlot { key = key, cardType = (CardType)cardType, copy = indexOfCopy };
        }
       
        protected void AddCard(CardSlot c)
        {
            Cards.Add(c);
        }
        #endregion

    }
}

