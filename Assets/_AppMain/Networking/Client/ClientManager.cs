using Gameplay.Networking;
using RiptideNetworking;
using RiptideNetworking.Utils;
using SimpleSQL.Demos;
using System;
using UnityEngine;

public enum s2c : ushort
{
    playerAdded = 2,
}

public enum c2s : ushort
{
    registerPlayer = 1,
}

public class ClientManager : MonoBehaviour
{


    public static ClientManager Instance { get; private set; }
    //public static ClientManager Instance
    //{
    //    get => _Instance;
    //    set
    //    {
    //        if (_Instance == null)
    //            _Instance = value;
    //        else if (_Instance != value)
    //        {
    //            Debug.Log($"{nameof(ClientManager)} instance already exists, destroying duplicate!");
    //            Destroy(value);
    //        }
    //    }
    //}

    public Client Client { get; private set; }
    public bool hasClient
    {
        get => Client != null;
    }

    public static readonly string localIp = "127.0.0.1";
    [SerializeField] private string ip;
    [SerializeField] private ushort port;
    public bool isHost { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Create()
    {
        if (Client == null)
        {
            
            RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

            Client = new Client();
            Client.Connected += DidConnect;
            Client.ConnectionFailed += FailedToConnect;
            Client.ClientDisconnected += PlayerLeft;
            Client.Disconnected += DidDisconnect;
        }
    }

    private void FixedUpdate()
    {
        if (hasClient)
        {
            Client.Tick();
        }
        
    }

    private void OnApplicationQuit()
    {
        if (hasClient)
        {
            Client.Disconnect();
        }
        
    }

    public void Connect()
    {
        if (Client == null) { Create(); }
        Client.Connect($"{ip}:{port}");
    }
    public void ConnectAsHost(ushort port)
    {
        ip = localIp;
        this.port = port;
        isHost = true;
        Connect();
    }

    private void DidConnect(object sender, EventArgs e)
    {
        //GameManager.SendPlayer();

    }
    
    private void FailedToConnect(object sender, EventArgs e)
    {
        UIManager.Singleton.BackToMain();
    }

    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        //Destroy(OnlinePlayer.list[e.Id].gameObject);
    }

    private void DidDisconnect(object sender, EventArgs e)
    {
        UIManager.Singleton.BackToMain();
    }

    private void OnDestroy()
    {
        if (Instance != null) { Instance = null; }
    }
}
