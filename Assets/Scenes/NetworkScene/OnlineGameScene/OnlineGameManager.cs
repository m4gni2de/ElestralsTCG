using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Decks;
using Defective.JSON;
using Gameplay;
using Gameplay.Turns;
using RiptideNetworking;
using UnityEngine;

public class OnlineGameManager : GameManager
{
    #region Static Functions
    public static readonly string OnlineGameScene = "OnlineGame";
    private static OnlineGameManager OnlineInstance
    {
        get
        {
            return (OnlineGameManager)Instance;
        }
    }

    public static bool isInverted
    {
        get
        {
            if (OnlineInstance == null) { return false; }

            return Player.LocalPlayer.gameField == OnlineInstance.arena.FarField;
        }
    }
    #endregion

    protected List<UploadedDeckDTO> RemoteDeckChoices  = new List<UploadedDeckDTO>();

    #region General Overrides
    protected override void LoadGame() { }
   
    public override void Go()
    {
        NetworkPipeline.SendClientReady();
    }
    
    #endregion


    #region Message Pairs
    public static void CreateGame(string gameId)
    {
        ActiveGame = Game.ConnectToNetwork(gameId);
        OnGameLoaded += AsHost;
        NetworkPipeline.OnPlayerJoined += NewPlayerJoined;
        App.ChangeScene(OnlineGameScene);
    }
    public static void JoinGame(string gameId, NetworkPlayer opponent)
    {
        ActiveGame = Game.ConnectToNetwork(gameId);
        Player opp = new Player(opponent.networkId, opponent.userId, false);
        opp.SetBlankDeck(opponent.deckKey, opponent.deckName);
        ActiveGame.AddPlayer(opp);
        OnGameLoaded += AsJoinedPlayer;
        App.ChangeScene(OnlineGameScene);
    }
    public static void NewPlayerJoined(ushort networkId, string userId)
    {
        NetworkPipeline.OnPlayerJoined -= NewPlayerJoined;
        Player opp = new Player(networkId, userId, false);
        ActiveGame.AddPlayer(opp);
        OnlineInstance.arena.SetPlayer(opp);
    }
    #endregion

    #region Watchers
    protected override void SetGameWatchers()
    {
        
    }
    protected override void RemoveGameWatchers()
    {
       
    }

    
    
    #endregion


    
   
    private static void AsHost()
    {
        OnGameLoaded -= AsHost;
        Camera.main.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        OnlineInstance.LoadLocalPlayer();
    }
    private static void AsJoinedPlayer()
    {
        OnGameLoaded -= AsJoinedPlayer;
        Camera.main.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
        OnlineInstance.arena.SetPlayer(ActiveGame.players[0]);
        OnlineInstance.LoadLocalPlayer();

    }
    
   
    protected void LoadLocalPlayer()
    {
        Player p = new Player(NetworkManager.Instance.Client.Id, App.Account.Id, true);
        Instance.turnManager.LoadGame(ActiveGame);
        ActiveGame.AddPlayer(p);
        arena.SetPlayer(p);
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
        
        NetworkPipeline.SendDeckSelection(deck.deckKey, deck.title);
        //send deck to server here
        Decklist decklist = deck;
        //send each card from the deck as they get their network IDs, instead of sending all info as one long array. this way, server and client will have synced CardIDs

        Player p = Player.LocalPlayer;
        p.LoadDeckList(decklist, false);
        ShuffleDeck(p);
        SendDeck(p);
        p.gameField.AllocateCards();
    }
    private void ShuffleDeck(Player p)
    {
        p.deck.Shuffle();
    }
    private void SendDeck(Player p)
    {
        List<string> deckInOrder = new List<string>();
        for (int i = 0; i < p.deck.MainDeck.InOrder.Count; i++)
        {
            GameCard c = p.AtPosition(true, i);
            deckInOrder.Add(c.cardId);
        }
        NetworkPipeline.SendDeckOrder(deckInOrder);
    }

    
    public static void LoadPlayer(ushort playerId, string deckKey, string title)
    {

        Player player = ByNetworkId(playerId);
        if (!player.IsLocal)
        {
            player.SetBlankDeck(deckKey, title);
        }
        
    }
    public static void SyncPlayerCard(ushort playerId, int cardIndex, string realId, string uniqueId)
    {
        Player player = ByNetworkId(playerId);
        if (!player.IsLocal)
        {
            player.AddRemoteCardToDecklist(cardIndex, realId, uniqueId);
            int cardCount = player.deck.Cards.Count;
            if (cardCount == 60)
            {
                player.gameField.AllocateCards();
                //OnlineInstance.arena.SetPlayer(player);
            }
        }
    }


