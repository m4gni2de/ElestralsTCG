using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Decks;
using Cards;
using Gameplay.GameCommands;
using System;
using Gameplay.Data;
using Gameplay.Turns;
using UnityEngine.Events;

namespace Gameplay
{
    public enum GameMode
    {
        PlayMode = 0,
        TargetMode = 1
    }

    [System.Serializable]
    public class Game
    {
        
        #region Global Properties
        protected Player _You = null;
        public Player You
        {
            get
            {
                if (_You == null)
                {
                    for (int i = 0; i < players.Count; i++)
                    {
                        if (players[i].userId == App.Account.Id)
                        {
                            _You = players[i];
                        }
                    }
                }
                return _You;
            }
        }

        public Player ActivePlayer
        {
            get
            {
                return GameManager.Instance.ActivePlayer;
            }
        }
        
        public Player GetOpponent(Player p)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i] != p)
                {
                    return players[i];
                }
            }
            App.LogError("Can't find opponent.");
            return null;
        }
        #endregion

        #region Properties
        private List<Player> _players = null;
        public List<Player> players { get { _players ??= new List<Player>(); return _players; } }
        public string gameId { get; set; }

        public GameLog gameLog { get { return GameManager.Instance.gameLog; } }

        public GameData gameData { get; set; }
        public GameMode gameMode { get; set; }
        
        #endregion

     
        #region Initialization
        public static Game NewGame(string id)
        {
            return new Game(id);
        }
        public static Game RandomGame()
        {
            GameData data = GameData.RandomGame();
            return new Game(data);
        }

        public Game(string id)
        {
            gameId = id;
        }
        public Game(GameData data)
        {
            gameId = data.gameId;
            AddPlayer(data.opponent);
            
        }

        
        public void AddPlayer(string userid, string deckKey)
        {
            Decklist decklist = Decklist.Load(deckKey);
            Player player = new Player(userid, decklist);
            AddPlayer(player);
        }
       
        protected void AddPlayer(Player p)
        {
            if (!players.Contains(p))
            {
                players.Add(p);
            }
        }
        #endregion

        #region Game Mode
       
        
        public void SetGameMode(GameMode mode)
        {
            GameMode prev = gameMode;
            gameMode = mode;
            if (prev != mode)
            {
                
                
            }
            
        }
        #endregion

        #region Game Events
        public static event Action<TargetArgs> OnNewTargetParams;
        public static void SetTargetParams(TargetArgs args = null)
        {
            OnNewTargetParams?.Invoke(args);

        }

        public static event Action<Turn> OnNewTurnStart;
        public static void StartNewTurn(Turn turn)
        {
            OnNewTurnStart?.Invoke(turn);
        }

        public static event Action<Turn, int> OnNewPhaseStart;
        public static void StartNewPhase(Turn turn, int index)
        {
            OnNewPhaseStart?.Invoke(turn, index);
        }

        public static event Action<GameMode> OnGameModeSet;
        #endregion

        #region Game Information
        public int CardsInHand(Player p)
        {
            int count = 0;
            for (int i = 0; i < p.deck.Cards.Count; i++)
            {
                if (p.deck.Cards[i].location == CardLocation.Hand)
                {
                    count += 1;
                }
            }
            return count;
        }
        #endregion

    }
}

