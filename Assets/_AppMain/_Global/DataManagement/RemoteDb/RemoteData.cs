using System.Collections;
using System.Collections.Generic;
using Decks;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using Defective.JSON;
using Gameplay.Decks;
using JetBrains.Annotations;
using System.Security.Cryptography;
using System.Security.Policy;
using Gameplay;
using System.Net;
using SimpleSQL;
using System;
using nsSettings;
using Gameplay.P2P;

public class RemoteData
{


    private readonly static string pvpDecks = "http://149.28.60.66/pvpDeck.php?";
    private readonly static string pvpLobby = "http://149.28.60.66/pvpLobby.php?";
    private readonly static string serverInfo = "http://149.28.60.66/pvpServer.php?";




    public static async Task<string> DoRemoteQuery(string url, WWWForm form)
    {

        string result = await AppManager.DoPostRequestWithPayload(url, form);
       
        return result;
    }

    private static bool HasResults(string json)
    {
        if (json == "error") { return false; }
        if (string.IsNullOrEmpty(json)) { return false; }
        if (json == null) { return false; }
        if (json == "[]") { return false; }
        return true;
    }

    public static async Task<DownloadedDeckDTO> SearchDeck(string deckKey)
    {
        WWWForm form = new WWWForm();
        form.AddField("action", "search");
        form.AddField("deckKey", deckKey);

        string results = await DoRemoteQuery(pvpDecks, form);
        if (results != "error" && results != "" && results != null)
        {
            var array = new JSONObject(results);
            if (!array.isNull)
            {
                foreach (var prop in array)
                {
                    DownloadedDeckDTO deck = new DownloadedDeckDTO();
                    deck.deckKey = prop[0].stringValue;
                    deck.title = prop[1].stringValue;
                    DataList d = JsonConvert.DeserializeObject<DataList>(prop[2].stringValue);
                    deck.deck = d.items;
                    deck.owner = prop[3].stringValue;
                    deck.whenUpload = App.UnixTimestampToDateTime(prop[4].doubleValue);
                    deck.downloads = prop[5].intValue;
                    deck.lastDownload = App.UnixTimestampToDateTime(prop[6].doubleValue);
                    return deck;
                }

                return null;
            }
            else
            {
                return null;
            }

        }
        else
        {
            return null;
        }

    }

    public static async Task<bool> AddDeckToRemoteDB(Decklist deck)
    {

        WWWForm form = new WWWForm();
        form.AddField("action", "insert");
        form.AddField("deckKey", deck.UploadCode);
        form.AddField("title", deck.DeckName);
        form.AddField("deck", deck.GetCardList);
        form.AddField("owner", deck.Owner);
        //form.AddField("name", deck.deckName);

        string result = await DoRemoteQuery(pvpDecks, form);

        if (result == "error")
        {
            return false;
        }

        return true;
    }

    public static async Task<bool> RemoveDeckFromRemoteDB(Decklist deck)
    {

        WWWForm form = new WWWForm();
        form.AddField("action", "remove");
        form.AddField("deckKey", deck.UploadCode);
        string result = await DoRemoteQuery(pvpDecks, form);

        if (result == "error")
        {
            return false;
        }

        return true;
    }


    public static async Task<List<UploadedDeckDTO>> ViewDecks(string whereClause)
    {
        List<UploadedDeckDTO> decks = new List<UploadedDeckDTO>();
        WWWForm form = new WWWForm();
        form.AddField("action", "view");

        string results = await DoRemoteQuery(pvpDecks, form);
        if (results != "error" && results != "" && results != null)
        {
            var array = new JSONObject(results);
            if (!array.isNull)
            {
                foreach (var prop in array)
                {
                    UploadedDeckDTO deck = new UploadedDeckDTO();
                    deck.deckKey = prop[0].stringValue;
                    deck.title = prop[1].stringValue;
                    DataList d = JsonConvert.DeserializeObject<DataList>(prop[2].stringValue);
                    deck.deck = d.items;
                    decks.Add(deck);
                }
            }
        }

        return decks;
    }

   

   

    #region Server List
    public static async Task<List<ServerDTO>> ServerList()
    {

        List<ServerDTO> list = new List<ServerDTO>();
        WWWForm form = new WWWForm();
        form.AddField("action", "getall");

        string results = await DoRemoteQuery(serverInfo, form);
        if (HasResults(results))
        {
            var array = new JSONObject(results);
            if (!array.isNull)
            {
                foreach (var prop in array)
                {
                    ServerDTO dto = new ServerDTO();

                    dto.serverKey = prop[0].intValue;
                    dto.ip = prop[1].stringValue;
                    dto.port = prop[2].intValue;
                    dto.name = prop[3].stringValue;
                    dto.playersCurrent = prop[4].intValue;
                    dto.playersMax = prop[5].intValue;
                    dto.connType = prop[6].intValue;
                    list.Add(dto);
                }
            }

        }
       
        return list;
    }
    #endregion


