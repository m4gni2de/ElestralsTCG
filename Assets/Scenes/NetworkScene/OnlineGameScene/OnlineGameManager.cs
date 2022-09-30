using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Decks;
using Defective.JSON;
using Gameplay;
using RiptideNetworking;



public class OnlineGameManager : GameManager
{
    public static readonly string OnlineGameScene = "OnlineGame";
    private static OnlineGameManager OnlineInstance
    {
        get
        {
            return (OnlineGameManager)Instance;
        }
    }

    protected List<UploadedDeckDTO> RemoteDeckChoices  = new List<UploadedDeckDTO>();

    #region Message Pairs
    public static void CreateGame(string gameId)
    {
        ActiveGame = Game.ConnectToNetwork(gameId);
        OnGameLoaded += PrepareLocalPlayer;
        NetworkPipeline.OnPlayerJoined += NewPlayerJoined;
        App.ChangeScene(OnlineGameScene);
    }
    public static void JoinGame(string gameId, NetworkPlayer opponent)
    {
        ActiveGame = Game.ConnectToNetwork(gameId);
        Player opp = new Player(opponent.networkId, opponent.userId, false);
        ActiveGame.AddPlayer(opp);
        OnGameLoaded += PrepareLocalPlayer;
        App.ChangeScene(OnlineGameScene);
    }
    public static void NewPlayerJoined(ushort networkId, string userId)
    {
        NetworkPipeline.OnPlayerJoined -= NewPlayerJoined;
        Player opp = new Player(networkId, userId, false);
        ActiveGame.AddPlayer(opp);
    }
    #endregion


    protected override void LoadGame() { }
    public static void PrepareLocalPlayer()
    {
        OnGameLoaded -= PrepareLocalPlayer;
        OnlineInstance.LoadLocalPlayer();
    }
    protected void LoadLocalPlayer()
    {
           
        Player p = new Player(NetworkManager.Instance.Client.Id, App.Account.Id, true);
        Instance.turnManager.LoadGame(ActiveGame);
        ActiveGame.AddPlayer(p);
        ChooseDeck();
       
    }

    protected async void ChooseDeck()
    {
        //open a prompt to choose the deck here. once a deck is chosen, set the player to ready
        if (RemoteDeckChoices.Count == 0)
        {
            RemoteDeckChoices = new List<UploadedDeckDTO>();
            RemoteDeckChoices = await RemoteData.ViewDecks($"");
        }
        
            List<string> titles = new List<string>();
            for (int i = 0; i < RemoteDeckChoices.Count; i++)
            {
                titles.Add(RemoteDeckChoices[i].title);
            }
        
        App.ShowDropdown("Which deck do you want to use?", titles, AwaitDeckSelection);
    }

    private void AwaitDeckSelection(string selectedTitle)
    {
        if (string.IsNullOrEmpty(selectedTitle))
        {
            ChooseDeck();
           
        }
        else
        {
            int selected = -1;
            for (int i = 0; i < RemoteDeckChoices.Count; i++)
            {
                string title = RemoteDeckChoices[i].title;
                if (title.ToLower() == selectedTitle.ToLower())
                {
                    selected = i;
                    break;
                }
            }

            if (selected > -1)
            {
                UploadedDeckDTO chosenDeck = RemoteDeckChoices[selected];
                SendDeckSelection(chosenDeck);
            }
            else
            {
                ChooseDeck();
            }
        }
        
    }
   
    private void SendDeckSelection(UploadedDeckDTO deck)
    {
        //send deck to server here
        Decklist decklist = deck;
        Player.LocalPlayer.LoadDeckList(decklist, false);
        NetworkPipeline.OnDeckSelected += LoadPlayer;

        
        string[] ids = new string[Player.LocalPlayer.deck.Cards.Count];
        string[] realIds = new string[Player.LocalPlayer.deck.Cards.Count];

        Message message = Message.Create(MessageSendMode.reliable, (ushort)ToServer.DeckSelection);
        message.Add<string>(deck.deckKey);

        for (int i = 0; i < Player.LocalPlayer.deck.Cards.Count; i++)
        {
            ids[i] = Player.LocalPlayer.deck.Cards[i].NetworkId.ToString();
            realIds[i] = Player.LocalPlayer.deck.Cards[i].card.cardData.setKey;
        }

       
        message.AddStrings(ids, true, true);
        message.AddStrings(realIds, true, true);
        NetworkPipeline.SendMessageToServer(message);
    }

    private void LoadPlayer(ushort playerId, string deck)
    {
        if (playerId != Player.LocalPlayer.lobbyId) { return; }
        NetworkPipeline.OnDeckSelected -= LoadPlayer;
    }
    
    public override void ReadyPlayer(Player p)
    {
        _players.Add(p);

        CardSlot deck = p.gameField.DeckSlot;
        NetworkPipeline.SendDeckOrder(deck.index, p.deck.DeckOrder.ToList(), p.deck.NetworkDeckOrder.ToList());
        CardSlot spiritDeck = p.gameField.SpiritDeckSlot;
        NetworkPipeline.SendDeckOrder(spiritDeck.index, p.deck.SpiritOrder.ToList(), p.deck.NetworkSpiritOrder.ToList());
        //DO A NETWORK CALL HERE TO MAKE SURE EVERYONE IS READY

        if (_players.Count == ExpectedPlayers)
        {
            turnManager.StartGame();
        }


    }



    #region Messages
    public static void CreateGame()
    {
        
        //Message message = Message.Create(MessageSendMode.reliable, (ushort)C2S.CreateGame);
        //message.AddString(App.Account.Id);
        //message.AddString(App.ActiveDeck.GetCardList);
        //NetworkPipeline.SendMessageToServer(message);
    }
    //getting back the gameId that was sent, this is to confirm the lobby has been created. Once confirmed, create a local version of the game, send the decklist and wait for another player to connect on the server
   
    private static void GameCreated(Message message)
    {
        //string gameId = message.GetString();
        //string f1 = message.GetString();
        //string f2 = message.GetString();
        //CreateGame(gameId, f1, f2);
    }

    #endregion
}
