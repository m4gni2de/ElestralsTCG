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


    public static void Disconnect()
    {
        if (client != null)
        {
            client.Disconnect();
        }
        
    }
    
}
