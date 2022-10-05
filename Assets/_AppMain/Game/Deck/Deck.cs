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
        protected DeckType _deckType { get; set; }
        public DeckType deckType { get { return _deckType; } }

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
            _deckType = type;
        }

        public void AddCard(GameCard card, bool toTop = true)
        {
            if (card != null)
            {
                Cards.Add(card);
                if (toTop)
                {
                    ToTop(card);
                }
                else
                {
                    ToBottom(card);
                }
                
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

        public void ToBottom(GameCard card)
        {
            int index = InOrder.Count;
            AddAtIndex(index, card);
        }

        public void Shuffle(List<string> idsInOrder = null)
        {
            List<GameCard> newOrder = new List<GameCard>();

            if (idsInOrder == null)
            {
                for (int i = 0; i < InOrder.Count; i++)
                {
                    int rand = Random.Range(0, newOrder.Count);
                    newOrder.Insert(rand, InOrder[i]);
                }
            }
            else
            {
                for (int i = 0; i < idsInOrder.Count; i++)
                {
                    GameCard card = Game.FindCard(idsInOrder[i]);
                    newOrder.Insert(i, card);
                }
            }
            

            _inOrder = newOrder;
            ReorderCards();
        }

       
       public void Remove(GameCard c)
        {
            InOrder.Remove(c);
            c.SetDeckPosition(-1);
            ReorderCards();
        }
        public GameCard AtPosition(int atIndex)
        {
            return InOrder[atIndex];
        }
        #endregion
    }
}

