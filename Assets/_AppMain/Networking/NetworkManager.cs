using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;
using RiptideNetworking.Utils;
using System;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Decks;
using Gameplay;
using nsSettings;
using Gameplay.Decks;

public class NetworkManager : MonoBehaviour
{
    public enum NetworkMode
    {
        None = -1,
        Client = 0,
        Host = 1,
        Both = 2,
    }
#   region Instance Properties
    private static NetworkManager _instance = null;
    public static NetworkManager Instance
    {
        get => _instance;
        private set => _instance = value;
    }
    #endregion

    #region Properties
    private NetworkMode _networkMode = NetworkMode.None;
    public NetworkMode networkMode
    {
        get => _networkMode;
        private set
        {
            if (_networkMode != value)
            {
                if (_networkMode == NetworkMode.Client && Client != null)
                {
                    throw new System.Exception($"Network is running in Client Mode. Close your Connection before switching Network Modes.");
                }
                if (_networkMode == NetworkMode.Host && Server != null)
                {
                    throw new System.Exception($"Network is running in Host Mode. Ending this Connection will close the current Server. End all connections before switching Network Modes.");
                }
            }
            _networkMode = value;
        }
    }

    [SerializeField] private ushort port;

    #region Client Properpties
    public Client Client { get; private set; }
    public static readonly string localIp = "127.0.0.1";
    [SerializeField] private string ip;
    
    #endregion

    #region Server Properties
    public Server Server { get; private set; }
    public string myAddressLocal;
    public string myAddressGlobal;
    [SerializeField] private ushort maxClientCount = 2;
    #endregion

    #endregion


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

    private void FixedUpdate()
    {
        if (Client != null) { Client.Tick(); } else if (Server != null) { Server.Tick(); }

    }

    private void OnApplicationQuit()
    {
        if (Client != null) { Client.Disconnect(); } else if (Server != null) { Server.Stop(); }

    }

    public void Create(NetworkMode mode)
    {
        networkMode = mode;
        if (mode == NetworkMode.Client) { CreateClient(); } else if (mode == NetworkMode.Host) { CreateServer(); }
    }

    #region Client Mode
    private void CreateClient()
    {
        if (Client == null)
        {
            RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

            Client = new Client();
            Client.Connected += DidConnect;
            Client.ConnectionFailed += FailedToConnect;
            Client.ClientDisconnected += ClientLeft;
            Client.Disconnected += DidDisconnect;
        }
    }
    public void Connect(string ip)
    {
        this.ip = ip;
        if (Client == null) { CreateClient(); }
        Client.Connect($"{ip}:{port}");
    }

    private void DidConnect(object sender, EventArgs e)
    {
        PlayerConnected();
        
    }
    private async void PlayerConnected()
    {
        string code = await App.ActiveDeck.DoUpload();
       if (string.IsNullOrEmpty(code)) { Client.Disconnect(); return; }
        Player.CreateLocalPlayer(Client.Id, App.Account.Id, App.ActiveDeck);
        GameManager.OnGameLoaded += GameLoaded;
        App.ChangeScene(GameManager.SceneName);
    }
    
    private void GameLoaded()
    {
        GameManager.OnGameLoaded -= GameLoaded;
        Player.SendLocalPlayer();
    }
   
    private void FailedToConnect(object sender, EventArgs e)
    {
        //UIManager.Singleton.BackToMain();
    }

    private void ClientLeft(object sender, ClientDisconnectedEventArgs e)
    {
        //Destroy(OnlinePlayer.list[e.Id].gameObject);
    }

    private void DidDisconnect(object sender, EventArgs e)
    {
        //UIManager.Singleton.BackToMain();
    }
    
    #endregion





    #region Server Mode
    
    private void CreateServer()
    {
        if (Server == null)
        {
            Application.targetFrameRate = 60;

            RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

            Server = new Server();
            Server.Start(port, maxClientCount);
            Server.ClientDisconnected += PlayerLeft;
        }
        
    }
    void GetServerAddresses()
    {
        //Get the local IP
        IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in hostEntry.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                myAddressLocal = ip.ToString();
                break;
            } //if
        } //foreach
        //Get the global IP
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.ipify.org");
        request.Method = "GET";
        request.Timeout = 1000; //time in ms
        try
        {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                myAddressGlobal = reader.ReadToEnd();
            } //if
            else
            {
                Debug.LogError("Timed out? " + response.StatusDescription);
                myAddressGlobal = "127.0.0.1";
            } //else
        } //try
        catch (WebException ex)
        {
            Debug.Log("Likely no internet connection: " + ex.Message);
            myAddressGlobal = "127.0.0.1";
        } //catch
          //myAddressGlobal=new System.Net.WebClient().DownloadString("https://api.ipify.org"); //single-line solution for the global IP, but long time-out when there is no internet connection, so I prefer to do the method above where I can set a short time-out time
    } //Start
    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        //Destroy(OnlinePlayer.list[e.Id].gameObject);
    }
    #endregion
    private void OnDestroy()
    {
        if (Instance != null) { Instance = null; }
    }
}


