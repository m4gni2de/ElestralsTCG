using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RiptideNetworking;
using UnityEngine.UI;
using Decks;
using Gameplay;

public class NetworkScene : MonoBehaviour
{
    public static string SceneName = "NetworkLobby";

    public TMP_InputField lobbyInput;
    public GameObject mainObject;
    public Button hostButton, connectButton;

    private void Awake()
    {
        //lobbyInput.interactable = false;
        //hostButton.interactable = false;
        //connectButton.interactable = false;
        //mainObject.SetActive(false);

    }
    private void Start()
    {
        TryConnection();
    }

    private void OnDestroy()
    {
        NetworkManager.OnClientConnected -= OnClientConnected;
    }
    private void OnApplicationQuit()
    {
        NetworkManager.OnClientConnected -= OnClientConnected;
    }


    private void TryConnection()
    {
        NetworkManager.OnClientConnected -= OnClientConnected;
        NetworkManager.OnClientConnected += OnClientConnected;
        NetworkManager.Instance.Create(NetworkManager.NetworkMode.Client);
        NetworkManager.Instance.Connect(NetworkManager.localIp);
    }



    #region Connection Attempts
    private void OnClientConnected(ushort networkId)
    {
        NetworkManager.OnClientConnected -= OnClientConnected;
        lobbyInput.interactable = true;
        hostButton.interactable = true;
        connectButton.interactable = true;
        mainObject.SetActive(true);

        Message message = Message.Create(MessageSendMode.reliable, (ushort)ToServer.Connected);
        message.AddUShort(NetworkManager.Instance.Client.Id);
        message.AddString(App.Account.Id);
        NetworkPipeline.SendMessageToServer(message);
    }
   

    #endregion



    #region Buttons
    public void HostMode()
    {
        lobbyInput.interactable = false;
        mainObject.SetActive(false);
        //NetworkManager.Instance.Create(NetworkManager.NetworkMode.Host);

        //GameManager.HostGame();
        NetworkPipeline.OnGameCreated -= OnGameCreated;
        NetworkPipeline.OnGameCreated += OnGameCreated;
        NetworkPipeline.CreateGame();

    }
    private void OnGameCreated(string gameId)
    {
        NetworkPipeline.OnGameCreated -= OnGameCreated;
        OnlineGameManager.CreateGame(gameId);
    }

   

    public void ConnectMode()
    {

        lobbyInput.interactable = false;
        mainObject.SetActive(false);
        string lobby = lobbyInput.text;
        NetworkPipeline.OnGameJoined += OnGameJoined;
        NetworkPipeline.JoinNetworkLobby(lobby);   
    }
    private void OnGameJoined(string gameId, List<NetworkPlayer> otherPlayers)
    {
        NetworkPipeline.OnGameJoined -= OnGameJoined;

        if (otherPlayers.Count > 1) { throw new System.Exception("GAME DOES NOT SUPPORT MORE THAN 2 PLAYERS AT THIS TIME."); }
        NetworkPlayer player = otherPlayers[0];

        OnlineGameManager.JoinGame(gameId, player);
    }

    #endregion
}
