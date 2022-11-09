using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Gameplay;
using RiptideNetworking;
using RiptideNetworking.Utils;
using UnityEngine;
using RiptideNetworking.Transports;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor.Networking.PlayerConnection;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEditor.Rendering.LookDev;
using UnityEditor;
#endif
using System.Linq;

public enum ServerMode
{
    HostOnly = 0,
    Both = 1,
}
public static class ServerManager
{
    #region Static Properties
    public static ServerDTO LocalServer()
    {
        string serverName = "LocalServer";

        ServerDTO dto = new ServerDTO
        {
            serverKey = -1,
            ip = "",
            port = 7777,
            name = serverName,
            playersCurrent = 0,
            playersMax = 20,
            connType = (int)ConnectionType.P2P,

        };

        return dto;
    }
    #endregion

    #region Properties
    public static Server server { get { return NetworkManager.Instance.Server; } }
    private static Dictionary<ushort, string> _connectedPlayers = null;
    public static Dictionary<ushort, string> ConnectedPlayers
    {
        get
        {
            _connectedPlayers ??= new Dictionary<ushort, string>();
            return _connectedPlayers;
        }
    }
    public static List<IConnectionInfo> ConnectedClients { get { return NetworkManager.ConnectedClients; } }
    private static int serverKey { get { return NetworkManager.Instance.serverInfo.serverKey; } }
   
    public static ServerGame ActiveGame { get { return ServerGame.Instance; } }


    
    #endregion


    #region Server Functions
    public static void Tick()
    {
        if (server != null)
        {
            server.Tick();
        }
    }
    #endregion

    #region Player Management
    public static void OnClientConnected(object sender, ServerClientConnectedEventArgs e)
    {
        App.Log($"Client {e.Client.Id} connected to Server from.");

        //do something here to check if the client is reconnecting or connecting for the first time
    }

    public static void PlayerDisconnect(object sender, ClientDisconnectedEventArgs e)
    {
        App.Log($"Client {e.Id} Disconnected from the Server.");

        DisconnectPlayer(e.Id, true);

        //REMOVE THE LOBBY FROM THE REMOTE DB HERE, OR START A TIMER TO WAIT FOR PLAYER TO RETURN BEFORE REMOVING IT

    }

    public static void DisconnectPlayer(ushort playerId, bool removeFromGame)
    {
        UpdateLobby(false);

        
        if (ConnectedPlayers.ContainsKey(playerId))
        {
            ConnectedPlayers.Remove(playerId);
            Player p = GameManager.ByNetworkId(playerId);
            if (removeFromGame)
            {
                ServerGame.Instance.LeaveGame(playerId);
                
            }
            else
            {
                GameManager.AwaitReconnection(playerId);
            }
        }


    }

    public static async void UpdateLobby(bool isAddingPlayer)
    {
        int servKey = serverKey;
        bool updated = await RemoteData.UpdatePlayerCount(servKey, isAddingPlayer);
        if (updated)
        {
            //do something when lobby player count is updated. maybe figure out how to start a new instance
            //if it's full?
        }
    }
    #endregion



    #region Shutdown
    public static async void RemoveGame()
    {
        string id = ServerGame.Instance.gameId;
        App.Log($"Game {id} has been removed!");

        bool removed = await RemoteData.RemoveLobby(id);
        if (removed)
        {
            App.Log($"Game '{id}' has been removed from the Remote Database.");
        }

    
      
    }

    public static async void Shutdown()
    {
        if (server != null)
        {
            server.Stop();
            ConnectedClients.Clear();
            ConnectedPlayers.Clear();

            server.ClientConnected -= OnClientConnected;
            server.ClientDisconnected -= PlayerDisconnect;

            if (ServerGame.Instance != null)
            {
                ServerGame.EndGame();
            }

            await RemoteData.DeleteServer(serverKey);
        }
    }

    #endregion



    #region Host As Server Messages
    public static async void CreateGame()
    {

        string gameId = await RemoteData.CreateLobby(App.Account.Id);
        ServerGame.Create(gameId, NetworkManager.Instance.Client.Id, App.Account.Id);
    }
    #endregion

    #region Inbound Messages
    [MessageHandler((ushort)ToServer.Connected)]
    private static void PlayerConnected(ushort fromClientId, Message message)
    {
        ushort networkId = message.GetUShort();
        string playerId = message.GetString();
        if (!ConnectedPlayers.ContainsKey(networkId))
        {
            ConnectedPlayers.Add(networkId, playerId);
            App.Log($"UserID '{playerId}'(NetworkID: '{networkId}' has been added to the Lobby.");
            UpdateLobby(true);

        }

        string json = JsonUtility.ToJson(NetworkManager.Instance.serverInfo);
        Message outbound = Message.Create(MessageSendMode.reliable, FromServer.Connected);
        outbound.Add(fromClientId);
        outbound.Add(json);
        SendToClient(outbound, fromClientId);


    }

