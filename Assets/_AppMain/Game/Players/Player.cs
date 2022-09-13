using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Decks;
using System;
using Gameplay.CardActions;
using UnityEngine.Events;

namespace Gameplay
{
    public class Player 
    {
        #region Global Properties
        public bool IsActive
        {
            get
            {
                if (GameManager.Instance.ActivePlayer == this)
                {
                    return true;
                }
                return false;
            }
        }
        public Player Opponent
        {
            get
            {
                if (GameManager.ActiveGame == null) { return null; }
                for (int i = 0; i < GameManager.ActiveGame.players.Count; i++)
                {
                    if (GameManager.ActiveGame.players[i] != this)
                    {
                        return GameManager.ActiveGame.players[i];
                    }
                }
                return null;
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


        #region Drawing Actions
        public GameCard DrawPreview(bool isMainDeck)
        {
            if (isMainDeck)
            {
                return deck.Top;
            }
            return deck.SpiritDeck.AtPosition(0);
            
        }


        public GameCard AtPosition(bool isMainDeck, int index)
        {
            if (isMainDeck)
            {
                return deck.MainDeck.AtPosition(index);
            }
            return deck.SpiritDeck.AtPosition(index);

        }

        public void StartingDraw()
        {
            Draw(5, DrawAction.DrawType.GameStart);
        }
        public void Draw(int count, DrawAction.DrawType drawType)
        {
            for (int i = 0; i < count; i++)
            {
                GameManager.Instance.PlayerDraw(this, AtPosition(true, i), gameField.DeckSlot, gameField.HandSlot, drawType);
            }
        }
        
        public void Draw(int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameManager.Instance.PlayerDraw(this, AtPosition(true, i), gameField.DeckSlot, gameField.HandSlot, DrawAction.DrawType.FromEffect);
            }
        }
        public void Mill(int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameManager.Instance.PlayerDraw(this, AtPosition(true, i), gameField.DeckSlot, gameField.UnderworldSlot, DrawAction.DrawType.Mill);
            }
        }

        public void Shuffle()
        {
            GameManager.Instance.PlayerShuffle(this);
        }
        #endregion

        #region Event Watching
        public event Action<GameCard> OnCardDraw;
        public void SendCardDraw(GameCard card)
        {
            OnCardDraw?.Invoke(card);
        }
        #endregion

        #region Card Target Scope 
        public List<GameCard> GetTargets(TargetArgs args)
        {
            List<GameCard> targets = args.CardTargets(deck.Cards);
            return targets;
        }
        

       
        
        #endregion
    }
}

