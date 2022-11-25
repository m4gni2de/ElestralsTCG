using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using UnityEngine.UI;
using Decks;
using Gameplay;
using System.Threading.Tasks;
using UnityEditor.Rendering;
using System.Runtime.InteropServices.WindowsRuntime;
using System;
using PopupBox;
using System.Configuration;
using AppManagement;
using GameActions;
using RiptideNetworking;
using System.Net.Sockets;
using System.Net;
#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
#endif

public class NetworkScene : MonoBehaviour, iSceneScript
{
    #region Scene Loading
    public static string SceneName
    {
        get
        {
            return SceneHelpers.SceneName(typeof(NetworkScene));
        }
    }
    public static void LoadScene()
    {
        App.ChangeScene(SceneName);
    }
    #endregion

    #region Interface
    public void StartScene()
    {
        WorldCanvas.SetOverlay();
        DisplayManager.ClearButton();
        DisplayManager.ToggleVisible(true);
        

        GameAction dc = GameAction.Create(Disconnect);
        GameAction leave = GameAction.Create(App.TryChangeScene, "MainScene");
        DisplayManager.SetDefault(dc + leave);
    }
    #endregion


    public TMP_InputField lobbyInput;
    public GameObject mainObject;
    public Button hostButton, connectButton;
    


    public bool isConnecting = false;

    [Header("Lobby Managing")]
    public GameObject gamesListPanel;
    public VmGameSelect _templateGameSelect;
    [SerializeField]
    private ScrollRect gamesScroll;
    private Transform Content { get { return gamesScroll.content; } }
    private List<VmGameSelect> _gameList = null;
    public List<VmGameSelect> GameList
    {
        get
        {
            _gameList ??= new List<VmGameSelect>();
            return _gameList;
        }
    }
    [SerializeField]
    private Button randomGameButton;
    [SerializeField]
    private Button showGamesButton;
    [SerializeField]
    private Button closeGamesButton;

    //[Header("Server Connection")]
    //public GameObject connectionPanel;
    //public TMP_InputField ipInput;
    //public TMP_InputField portInput;
    //public Button btnServerJoin;


    private void Awake()
    {
        _templateGameSelect.Hide();
        ToggleMainMenu(false);
        ToggleGamesList(false);
        
        

    }
    private void Start()
    {
        StartScene();
        GetServerList();
        

    }

    private void OnDestroy()
    {
        NetworkManager.OnConnectAsClient -= OnClientConnected;
        NetworkManager.OnConnectionFailed -= OnClientConnectionFailed;
    }
    private void OnApplicationQuit()
    {
        NetworkManager.OnConnectAsClient -= OnClientConnected;
        NetworkManager.OnConnectionFailed -= OnClientConnectionFailed;
    }


    public void ShowMainMenu()
    {
        ToggleMainMenu(true);
    }
   
    public void ToggleMainMenu(bool show)
    {
        mainObject.SetActive(show);
        randomGameButton.interactable = show;
        showGamesButton.interactable = show;
        hostButton.interactable = show;

        //if (show)
        //{
        //    DisplayManager.OpenWindow(mainObject);
        //}


    }
    public void ToggleGamesList(bool show)
    {
        gamesListPanel.SetActive(show);
        closeGamesButton.interactable = show;

        //if (show)
        //{
        //    DisplayManager.OpenWindow(mainObject);
        //}
        //else
        //{
        //    DisplayManager.CloseWindow(mainObject);
        //}


    }
    
    #region Server/Game Choosing
    
    private async void GetServerList()
    {

        List<ServerDTO> servers = new List<ServerDTO>();
        ServerDTO local = ServerManager.LocalServer();
        servers.Add(local);
        List<ServerDTO> remoteServers = await RemoteData.ServerList();
        servers.AddRange(remoteServers);

        
        List<string> options = new List<string>();

        if (servers.Count > 0)
        {
            string msg = $"Select Server to Connect to.";
            App.ShowDropdown(msg, servers, ConnectToServer, "name", true);
            

        }
        else
        {
            App.DisplayError("There are no servers available to join.", GoHome);
            
        }

        

    }
    private void GoHome()
    {
        App.ChangeScene(SceneHelpers.SceneName(typeof(MainScene)));
    }

