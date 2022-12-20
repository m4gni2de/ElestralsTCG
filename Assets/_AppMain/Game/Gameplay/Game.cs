using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Decks;
using Cards;
using System;
using Gameplay.Data;
using Gameplay.Turns;
using UnityEngine.Events;
using System.Runtime.InteropServices.WindowsRuntime;
using Users;
using Gameplay.Networking;
using UnityEditor;
using UnityEngine.SocialPlatforms;
using System.Data;
#if UNITY_EDITOR
using static UnityEditor.Experimental.GraphView.GraphView;
#endif

namespace Gameplay
{
    public enum GameMode
    {
        PlayMode = 0,
        TargetMode = 1
    }

    public enum ConnectionType
    {
        Offline = 0,
        Remote = 1,
        P2P = 2,
    }

    public enum LocationScope
    {
        All = 0,
        OnTarget = 1,
        OnField = 2,
        OnYourField = 3,
        OnOpponentField = 4,
        InSpiritDeck = 5,
        InUnderWorld = 6,
        InDeck = 7,
    }

    public enum PlayerScope
    {
        None = 0,
        All = 100,
        User = 101,
        Opponent = 102,
    }

    [System.Serializable]
    public class Game : CommandSystem
    {
        #region Network Properties
        
        public virtual bool IsOnline() { return false; }
        public ConnectionType connType { get; private set; }

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
                    if (item.slotId.ToLower() == slotId.ToLower()) { return item; }
                }
            }
            //App.LogFatal($"No Card Slot with Id {slotId} exists in this Game.");
            return null;
        }

        public static List<GameCard> GetPlayerCardsInPlay(Player p)
        {
            Field f = p.gameField;
            List<GameCard> list = new List<GameCard>();
            for (int i = 0; i < f.cardSlots.Count; i++)
            {
                CardSlot slot = f.cardSlots[i];
                if (!slot.IsInPlay) { continue; }
                for (int j = 0; j < slot.cards.Count; j++)
                {
                    list.Add(slot.cards[j]);
                }
            }
            return list;
        }

        public static List<GameCard> GetAllCardsInPlay()
        {

            List<GameCard> list = new List<GameCard>();
            foreach (var item in GameManager.ActiveGame.players)
            {
                Field f = item.gameField;
                for (int i = 0; i < f.cardSlots.Count; i++)
                {
                    CardSlot slot = f.cardSlots[i];
                    if (!slot.IsInPlay) { continue; }
                    for (int j = 0; j < slot.cards.Count; j++)
                    {
                        list.Add(slot.cards[j]);
                    }
                }
            }

            return list;
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
                        if (players[i].userId == App.Account.Id || players[i].IsLocal)
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

        public static void SetGameState(Game game)
        {
            game.gameState = new GameState(game);
        }
        private GameState _state = null;
        public GameState gameState
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                if (_state != null)
                {
                    _state.Print(false);
                }
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
        public static Game ConnectToNetwork(string gameId, ConnectionType connType)
        {
            Game g = new Game();
            g.gameId = gameId;
            g.isOnline = true;
            g.connType = connType;
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

        
        //public void AddPlayer(string userid, string deckKey, bool isLocal)
        //{
        //    Decklist decklist = Decklist.Load(deckKey);
        //    Player player = new Player(userid, decklist, isLocal);
        //    AddPlayer(player);
        //}

        public void AddLocalPlayer(string userid, string deckKey, string username)
        {
            Decklist decklist = null;
            for (int i = 0; i < App.Account.DeckLists.Count; i++)
            {
                Decklist d = App.Account.DeckLists[i];
                if (d.DeckKey == deckKey)
                {
                    decklist = d;
                }
            }
            Player player = new Player(0, userid, username, decklist, true);
            AddPlayer(player);
        }
        

        
        public void AddPlayer(Player p)
        {
            if (!players.Contains(p))
            {
                players.Add(p);
                //gameLog.WriteLog($"Added Player {p.userId}"!);

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
        private static CardEventSystem _eventSystem = null;
        private static CardEventSystem eventSystem { get { _eventSystem ??= new CardEventSystem(true); return _eventSystem; } }

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
        public static void PhaseStart(Turn turn, int index)
        {
            OnNewPhaseStart?.Invoke(turn, index);
            eventSystem.PhaseStart.Call(turn, turn.Phases[index]);
        }

        //public static event Action<GameMode> OnGameModeSet;
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

      


        #region Rune Empowering

        //public List<GameCard> EmpoweringRunes(GameCard elestral)
        //{
        //    List<GameCard> result = new List<GameCard>();
        //    foreach (var item in EmpoweredRunes)
        //    {
        //        if (item.Value == elestral)
        //        {
        //            result.Add(item.Key);
        //        }
        //    }
        //    return result;
        //}
        private Dictionary<GameCard, GameCard> _EmpoweredRunes = null;
        public Dictionary<GameCard, GameCard> EmpoweredRunes
        {
            get
            {
                _EmpoweredRunes ??= new Dictionary<GameCard, GameCard>();
                return _EmpoweredRunes;
            }
        }

        public static void Empower(GameCard rune, GameCard elestral)
        {
            Game active = GameManager.ActiveGame;

            if (!active.EmpoweredRunes.ContainsKey(rune))
            {
                active.EmpoweredRunes.Add(rune, elestral);
                
                NetworkPipeline.SendEmpowerChange(rune.cardId, elestral.cardId, true);
            }
        }
       

        public static void UnEmpowerFromElestral(ElestralCard elestral)
        {
            Game active = GameManager.ActiveGame;
            List<GameCard> goners = new List<GameCard>();
            foreach (var item in active.EmpoweredRunes)
            {
                if (item.Value == elestral) { goners.Add(item.Key); }
            }
            for (int i = 0; i < goners.Count; i++)
            {
                active.EmpoweredRunes.Remove(goners[i]);
                NetworkPipeline.SendEmpowerChange(goners[i].cardId, elestral.cardId, false);
            }
            
        }
        public static void UnEmpower(GameCard rune)
        {
            Game active = GameManager.ActiveGame;


            if (active.EmpoweredRunes.ContainsKey(rune))
            {
                GameCard elestral = active.EmpoweredRunes[rune];
                active.EmpoweredRunes.Remove(rune);
                NetworkPipeline.SendEmpowerChange(rune.cardId, elestral.cardId, false);
            }
        }
        #endregion




        #region As Server
        public void SyncPlayerCard(ushort id, int index, string cardId, string realId, string slotId)
        {
            Player p = GameManager.ByNetworkId(id);
        }
        public void PlayerJoined(ushort player, string username, string gameId)
        {
            //if (this.gameId != gameId) { return; }

        }

        #endregion
    }
}

