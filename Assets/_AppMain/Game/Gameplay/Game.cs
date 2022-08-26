using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Decks;
using Cards;
using Gameplay.GameCommands;
using System;

namespace Gameplay
{
    [System.Serializable]
    public class Game
    {
        #region Static Properties
        public static Game ActiveGame
        {
            get
            {
                return GameManager.ActiveGame;
            }
        }
        #endregion
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
                        if (players[i].userId == App.Account.Name)
                        {
                            _You = players[i];
                        }
                    }
                }
                return _You;
            }
        }

        public Player ActivePlayer { get; set; }
        
        #endregion

        #region Properties
        private List<Player> _players = null;
        public List<Player> players { get { _players ??= new List<Player>(); return _players; } }
        public string gameId { get; set; }

        [SerializeField]
        private GameLog _gameLog = null;
        public GameLog gameLog { get { return _gameLog; } }

        #region Commands
        public GameCommand ActiveCommand { get; set; }
        private List<GameCommand> _commands = null;
        public List<GameCommand> Commands
        {
            get
            {
                _commands ??= new List<GameCommand>();
                return _commands;
            }
        }
        public static void LoadCommand<T>(T comm) where T : GameCommand
        {
            if (ActiveGame == null) { return; }
            ActiveGame.Commands.Add(comm);


        }
        #endregion


        #endregion



        #region Initialization
        public static Game NewGame()
        {
            return new Game();

        }

        Game()
        {
            gameId = UniqueString.GetTempId("gid");
            _gameLog = new GameLog(gameId);
        }

        public void AddPlayer(string userid, string deckKey)
        {
            Decklist decklist = Decklist.Load(deckKey);
            Player player = new Player(userid, decklist);
            players.Add(player);
        }
        public void AddPlayer(string userid, Decklist list)
        {
            Player player = new Player(userid, list);
            players.Add(player);
        }

        
    
        #endregion


        public void PlayerDraw(Player p, int count)
        {
            
            p.Draw(count);
        }



        
        #region Game Commands
        public void AddCommand(GameCommand comm)
        {
            if (!Commands.Contains(comm))
            {
                Commands.Add(comm);
                comm.OnEventComplete += RemoveCommand;

            }
        }

        protected void RemoveCommand(GameCommand comm)
        {
            comm.OnEventComplete -= RemoveCommand;
            if (Commands.Contains(comm))
            {
                Commands.Remove(comm);
                gameLog.Add(JsonUtility.ToJson(comm.Args));
            }
        }
        #endregion
    }
}

