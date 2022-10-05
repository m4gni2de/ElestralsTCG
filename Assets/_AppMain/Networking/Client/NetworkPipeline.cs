using System.Collections;
using System.Collections.Generic;
using RiptideNetworking;
using UnityEngine;
using Gameplay;
using Gameplay.Networking;
using Defective.JSON;
using System;
using Decks;
using UnityEditor;
using System.Linq;
using Gameplay.Decks;





public enum ServerFunction
{
    HasActiveAction = 1,
}


public enum ToServer : ushort
{
    Connected = 99,
    CreateGame = 98,
    JoinGame = 97,
    PlayerJoined = 96,
    DeckSelection = 95,
    GameReady = 94,
    NewCardData = 93,
    CardSelectChange = 92,
    DeckOrder = 91,
    CardMoved = 90,
    FreezeGame = 89,
    ActionDeclared = 88,
    ActionRecieved = 87,
    ActionEnd = 86,
    OpeningDraw = 85,
    SlotMapping = 84,
    StartGame = 83,
    CardPosition = 82,
}
public enum FromServer :ushort
{
    Connected = 99,
    CreateGame = 98,
    JoinGame = 97,
    PlayerJoined = 96,
    DeckSelection = 95,
    GameReady = 94,
    NewCardData = 93,
    CardSelectChange = 92,
    DeckOrder = 91,
    CardMoved = 90,
    FreezeGame = 89,
    ActionDeclared = 88,
    ActionRecieved = 87,
    ActionEnd = 86,
    OpeningDraw = 85,
    SlotMapping = 84,
    StartGame = 83,
    CardPosition = 82,


}

public class NetworkPipeline
{
    //[MenuItem("Remote DB/Upload Deck")]
    //public static void UploadDeck()
    //{
    //    string key = "ps01";
    //    List<Decklist> decks = App.Account.DeckLists;

    //    for (int i = 0; i < decks.Count; i++)
    //    {
    //        if (decks[i].DeckKey.ToLower() == key.ToLower())
    //        {
    //            Decklist deck = decks[i];
    //            UploadDeckByKey(deck);
    //            break;
    //        }
    //    }

    //}
    //private static async void UploadDeckByKey(Decklist d)
    //{
    //    await RemoteData.AddDeckToRemoteDB(d);
    //}
    #region Inbound Messages


    #region FromServer enum
    public static event Action OnPlayerRegistered;
    [MessageHandler((ushort)FromServer.Connected)]
    private static void PlayerIdRegistered(Message message)
    {
        ushort networkId = message.GetUShort();
        OnPlayerRegistered?.Invoke();
    }

    public static event Action<string> OnGameCreated;
    [MessageHandler((ushort)FromServer.CreateGame)]
    private static void GameCreated(Message message)
    {
        string gameId = message.GetString();
        OnGameCreated?.Invoke(gameId);
    }

    public static event Action<string, List<NetworkPlayer>> OnGameJoined;
    [MessageHandler((ushort)FromServer.JoinGame)]
    private static void GameJoined(Message message)
    {
        string lobbyId = message.GetString();
        int playersToAdd = message.GetInt();

        List<NetworkPlayer> otherPlayers = new List<NetworkPlayer>();
        for (int i = 0; i < playersToAdd; i++)
        {
            ushort netId = message.GetUShort();
            string user = message.GetString();
            string deckKey = message.GetString();
            string deckName = message.GetString();
            NetworkPlayer p = new NetworkPlayer(netId, user, deckKey, deckName, false);
            otherPlayers.Add(p);
        }
        OnGameJoined?.Invoke(lobbyId, otherPlayers);
    }

    public static event Action<ushort, string> OnPlayerJoined;
    [MessageHandler((ushort)FromServer.PlayerJoined)]
    private static void PlayerJoined(Message message)
    {
        ushort netId = message.GetUShort();
        string userId = message.GetString();
        OnPlayerJoined?.Invoke(netId, userId);
    }

    [MessageHandler((ushort)FromServer.DeckSelection)]
    private static void DeckSelected(Message message)
    {
        ushort networkId = message.GetUShort();
        string deck = message.GetString();
        string title = message.GetString();

        OnlineGameManager.LoadPlayer(networkId, deck, title);

    }


