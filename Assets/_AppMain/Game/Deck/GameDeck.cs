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
        public string deckName { get; set; }
        [SerializeField] private Deck _mainDeck = null;
        public Deck MainDeck { get { return _mainDeck; } }

        private Deck _spiritDeck = null;
        public Deck SpiritDeck { get { return _spiritDeck; } }

        public List<GameCard> Cards
        {
            get
            {
                List<GameCard> _cards = new List<GameCard>();
                for (int i = 0; i < SpiritDeck.Cards.Count; i++)
                {
                    _cards.Add(SpiritDeck.Cards[i]);
                }
                for (int i = 0; i < MainDeck.Cards.Count; i++)
                {
                    _cards.Add(MainDeck.Cards[i]);
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

       

        public GameDeck(Decklist list)
        {
            uniqueId = UniqueString.GetShortId("dk");
            deckName = list.DeckName;

            _spiritDeck = Deck.SpiritDeck();
            _mainDeck = Deck.MainDeck(); 
        }

        #region From Remote
        public GameDeck(string key, string title)
        {
            uniqueId = key;
            deckName = title;
            _spiritDeck = Deck.SpiritDeck();
            _mainDeck = Deck.MainDeck();
        }
        public static GameDeck FromRemote(string key, string title)
        {
            return new GameDeck(key, title);
        }

       
        public void AddCard(GameCard card, bool toTop = true)
        {
            if (card.CardType == CardType.Spirit)
            {
                SpiritDeck.AddCard(card);
            }
            else
            {
                MainDeck.AddCard(card, toTop);
            }
        }

        #endregion


        public void SeparateCards(Decklist list)
        {
            
            for (int i = 0; i < list.Cards.Count; i++)
            {
                
                if (list.Cards[i].cardType == CardType.Spirit)
                {
                    GameCard card = NewCard(list.Cards[i].key, CardType.Spirit, list.Cards[i].copy);
                    card.SetNetId(i);
                    card.SetId($"{uniqueId}-{i}");
                    SpiritDeck.AddCard(card);
                    card.ToggleNetwork(true);

                }
                else
                {
                    GameCard card = NewCard(list.Cards[i].key, list.Cards[i].cardType, list.Cards[i].copy);
                    card.SetNetId(i);
                    card.SetId($"{uniqueId}-{i}");
                    MainDeck.AddCard(card);
                    card.ToggleNetwork(true);
                }

                

            }
        }

       
        public void Shuffle(Deck deck = null)
        {
            if (deck == null)
            {
                deck = MainDeck;
            }
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

