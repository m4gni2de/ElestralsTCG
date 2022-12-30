using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Gameplay;
using Gameplay.Networking;
using Defective.JSON;
using System;
using Decks;
using UnityEditor;
using System.Linq;
using Gameplay.Decks;
using RiptideNetworking;
using nsSettings;

public enum ServerFunction
{
    HasActiveAction = 1,
}


public enum ToServer : ushort
{
    ServerTime = 100,
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
    PlayerLeft = 82,
    JoinFailed = 81,
    EmpowerChanged = 80,
    ActionConfirmed = 79,
}
public enum FromServer :ushort
{
    ServerTime = 100,
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
    PlayerLeft = 82,
    JoinFailed = 81,
    EmpowerChanged = 80,
    ActionConfirmed = 79,


}

public enum PlayerActivity
{
    Joined = 899,
    LostConnection = 898,
    Reconnected = 897,
    Left = 896
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
    [MessageHandler((ushort)FromServer.ServerTime)]
    private static void GetServerTime(Message message)
    {
        

    }


    public static event Action<string> OnJoinedFailed;
    [MessageHandler((ushort)FromServer.JoinFailed)]
    private static void JoinLobbyFailed(Message message)
    {
        string lobbyId = message.GetString();
        OnJoinedFailed?.Invoke(lobbyId);
    }


    public static event Action<ushort> OnPlayerRegistered;
    [MessageHandler((ushort)FromServer.Connected)]
    private static void PlayerIdRegistered(Message message)
    {
        ushort networkId = message.GetUShort();
        OnPlayerRegistered?.Invoke(networkId);
    }

    public static event Action<string> OnGameCreated;
    [MessageHandler((ushort)FromServer.CreateGame)]
    private static void GameCreated(Message message)
    {
        string gameId = message.GetString();
        
        OnGameCreated?.Invoke(gameId);
    }




    //public static event Action<string, List<NetworkPlayer>> OnGameJoined;
    //[MessageHandler((ushort)FromServer.JoinGame)]
    //private static void GameJoined(Message message)
    //{
    //    string lobbyId = message.GetString();
    //    int playersToAdd = message.GetInt();

    //    List<NetworkPlayer> otherPlayers = new List<NetworkPlayer>();
    //    for (int i = 0; i < playersToAdd; i++)
    //    {
    //        ushort netId = message.GetUShort();
    //        string user = message.GetString();
    //        string deckKey = message.GetString();
    //        string deckName = message.GetString();
    //        string username = message.GetString();
    //        int sleeves = message.GetInt();
    //        int playmatt = message.GetInt();
    //        NetworkPlayer p = new NetworkPlayer(netId, user, deckKey, deckName, username);
    //        p.sleeves = sleeves;
    //        p.playmatt = playmatt;
    //        otherPlayers.Add(p);
    //    }
    //    OnGameJoined?.Invoke(lobbyId, otherPlayers);
    //}

    public static event Action<string, List<string>> OnGameJoined;
    [MessageHandler((ushort)FromServer.JoinGame)]
    private static void GameJoined(Message message)
    {
        string lobbyId = message.GetString();
        int playersToAdd = message.GetInt();

        //List<NetworkPlayer> otherPlayers = new List<NetworkPlayer>();
        List<string> otherPlayers = new List<string>();
        for (int i = 0; i < playersToAdd; i++)
        {

            string user = message.GetString();
            otherPlayers.Add(user);
        }
        OnGameJoined?.Invoke(lobbyId, otherPlayers);
    }

    public static event Action<ConnectedPlayerDTO> OnPlayerJoined;
    public static void DoPlayerJoined(ConnectedPlayerDTO dto)
    {
        OnPlayerJoined?.Invoke(dto);
    }
    [MessageHandler((ushort)FromServer.PlayerJoined)]
    private static async void PlayerJoined(Message message)
    {
        string userId = message.GetString();


        ConnectedPlayerDTO dto = await RemoteData.FindPlayer(userId);
        if (dto != null)
        {
            DoPlayerJoined(dto);
        }
    }




    [MessageHandler((ushort)FromServer.DeckSelection)]
    private static void DeckSelected(Message message)
    {
        ushort networkId = message.GetUShort();
        string deck = message.GetString();
        string title = message.GetString();

        GameManager.LoadPlayer(networkId, deck, title);

    }


    //[MessageHandler((ushort)FromServer.GameReady)]
    //private static void GameReady(Message message)
    //{
    //    ushort sender = message.GetUShort();
    //    if (sender != Player.LocalPlayer.lobbyId)
    //    {
    //        UploadedDeckDTO dto = new UploadedDeckDTO();
    //        dto.deckKey = message.GetString();
    //        dto.title = message.GetString();
    //        string[] realIds = message.GetStrings(true);
    //        dto.deck = realIds.ToList();
    //        GameManager.Instance.SyncPlayer(sender, dto);

    //    }


    //}

    [MessageHandler((ushort)FromServer.NewCardData)]
    private static void GetCardData(Message message)
    {
        ushort sender = message.GetUShort();
        int cardIndex = message.GetInt();
        string cardRealId = message.GetString();
        string cardNetworkId = message.GetString();

        GameManager.SyncPlayerCard(sender, cardIndex, cardRealId, cardNetworkId);

    }

