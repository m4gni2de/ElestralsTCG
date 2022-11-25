//
//using System.Collections.Generic;
//using System.Runtime.InteropServices.WindowsRuntime;
//using UnityEngine;
//using UnityEngine.Networking.Types;
//using UnityEngine.SocialPlatforms;

//[System.Serializable]
//public class ServerPlayer
//{
//    public static readonly int DeckSize = 60;


//    #region Operators
   
//    public ServerCard CardFromId(string uniqueId)
//    {
//        foreach (var item in Cards)
//        {
//            if (item.Value.uniqueId.ToLower() == uniqueId.ToLower())
//            {
//                return item.Value;
//            }
//        }
//        return null;
//    }
//    #endregion

//    #region Properties
//    public ushort networkId { get; private set; }
//    public string userId { get; private set; }
//    public string gameId { get; private set; }
//    public int playerIndex { get; private set; }

//    public bool deckLoaded = false;
//    public bool isReady = false;
//    public bool isConnected = false;

//    private Dictionary<int, ServerCard> _cards = null;

//    public Dictionary<int, ServerCard> Cards
//    {
//        get
//        {
//            _cards ??= new Dictionary<int, ServerCard>();
//            return _cards;
//        }
//        set
//        {
//            _cards = value;
//        }
//    }

//    private UploadedDeckDTO _UploadedDeck = null;
//    public UploadedDeckDTO UploadedDeck
//    {
//        get
//        {
//            if (_UploadedDeck == null)
//            {
//                _UploadedDeck = new UploadedDeckDTO();
//            }
//            return _UploadedDeck;
//        }
//    }

//    private List<ServerCard> _mainDeckOrder = null;
//    public List<ServerCard> MainDeckOrder
//    {
//        get
//        {
//            _mainDeckOrder ??= new List<ServerCard>();
//            return _mainDeckOrder;
//        }
//    }
//    private List<ServerCard> _spiritDeckOrder = null;
//    public List<ServerCard> SpiritDeckOrder
//    {
//        get
//        {
//            _spiritDeckOrder ??= new List<ServerCard>();
//            return _spiritDeckOrder;
//        }
//    }

//    #endregion

//    #region Functions
//    public int[] LocalCardIDs
//    {
//        get
//        {
//            int[] s = new int[Cards.Count];
//            for (int i = 0; i < Cards.Count; i++)
//            {
//                s[i] = Cards[i].localIndex;
//            }
//            return s;
//        }
//    }
//    public string[] RealCardKeys
//    {
//        get
//        {
//            string[] s = new string[Cards.Count];
//            for (int i = 0; i < Cards.Count; i++)
//            {
//                s[i] = Cards[i].setKey;
//            }
//            return s;
//        }
//    }
//    private List<CardSlot> _slots = null;
//    public List<CardSlot> Slots
//    {
//        get
//        {
//            if (_slots == null)
//            {
//                _slots = new List<CardSlot>();
//                List<CardSlot> list = new List<CardSlot>();
//                Game current = GameManager.ActiveGames[gameId];

//                for (int i = 0; i < current.fields.Count; i++)
//                {
//                    GameField g = current.fields[i];
//                    if (g.Owner == networkId)
//                    {
//                        _slots.AddRange(g.Slots);
//                    }
//                }
//            }
//            return _slots;

//        }
//    }
//    public CardSlot SlotById(string id)
//    {
//        foreach (var item in Slots)
//        {
//            if (item.slotId.ToLower() == id.ToLower())
//            {
//                return item;
//            }
//        }
//        return null;
//    }

//    public Game CurrentGame()
//    {
//        return GameManager.ActiveGames[gameId];
//    }

//    public CardSlot _hand = null;
//    public CardSlot Hand
//    {
//        get
//        {
//            if (_hand == null)
//            {
//                for (int i = 0; i < Slots.Count; i++)
//                {
//                    if (Slots[i].slotType == CardSlot.CardLocation.Hand)
//                    {
//                        _hand = Slots[i];
//                    }
//                }
//            }
//            return _hand;