    [MessageHandler((ushort)FromServer.GameReady)]
    private static void GameReady(Message message)
    {
        ushort sender = message.GetUShort();
        if (sender != Player.LocalPlayer.lobbyId)
        {
            UploadedDeckDTO dto = new UploadedDeckDTO();
            dto.deckKey = message.GetString();
            dto.title = message.GetString();
            string[] realIds = message.GetStrings(true);
            dto.deck = realIds.ToList();
            GameManager.Instance.SyncPlayer(sender, dto);

        }


    }

    [MessageHandler((ushort)FromServer.NewCardData)]
    private static void GetCardData(Message message)
    {
        ushort sender = message.GetUShort();
        int cardIndex = message.GetInt();
        string cardRealId = message.GetString();
        string cardNetworkId = message.GetString();


        OnlineGameManager.SyncPlayerCard(sender, cardIndex, cardRealId, cardNetworkId);

    }

    [MessageHandler((ushort)FromServer.CardSelectChange)]
    private static void SelectionChanged(Message message)
    {
        string cardId = message.GetString();
        bool isSelected = message.GetBool();

        GameCard card = Game.FindCard(cardId);
        card.SelectCard(isSelected, false);

    }

    [MessageHandler((ushort)FromServer.CardMoved)]
    private static void CardMoved(Message message)
    {
        ushort owner = message.GetUShort();
        string cardId = message.GetString();
        string newIndex = message.GetString();

        OnlineGameManager.RemoteCardSlotChange(owner, cardId, newIndex);

    }


    [MessageHandler((ushort)FromServer.CardPosition)]
    private static void CardPositionChange(Message message)
    {
        ushort owner = message.GetUShort();
        string cardId = message.GetString();
        Vector3 localPos = message.GetVector3();

        //OnlineGameManager.RemoteCardSlotChange(owner, cardId, newIndex);

    }


    //LEAVING OFF WITH NOT HAVING A CLIENT RECEIVER METHOD FOR THE CARD MOVED MESSAGE
    #endregion


    #endregion



    #region Outbound Messages

    #region Constructors
    public static Message OutboundMessage<T>(MessageSendMode sendMode, ushort fromClientId, T obj)
    {
        Message m = Message.Create(sendMode, fromClientId);
        m.Add(obj);
        return m;
    }
    public static Message OutboundMessage<T1, T2>(MessageSendMode sendMode, ushort fromClientId, T1 obj1, T2 obj2)
    {
        Message m = Message.Create(sendMode, fromClientId);
        m.Add(obj1);
        m.Add(obj2);
        return m;
    }
    public static Message OutboundMessage<T1, T2, T3>(MessageSendMode sendMode, ushort fromClientId, T1 obj1, T2 obj2, T3 obj3)
    {
        Message m = Message.Create(sendMode, fromClientId);
        m.Add(obj1);
        m.Add(obj2);
        m.Add(obj3);
        return m;
    }
    #endregion
    public static void SendMessageToServer(Message message)
    {
        if (NetworkManager.Instance != null && NetworkManager.Instance.Client != null)
        {
            NetworkManager.Instance.Client.Send(message);
        }

    }

