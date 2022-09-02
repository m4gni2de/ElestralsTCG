using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Decks;


namespace Gameplay
{
    public class Player 
    {
        #region Global Properties
        public bool IsActive
        {
            get
            {
                if (GameManager.ActivePlayer == this)
                {
                    return true;
                }
                return false;
            }
        }
        #endregion

        #region Properties
        private string _userId = "";
        public string userId
        {
            get { return _userId; }
        }

        public Decklist decklist { get; private set; }
        private GameDeck _deck = null;
        public GameDeck deck { get { return _deck; } }

        private Field _gameField = null;
        public Field gameField
        {
            get
            {
                if (_gameField == null)
                {
                    _gameField = GameManager.Instance.arena.GetPlayerField(this);
                }
                return _gameField;
            }
        }
        #endregion

        public Player(string user, Decklist list)
        {
            _userId = user;
            decklist = list;
            _deck = new GameDeck(list);
        }

        #region In Game Properties
        public bool isReady = false;
        public void ToggleReady(bool ready)
        {
            isReady = ready;
        }
        #endregion

        public void Draw(int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameCard c = deck.Top;
                deck.RemoveCard(c, deck.MainDeck);
                GameManager.Instance.MoveCard(gameField.DeckSlot, c, gameField.HandSlot);
            }
        }
        public void Mill(int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameCard c = deck.Top;
                deck.RemoveCard(c, deck.MainDeck);
                GameManager.Instance.MoveCard(gameField.DeckSlot, c, gameField.UnderworldSlot);
            }
        }
    }
}