//        }
//    }

//    #endregion
//    public ServerPlayer(ushort netId, string user, string lobbyId, int index)
//    {
//        networkId = netId;
//        userId = user;
//        gameId = lobbyId;
//        playerIndex = index;
//        isConnected = true;
//        Cards.Clear();

//        AllPlayers.Add(netId, this);
//    }




//    public void NewCard(int localIndex, string uniqueId, string setKey, string slotId)
//    {
//        ServerCard card = new ServerCard(userId, localIndex, uniqueId, setKey, slotId);
//        Cards.Add(card.localIndex, card);

//        if (ValidateDeck())
//        {
//            deckLoaded = true;
//        }
//    }
//    private bool ValidateDeck()
//    {
//        if (Cards.Count < DeckSize) { return false; }
//        if (string.IsNullOrEmpty(UploadedDeck.deckKey) || string.IsNullOrEmpty(UploadedDeck.title)) { return false; }
//        return true;
//    }

//    public void CreateDeck(string key, string title)
//    {
//        UploadedDeck.deckKey = key;
//        UploadedDeck.title = title;

//        Message message = NetworkPipeline.DeckSelectionOutbound(this, key, title);
//        CurrentGame().MessageSendToOpponent(message, this);


//    }

//    public void SetDeckOrder(List<string> cardsInOrder)
//    {
//        MainDeckOrder.Clear();

//        for (int i = 0; i < cardsInOrder.Count; i++)
//        {
//            string id = cardsInOrder[i];
//            foreach (var item in Cards)
//            {
//                ServerCard card = item.Value;
//                if (card.uniqueId == id)
//                {
//                    MainDeckOrder.Add(card);
//                    break;
//                }

//            }
//        }

//    }

//    public void SendDeckToOpponent()
//    {
//        for (int i = 0; i < MainDeckOrder.Count; i++)
//        {
//            ServerCard card = MainDeckOrder[i];

//            Message outbound = NetworkPipeline.NewCardDataOutbound(this, card.localIndex, card.setKey, card.uniqueId);
//            CurrentGame().MessageSendToOpponent(outbound, this);
//        }

//        for (int i = 0; i < Cards.Count; i++)
//        {
//            ServerCard card = Cards[i];
//            if (!MainDeckOrder.Contains(card))
//            {
//                Message outbound = NetworkPipeline.NewCardDataOutbound(this, card.localIndex, card.setKey, card.uniqueId);
//                CurrentGame().MessageSendToOpponent(outbound, this);
//            }
//        }

//    }

//    #region Card Management
//    public void ChangeCardSlot(string cardId, string newSlot, int cardMode)
//    {
//        foreach (CardSlot slot in Slots)
//        {
//            if (slot.cards.Contains(cardId))
//            {
//                slot.RemoveCard(cardId);
//                break;
//            }
//        }

//        CurrentGame().ChangeCardSlot(cardId, newSlot, cardMode);
//        //ServerCard card = CardFromId(cardId);
//        //card.slotId = newSlot;

//        Message outbound = NetworkPipeline.CardMovedOutbound(networkId, cardId, newSlot, cardMode);
//        CurrentGame().MessageSendToOpponent(outbound, this);
//    }
//    #endregion



//    #region Turn Management
//    public void SendOpeningDraw()
//    {
//        Message message = Message.Create(MessageSendMode.reliable, (ushort)FromServer.OpeningDraw);
//        GameManager.Instance.Server.Send(message, networkId);
//    }
//    #endregion


//    //#region Slot Management
//    //public void MapSlot(int index, string slotId, int location)
//    //{
//    //    Slots[index].SetId(slotId);
//    //    Slots[index].SetSlotType(location);
//    //}
//    //#endregion

//}