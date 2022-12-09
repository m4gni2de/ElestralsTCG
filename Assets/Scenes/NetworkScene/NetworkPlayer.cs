using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Decks;
using Gameplay;
using RiptideNetworking;
using SimpleSQL.Demos;
using UnityEngine;

public class NetworkPlayer 
{
    public static readonly int DeckSize = 60;
    public ushort networkId { get; private set; }
    public string userId { get; private set; }
    public string username { get; private set; }
    public int index { get; private set; }
    public string deckKey { get; private set; }
    public string deckName { get; private set; }

    public bool isConnected = false;

    private List<ServerCard> _cards = null;
    public List<ServerCard> Cards { get { _cards ??= new List<ServerCard>(); return _cards; } }

    private List<CardSlotData> _slots = null;
    public List<CardSlotData> Slots { get { _slots ??= new List<CardSlotData>(); return _slots; } }


    private List<ServerCard> _mainDeckOrder = null;
    public List<ServerCard> MainDeckOrder
    {
        get
        {
            _mainDeckOrder ??= new List<ServerCard>();
            return _mainDeckOrder;
        }
    }
    private List<ServerCard> _spiritDeckOrder = null;
    public List<ServerCard> SpiritDeckOrder
    {
        get
        {
            _spiritDeckOrder ??= new List<ServerCard>();
            return _spiritDeckOrder;
        }
    }

    public bool deckLoaded = false;
    public bool isReady = false;


    public NetworkPlayer(ushort netId, string id, string key, string name, string username)
    {
        networkId = netId;
        userId = id;
        deckKey = key;
        deckName = name;
        isConnected = true;
        this.username = username;
        Cards.Clear();
        CreateSlots();

    }

    protected void CreateSlots()
    {
        Slots.Clear();

        for (int i = 0; i < 13; i++)
        {
            CardSlotData slot = new CardSlotData(i);
            slot.SetId(networkId.ToString());
            Slots.Add(slot);
        }
    }

    public void AddDeck(string key, string name)
    {
        deckKey = key;
        deckName = name;
    }

    public void SyncServerCard(ServerCard card)
    {
        Cards.Add(card);

        if (ValidateDeck())
        {
            deckLoaded = true;
        }
    }

    private bool ValidateDeck()
    {
        if (Cards.Count < DeckSize) { return false; }
        if (string.IsNullOrEmpty(deckKey) || string.IsNullOrEmpty(deckName)) { return false; }
        return true;
    }


    //get the list of cards in the main deck, and add those to the main deck. then, all of the cards leftover are in the spirit deck by default.
    public void SetDeckOrder(List<string> cardsInOrder)
    {
        MainDeckOrder.Clear();

        for (int i = 0; i < cardsInOrder.Count; i++)
        {
            string id = cardsInOrder[i];
            foreach (var item in Cards)
            {
                ServerCard card = item;
                if (card.uniqueId == id)
                {
                    MainDeckOrder.Add(card);
                    break;
                }

            }
        }

    }

    public void SendDeckToOpponent()
    {
        for (int i = 0; i < MainDeckOrder.Count; i++)
        {
            ServerCard card = MainDeckOrder[i];

            Message outbound = ServerManager.NewCardDataOutbound(this, card.localIndex, card.setKey, card.uniqueId);
            ServerGame.Instance.MessageSendToOpponent(outbound, this);
        }

        for (int i = 0; i < Cards.Count; i++)
        {
            ServerCard card = Cards[i];
            if (!MainDeckOrder.Contains(card))
            {
                Message outbound = ServerManager.NewCardDataOutbound(this, card.localIndex, card.setKey, card.uniqueId);
                ServerGame.Instance.MessageSendToOpponent(outbound, this);
            }
        }

    }

    public void SendOpeningDraw()
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)FromServer.OpeningDraw);
        ServerManager.SendToClient(message, networkId);
    }

    public void ChangeCardSlot(string cardId, string newSlot, int cardMode)
    {
        foreach (CardSlotData slot in Slots)
        {
            if (slot.cards.Contains(cardId))
            {
                slot.RemoveCard(cardId);
                break;
            }
        }

        ServerGame.Instance.ChangeCardSlot(cardId, newSlot, cardMode);
        Message outbound = ServerManager.CardMovedOutbound(networkId, cardId, newSlot, cardMode);
        ServerGame.Instance.MessageSendToOpponent(outbound, this);
    }
}