    protected void ConnectToServer(ServerDTO dto = null)
    {
        if (dto == null)
        {
            GetServerList();
        }
        else
        {
            if (dto.name == "LocalServer")
            {
                CreateServerAsHost();
            }
            else
            {
                string ip = dto.ip;
                ushort port = (ushort)dto.port;
                TryConnection(ip, port);
            }
            
        }
        
 
    }

   
    #endregion
    private void TryConnection(string ip, ushort port)
    {
        //btnServerJoin.interactable = false;
        //string connectIp;
        //int connectPort;
        //using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
        //{
        //    socket.Connect(ip, port);
        //    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
        //    connectIp = endPoint.Address.ToString();
        //    connectPort = (ushort)endPoint.Port;
        //}
        NetworkManager.OnConnectAsClient -= OnClientConnected;
        NetworkManager.OnConnectAsClient += OnClientConnected;
        NetworkManager.OnConnectionFailed -= OnClientConnectionFailed;
        NetworkManager.OnConnectionFailed += OnClientConnectionFailed;
        NetworkManager.Instance.Create(NetworkManager.NetworkMode.Client);
        NetworkManager.Instance.ConnectClient(ip, port);
        isConnecting = true;

    }



    #region Connection Attempts
    private void OnClientConnected(ushort networkId)
    {
        isConnecting = false;
        NetworkManager.OnConnectAsClient -= OnClientConnected;
        ToggleMainMenu(true);
        //DisplayManager.SetAction(() => Disconnect());
        

        Message message = Message.Create(MessageSendMode.reliable, (ushort)ToServer.Connected);
        message.AddUShort(NetworkManager.Instance.Client.Id);
        message.AddString(App.Account.Id);
        NetworkPipeline.SendMessageToServer(message);

        //CloseConnectionPanel();
    }

    protected void OnClientConnectionFailed()
    {
        NetworkManager.OnConnectionFailed -= OnClientConnectionFailed;
        NetworkManager.OnConnectAsClient -= OnClientConnected;
        //OpenConnectionPanel();
       // btnServerJoin.interactable = true;

    }


    #endregion

    #region Buttons
    public void CreateServerAsHost()
    {
        

        if (NetworkManager.Instance.Server == null)
        {
            NetworkManager.OnConnectAsClient += OnClientConnected;
            NetworkManager.OnConnectionFailed += OnClientConnectionFailed;
            NetworkManager.Instance.Create(NetworkManager.NetworkMode.Both);
        }
    }
    
    public void RemoteConnectButton()
    {
        GetServerList();
    }
    
    public void HostMode()
    {
        //lobbyInput.interactable = false;

        //NetworkManager.Instance.Create(NetworkManager.NetworkMode.Host);

        //GameManager.HostGame();
        ToggleMainMenu(false);
        NetworkPipeline.OnGameCreated -= OnGameCreated;
        NetworkPipeline.OnGameCreated += OnGameCreated;
        NetworkPipeline.CreateGame();

    }

    /// <summary>
    /// this is REMOTE because it's awaiting the Event OnGameCreated, which can only come from the remote server
    /// </summary>
    /// <param name="gameId"></param>
    private void OnGameCreated(string gameId)
    {
        NetworkPipeline.OnGameCreated -= OnGameCreated;
        GameManager.CreateOnlineGame(gameId, ClientManager.ConnectionType);
    }

    public void RandomGame()
    {
        JoinRandomGame();
    }
    protected async void JoinRandomGame()
    {
        List<RemoteLobbyDTO> lobbies = await RemoteData.GetAllLobbies();
        if (lobbies.Count == 0)
        {
            App.DisplayError("There are currently no active games!");
            ToggleGamesList(false);
            ClearLobbies();
            ToggleMainMenu(true);
        }
        else
        {
            int rand = UnityEngine.Random.Range(0, lobbies.Count);
            RemoteLobbyDTO dto = lobbies[rand];
            TryConnectToLobby(dto.lobbyKey);
        }
    }

