using System.Collections;
using System.Collections.Generic;
using Gameplay;
using RiptideNetworking;

using UnityEngine;
using Users;

public class ServerGame
{

    public static ServerGame Instance { get; private set; }
    #region Properties
    public string gameId;
    public int expectedPlayers = 2;
    public ConnectionType connType;

    private List<NetworkPlayer> _players = null;
    public List<NetworkPlayer> Players { get { _players ??= new List<NetworkPlayer>(); return _players; } }


    public static NetworkPlayer FromId(ushort id)
    {
        for (int i = 0; i < Instance.Players.Count; i++)
        {
            if (Instance.Players[i].networkId == id) { return Instance.Players[i]; }
        }
        return null;
    }
    #endregion

    #region Functions
    public NetworkPlayer OpponentOf(NetworkPlayer other)
    {
        NetworkPlayer opp = null;
        for (int i = 0; i < Players.Count; i++)
        {
            NetworkPlayer a = Players[i];
            if (Players[i].networkId != other.networkId)
            {
                opp = a;
                break;
            }
        }
        return opp;
    }

    protected int CountOfLoadedPlayers
    {
        get
        {
            int count = 0;
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].deckLoaded) { count += 1; }
            }
            return count;
        }
    }
    protected int CountOfReadyPlayers
    {
        get
        {
            int count = 0;
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].isReady) { count += 1; }
            }
            return count;
        }
    }

    public void CheckPlayersLoaded()
    {

        if (CountOfLoadedPlayers == expectedPlayers)
        {
            for (int i = 0; i < Players.Count; i++)
            {
                Players[i].SendDeckToOpponent();

            }
        }
    }

    public void ReadyPlayer(ushort playerId)
    {
        NetworkPlayer p = FromId(playerId);
        p.isReady = true;
        if (CountOfReadyPlayers == expectedPlayers)
        {
            for (int i = 0; i < Players.Count; i++)
            {
                NetworkPlayer pl = Players[i];
                pl.SendOpeningDraw();
            }

        }

    }

    public CardSlotData SlotById(string id)
    {
        foreach (var p in Players)
        {
            for (int i = 0; i < p.Slots.Count; i++)
            {
                if (p.Slots[i].slotId.ToLower() == id.ToLower())
                {
                    return p.Slots[i];
                }
            }
        }

        return null;
    }
    #endregion


    public static void Create(string lobbyId, ushort host, string userId, string username, int players = 2)
    {
        //NetworkPlayer p = new NetworkPlayer(host, userId, "", "", username);
        //Instance = new ServerGame(lobbyId, p, players);
    }

    ServerGame(string lobbyId, NetworkPlayer p, int players)
    {
        gameId = lobbyId;
        Players.Add(p);
        expectedPlayers = 2;
        connType = ConnectionType.P2P;
    }


    public void AddPlayer(ushort id, string username, string gameId)
    {
        NetworkPlayer p = new NetworkPlayer(id, username, "", "", "");

        List<NetworkPlayer> otherPlayers = new List<NetworkPlayer>();
        foreach (NetworkPlayer player in Players)
        {
            Message message = ServerManager.JoinedPlayerOutbound(p);
            ServerManager.SendToClient(message, player.networkId);
            otherPlayers.Add(player);
        }
        Players.Add(p);

        Message outbound = ServerManager.JoinedGameOutbound(gameId, otherPlayers);
        ServerManager.server.Send(outbound, p.networkId);
    }




    public static void LoadDeckDetails(ushort playerId, string deckKey, string deckTitle)
    {
        NetworkPlayer p = FromId(playerId);
        p.AddDeck(deckKey, deckTitle);

        Instance.MessageSendToOpponent(ServerManager.DeckSelectionOutbound(p, deckKey, deckTitle), p);

    }

    public static void SyncServerCard(ushort player, int index, string cardId, string realId, string slotId)
    {
        NetworkPlayer p = FromId(player);

        ServerCard card = new ServerCard(p.userId, index, cardId, realId, slotId);
        p.SyncServerCard(card);

        Instance.CheckPlayersLoaded();
    }

    public static void SetDeckOrder(ushort player, List<string> deckOrder)
    {
        NetworkPlayer p = FromId(player);
        p.SetDeckOrder(deckOrder);
    }

    public void ChangeCardSlot(string cardId, string newSlot, int cardMode)
    {

        CardSlotData slot = SlotById(newSlot);
        slot.AddCard(cardId);
    }


    public void LeaveGame(ushort netId)
    {
        NetworkPlayer player = FromId(netId);
        Players.Remove(player);
        //for now, just end the game is a player leaves. eventually, allow lobby to stay up in case player reconnects
        if (Players.Count == 0)
        {
            EndGame();
        }
        else
        {
            Message outbound = ServerManager.PlayerDisconnectOutbound(player.networkId, player.userId);
            MessageSendToOpponent(outbound, player);

        }
    }


    public static async void EndGame()
    {
        string id = ServerGame.Instance.gameId;
        App.Log($"Game {id} has been removed!");

        bool removed = await RemoteData.RemoveLobby(id);
        if (removed)
        {
            App.Log($"Game '{id}' has been removed from the Remote Database.");
        }

        if (GameManager.Instance)
        {
            GameManager.Instance.LeaveGame();
        }
        
    }

   


    #region Messages
    public void MessageSendToOpponent(Message message, NetworkPlayer caller)
    {
        if (OpponentOf(caller) != null)
        {
            NetworkPlayer recipient = OpponentOf(caller);
            ServerManager.SendToClient(message, recipient.networkId);
        }

    }
    public void MessageSendToOpponent(Message message, ushort callerId)
    {
        NetworkPlayer caller = FromId(callerId);
        MessageSendToOpponent(message, caller);
    }
    #endregion
}
