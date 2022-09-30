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
using System.Runtime.InteropServices.WindowsRuntime;
using Users;
using RiptideNetworking;
using Gameplay.Networking;

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
        #region Network Properties
        public static OnlineGame ServerGame { get; set; }
        public virtual bool IsOnline() { return false; }


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
        protected List<Player> _players = null;
        public List<Player> players { get { _players ??= new List<Player>(); return _players; } }
        public string gameId { get; set; }

        public GameLog gameLog { get { return GameManager.Instance.gameLog; } }

        public GameData gameData { get; set; }
        public GameMode gameMode { get; set; }
        public bool isOnline { get; private set; } = false;

        #endregion


       

        #region Initialization
        protected Game()
        {

        }
        public static Game ConnectTo(string gameId)
        {
            Game g = new Game();
            g.gameId = gameId;
            g.isOnline = true;
            return g;
        }
        public static Game ConnectToNetwork(string gameId)
        {
            Game g = new Game();
            g.gameId = gameId;
            g.isOnline = true;
            return g;
        }
       
        public static Game RandomGame()
        {
            GameData data = GameData.RandomGame();
            Game g = new Game(data);
            return g;
        }

       
        public Game(GameData data)
        {
            gameId = data.gameId;
            AddPlayer(data.opponent);
            
        }

        
        public void AddPlayer(string userid, string deckKey, bool isLocal)
        {
            Decklist decklist = Decklist.Load(deckKey);
            Player player = new Player(userid, decklist, isLocal);
            AddPlayer(player);
        }
        

        
        public void AddPlayer(Player p)
        {
            if (!players.Contains(p))
            {
                players.Add(p);
                gameLog.WriteLog($"Added Player {p.userId}"!);

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

        #region Turn Based Events
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

        #region Action Events
        public static event Action<CardAction> OnActionDeclared;
        public static void ActionDeclared(CardAction ac)
        {
            OnActionDeclared?.Invoke(ac);
        }
        #endregion

        #region Card Based Events
        public static event Action<GameCard, GameCard> OnEnchantment;
        public static void OnEnchantmentSend(GameCard source, GameCard spirit)
        {
            OnEnchantment?.Invoke(source, spirit);
        }
        public static event Action<GameCard, GameCard> OnDisEnchantment;
        public static void OnDisEnchantmentSend(GameCard source, GameCard spirit)
        {
            OnDisEnchantment?.Invoke(source, spirit);
        }
        #endregion

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
        public static Player FindPlayer(string userId)
        {
            for (int i = 0; i < GameManager.ActiveGame.players.Count; i++)
            {
                Player p = GameManager.ActiveGame.players[i];
                if (p.userId == userId) { return p; }
            }
            App.LogFatal($"No player with Id {userId} exists in this Game.");
            return null;
        }
        public static GameCard FindCard(string cardId)
        {
            for (int i = 0; i < GameManager.ActiveGame.players.Count; i++)
            {
                Player p = GameManager.ActiveGame.players[i];
                foreach (var item in p.deck.Cards)
                {
                    if (item.cardId == cardId) { return item; }
                }
            }
            //App.LogFatal($"No card with Id {cardId} exists in this Game.");
            return null;
        }

        public static CardSlot FindSlot(string slotId)
        {
            for (int i = 0; i < GameManager.ActiveGame.players.Count; i++)
            {
                Player p = GameManager.ActiveGame.players[i];
                foreach (var item in p.gameField.cardSlots)
                {
                    if (item.slotId == slotId) { return item; }
                }
            }
            //App.LogFatal($"No Card Slot with Id {slotId} exists in this Game.");
            return null;
        }

        public static CardSlot FindSlot(int slotIndex)
        {
            for (int i = 0; i < GameManager.ActiveGame.players.Count; i++)
            {
                Player p = GameManager.ActiveGame.players[i];
                foreach (var item in p.gameField.cardSlots)
                {
                    if (item.index == slotIndex) { return item; }
                }
            }
            //App.LogFatal($"No Card Slot with Id {slotId} exists in this Game.");
            return null;
        }
        #endregion

    }
}

