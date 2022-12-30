using Gameplay;
using Gameplay.Networking;
using RiptideNetworking;
using SimpleSQL.Demos;
using System;
using UnityEngine;

public static class ClientManager 
{

    public static Client client { get { return NetworkManager.Instance.Client; } }
    public static float maxConnectTime = 10f;


    public static bool IsConnected() { return client != null && client.IsConnected;  }
    public static bool IsConnecting() { return client != null && client.IsConnecting; }

    public static ConnectionType ConnectionType
    {
        get
        {
            if (client == null) { return ConnectionType.Offline; }
            if (NetworkManager.Instance.serverInfo != null)
            {
                return (ConnectionType)NetworkManager.Instance.serverInfo.connType;
            }
            return ConnectionType.Remote;
        }
    }

    public static ushort GetClientId()
    {
        if (client == null) { return 0; }
        return client.Id;
    }


    public static void Disconnect()
    {
        if (client != null)
        {
            client.Disconnect();
        }
        
    }

    #region ConnectedPlayer
    private static ConnectedPlayer _player = null;
    public static ConnectedPlayer Player
    {
        get { return _player; }
        set { _player = value; }
    }

    public static void SetPlayer(ushort serverId)
    {
        if (Player == null)
        {
            Player = ConnectedPlayer.Create(serverId);
        }
        else
        {
            if (serverId != Player.ServerId)
            {
                Player.SetDirty(true);
            }
        }
       
    }
    public static void RemovePlayer()
    {
        if (Player != null)
        {
            string userId = Player.playerData.userId;
            RemoteData.DeletePlayer(userId);
        }
    }
    #endregion

}
