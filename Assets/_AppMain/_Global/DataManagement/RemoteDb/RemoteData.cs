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

public class RemoteData
{
    
    private readonly static string singleDeck = "http://45.77.157.225/singledeck.php?";
    public readonly static string deckSearch = "http://45.77.157.225/decksearch.php?";
    public readonly static string profilesUrl = "http://45.77.157.225/profiles.php?";
    public readonly static string pricesUrl = "http://45.77.157.225/price.php?";


    private readonly static string pvpDecks = "http://45.77.157.225/pvpDeck.php?";
    private readonly static string pvpLobby = "http://45.77.157.225/pvpLobby.php?";
    private readonly static string serverInfo = "http://45.77.157.225/pvpServer.php?";




    public static async Task<string> DoRemoteQuery(string url, WWWForm form)
    {

        string result = await AppManager.DoPostRequestWithPayload(url, form);
        //if (result == "error")
        //{
        //    string title = "Connection Error";
        //    string msg = "There was a problem connecting to the server. Please check your connection and try again.";
        //    GameManager.Instance.ShowMessage(title, msg);
        //}
       
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

    public static async Task<UploadedDeckDTO> SearchDeck(string deckKey)
    {
        WWWForm form = new WWWForm();
        form.AddField("action", "search");
        form.AddField("uploadCode", deckKey);

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
        form.AddField("deckKey", deck.DeckKey);
        form.AddField("title", deck.Name);
        form.AddField("deck", deck.GetCardList);
        //form.AddField("created", deck.created.ToString());
        //form.AddField("name", deck.deckName);

        string result = await DoRemoteQuery(pvpDecks, form);

        if (result == "error")
        {
            return false;
        }

        return true;
    }

    public static async Task<bool> RemoveDeckFromRemoteDB(string key)
    {
        WWWForm form = new WWWForm();
        form.AddField("action", "delete");
        form.AddField("code", key);

        string result = await DoRemoteQuery(singleDeck, form);

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
        //form.AddField("whereClause", "whereClause");

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

   

    public static async void DownloadDeck(string key)
    {
        WWWForm form = new WWWForm();
        form.AddField("action", "download");
        form.AddField("code", key);

        await DoRemoteQuery(singleDeck, form);
    }


    #region Server Management
    public static async Task<List<ServerDTO>> ServerList()
    {

        List<ServerDTO> list = new List<ServerDTO>();
        WWWForm form = new WWWForm();
        form.AddField("action", "getall");
        form.AddField("port", 7777);

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
    #endregion

    #region Lobby Management
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
        form.AddField("ip", NetworkManager.Instance.myAddressLocal);
#else
         ipAddress = NetworkManager.Instance.myAddressGlobal;
        form.AddField("ip", NetworkManager.Instance.myAddressGlobal);
#endif

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


#endregion