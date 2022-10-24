using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RiptideNetworking;
using UnityEngine.UI;
using Decks;
using Gameplay;
using System.Threading.Tasks;
using UnityEditor.Rendering;
using System.Runtime.InteropServices.WindowsRuntime;
using System;
using PopupBox;
using System.Configuration;

public class NetworkScene : MonoBehaviour
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
        //connectionPanel.SetActive(false);
        //gameLobby.SetActive(false);

        //btnServerJoin.gameObject.SetActive(false);
        //btnServerJoin.gameObject.SetActive(false);
        //lobbyInput.interactable = false;
        //hostButton.interactable = false;
        //connectButton.interactable = false;
        //mainObject.SetActive(true);

        _templateGameSelect.Hide();
        ToggleMainMenu(false);
        ToggleGamesList(false);
//#if UNITY_EDITOR
//        btnServerJoin.gameObject.SetActive(false);
//        lobbyInput.interactable = false;
//        hostButton.interactable = false;
//        connectButton.interactable = false;
       
//#else
//        btnServerJoin.gameObject.SetActive(true);
//        lobbyInput.interactable = true;
//        hostButton.interactable = true;
//        connectButton.interactable = true;
//#endif


    }
    private void Start()
    {
#if UNITY_EDITOR
        //TryConnection(NetworkManager.localIp, 7777);
#endif
        GetServerList();

    }

    private void OnDestroy()
    {
        NetworkManager.OnClientConnected -= OnClientConnected;
        NetworkManager.OnConnectionFailed -= OnClientConnectionFailed;
    }
    private void OnApplicationQuit()
    {
        NetworkManager.OnClientConnected -= OnClientConnected;
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

    }
    public void ToggleGamesList(bool show)
    {
        gamesListPanel.SetActive(show);
        closeGamesButton.interactable = show;

      
    }
    
    #region Server/Game Choosing
    private async void GetServerList()
    {
        List<ServerDTO> servers = await RemoteData.ServerList();
        List<string> options = new List<string>();

        if (servers.Count > 0)
        {
            string msg = $"Select Server to Connect to.";
            App.ShowDropdown(msg, servers, ConnectToServer, "name");

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

    protected void ConnectToServer(ServerDTO dto)
    {
        if (dto == null)
        {
            GetServerList();
        }
        else
        {
            string ip = dto.ip;
            ushort port = (ushort)dto.port;
            TryConnection(ip, port);
        }
        
 
    }

   
    #endregion
    private void TryConnection(string ip, ushort port)
    {
        //btnServerJoin.interactable = false;

        NetworkManager.OnClientConnected -= OnClientConnected;
        NetworkManager.OnClientConnected += OnClientConnected;
        NetworkManager.OnConnectionFailed -= OnClientConnectionFailed;
        NetworkManager.OnConnectionFailed += OnClientConnectionFailed;
        NetworkManager.Instance.Create(NetworkManager.NetworkMode.Client);
        NetworkManager.Instance.Connect(ip, port);
        isConnecting = true;

    }



    #region Connection Attempts
    private void OnClientConnected(ushort networkId)
    {
        isConnecting = false;
        NetworkManager.OnClientConnected -= OnClientConnected;
        ToggleMainMenu(true);

        Message message = Message.Create(MessageSendMode.reliable, (ushort)ToServer.Connected);
        message.AddUShort(NetworkManager.Instance.Client.Id);
        message.AddString(App.Account.Id);
        NetworkPipeline.SendMessageToServer(message);

        //CloseConnectionPanel();
    }

    protected void OnClientConnectionFailed()
    {
        NetworkManager.OnConnectionFailed -= OnClientConnectionFailed;
        NetworkManager.OnClientConnected -= OnClientConnected;
        //OpenConnectionPanel();
       // btnServerJoin.interactable = true;

    }


    #endregion



    #region Buttons
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
    private void OnGameCreated(string gameId)
    {
        NetworkPipeline.OnGameCreated -= OnGameCreated;
        GameManager.CreateOnlineGame(gameId);
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
        ToggleGamesList(true);
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

    //#region Device Connection
    //public void ServerConnectButton()
    //{
    //    CloseConnectionPanel();

    //    if (ConnectionValid())
    //    {
    //        string ip = ipInput.text.Trim();
    //        if (ClientManager.client == null)
    //        {
    //            TryConnection(ip, 7777);
    //        }
    //        else
    //        {
    //            if (!ClientManager.IsConnected())
    //            {
    //                TryConnection(ip, 7777);
    //            }
    //            else
    //            {
    //                App.ShowMessage($"You are already connected to the server!");
    //            }
    //        }
            
    //    }
       
    //}

    //public void ToggleConnectionPanel()
    //{
    //    if (!isConnecting)
    //    {
    //        if (connectionPanel.activeSelf == true)
    //        {
    //            CloseConnectionPanel();
    //        }
    //        else
    //        {
    //            OpenConnectionPanel();
    //        }
    //    }
        
    //}
    //private bool ConnectionValid()
    //{
    //    if (string.IsNullOrEmpty(ipInput.text)) { return App.DisplayError($"IP Address cannot be left blank!", OpenConnectionPanel); }
    //    if (string.IsNullOrEmpty(portInput.text)) { return App.DisplayError($"Port cannot be left blank!", OpenConnectionPanel); }
    //    return true;
    //}
    //public void OpenConnectionPanel()
    //{
    //    mainObject.SetActive(false);
    //    connectionPanel.SetActive(true);
    //    btnServerJoin.gameObject.SetActive(true);
       

    //    if (ClientManager.IsConnected() || ClientManager.IsConnecting())
    //    {
    //        ipInput.interactable = false;
    //        portInput.interactable = false;
    //        btnServerJoin.interactable = false;
    //    }
    //    else
    //    {
    //        ipInput.interactable = true;
    //        portInput.interactable = true;
    //        btnServerJoin.interactable = true;
    //    }

        
    //}
    //public void CloseConnectionPanel()
    //{
    //    mainObject.SetActive(true);
    //    connectionPanel.SetActive(false);
    //    btnServerJoin.gameObject.SetActive(true);
    //    btnServerJoin.interactable = true;
    //    ipInput.interactable = false;
    //    portInput.interactable = false;
    //}



    //#endregion


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