    #region Lobby Management
    public static async Task<string> CreateLobby(Decklist deck)
    {
        string lobbyId = UniqueString.CreateId(14);
        RemotePlayerDTO p = new RemotePlayerDTO { userId = App.Account.Id, deckList = deck.GetCardList };

        string playerString = JsonUtility.ToJson(p);
        
        WWWForm form = new WWWForm();
        form.AddField("action", "Create");
        form.AddField("lobbyCode", lobbyId);
        form.AddField("owner", App.Account.Id);

        await DoRemoteQuery(pvpLobby, form);
        return lobbyId;
        
    }

    public static async Task<bool> JoinLobby(string lobby, string playerId)
    {

        WWWForm form = new WWWForm();
        form.AddField("action", "join");
        form.AddField("player2", playerId);
        form.AddField("lobbyKey", lobby);


        string results = await DoRemoteQuery(pvpLobby, form);
        return results != "error";
        //if (results == "error" || results != "" || results == null)
        //{

        //}

    }

    public static async Task<RemoteLobbyDTO> GetLobby(string lobbyId)
    {

        WWWForm form = new WWWForm();
        form.AddField("action", "find");
        form.AddField("lobbyKey", lobbyId);

        string results = await DoRemoteQuery(pvpLobby, form);
        if (results != "error" && results != "" && results != null)
        {
            var array = new JSONObject(results);
            if (!array.isNull)
            {
                foreach (var prop in array)
                {
                    RemoteLobbyDTO lobby = new RemoteLobbyDTO();
                    lobby.lobbyKey = prop[0].stringValue;
                    lobby.joinIp = prop[1].stringValue;
                    lobby.player1 = prop[2].stringValue;
                    lobby.player2 = prop[3].stringValue;
                    return lobby;
                }
               
            }

        }
        return null;

    }

    public static async Task<List<RemoteLobbyDTO>> GetAllLobbies()
    {
        List<RemoteLobbyDTO> list = new List<RemoteLobbyDTO>();
        WWWForm form = new WWWForm();
        form.AddField("action", "alllobby");

        string results = await DoRemoteQuery(pvpLobby, form);
        if (HasResults(results))
        {
            var array = new JSONObject(results);
            if (!array.isNull)
            {
                foreach (var prop in array)
                {
                    RemoteLobbyDTO lobby = new RemoteLobbyDTO();
                    lobby.lobbyKey = prop[0].stringValue;
                    lobby.joinIp = prop[1].stringValue;
                    lobby.player1 = prop[2].stringValue;
                    lobby.player2 = prop[3].stringValue;
                    list.Add(lobby);
                }

                
            }
        }

        return list;
    }

    public static async Task<bool> RegisterPlayer(int serverId)
    {
        WWWForm form = new WWWForm();
        form.AddField("action", "registerPlayer");
        form.AddField("serverId", serverId);
        form.AddField("userId", App.Account.Id);
        form.AddField("username", App.Account.Name);
        form.AddField("sleeves", SettingsManager.Account.Settings.Sleeves);
        form.AddField("playmatt", SettingsManager.Account.Settings.Playmatt);

        string results = await DoRemoteQuery(pvpLobby, form);
        return results != "error";
    }

    public static async Task<bool> UpdatePlayer(int serverId)
    {
        WWWForm form = new WWWForm();
        form.AddField("action", "updatePlayer");
        form.AddField("serverId", serverId);

        string results = await DoRemoteQuery(pvpLobby, form);
        return results != "error";
    }
    public static async Task<ConnectedPlayerDTO> FindPlayer(string id)
    {

        WWWForm form = new WWWForm();
        form.AddField("action", "findPlayer");
        form.AddField("userId", id);

        string results = await DoRemoteQuery(pvpLobby, form);
        if (HasResults(results))
        {
            var array = new JSONObject(results);
            if (!array.isNull)
            {
                foreach (var prop in array)
                {
                    ConnectedPlayerDTO player = new ConnectedPlayerDTO();
                    player.serverId = prop[0].intValue;
                    player.userId = prop[1].stringValue;
                    player.username = prop[2].stringValue;
                    player.whenConnect = DateTime.Parse(prop[3].stringValue);
                    player.sleeves = prop[4].intValue;
                    player.playmatt = prop[5].intValue;
                    player.lobby = prop[6].stringValue;
                    return player;
                }


            }
        }
        return null;
    }

    public static async void DeletePlayer(string userId)
    {
        WWWForm form = new WWWForm();
        form.AddField("action", "deletePlayer");
        form.AddField("id", userId);

        string results = await DoRemoteQuery(pvpLobby, form);
    }
    #endregion

    #region Lobby Management(AS SERVER)
    public static async Task<string> CreateLobby(string player1)
    {
        string lobbyId = UniqueString.CreateId(5);

        WWWForm form = new WWWForm();
        form.AddField("action", "create");
        form.AddField("lobbyKey", lobbyId);
        form.AddField("joinIp", NetworkManager.Instance.myAddressGlobal);
        form.AddField("player1", player1);

        await DoRemoteQuery(pvpLobby, form);
        return lobbyId;

    }

  
   