    public void ShowGamesList()
    {
        RefreshLobbies();
        GetActiveLobbies();

        //DisplayManager.SetAction(() => HideGamesDisplayLobby());
        DisplayManager.AddAction(HideGamesDisplayLobby);
    }

    protected async void GetActiveLobbies()
    {
        List<RemoteLobbyDTO> lobbies = await RemoteData.GetAllLobbies();
        if (lobbies.Count == 0)
        {
            App.DisplayError("There are currently no active games!");
            ToggleGamesList(false);
            ClearLobbies();
            ToggleMainMenu(true);
        }
        else
        {
            ToggleGamesList(true);
            LoadLobbies(lobbies);
            ToggleMainMenu(false);
        }
    }
    
    public void ConnectMode(VmGameSelect vm)
    {
        RemoteLobbyDTO dto = vm.Lobby;

        for (int i = 0; i < GameList.Count; i++)
        {
            GameList[i].Freeze();
        }

        TryConnectToLobby(dto.lobbyKey);


    }
    private void TryConnectToLobby(string lobby)
    {
        //lobbyInput.interactable = false;
        //hostButton.interactable = false;
        //connectButton.interactable = false;

        ToggleGamesList(false);
        NetworkPipeline.OnGameJoined += OnGameJoined;
        NetworkPipeline.OnJoinedFailed += JoinLobbyFailed;
        NetworkPipeline.JoinNetworkLobby(lobby);
       

    }

    private void JoinLobbyFailed(string lobbyId)
    {
        NetworkPipeline.OnGameJoined -= OnGameJoined;
        NetworkPipeline.OnJoinedFailed -= JoinLobbyFailed;

        App.DisplayError($"Failed to join Game!");

        ShowGamesList();
        
        //lobbyInput.interactable = true;
        //hostButton.interactable = true;
        //connectButton.interactable = true;
    }
    //private async Task<bool> ConnectToIP()
    //{
    //    RemoteLobbyDTO dto = await RemoteData.Lob
    //}


    private void OnGameJoined(string gameId, List<NetworkPlayer> otherPlayers)
    {
        NetworkPipeline.OnGameJoined -= OnGameJoined;
        NetworkPipeline.OnJoinedFailed -= JoinLobbyFailed;

        if (otherPlayers.Count > 1) { throw new System.Exception("GAME DOES NOT SUPPORT MORE THAN 2 PLAYERS AT THIS TIME."); }
        NetworkPlayer player = otherPlayers[0];

        GameManager.JoinGame(gameId, player);
    }

    #endregion


    #region Back Commands
    private void Disconnect()
    {
        ClientManager.Disconnect();
        ToggleMainMenu(false);
        ToggleGamesList(false);
        GetServerList();
    }
    private void HideGamesDisplayLobby()
    {
        ToggleMainMenu(true);
        ToggleGamesList(false);
    }
    #endregion

    #region Game Lobbies
    private bool ContainsLobby(string lobbyKey)
    {
        for (int i = 0; i < GameList.Count; i++)
        {
            if (GameList[i].Lobby != null && GameList[i].Lobby.lobbyKey.ToLower() == lobbyKey.ToLower()) { return true; }
        }
        return false;
    }
    protected void RefreshLobbies()
    {
        for (int i = 0; i < GameList.Count; i++)
        {
            GameList[i].Refresh();
        }
       
    }
    protected void ClearLobbies()
    {
        for (int i = 0; i < GameList.Count; i++)
        {
            Destroy(GameList[i]);
        }
        GameList.Clear();
    }
    protected void LoadLobbies(List<RemoteLobbyDTO> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (!ContainsLobby(list[i].lobbyKey))
            {
                VmGameSelect sel = Instantiate(_templateGameSelect, Content);
                sel.Load(list[i]);
                sel.Show();
            }
            
        }
    }
    #endregion
}
