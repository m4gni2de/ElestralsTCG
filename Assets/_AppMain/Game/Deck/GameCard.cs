using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;
using CardsUI;
using Gameplay.GameCommands;
using System.Security.Cryptography;
using System;

namespace Gameplay
{
    public enum CardLocation
    {
        removed = -1,
        Elestral = 0,
        Rune = 1,
        Stadium = 2,
        Underworld = 3,
        Deck = 4,
        SpiritDeck = 5,
        Hand = 6
    }
    public enum CardMode
    {
        None = 0,
        Attack = 1,
        Defense = 2,
    }

   

    [System.Serializable]
    public class GameCard: iGameMover
    {
        public string name;
        public CardStats cardStats;
        #region Visual Properties
        public bool IsFaceUp { get { return cardObject.IsFaceUp; } }
        protected bool isSelected { get; private set; }
        #endregion

        #region Network Linking
        //the id of the card in as it appears in order on the uploaded deck list. 
        private int _networkId;
        public int NetworkId { get => _networkId; }
        public void SetNetId(int id)
        {
            _networkId = id;
        }
        public bool IsNetwork = false;
       
        #endregion

        #region Static Constructors
        public static GameCard Spirit(CardData data, int copy)
        {
            Spirit spirit = new Spirit(data);
            return new GameCard(spirit, copy);
        }
        public static GameCard Elestral(ElestralData data, int copy)
        {
            Elestral card = new Elestral(data);
            return new GameCard(card, copy);
        }
        public static GameCard Rune(RuneData data, int copy)
        {
            Rune card = new Rune(data);
            return new GameCard(card, copy);
        }
        #endregion

        #region Properties
        private Card _card = null;
        public Card card { get { return _card; } }
        public CardLocation location;
        public CardMode mode;
        public int deckPosition;
        public string cardId;

        private string _slotId = "";
            
        public string slotId { get { return _slotId; } }

       

        public CardView cardObject { get; set; }
        private NetworkCard _networkCard = null;
        public NetworkCard NetworkCard
        {
            get
            {
                if (_networkCard == null)
                {
                    _networkCard = cardObject.GetComponent<NetworkCard>();
                }
                return _networkCard;
            }
        }
        private RectTransform _rect = null;
        public RectTransform rect
        {
            get
            {
                if (cardObject == null) { return null; }
                if (_rect == null)
                {
                    _rect = cardObject.GetComponent<RectTransform>();
                }
                return _rect;
            }
        }

        
        public List<ElementCode> EnchantingSpiritTypes
        {
            get
            {
                List<ElementCode> list = new List<ElementCode>();
                if (CurrentSlot == null || CurrentSlot.MainCard == null) { return list; }
                SingleSlot slot = (SingleSlot)CurrentSlot;
                for (int i = 0; i < slot.EnchantingSpirits.Count; i++)
                {
                    list.AddRange(slot.EnchantingSpirits[i].cardStats.CardElements);

                }
                return list;
            }
        }

        public List<GameCard> EnchantingSpirits
        {
            get
            {
                List<GameCard> list = new List<GameCard>();
                if (CurrentSlot == null || CurrentSlot.GetType() != typeof(SingleSlot)) { return list; }
                SingleSlot slot = (SingleSlot)CurrentSlot;
                list.AddRange(slot.EnchantingSpirits);
                return list;
            }
        }
        #endregion

        #region Functions
        public Player Owner
        {
            get
            {
                for (int i = 0; i < GameManager.ActiveGame.players.Count; i++)
                {
                    Player p = GameManager.ActiveGame.players[i];
                    if (p.deck.Cards.Contains(this))
                    {
                        return p;
                    }
                }
                return null;
            }
        }

        private CardSlot _CurrentSlot = null;
        public CardSlot CurrentSlot
        {
            get { return _CurrentSlot; }
        }

        public bool IsYours
        {
            get
            {
                return Owner == GameManager.ActiveGame.You;
            }
        }
        #endregion


       
        GameCard(Card data, int copy)
        {
            _card = data;
            location = CardLocation.Deck;
            name = $"{_card.cardData.cardName} - {copy}";
            cardStats = new CardStats(this);
            isSelected = false;
        }
        public void SetId(string id)
        {
            cardId = id;
        }

       
        public void SetObject(CardView obj)
        {
            //do something to tie the carddata of the Object to the GameCard
            cardObject = obj;
            obj.CardName = name;
            obj.CardSessionId = cardId;
        }


