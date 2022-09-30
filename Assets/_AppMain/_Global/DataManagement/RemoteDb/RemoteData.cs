using System.Collections;
using System.Collections.Generic;
using Decks;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using Defective.JSON;
using Gameplay.Decks;
using JetBrains.Annotations;

public class RemoteData
{
    private readonly static string reqUrl = "http://45.77.157.225/reqquery.php?";
    private readonly static string changeUrl = "http://45.77.157.225/deckmanage.php?";
    private readonly static string singleDeck = "http://45.77.157.225/singledeck.php?";
    public readonly static string deckSearch = "http://45.77.157.225/decksearch.php?";
    public readonly static string profilesUrl = "http://45.77.157.225/profiles.php?";
    public readonly static string pricesUrl = "http://45.77.157.225/price.php?";


    private readonly static string pvpDecks = "http://45.77.157.225/pvpDeck.php?";
    private readonly static string pvpLobby = "http://45.77.157.225/pvpLobby.php?";

   


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
    #endregion
}

[System.Serializable]
public class RemotePlayerDTO
{
    public string userId { get; set; }
    public string deckList { get; set; }
}