    public static async Task<bool> RemoveLobby(string lobbyId)
    {
        WWWForm form = new WWWForm();
        form.AddField("action", "remove");
        form.AddField("lobbyKey", lobbyId);

        string results = await DoRemoteQuery(pvpLobby, form);
        return results != "error";
    }

    public static async Task<bool> AddDeckToLobby(string lobbyId, Player player)
    {
        int playerIndex = 1;
        string actionColumn = "update1";
        if (!player.IsLocal) { playerIndex = 2; }
        if (playerIndex == 2) { actionColumn = "update2"; }

        //List<string> cards = cardlist.ToList();

        WWWForm form = new WWWForm();
        form.AddField("action", actionColumn);
        form.AddField("deckList", "");
        form.AddField("lobbyKey", lobbyId);

        string results = await DoRemoteQuery(serverInfo, form);
        return results != "error";
    }
    #endregion

    #region Server Management
    public static async Task<ServerDTO> AddServer()
    {

        string serverName = UniqueString.Create("srv", 14);
        int servKey = await ServerCount();
        WWWForm form = new WWWForm();
        form.AddField("action", "set");
        form.AddField("serverKey", servKey);
        string ipAddress = NetworkManager.Instance.myAddressLocal;
#if UNITY_EDITOR
#else
         ipAddress = NetworkManager.Instance.myAddressGlobal;
#endif

        form.AddField("ip", ipAddress);
        form.AddField("port", (int)NetworkManager.Instance.Server.Port);
        form.AddField("name", serverName);
        form.AddField("playersCurrent", 0);
        form.AddField("playersMax", (int)NetworkManager.Instance.Server.MaxClientCount);
        form.AddField("connType", (int)ConnectionType.P2P);

        ServerDTO dto = new ServerDTO
        {
            serverKey = servKey,
            ip = ipAddress,
            port = (int)NetworkManager.Instance.Server.Port,
            name = serverName,
            playersCurrent = 0,
            playersMax = (int)NetworkManager.Instance.Server.MaxClientCount,
            connType = (int)ConnectionType.P2P,

        };
        string results = await DoRemoteQuery(serverInfo, form);
        if (results != "error")
        {
            return dto;
        }
        return null;
    }

    protected static async Task<int> ServerCount()
    {

        WWWForm form = new WWWForm();
        form.AddField("action", "getall");

        string results = await DoRemoteQuery(serverInfo, form);
        if (results != "error")
        {
            if (results == "" || results == null || results == "[]") { return 0; }
            var array = new JSONObject(results);
            return array.count;
        }
        return 0;
    }

   

    public static async Task<bool> UpdatePlayerCount(int servKey, bool isAdding)
    {

        int addInt = 0;
        if (isAdding) { addInt = 1; }

        WWWForm form = new WWWForm();
        form.AddField("action", "update");
        form.AddField("serverId", servKey);
        form.AddField("isAdding", addInt);


        string results = await DoRemoteQuery(serverInfo, form);
        return results != "error";

    }

    public static async Task<bool> DeleteServer(int servKey)
    {


        WWWForm form = new WWWForm();
        form.AddField("action", "delete");
        form.AddField("serverKey", servKey);

        string results = await DoRemoteQuery(serverInfo, form);
        return results != "error";

    }
    #endregion


    #region Chat Management
    public static async Task<bool> AddChatMessage(ChatDTO dto)
    {

        WWWForm form = new WWWForm();
        form.AddField("action", "addChat");
        form.AddField("chatId", dto.id);
        form.AddField("sender", dto.sender);
        form.AddField("originalContent", dto.originalContent);
        form.AddField("updatedContent", dto.updatedContent);
        form.AddField("type", dto.type);
        form.AddField("whenSend", dto.whenSend);
        form.AddField("deleted", dto.deleted);
        form.AddField("edited", dto.edited);
        //form.AddField("name", deck.deckName);

        string result = await DoRemoteQuery(pvpLobby, form);

        if (result == "error")
        {
            return false;
        }

        return true;
    }
    #endregion
}


#region Remote DTOs
[System.Serializable]
public class RemotePlayerDTO
{
    public string userId { get; set; }
    public string deckList { get; set; }
}

[System.Serializable]
public class RemoteLobbyDTO
{
    public string lobbyKey { get; set; }
    public string joinIp { get; set; }
    public string player1 { get; set; }
    public string player2 { get; set; }
}

[System.Serializable]
public class ServerDTO
{
    public int serverKey { get; set; }
    public string ip { get; set; }
    public int port { get; set; }
    public string name { get; set; }
    public int playersCurrent { get; set; }
    public int playersMax { get; set; }
    public int connType { get; set; }
}

public class ConnectedPlayerDTO
{
    public int serverId { get; set; }
    public string userId { get; set; }
    public string username { get; set; }
    public DateTime whenConnect { get; set; }
    public int sleeves { get; set; }
    public int playmatt { get; set; }
    public string lobby { get; set; }
}

#endregion