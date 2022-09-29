using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
using Gameplay.Networking;
using Gameplay;

public class NetworkGameManager
{

    private static NetworkGameManager _instance = null;
    public static NetworkGameManager Instance { get; private set; }

    public OnlineGame ActiveGame { get; set; }

    #region Network Setup
    NetworkGameManager(OnlineGame game)
    {
        ActiveGame = game;
        GameManager.OnGameLoaded += HostGameWatcher;
        App.ChangeScene(GameManager.SceneName);
    }
    public static void HostGame()
    {
        OnlineGame game = OnlineGame.NewGame(ServerManager.Instance.myAddressGlobal, ServerManager.Instance.Server.Port);
        _instance = new NetworkGameManager(game);      
    }
    protected static void HostGameWatcher()
    {
        GameManager.OnGameLoaded -= HostGameWatcher;
        Instance.LoadHostClient();
    }
    protected void LoadHostClient()
    {
        GameManager.Instance.RegisterFields();
        ClientManager.Instance.ConnectAsHost(Instance.ActiveGame.port);
    }

    #endregion
}
