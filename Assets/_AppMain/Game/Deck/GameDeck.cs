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
                CardData data = CardService.FindSpiritCard(key);
                return GameCard.Spirit(data, copy);
            }

            if (type == CardType.Elestral)
            {

                ElestralData data = CardService.FindElestralCard(key);
                return GameCard.Elestral(data, copy);
            }
            if (type == CardType.Rune)
            {

                RuneData data = CardService.FindRuneCard(key);
                return GameCard.Rune(data, copy);
            }
            return null;
        }
        #endregion

        public GameDeck(Decklist list)
        {
            uniqueId = UniqueString.GetTempId("dk");
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
                    SpiritDeck.AddCard(card);

                }
                else
                {
                    GameCard card = NewCard(list.Cards[i].key, list.Cards[i].cardType, list.Cards[i].copy);
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
                return MainDeck.AtPosition(0);
            }
        }

        public void RemoveCard(GameCard c, Deck deck)
        {
            deck.InOrder.Remove(c);
        }

        
        public void Draw(Deck deck, int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameCard c = deck.AtPosition(0);
                c.AllocateTo(CardLocation.Hand);
                deck.InOrder.Remove(c);
            }
            
        }
        #endregion
       
    }
}

