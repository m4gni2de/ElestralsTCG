using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Decks;
using System;
using Gameplay.CardActions;
using UnityEngine.Events;
using Databases;

using Gameplay.Networking;
using nsSettings;
using System.Threading.Tasks;
using Users;

namespace Gameplay
{
    public class Player
    {


        public static int DeckSize = 60;
        public static Player LocalPlayer
        {
            get
            {
                for (int i = 0; i < GameManager.ActiveGame.players.Count; i++)
                {
                    Player p = GameManager.ActiveGame.players[i];
                    if (p.IsLocal) { return p; }
                }

                return null;
            }
        }
        #region Network Properties
        public ushort lobbyId { get; private set; }
        public bool IsLocal { get; private set; }
        public bool IsHost { get; private set; }
        public string deckUploadKey { get; private set; }
        public bool IsLoaded { get; private set; } = false;
        private List<ServerCard> _serverCards = null;
        public List<ServerCard> ServerCards { get { _serverCards ??= new List<ServerCard>(); return _serverCards; } }
        #endregion

        #region Networking
        


        #region Message Responses

        #endregion

        #endregion

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
        private CardAction _activeAction = null;
        public CardAction ActiveAction
        {
            get
            {
                return _activeAction;
            }
        }
        #endregion

        #region Properties
        private string _userId = "";
        public string userId
        {
            get { return _userId; }
        }

        //eventually have this changed to show the actual username
        public string username
        {
            get { return userId; }
        }

        public Decklist decklist { get; private set; }
        public string[] GetDeckList()
        {
            string[] list = new string[decklist.Cards.Count];
            for (int i = 0; i < decklist.Cards.Count; i++)
            {
                list[i] = decklist.Cards[i].key;
            }
            return list;
        }
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

        #region Constructors
        Player(string userName, bool isLocal)
        {
            _userId = userName;
            this.IsLocal = isLocal;

        }
        public Player(ushort tempGameId, string userId, bool isLocal) : this(userId, isLocal)
        {
            lobbyId = tempGameId;
        }

        public Player(string user, Decklist list, bool isLocal) : this(user, isLocal)
        {
            LoadDeckList(list);
        }
        public void SetOfflineLobbyId(ushort id)
        {
            lobbyId = id;
        }

        public void LoadDeckList(Decklist list, bool shuffle = true)
        {
            decklist = list;
            _deck = new GameDeck(list);
            deck.SeparateCards(list);
            if (shuffle)
            {
                deck.Shuffle(deck.MainDeck);
            }
            IsLoaded = true;
        }
        //public void VerifyDeck(UploadedDeckDTO dto)
        //{
        //    if (decklist == null)
        //    {
        //        Decklist de = dto;
        //        LoadDeckList(de, false);
        //    }

        //}
        public void SetBlankDeck(string key, string title)
        {
            decklist = Decklist.Empty(userId, key, title);
            _deck = GameDeck.FromRemote(key, title);

        }
        public void AddRemoteCardToDecklist(int cardIndex, string realId, string uniqueId)
        {
            Decklist.DeckCard card = decklist.AddCard(realId);
            GameCard remoteCard = deck.NewCard(card.key, card.cardType, card.copy);
            remoteCard.SetId(uniqueId);
            remoteCard.SetNetId(cardIndex);
            _deck.AddCard(remoteCard, false);
        }
        #endregion

        #region In Game Properties
        public bool isReady = false;
        public void ToggleReady(bool ready)
        {
            isReady = ready;
        }
        public bool CanEnchantCard(GameCard card, int spiritCount = -1)
        {
            if (spiritCount < 0) { spiritCount = card.card.SpiritsReq.Count; }

            return deck.SpiritDeck.InOrder.Count >= spiritCount;
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
            Draw(5, DrawAction.DrawActionType.GameStart);
        }
        public void Draw(int count, DrawAction.DrawActionType drawType)
        {
            for (int i = 0; i < count; i++)
            {
                //GameManager.Instance.PlayerDraw(this, AtPosition(true, i), gameField.DeckSlot, gameField.HandSlot, drawType);
                DrawAction ac = new DrawAction(this, AtPosition(true, i), gameField.DeckSlot, gameField.HandSlot, drawType);
                GameManager.Instance.PlayerDraw(ac);
            }
        }

        public void Draw(int count)
        {
            for (int i = 0; i < count; i++)
            {
                //GameManager.Instance.PlayerDraw(this, AtPosition(true, i), gameField.DeckSlot, gameField.HandSlot, DrawAction.DrawActionType.FromEffect);
                DrawAction ac = new DrawAction(this, AtPosition(true, i), gameField.DeckSlot, gameField.HandSlot, DrawAction.DrawActionType.FromEffect);
                GameManager.Instance.PlayerDraw(ac);
            }
        }
        public void Mill(int count)
        {
            for (int i = 0; i < count; i++)
            {
                //GameManager.Instance.PlayerDraw(this, AtPosition(true, i), gameField.DeckSlot, gameField.UnderworldSlot, DrawAction.DrawActionType.Mill);
                DrawAction ac = new DrawAction(this, AtPosition(true, i), gameField.DeckSlot, gameField.UnderworldSlot, DrawAction.DrawActionType.Mill);
                GameManager.Instance.PlayerDraw(ac);
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

        #region Remote Connection

        #endregion


    }
}