    [MessageHandler((ushort)ToServer.CreateGame)]
    private static async void CreateGame(ushort fromClientId, Message message)
    {
        ushort netId = fromClientId;
        string creator = ConnectedPlayers[netId];
        string gameId = await RemoteData.CreateLobby(creator);

        ServerGame.Create(gameId, netId, creator);

        Message outbound = Message.Create(MessageSendMode.reliable, (ushort)FromServer.CreateGame);
        outbound.AddString(gameId);
        SendToClient(outbound, netId);
    }


    [MessageHandler((ushort)ToServer.JoinGame)]
    private static async void JoinGame(ushort fromClientId, Message message)
    {
        ushort netId = fromClientId;
        string gameId = message.GetString();
        string player = ConnectedPlayers[netId];

        //also do validation that the lobby is available
        bool canJoin = await RemoteData.JoinLobby(gameId, player);
        if (canJoin)
        {
            App.Log($"Client {netId}(userId: {player}) has Joined Game '{gameId}'!");
            ServerGame.Instance.AddPlayer(netId, player, gameId);
        }
        else
        {
            //do a disconnect message here so the player leaves
            App.Log($"Client {netId}(userId: {player}) was not able to Join Game '{gameId}'!");

            Message outbound = Message.Create(MessageSendMode.reliable, (ushort)FromServer.JoinFailed);
            outbound.AddString(gameId);
            SendToClient(outbound, fromClientId);
        }


    }


    [MessageHandler((ushort)ToServer.DeckSelection)]
    private static void ChooseDeck(ushort fromClientId, Message message)
    {
        ushort netId = fromClientId;
        string deckKey = message.GetString();
        string title = message.GetString();
        ServerGame.LoadDeckDetails(netId, deckKey, title);

    }

    [MessageHandler((ushort)ToServer.NewCardData)]
    private static void NewCardData(ushort fromClientId, Message message)
    {

        int cardIndex = message.GetInt();
        string cardNetworkId = message.GetString();
        string cardRealId = message.GetString();
        string slotId = message.GetString();

        ServerGame.SyncServerCard(fromClientId, cardIndex, cardNetworkId, cardRealId, slotId);
    }

    [MessageHandler((ushort)ToServer.DeckOrder)]
    private static void OrderDeck(ushort fromClientId, Message message)
    {
        List<string> deckOrder = message.GetStrings(true).ToList();
        ServerGame.SetDeckOrder(fromClientId, deckOrder);

    }

    [MessageHandler((ushort)ToServer.GameReady)]
    private static void ClientReady(ushort fromClientId, Message message)
    {
        ServerGame.Instance.ReadyPlayer(fromClientId);
    }




    [MessageHandler((ushort)ToServer.CardSelectChange)]
    private static void SelectionChanged(ushort fromClientId, Message message)
    {
        string cardId = message.GetString();
        int boolVal = message.GetInt();

        Message outbound = NetworkPipeline.OutboundMessage<string, int, ushort>(MessageSendMode.reliable, (ushort)FromServer.CardSelectChange, cardId, boolVal, fromClientId);
        ServerGame.Instance.MessageSendToOpponent(outbound, fromClientId);
    }

   
    [MessageHandler((ushort)ToServer.CardMoved)]
    private static void CardMovedSlot(ushort fromClientId, Message message)
    {
        ushort netId = fromClientId;
        string cardId = message.GetString();
        string newSlot = message.GetString();
        int cardMode = message.GetInt();

        NetworkPlayer player = ServerGame.FromId(fromClientId);
        player.ChangeCardSlot(cardId, newSlot, cardMode);


    }

    [MessageHandler((ushort)ToServer.ActionDeclared)]
    private static void ActionDeclared(ushort fromClientId, Message message)
    {
        ushort netId = fromClientId;
        string json = message.GetString();
        CardActionData data = CardActionData.FromData(json);
        App.Log($"Action Declared: {data.GetJson}");
        ServerGame.Instance.MessageSendToOpponent(DeclareActionOutbound(data), fromClientId);
        
    }



