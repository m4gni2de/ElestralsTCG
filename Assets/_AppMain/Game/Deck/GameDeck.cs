using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Decks;
using Cards;
using Gameplay.Decks;


namespace Gameplay
{
    public class GameDeck
    {
        #region Properties
        public string uniqueId { get; set; }
        private Deck _mainDeck = null;
        public Deck MainDeck { get { return _mainDeck; } }

        private Deck _spiritDeck = null;
        public Deck SpiritDeck { get { return _spiritDeck; } }

        private List<GameCard> _cards = null;
        public List<GameCard> Cards
        {
            get
            {
                if (_cards == null)
                {
                    _cards = new List<GameCard>();
                    for (int i = 0; i < SpiritDeck.Cards.Count; i++)
                    {
                        _cards.Add(SpiritDeck.Cards[i]);
                    }
                    for (int i = 0; i < MainDeck.Cards.Count; i++)
                    {
                        _cards.Add(MainDeck.Cards[i]);
                    }
                }
                return _cards;
            }
        }


        public List<GameCard> CardsInHand
        {
            get
            {
                List<GameCard> cards = new List<GameCard>();

                for (int i = 0; i < Cards.Count; i++)
                {
                    if (Cards[i].location == CardLocation.Hand)
                    {
                        cards.Add(Cards[i]);
                    }
                }

                return cards;
            }
        }


        public GameCard NewCard(string key, CardType type, int copy)
        {
            if (type == CardType.Spirit)
            {
                CardData data = CardService.FindSpirit(key);
                return GameCard.Spirit(data, copy);
            }

            if (type == CardType.Elestral)
            {

                ElestralData data = CardService.FindElestral(key);
                return GameCard.Elestral(data, copy);
            }
            if (type == CardType.Rune)
            {

                RuneData data = CardService.FindRune(key);
                return GameCard.Rune(data, copy);
            }
            return null;
        }
        #endregion

        #region Functions
        public string[] SpiritOrder
        {
            get
            {
                List<string> cards = new List<string>();
                for (int i = 0; i < SpiritDeck.InOrder.Count; i++)
                {
                    cards.Add(SpiritDeck.InOrder[i].cardId);
                }
                return cards.ToArray();
            }
        }

        public string[] NetworkSpiritOrder
        {
            get
            {
                List<string> cards = new List<string>();
                for (int i = 0; i < SpiritDeck.InOrder.Count; i++)
                {
                    cards.Add(SpiritDeck.InOrder[i].card.cardData.cardName);
                }
                return cards.ToArray();
            }
        }
        public string[] DeckOrder
        {
            get
            {
                List<string> cards = new List<string>();
                for (int i = 0; i < MainDeck.InOrder.Count; i++)
                {
                    cards.Add(MainDeck.InOrder[i].cardId);
                }
                return cards.ToArray();
            }
        }

        public string[] NetworkDeckOrder
        {
            get
            {
                List<string> cards = new List<string>();
                for (int i = 0; i < MainDeck.InOrder.Count; i++)
                {
                    cards.Add(MainDeck.InOrder[i].card.cardData.cardName);
                }
                return cards.ToArray();
            }
        }
        #endregion

        public GameDeck(Decklist list)
        {
            if (list.IsUploaded)
            {
                uniqueId = list.UploadCode;
            }
            else
            {
                uniqueId = UniqueString.GetShortId("dk");
            }
            
            _spiritDeck = Deck.SpiritDeck();
            _mainDeck = Deck.MainDeck();

            SeparateCards(list);
            Shuffle(MainDeck);
        }

       


        protected void SeparateCards(Decklist list)
        {

            for (int i = 0; i < list.Cards.Count; i++)
            {
                
                if (list.Cards[i].cardType == CardType.Spirit)
                {
                    GameCard card = NewCard(list.Cards[i].key, CardType.Spirit, list.Cards[i].copy);
                    card.SetNetId(i);
                    card.SetId($"{uniqueId}-{i}");
                    SpiritDeck.AddCard(card);

                }
                else
                {
                    GameCard card = NewCard(list.Cards[i].key, list.Cards[i].cardType, list.Cards[i].copy);
                    card.SetNetId(i);
                    card.SetId($"{uniqueId}-{i}");
                    MainDeck.AddCard(card);
                }

            }
        }

       
        protected void Shuffle(Deck deck)
        {
            deck.Shuffle();
        }

       

        #region Deck Commands
        public GameCard Top
        {
            get
            {
                if (MainDeck.Cards.Count == 0)
                {
                    //do some end game stuff here
                }
                return MainDeck.AtPosition(0);
            }
        }
        

        public void RemoveCard(GameCard c, Deck deck)
        {
            deck.Remove(c);
        }

        
        //public void Draw(Deck deck, int count)
        //{
        //    for (int i = 0; i < count; i++)
        //    {
        //        GameCard c = deck.AtPosition(0);
        //        c.AllocateTo(CardLocation.Hand);
        //        deck.InOrder.Remove(c);
        //    }
            
        //}
        #endregion
       
    }
}

