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
using PopupBox;
using UnityEngine.Events;
using RiptideNetworking.Transports;
#if UNITY_EDITOR
using UnityEditor.Networking.PlayerConnection;
#endif
using System.Threading.Tasks;

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
    public static bool IsServer
    {
        get
        {
            if (!Instance) { return false; }
            return Instance.Server.IsRunning;
        }
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
    public ServerDTO connectedServer { get; set; }

    #endregion

    #region Server Properties
    public Server Server { get; private set; }
    [SerializeField] private ushort maxClientCount;
    public string myAddressLocal;
    public string myAddressGlobal;
    public static List<IConnectionInfo> ConnectedClients = new List<IConnectionInfo>();
    private ServerDTO _serverInfo = null;
    public ServerDTO serverInfo { get { return _serverInfo; } }
    public int serverKey = -1;
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
        ServerManager.Tick();
        if (Client != null) { Client.Tick(); }
        

    }

   

    public void Create(NetworkMode mode)
    {
        networkMode = mode;
        if (mode == NetworkMode.Client) { CreateClient(); } else if (mode == NetworkMode.Host) { CreateServer(); } else { CreateServerAsHost();  }
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

    public void ConnectClient(string serverIp, ushort serverPort = 7777)
    {
        this.ip = serverIp;
        this.port = serverPort;
        if (Client == null) { CreateClient(); }
        Client.Connect($"{ip}:{port}");
        DisplayBox box = App.ShowWaitingMessage($"Connecting to server...");
        PopupManager.Instance.AddCloseWatcher(box, OnConnectionChanged);
    }

    private static UnityEvent _OnConnectionChanged = null;
    public static UnityEvent OnConnectionChanged
    {
        get
        {
            _OnConnectionChanged ??= new UnityEvent();
            return _OnConnectionChanged;
        }
    }
    public static event Action<ushort> OnConnectAsClient;
    private void DidConnect(object sender, EventArgs e)
    {
        OnConnectAsClient?.Invoke(Client.Id);
        OnConnectionChanged.Invoke();

        //PlayerConnected();
    }
    //private async void PlayerConnected()
    //{
    //   string code = await App.ActiveDeck.DoUpload();
    //   if (string.IsNullOrEmpty(code)) { Client.Disconnect(); return; }
    //   Player.CreateLocalPlayer(Client.Id, App.Account.Id, App.ActiveDeck);
    //}
    
   

    public static event Action OnConnectionFailed;
    private void FailedToConnect(object sender, EventArgs e)
    {
        OnConnectionChanged.Invoke();
        App.DisplayError($"Connection failed!");
        //UIManager.Singleton.BackToMain();
    }

    public static event Action OnClientLeave;
    private void ClientLeft(object sender, ClientDisconnectedEventArgs e)
    {
        //Destroy(OnlinePlayer.list[e.Id].gameObject);
    }

    public static event Action OnClientDisconnected;
    private void DidDisconnect(object sender, EventArgs e)
    {
        OnConnectionChanged.Invoke();
        OnClientDisconnected?.Invoke();
        connectedServer = null;
    }
    
    #endregion


    public void SendMessageToServer(Message message)
    {
        if (networkMode == NetworkMode.Client)
        {
            if (Client != null)
            {
                Client.Send(message);
            }
            
        }
        //else if (networkMode == NetworkMode.Both)
        //{
        //    if (Server != null)
        //    {
        //        Server.
        //        ServerManager.SendToClientsAll(message);
        //    }
        //}
    }



    #region Server Mode
    
    private void CreateServer()
    {
        if (Server == null)
        {
            GetServerAddresses();
            Application.targetFrameRate = 60;

            RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
            Server = new Server();
            Server.Start(port, maxClientCount);
            Server.ClientConnected += ServerManager.OnClientConnected;
            Server.ClientDisconnected += ServerManager.PlayerDisconnect;
            GetServerKey();
        }
        
    }

    private async void CreateServerAsHost()
    {
        if (Server == null)
        {
            GetServerAddresses();
            Application.targetFrameRate = 60;

            
            RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
            Server = new Server();
            Server.Start(port, maxClientCount);
            Server.ClientConnected += ServerManager.OnClientConnected;
            Server.ClientDisconnected += ServerManager.PlayerDisconnect;

            _serverInfo = await RemoteData.AddServer();
            if (_serverInfo == null) { return; }
            serverKey = _serverInfo.serverKey;

            ConnectClient(_serverInfo.ip, (ushort)_serverInfo.port);


        }
    }
    protected async void GetServerKey()
    {
        _serverInfo = await RemoteData.AddServer();
        if (_serverInfo != null)
        {
            serverKey = _serverInfo.serverKey;
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


   
    private void Shutdown()
    {
        if (Client != null)
        {
            Client.Connected -= DidConnect;
            Client.ConnectionFailed -= FailedToConnect;
            Client.ClientDisconnected -= ClientLeft;
            Client.Disconnected -= DidDisconnect;
            Client.Disconnect();
            ClientManager.Disconnect();
            Client = null;

        }
        if (Server != null)
        {
            ServerManager.Shutdown();
        }

        
    }
    #endregion
    private void OnDestroy()
    {
        Shutdown();
        if (Instance != null) { Instance = null; }

    }
    private void OnApplicationQuit()
    {
        Shutdown();

    }



    public void AwaitReconnection(NetworkPlayer player)
    {
        player.isConnected = false;
        float waitTime = 5f;
        StartCoroutine(AwaitReconnect(player, waitTime)); 
    }

    protected IEnumerator AwaitReconnect(NetworkPlayer player, float waitTime)
    {

        float acumTime = 0f;
        do
        {

            yield return new WaitForEndOfFrame();
        } while (true && acumTime <= waitTime);

        if (acumTime >= waitTime)
        {
            ServerGame.EndGame();

        }
    }
}