    [MessageHandler((ushort)FromServer.CardSelectChange)]
    private static void SelectionChanged(Message message)
    {
        string cardId = message.GetString();
        int boolVal = message.GetInt();
        bool isSelected = boolVal == 1;
        ushort sender = message.GetUShort();

        GameCard card = Game.FindCard(cardId);
        card.SelectCardAsPlayer(isSelected, sender, false);

    }

    [MessageHandler((ushort)FromServer.CardMoved)]
    private static void CardMoved(Message message)
    {
        ushort owner = message.GetUShort();
        string cardId = message.GetString();
        string newIndex = message.GetString();
        int cardMode = message.GetInt();

        GameManager.RemoteCardSlotChange(owner, cardId, newIndex, cardMode);

    }




    [MessageHandler((ushort)FromServer.EmpowerChanged)]
    private static void CardEmpowerChange(Message message)
    {
        Message outbound = Message.Create(MessageSendMode.reliable, (ushort)ToServer.EmpowerChanged);
        ushort owner = message.GetUShort();
        string rune = message.GetString();
        string elestral = message.GetString();
        bool toAdd = message.GetBool();

        GameManager.RemoteEmpowerChange(owner, rune, elestral, toAdd);

    }


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
        //if (NetworkManager.Instance != null)
        //{
        //    if (NetworkManager.Instance.networkMode == NetworkManager.NetworkMode.Client)
        //    {
        //        if (NetworkManager.Instance.Client != null) { NetworkManager.Instance.Client.Send(message); }
        //    }
        //    else if (NetworkManager.Instance.networkMode == NetworkManager.NetworkMode.Both)
        //    {
        //            ServerManager.SendToClientsAll(message);
        //    }
            
        //}

    }

    #region To Server Messages

    public static void OnConnectedToServer()
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ToServer.Connected);
        ClientManager.SetPlayer(NetworkManager.Instance.Client.Id);

        ConnectedPlayer p = ClientManager.Player;
        
        message.AddUShort(p.ServerId);
        message.AddString(p.playerData.userId);
        message.AddString(p.playerData.username);
        message.AddInt(p.playerData.sleeves);
        message.AddInt(p.playerData.playmatt);

        SendMessageToServer(message);
    }
    public static void CreateGame()
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ToServer.CreateGame);
        SendMessageToServer(message);
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
        message.AddString(data.cardKey);
        message.AddString(data.slotId);
        SendMessageToServer(message);

    }

    public static void SendCardSelect(GameCard card, bool isSelected)
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ToServer.CardSelectChange);
        message.AddString(card.cardId);

        int boolVal = 0;
        if (isSelected) { boolVal = 1; }
        message.AddInt(boolVal);
        SendMessageToServer(message);

    }

    public static void SendDeckOrder(List<string> deckInOrder)
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ToServer.DeckOrder);
        message.AddStrings(deckInOrder.ToArray(), true);
        SendMessageToServer(message);


    }
    public static void SendClientReady()
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ToServer.GameReady);
        SendMessageToServer(message);
    }

    public static void SendNewCardSlot(string cardId, string newSlot, CardMode mode)
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ToServer.CardMoved);
        message.AddString(cardId);
        message.AddString(newSlot);
        message.AddInt((int)mode);
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

    public static void SendActionComplete(CardAction action)
    {
        Message outbound = Message.Create(MessageSendMode.reliable, (ushort)ToServer.ActionEnd);
        outbound.Add(action.ActionData.GetJson);
        SendMessageToServer(outbound);

    }


    public static void SendActionResponse(string actionId, string responseCardId)
    {
        Message outbound = Message.Create(MessageSendMode.reliable, (ushort)ToServer.ActionRecieved);
        outbound.Add(actionId);
        outbound.Add(responseCardId);
        SendMessageToServer(outbound);
    }
   
    //public static void EndActionOutbound(string actionId, ActionResult result)
    //{
    //    Message outbound = Message.Create(MessageSendMode.reliable, (ushort)ToServer.ActionEnd);
    //    outbound.Add(actionId);
    //    outbound.AddInt((int)result);
    //    SendMessageToServer(outbound);
    //}

    public static void OpeningDrawsOutbound()
    {
        Message outbound = Message.Create(MessageSendMode.reliable, (ushort)ToServer.OpeningDraw);
        SendMessageToServer(outbound);
    }
    public static void SendEmpowerChange(string runeId, string elestralId, bool adding)
    {
        Message outbound = Message.Create(MessageSendMode.reliable, (ushort)ToServer.EmpowerChanged);
        outbound.Add(runeId);
        outbound.Add(elestralId);
        outbound.AddBool(adding);
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


        Vector3 newPos = Camera.main.ScreenToWorldPoint(pos);
        card.cardObject.transform.position = pos / WorldCanvas.Instance.ScreenScale;



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

    #region Player Activity
    [MessageHandler((ushort)PlayerActivity.LostConnection)]
    private static void PlayerDisconnected(Message message)
    {
        ushort netId = message.GetUShort();
        string userId = message.GetString();
        GameManager.PlayerDisconnected(netId, userId);
    }

    
    
    public static void SendPlayerQuit(Player player)
    {
        Message outbound = Message.Create(MessageSendMode.reliable, (ushort)PlayerActivity.Left);
        outbound.AddString(player.userId);
        SendMessageToServer(outbound);
    }

    #endregion

}