    [MessageHandler((ushort)ToServer.EmpowerChanged)]
    private static void CardEmpower(ushort fromClientId, Message message)
    {
        ushort netId = fromClientId;
        Player player = GameManager.ByNetworkId(netId);

        string runeId = message.GetString();
        string elestralId = message.GetString();
        bool isAdding = message.GetBool();

        Message outbound = Message.Create(MessageSendMode.unreliable, (ushort)FromServer.EmpowerChanged);
        outbound.AddUShort(netId);
        outbound.AddString(runeId);
        outbound.AddString(elestralId);
        outbound.AddBool(isAdding);
        //g.MessageSendToOpponent(outbound, player);

    }

    #region Not in Use
    [MessageHandler((ushort)ToServer.ActionRecieved)]
    private static void ActionRecieved(ushort fromClientId, Message message)
    {
        //ushort netId = fromClientId;
        //Player player = Player.AllPlayers[netId];
        //Game g = GameManager.ActiveGames[player.gameId];

        //string actionId = message.GetString();
        //string cardResponse = message.GetString();
        //g.MessageSendToOpponent(ActionResponseOutbound(actionId, cardResponse), player);
    }
    [MessageHandler((ushort)ToServer.ActionEnd)]
    private static void ActionEnd(ushort fromClientId, Message message)
    {
        //ushort netId = fromClientId;
        //Player player = Player.AllPlayers[netId];
        //Game g = GameManager.ActiveGames[player.gameId];

        //string actionId = message.GetString();
        //int result = message.GetInt();
        //g.EndAction(netId, actionId, result);
    }
    [MessageHandler((ushort)ToServer.SlotMapping)]
    private static void SlotMapping(ushort fromClientId, Message message)
    {
        //ushort netId = fromClientId;
        //Player player = Player.AllPlayers[netId];
        //int index = message.GetInt();
        //string slotId = message.GetString();
        //int location = message.GetInt();
        //player.MapSlot(index, slotId, location);
    }
    #endregion
    #endregion

    #region Outbound
    public static void SendToClient(Message message, ushort toClientId, bool requireConfirm = false)
    {
        server.Send(message, toClientId, requireConfirm);


    }
    public static void SendToClientsAll(Message message, bool requireConfirm = false)
    {
        //if (ClientManager.client != null)
        //{
        //    server.SendToAll(message, ClientManager.client.Id);
        //}
        //else
        //{
        //    server.SendToAll(message);
        //}
        server.SendToAll(message);


    }

    public static Message JoinedPlayerOutbound(NetworkPlayer p)
    {
        Message outbound = Message.Create(MessageSendMode.reliable, (ushort)FromServer.PlayerJoined);
        outbound.AddUShort(p.networkId);
        outbound.AddString(p.userId);
        return outbound;
    }

    public static Message JoinedGameOutbound(string gameId, List<NetworkPlayer> otherPlayers)
    {
        Message outbound = Message.Create(MessageSendMode.reliable, (ushort)FromServer.JoinGame);
        outbound.Add<string>(gameId);
        outbound.Add<int>(otherPlayers.Count);

        for (int i = 0; i < otherPlayers.Count; i++)
        {
            NetworkPlayer other = otherPlayers[i];
            outbound.Add<ushort>(other.networkId);
            outbound.Add<string>(other.userId);
            outbound.Add<string>(other.deckKey);
            outbound.Add<string>(other.deckName);

        }

        return outbound;

    }

    public static Message DeckSelectionOutbound(NetworkPlayer sender, string deckKey, string deckTitle)
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)FromServer.DeckSelection);
        message.AddUShort(sender.networkId);
        message.AddString(deckKey);
        message.AddString(deckTitle);
        return message;
    }

    public static Message NewCardDataOutbound(NetworkPlayer sender, int localIndex, string setKey, string uniqueId)
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)FromServer.NewCardData);
        message.AddUShort(sender.networkId);
        message.AddInt(localIndex);
        message.AddString(setKey);
        message.AddString(uniqueId);

        return message;
    }

    public static Message CardMovedOutbound(ushort owner, string cardId, string newSlot, int cardMode)
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)FromServer.CardMoved);
        message.AddUShort(owner);
        message.AddString(cardId);
        message.AddString(newSlot);
        message.AddInt(cardMode);
        return message;

    }

    public static Message DeclareActionOutbound(CardActionData data)
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)FromServer.ActionDeclared);
        message.AddString(data.GetJson);
        return message;
    }

    public static Message PlayerDisconnectOutbound(ushort playerId, string userId)
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)PlayerActivity.LostConnection);
        message.AddUShort(playerId);
        message.AddString(userId);
        return message;
    }
    #endregion




}
