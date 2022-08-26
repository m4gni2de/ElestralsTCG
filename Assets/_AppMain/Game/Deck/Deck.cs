using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Decks;
using Cards;

namespace Gameplay.Decks
{
    public class Deck
    {
        #region Static Constructors
        public static Deck SpiritDeck()
        {
            return new Deck(DeckType.Spirit);
        }
        public static Deck MainDeck()
        {
            return new Deck(DeckType.Main);
        }
        #endregion

        #region Indexer
        protected GameCard this[int index]
        {
            get
            {
                return InOrder[index];
            }
        }
        #endregion
        public enum DeckType
        {
            Spirit = 0,
            Main = 1,
            Side = 2
        }

        

        #region Properties
        protected DeckType deckType { get; set; }

        private List<GameCard> _cards = null;
        public List<GameCard> Cards
        {
            get
            {
                _cards ??= new List<GameCard>();
                return _cards;
            }
        }

        private List<GameCard> _inOrder = null;
        public List<GameCard> InOrder
        {
            get
            {
                _inOrder ??= new List<GameCard>();
                return _inOrder;
            }
        }
        #endregion

        Deck(DeckType type)
        {
            deckType = type;
        }

        public void AddCard(GameCard card)
        {
            if (card != null)
            {
                Cards.Add(card);
                ToTop(card);
            }
        }

        

        protected void AddAtIndex(int pos, GameCard card)
        {

            if (pos > InOrder.Count) { pos = InOrder.Count - 1; }
            InOrder.Insert(pos, card);
            ReorderCards();
        }


        #region Deck Commands
        protected void ReorderCards()
        {
            for (int i = 0; i < InOrder.Count; i++)
            {
                InOrder[i].SetDeckPosition(i);
            }
        }
        public void ToTop(GameCard card)
        {
            AddAtIndex(0, card);
        }

        public void Shuffle()
        {
            List<GameCard> newOrder = new List<GameCard>();

            for (int i = 0; i < InOrder.Count; i++)
            {
                int rand = Random.Range(0, newOrder.Count);
                newOrder.Insert(rand, InOrder[i]);
            }

            _inOrder = newOrder;
        }

       
        public void Draw()
        {
            GameCard c = AtPosition(0);
            c.AllocateTo(CardLocation.Hand);
            InOrder.Remove(c);
        }
        public GameCard AtPosition(int atIndex)
        {
            return InOrder[atIndex];
        }
        #endregion
    }
}