    #region Card Action Management
    public static void RemoteCardSlotChange(ushort player,string cardId, string newIndex)
    {
        Player p = ByNetworkId(player);
        if (p.IsLocal) { return; }
        GameCard card = Game.FindCard(cardId);
        CardSlot slot = Game.FindSlot(newIndex);
        slot.GetRemoteAllocateTo(card);


    }
    #endregion

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


    public override void DragCard(GameCard card, CardSlot from)
    {


        StartCoroutine(DoDragCard(card, from, newSlot =>
        {
            if (newSlot == from)
            {
                card.ReAddToSlot(false);


            }
            else
            {
                newSlot.AllocateTo(card);
            }
        }));
    }
    protected override IEnumerator DoDragCard(GameCard card, CardSlot from, System.Action<CardSlot> callBack)
    {
        Field f = arena.GetPlayerField(ActiveGame.You);

        // card.cardObject.transform.SetParent(f.transform);
        Vector2 newScale = new Vector3(8f, 8f, 1f);
        card.cardObject.SetScale(newScale);

        card.cardObject.SetAsChild(f.transform, newScale);

        card.NetworkCard.SendParent(from.slotId);
        card.NetworkCard.SendSorting();
        do
        {
            DoFreeze();
            yield return new WaitForEndOfFrame();
            var newPos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));

            card.cardObject.transform.position = new Vector3(newPos.x, newPos.y, -2f);
            card.NetworkCard.SendPosition();
            f.ValidateSlots(card);

        } while (true && Input.GetMouseButton(0));

        card.cardObject.SetColor(Color.white);
        CardSlot slot = f.SelectedSlot;

        if (slot == null)
        {
            callBack(from);
            f.SetSlot();
        }
        else
        {
            if (slot.ValidateCard(card))
            {

                callBack(slot);
            }
            else
            {

                callBack(from);
            }

        }
        f.SetSlot();
        DoThaw();
    }


    #region Action Management
    [MessageHandler((ushort)FromServer.ActionDeclared)]
    private static void ActionDeclared(Message message)
    {
        string dataString = message.GetString();
        CardActionData data = CardActionData.FromData(dataString);
        CardAction ac = CardActionData.ParseData(data);
        OnlineInstance.ShowRemoteAction(ac);
        //OnlineInstance.AddAction(ac);
       
    }

    

    [MessageHandler((ushort)FromServer.ActionRecieved)]
    private static void ActionRecieved(Message message)
    {
        string id = message.GetString();
        if (id == OnlineInstance.ActiveAction.id)
        {
            
            //OnlineInstance.DoRemoteAction();
            
        }

    }
    [MessageHandler((ushort)FromServer.ActionEnd)]
    private static void ActionEnd(Message message)
    {
        string id = message.GetString();
        int result = message.GetInt();
        if (id == OnlineInstance.ActiveAction.id)
        {
            //OnlineInstance.EndRemoteAction();
        }

    }

    [MessageHandler((ushort)FromServer.OpeningDraw)]
    private static void OpeningDraw(Message message)
    {
        OnlineInstance.OpeningDraw();

    }
    protected void OpeningDraw()
    {
        StartCoroutine(AwaitOpeningDraw());
        Player.LocalPlayer.StartingDraw();
    }
    private IEnumerator AwaitOpeningDraw()
    {
        do
        {
            yield return new WaitForEndOfFrame();

        } while (true && Player.LocalPlayer.gameField.HandSlot.cards.Count < 5);
        Instance.turnManager.OnlineStartTurn();


    }


    public void ShowRemoteAction(CardAction ac)
    {
        gameLog.LogAction(ac);
        StartCoroutine(ac.DisplayRemoteAction());
    }
   

    #endregion
}