    #region To Server Messages
    public static void CreateGame()
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ToServer.CreateGame);
        NetworkPipeline.SendMessageToServer(message);
    }
    public static void JoinNetworkLobby(string lobby)
    {

        Message message = Message.Create(MessageSendMode.reliable, (ushort)ToServer.JoinGame);
        message.AddString(lobby);
        SendMessageToServer(message);
    }
    public static void SendDeckSelection(string deckKey, string deckName)
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ToServer.DeckSelection);
        message.Add<string>(deckKey);
        message.Add<string>(deckName);
        SendMessageToServer(message);
    }
    public static void SendNewCard(GameCard.NetworkData data)
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ToServer.NewCardData);
        message.AddInt(data.networkId);
        message.AddString(data.sessionId);
        message.AddString(data.setKey);
        message.AddString(data.slotId);
        SendMessageToServer(message);
    }

    public static void SendCardSelect(GameCard card, bool isSelected)
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ToServer.CardSelectChange);
        message.AddString(card.cardId);
        message.AddBool(isSelected);
        SendMessageToServer(message);
    }

    public static void SendDeckOrder(List<string> deckInOrder)
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ToServer.DeckOrder);
        message.AddStrings(deckInOrder.ToArray(), true, true);
        SendMessageToServer(message);

    }
    public static void SendClientReady()
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ToServer.GameReady);
        SendMessageToServer(message);
    }

    public static void SendNewCardSlot(string cardId, string newSlot)
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ToServer.CardMoved);
        message.AddString(cardId);
        message.AddString(newSlot);
        SendMessageToServer(message);


    }

    public static void SendGameFreeze(bool isFreeze)
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ToServer.FreezeGame);
        message.AddBool(isFreeze);
        SendMessageToServer(message);
    }

    public static void SendActionDeclare(CardAction action)
    {
        Message outbound = Message.Create(MessageSendMode.reliable, (ushort)ToServer.ActionDeclared);
        outbound.Add(action.ActionData.GetJson);
        SendMessageToServer(outbound);

    }


    public static void ConfirmActionRecieved(string actionId)
    {
        Message outbound = Message.Create(MessageSendMode.reliable, (ushort)ToServer.ActionRecieved);
        outbound.Add(actionId);
        SendMessageToServer(outbound);
    }
    public static void EndActionOutbound(string actionId, ActionResult result)
    {
        Message outbound = Message.Create(MessageSendMode.reliable, (ushort)ToServer.ActionEnd);
        outbound.Add(actionId);
        outbound.AddInt((int)result);
        SendMessageToServer(outbound);
    }

    public static void OpeningDrawsOutbound()
    {
        Message outbound = Message.Create(MessageSendMode.reliable, (ushort)ToServer.OpeningDraw);
        SendMessageToServer(outbound);
    }
    //public static void SlotMappingOutbound(int index, string slotId, CardLocation slotType)
    //{
    //    Message outbound = Message.Create(MessageSendMode.reliable, (ushort)ToServer.SlotMapping);
    //    outbound.AddInt(index);
    //    outbound.AddString(slotId);
    //    outbound.Add((int)slotType);
    //    SendMessageToServer(outbound);
    //}

    public static void SendCardPosition(string cardId, Vector3 pos)
    {
        Message outbound = Message.Create(MessageSendMode.unreliable, (ushort)ToServer.CardPosition);
        outbound.Add(cardId);
        outbound.AddVector3(pos);
        SendMessageToServer(outbound);
    }
    #endregion


    #endregion



    #region Network Card
    [MessageHandler((ushort)Receivers.Position)]
    private static void PositionChange(Message message)
    {
        string cardId = message.GetString();
        GameCard card = Game.FindCard(cardId);

        Vector3 pos = message.GetVector3();
       
        float height = WorldCanvas.Height;
        float width = WorldCanvas.Width;

        float percWidth = width * pos.x;
        float percHeight = height * pos.y;

        card.cardObject.transform.position = new Vector3(percWidth, percHeight, 0f);



    }

    [MessageHandler((ushort)Receivers.Rotation)]
    private static void RotationChange(Message message)
    {
        string cardId = message.GetString();
        GameCard card = Game.FindCard(cardId);

        Vector3 rotation = message.GetVector3();

        card.cardObject.transform.localEulerAngles = rotation;
    }

    [MessageHandler((ushort)Receivers.Parent)]
    private static void ParentChange(Message message)
    {
        string cardId = message.GetString();
        GameCard card = Game.FindCard(cardId);

        string parent = message.GetString();
        int sibling = message.GetInt();
        Vector2 scale = message.GetVector2();


        CardSlot parentSlot = Game.FindSlot(parent);
        card.cardObject.SetAsChild(parentSlot.transform, scale, "", sibling);
    }


    [MessageHandler((ushort)Receivers.Sorting)]
    private static void SortingChange(Message message)
    {
        string cardId = message.GetString();
        GameCard card = Game.FindCard(cardId);

        string layer = message.GetString();
        int cardOrder = message.GetInt();

        if (!string.IsNullOrEmpty(layer)) { card.cardObject.SetSortingLayer(layer); }
        card.cardObject.SetSortingOrder(cardOrder);
    }

    [MessageHandler((ushort)Receivers.Scale)]
    private static void ScaleChange(Message message)
    {
        string cardId = message.GetString();
        GameCard card = Game.FindCard(cardId);

        Vector2 scale = message.GetVector2();


        card.cardObject.SetScale(scale);
    }

    [MessageHandler((ushort)Receivers.Flip)]
    private static void FlipCard(Message message)
    {
        string cardId = message.GetString();
        GameCard card = Game.FindCard(cardId);

        bool toFacedown = message.GetBool();


        card.cardObject.Flip(toFacedown);
    }
    #endregion

}