        public void SetDeckPosition(int index)
        {
            deckPosition = index;
        }
        public void SetCardMode(CardMode cardMode, bool sendToNetwork = true)
        {
            mode = cardMode;
            if (CardType == CardType.Elestral)
            {
                if (mode == CardMode.Defense)
                {
                    cardObject.Rotate(true);
                    rect.sizeDelta = new Vector2(CurrentSlot.rect.sizeDelta.y, CurrentSlot.rect.sizeDelta.x);
                }
                else
                {
                    cardObject.Rotate(false);
                    rect.sizeDelta = CurrentSlot.rect.sizeDelta;
                }
                
                if (sendToNetwork)
                {
                    NetworkCard.SendRotation();
                }
                
            }
           
        }
        

        #region Slot Management
        public void ToggleValidSlot(bool isValid)
        {

        }
        public void AllocateTo(CardSlot slot, bool sendToServer = true)
        {
           
            _CurrentSlot = slot;
            location = slot.slotType;
            _slotId = slot.slotId;
            rect.sizeDelta = slot.rect.sizeDelta;

            if (sendToServer)
            {
                NetworkPipeline.SendNewCardSlot(cardId, _slotId);
            }
            
        }
       
       
        public void RemoveFromSlot()
        {
            if (CurrentSlot == null) { return; }
            CurrentSlot.RemoveCard(this);
        }
        public void ReAddToSlot(bool reAddCommands)
        {
            if (CurrentSlot == null) { return; }
            CurrentSlot.ReAddCard(this, reAddCommands);
        }
        public void RefreshAtSlot()
        {
            if (CurrentSlot == null) { return; }
            CurrentSlot.RefreshAtSlot(this);
        }


        public event Action<bool> OnSelectChanged;
        public void SelectCard(bool toggle, bool sendToServer = true)
        {
            bool current = isSelected;
            if (toggle)
            {
                cardObject.Images.SetColor("Border", Color.yellow);
                cardObject.Images.ShowSprite("Border");
            }
            else
            {
                cardObject.Images.SetColor("Border", Color.clear);
                cardObject.Images.HideSprite("Border");
            }

            isSelected = toggle;

            if (sendToServer)
            {
                if (current != toggle)
                {
                    OnSelectChanged?.Invoke(toggle);
                }
            }
            
            //cardObject.SelectCard(toggle);
        }
        #endregion

        #region Information/Stats Management
        public CardType CardType { get { return cardStats.cardType; } }
        public CardType DefaultCardType { get { return card.CardType; } }
        #endregion

       
        #region Card Events
        protected void OnEnchantment(GameCard source, GameCard spirit)
        {
            Game.OnEnchantmentSend(source, spirit);
            
        }

        #endregion



        #region Network Events
        public void ToggleNetwork(bool network)
        {
            
            IsNetwork = network;
           
        }

        private void SendSelectStatus(bool isSelected)
        {
            if (IsNetwork)
            {
                NetworkPipeline.SendCardSelect(this, isSelected);
            }
        }
        public struct NetworkData
        {
            //public ushort OwnerId { get; set; }
            public int networkId { get; set; }
            public string sessionId { get; set; }
            public string setKey { get; set; }
            public string slotId { get; set; }
        }

        public void SendCardToServer()
        {
            NetworkData data = CardNetworkData();
            NetworkPipeline.SendNewCard(data);
        }

        public NetworkData CardNetworkData()
        {
            NetworkData data = new NetworkData();
            //data.OwnerId = Owner.lobbyId;
            data.networkId = NetworkId;
            data.sessionId = cardId;
            data.setKey = card.cardData.setKey;
            data.slotId = slotId;

            return data;
        }

        #endregion


        #region Card Object 
        public void SetPosition(Vector3 newPos, bool sendToNetwork = true)
        {
            cardObject.transform.position = newPos;

            if (sendToNetwork)
            {
                NetworkCard.SendPosition();
            }
        }
        public void MovePosition(Vector3 moveBy, bool sendToNetwork = true)
        {
            this.MoveGamePosition(cardObject.transform, moveBy);
            if (sendToNetwork)
            {
                NetworkCard.SendPosition();
            }
        }
        public void FlipCard(bool toFaceDown, bool sendToNetwork = true)
        {
            cardObject.Flip(toFaceDown);
            if (sendToNetwork)
            {
                NetworkCard.Flip(toFaceDown);
            }
        }
        public void SetScale(Vector3 newScale, bool sendToNetwork = true)
        {
            cardObject.SetScale(newScale);
            if (sendToNetwork)
            {
                NetworkCard.SendScale(newScale);
            }
        }
        #endregion

    }


}

