using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameplay;
using UnityEngine.SceneManagement;
using Gameplay.Menus;
using Gameplay.GameCommands;
using Gameplay.CardActions;
using System;
using Gameplay.Turns;
using Gameplay.Networking;
using System.CodeDom;
using nsSettings;
using RiptideNetworking;
using Decks;
using System.Linq;
using System.Drawing.Text;
using TMPro;
#if UNITY_EDITOR
using static UnityEditor.Experimental.GraphView.GraphView;
#endif

public class GameManager : MonoBehaviour, iFreeze, iSceneScript
{

    #region Interface
    public void StartScene()
    {
        DisplayManager.ClearButton();
        DisplayManager.ToggleVisible(false);
        DisplayManager.ToggleActive(true);
        DisplayManager.SetDefault(() => ZeroField());
    }
    #endregion

    public static readonly string SceneName = "OnlineGame";

    #region Instance 
    public static Game ActiveGame { get; protected set; }
    public static GameManager Instance { get; protected set; }

    public Player ActivePlayer { get { return turnManager.ActiveTurn.ActivePlayer; } }
    public Turn ActiveTurn { get { return turnManager.ActiveTurn; } }

    public static void StartLocalGame()
    {
        if (ActiveGame != null) { App.LogError("There is already an active game."); }

        IsOnline = false;
        OnGameLoaded += LoadLocalGame;
        App.ChangeScene(SceneName);
    }

    protected static void LoadLocalGame()
    {
        OnGameLoaded -= LoadLocalGame;
        ActiveGame = Game.RandomGame();
        Instance.turnManager.LoadGame(ActiveGame);
        Instance.gameLog = GameLog.Create(ActiveGame.gameId, false);
        Instance.gameLog.AddLog($"Game '{ActiveGame.gameId}' has been started.");
        //ActiveGame.AddPlayer(App.Account.Id, "1", true);
        ActiveGame.AddLocalPlayer(App.Account.Id, "1");
        Instance.SetGameWatchers();
        Instance.SetPlayerFields();

    }
   
    #endregion

    #region Properties
    private GameLog _gameLog = null;
    public GameLog gameLog
    {
        get
        {
            if (_gameLog == null)
            {
                _gameLog = GameLog.Create(ActiveGame.gameId, false);
                gameLog.AddLog($"Game '{ActiveGame.gameId}' has been started.");
            }
            return _gameLog;
        }
        set
        {
            _gameLog = value;
        }
    }
    public int ExpectedPlayers = 2;
    public static bool IsOnline = false;
    [SerializeField]
    private Arena _arena;
    public Arena arena { get { return _arena; } }
    public List<Player> _players = new List<Player>();

    #region Menus
    public CardView cardTemplate { get { return CardFactory.Instance.templateCard; } }
    public Canvas UICanvas;
    public Blocker m_Blocker;
    public HandMenu handMenu;
    public CardBrowseMenu browseMenu;
    public PopupMenu popupMenu;
    public TurnMenu turnMenu;
    public CardSlotMenu cardSlotMenu;
    public MessageController messageControl;
    public LocationMenu locationMenu;
    public TMP_Text txtGameId;
    public PauseMenu pauseMenu;
    public RotatingMenu optionsMenu;
    #endregion


    #endregion

    #region Turn Management
    public TurnManager turnManager { get { return TurnManager.Instance; } }
    #endregion

    #region Select Mode
    private bool _isSelecting = false;
    public bool IsSelecting { get { return _isSelecting; } }

    private SlotSelector _currentSelector = null;
    public SlotSelector currentSelector
    {
        get
        {
            return _currentSelector;
        }
        set
        {
            _currentSelector = value;
            if (value != null)
            {
                _isSelecting = true;
            }
            else
            {
                _isSelecting = false;
            }

        }
    }

    public void SetSelector(SlotSelector selector = null)
    {
        if (selector != null) { currentSelector = selector; } else { currentSelector = null; }
    }
    private CardSlot _SelectedSlot = null;
    public CardSlot SelectedSlot
    {
        get
        {
            return _SelectedSlot;
        }
    }

    public CardView DisplayedCard;
    public void DisplayCard(GameCard card = null, string sortingLayer = "")
    {
        if (card != null)
        {
            DisplayedCard.gameObject.SetActive(true);
            DisplayedCard.LoadCard(card.card);
            if (!string.IsNullOrEmpty(sortingLayer)) { DisplayedCard.SetSortingLayer(sortingLayer); }
            
        }
        else
        {
            DisplayedCard.LoadCard();
        }
        
    }
    public void HideDisplayCard()
    {
        DisplayedCard.gameObject.SetActive(false);
    }
    #endregion

   

    #region Setup
    private void Awake()
    {
        
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

    }



    public static event Action OnGameLoaded;
    private void Start()
    {
        OnGameLoaded?.Invoke();
        StartScene();
        

    }

    void SetPlayerFields()
    {
        for (int i = 0; i < ActiveGame.players.Count; i++)
        {
            Player p = ActiveGame.players[i];
            _arena.SetPlayerOffline(p);
        }
    }

    public void ReadyPlayer(Player p)
    {
        _players.Add(p);
        if (_players.Count == ExpectedPlayers)
        {
            Go();
        }
    }
    public void SetPlayer(Player p)
    {
        _players.Add(p);

    }
    public void Go()
    {
        if (!IsOnline)
        {
            turnManager.StartGame();
        }
        else
        {
            NetworkPipeline.SendClientReady();
        }

    }

   

    #endregion

    #region Update
    private void Update()
    {

    }
    #endregion



    #region Card Actions

    public virtual void AddAction(CardAction ac)
    {

        CardActions.Add(ac);
        if (CardActions.Count == 1)
        {
            StartCoroutine(DoActions());
        }
    }

    
    protected IEnumerator DoActions()
    {

        do
        {
            DoFreeze();
            ActiveAction = CardActions[0];
            yield return StartCoroutine(ActiveAction.DeclareAction());
            yield return StartCoroutine(ActiveAction.Do());
            CardActions.RemoveAt(0);
            gameLog.LogAction(ActiveAction);
            yield return new WaitForEndOfFrame();

        } while (true && CardActions.Count > 0);
        DoThaw();
    }


    //public void ConfirmActiveAction(ActionResult result)
    //{
    //    ActiveAction.ConfirmAttempt(result);
    //    StartCoroutine(DoNetworkAction());
    //}
    //private IEnumerator DoNetworkAction()
    //{
    //    yield return StartCoroutine(ActiveAction.Do());
    //    //NetworkPipeline.SendActionEnd(ActiveAction.ActionData);
    //    gameLog.LogAction(ActiveAction);
    //    DoThaw();
    //}



    public static void DeclareCardAction(CardAction ac)
    {
        CardAction declaredAction = ac;
        Instance.OnActionDeclared?.Invoke(declaredAction);
    }




    
    public void PlayerDraw(DrawAction draw)
    {
        AddAction(draw);
    }

    #region Enchant Actions
    public void Cast(Player p, GameCard source, List<GameCard> spirits, CardSlot to, CardMode cMode)
    {
        CastAction enchant = CastAction.Cast(p, source, spirits, to, cMode);
        AddAction(enchant);
    }
    public void Enchant(Player p, GameCard source, List<GameCard> spirits)
    {
        CastAction enchant = CastAction.Enchant(p, source, spirits);
        AddAction(enchant);
    }
    public void SetCast(Player p, GameCard source, CardSlot to)
    {
        CastAction enchant = CastAction.Set(p, source, to);
        AddAction(enchant);
    }
    public void DisEnchant(Player p, GameCard source, List<GameCard> spirits, CardSlot to)
    {
        CastAction enchant = CastAction.DisEnchant(p, source, spirits, to);
        AddAction(enchant);
    }
    public void FlipUpCast(Player p, GameCard source, List<GameCard> spirits)
    {
        CastAction enchant = CastAction.FromFaceDown(p, source, spirits);
        AddAction(enchant);
    }
    public void DoCast(CastAction ac)
    {
        AddAction(ac);
    }
    #endregion

    public void PlayerShuffle(Player p)
    {
        ShuffleAction ac = ShuffleAction.Shuffle(p, p.deck.MainDeck);
        AddAction(ac);
    }
    public void ChangeCardMode(Player p, GameCard source, CardMode newCardMode)
    {
        if (newCardMode == CardMode.Attack)
        {
            ModeAction ac = ModeAction.AttackMode(p, source);
            AddAction(ac);
            return;
        }
        if (newCardMode == CardMode.Defense)
        {
            ModeAction ac = ModeAction.DefenseMode(p, source);
            AddAction(ac);
            return;
        }
    }

    public void ElestralAttack(GameCard attacker, CardSlot defender)
    {
        AttackAction ac = AttackAction.ElestralAttack(attacker, defender);
        AddAction(ac);

    }
    public void Nexus(Player p, GameCard source, GameCard target, List<GameCard> spirits)
    {
        NexusAction ac = NexusAction.Create(p, source, target, spirits);
        AddAction(ac);

    }

    public void Ascend(AscendAction ac)
    {
        AddAction(ac);
    }
    public void MoveCard(Player p, GameCard source, CardSlot to)
    {
        MoveAction ac = new MoveAction(p, source, to);
        MoveCard(ac);
    }
    public void MoveCard(MoveAction ac)
    {
        AddAction(ac);
    }
    #endregion



    #region Rules/Validation
    public bool UseGameRules = false;
    public void ToggleGameRules(bool useRules = true)
    {
        UseGameRules = useRules;
    }
    #endregion

    #region Global Properties
    public bool IsFrozen { get { return AppManager.IsFrozen; } }
    public static EdgeMenu ActiveEdgeMenu = null;
    public static void OpenEdgeMenu(EdgeMenu menu = null)
    {
        ActiveEdgeMenu = menu;
        if (menu == null)
        {
            if (Instance != null)
            {
                Instance.m_Blocker.HideBlocker();
            }

        }
        else
        {
            Instance.m_Blocker.ShowBlocker();
        }

    }


    private static GameCard _SelectedCard = null;
    public static GameCard SelectedCard
    {
        get
        {
            return _SelectedCard;
        }
        set
        {
            if (_SelectedCard != null && value != _SelectedCard)
            {
                _SelectedCard.SelectCard(false);
            }
            if (value != null)
            {
                value.SelectCard(true);
            }
            _SelectedCard = value;

        }
    }
    #endregion

    #region Global Commands

    protected List<CardAction> _CardActions = null;
    public List<CardAction> CardActions { get { _CardActions ??= new List<CardAction>(); return _CardActions; } }
    public CardAction ActiveAction = null;
    public static void SetActiveAction(CardAction ac)
    {
        Instance.ActiveAction = ac;

    }

    public event Action<CardAction> OnActionDeclared;

    #endregion

    #region Card Dragging

   
    public void DragCard(GameCard card, CardSlot from)
    {
        StartCoroutine(DoDragCard(card, from, newSlot =>
        {
            if (newSlot == from)
            {
                card.ReAddToSlot(false);
            }
            else
            {
                //this is for forcing a Cast if moved to a slot in play

                //newSlot.EnchantToCommand(card, from);

                //this just adds the card to the slot without needing an enchantment
                //newSlot.AllocateTo(card, true);

                if (UseGameRules)
                {
                    if (newSlot is SingleSlot)
                    {
                        SingleSlot s = (SingleSlot)newSlot;
                        s.CastToSlotCommand(card, from);
                    }
                    else
                    {
                        newSlot.AllocateTo(card, true);
                    }
                }
                else
                {
                    newSlot.AllocateTo(card, true);
                }

                
                
            }
        }));
    }


    protected IEnumerator DoDragCard(GameCard card, CardSlot from, System.Action<CardSlot> callBack)
    {
        
        Field f = arena.GetPlayerField(ActiveGame.You);

        Vector2 newScale = new Vector3(8f, 8f, 1f);
        card.cardObject.SetScale(newScale);
        card.cardObject.SetAsChild(f.transform, newScale);

        float yOffset = card.cardObject.GetComponent<RectTransform>().sizeDelta.y / 4f;

       
        if (IsOnline)
        {
            card.NetworkCard.SendParent(from.slotId);
            card.NetworkCard.SendSorting();
        }
        do
        {
            DoFreeze();
            yield return new WaitForEndOfFrame();
            var newPos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y + yOffset));
            card.SetPosition(new Vector3(newPos.x, newPos.y, -2f), IsOnline);
            f.ValidateSlots(card);

        } while (true && Input.GetMouseButton(0));

        card.cardObject.ResetColors();
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
    #endregion

    #region Game Touch/Display Control
    protected virtual void DoFreeze()
    {
        this.Freeze();
    }

    protected virtual void DoThaw()
    {
        this.Thaw();
    }

    protected void ZeroField()
    {
        //do something here that resets the player's field from all selected cards and menus and stuff
    }
    #endregion

    #region Event Watching
    protected virtual void SetGameWatchers()
    {
        //Game.OnNewTargetParams += TargetModeWatcher;
    }
    protected virtual void RemoveGameWatchers()
    {
        //Game.OnNewTargetParams -= TargetModeWatcher;
    }
    #endregion

    #region Ending
    private void OnDestroy()
    {
        RemoveGameWatchers();
    }
    #endregion



    #region Networking

    protected List<UploadedDeckDTO> RemoteDeckChoices = new List<UploadedDeckDTO>();
    public static bool isInverted
    {
        get
        {
            if (Instance == null) { return false; }

            return Player.LocalPlayer.gameField == Instance.arena.FarField;
        }
    }

    public static Player ByNetworkId(ushort id)
    {
        for (int i = 0; i < ActiveGame.players.Count; i++)
        {
            Player p = ActiveGame.players[i];
            if (p.lobbyId == id)
            {

                return p;
            }
        }
        return null;
    }

   
    [MessageHandler((ushort)FromServer.ActionRecieved)]
    private static void ActionRecieved(Message message)
    {
        string id = message.GetString();
        if (id == Instance.ActiveAction.id)
        {

            //OnlineInstance.DoRemoteAction();

        }

    }
    [MessageHandler((ushort)FromServer.ActionEnd)]
    private static void ActionEnd(Message message)
    {
        string id = message.GetString();
        int result = message.GetInt();
        if (id == Instance.ActiveAction.id)
        {
            //OnlineInstance.EndRemoteAction();
        }

    }

    [MessageHandler((ushort)FromServer.OpeningDraw)]
    private static void OpeningDraw(Message message)
    {
        Instance.OpeningDraw();

    }
    #endregion


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

    #region Message Pairs
    //public void SyncPlayer(ushort playerId, UploadedDeckDTO dto)
    //{
    //    for (int i = 0; i < ActiveGame.players.Count; i++)
    //    {
    //        Player p = ActiveGame.players[i];
    //        if (p.lobbyId == playerId)
    //        {
    //            //p.VerifyDeck(dto);
    //            arena.SetPlayer(p);
    //            Go();
    //        }
    //    }
    //}

    public static void CreateOnlineGame(string gameId, ConnectionType connType)
    {
        IsOnline = true;
        ActiveGame = Game.ConnectToNetwork(gameId, connType);
        OnGameLoaded += AsHost;
        NetworkPipeline.OnPlayerJoined += NewPlayerJoined;
        App.ChangeScene(SceneName);
    }
    public static void JoinGame(string gameId, NetworkPlayer opponent)
    {
        IsOnline = true;
        ConnectionType ty = ClientManager.ConnectionType;
        ActiveGame = Game.ConnectToNetwork(gameId, ty);
        Player opp = new Player(opponent.networkId, opponent.userId, false);
        opp.SetBlankDeck(opponent.deckKey, opponent.deckName);
        ActiveGame.AddPlayer(opp);
        OnGameLoaded += AsJoinedPlayer;
        App.ChangeScene(SceneName);
    }
    public static void NewPlayerJoined(ushort networkId, string userId)
    {
        NetworkPipeline.OnPlayerJoined -= NewPlayerJoined;
        Player opp = new Player(networkId, userId, false);
        MessageController.Instance.ShowMessage($"{opp.username} has joined the Battle!", true);
        ActiveGame.AddPlayer(opp);
        Instance.arena.SetPlayer(opp);
    }

    private static void AsHost()
    {
        OnGameLoaded -= AsHost;
        Instance.txtGameId.text = ActiveGame.gameId;
        Camera.main.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        Instance.UICanvas.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        Instance.LoadLocalPlayer();
        

    }
    private static void AsJoinedPlayer()
    {
        OnGameLoaded -= AsJoinedPlayer;
        Camera.main.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
        Instance.UICanvas.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
        Instance.txtGameId.text = ActiveGame.gameId;
        Instance.arena.SetPlayer(ActiveGame.players[0]);
        Instance.LoadLocalPlayer();

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

        //do this is the server is REMOTE
        NetworkPipeline.SendDeckSelection(deck.deckKey, deck.title);
        //send deck to server here
        Decklist decklist = deck;
        //send each card from the deck as they get their network IDs, instead of sending all info as one long array. this way, server and client will have synced CardIDs

        Player p = Player.LocalPlayer;
        p.LoadDeckList(decklist, false);
        ShuffleDeck(p);
        SendDeck(p);
        p.gameField.AllocateCards();
        //

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
    #endregion

    #region Card Action Management
    #region Network Actions

    [MessageHandler((ushort)FromServer.ActionDeclared)]
    private static void ActionDeclared(Message message)
    {
        string dataString = message.GetString();
        CardActionData data = CardActionData.FromData(dataString);

        Instance.PerformRemoteAction(data);
    }

    public static void RemoteCardSlotChange(ushort player, string cardId, string newIndex, int cardMode)
    {

        GameCard card = Game.FindCard(cardId);
        card.SetCardMode((CardMode)cardMode, false);
        CardSlot slot = Game.FindSlot(newIndex);
        slot.GetRemoteAllocateTo(card);
    }
    public static void RemoteEmpowerChange(ushort player, string runeId, string elestralId, bool isAdding)
    {
        Player p = ByNetworkId(player);
        if (p.IsLocal) { return; }
        GameCard card = Game.FindCard(elestralId);
        GameCard rune = Game.FindCard(runeId);

        if (isAdding)
        {
            Game.Empower(rune, card);
        }
        else
        {
            Game.UnEmpower(rune);
        }

    }

  
    public void PerformRemoteAction(CardActionData data)
    {

        //CardAction ac = ParseRemoteAction(data);
        CardAction ac = CardActionData.ParseData(data);

        gameLog.LogAction(ac);
        StartCoroutine(ac.DisplayRemoteAction());
    }

    public void DisplayRemoteAction(CardActionData data)
    {
        CardAction ac = CardActionData.ParseData(data);
    }

    //protected CardAction ParseRemoteAction(CardActionData data)
    //{
    //    CardAction ac = CardActionData.ParseData(data);
    //    if (ac.category == ActionCategory.Cast)
    //}

    #endregion
   
   

   



    #region Game Client Ending/Leaving
    public void LeaveGame()
    {
        if (IsOnline)
        {
            
            ClientManager.Disconnect();
        }

        ActiveGame = null;
        DisplayManager.ToggleActive(false);
        App.ChangeScene(MainScene.SceneName);
    }

    public static void PlayerDisconnected(ushort playerId, string username)
    {
        if (playerId != Player.LocalPlayer.lobbyId)
        {
            Player p = ByNetworkId(playerId);
            float endTime = 5f;
            Instance.messageControl.ShowMessage($"{username} has disconnected from the game. Game will end in {endTime} seconds.", false);
            Instance.StartCoroutine(Instance.AwaitGameEnd(endTime));
        }
    }

    public IEnumerator AwaitGameEnd(float waitTime)
    {
        float acumTime = 0f;
        do
        {

            yield return new WaitForEndOfFrame();
            acumTime += Time.deltaTime;
        } while (true && acumTime <= waitTime);

        LeaveGame();

    }

    public static void AwaitReconnection(ushort playerId)
    {
        Player p = ByNetworkId(playerId);
        float endTime = 5f;
        Instance.messageControl.ShowMessage($"{p.username} has disconnected from the game. Game will end in {endTime} seconds if they do not reconnect.", false);
        Instance.StartCoroutine(Instance.AwaitReconnect(p, endTime));
        //do some coroutine where the server host waits for the player
    }
    protected IEnumerator AwaitReconnect(Player player, float waitTime)
    {

        float acumTime = 0f;
        do
        {

            yield return new WaitForEndOfFrame();
        } while (true && acumTime <= waitTime);

        if (acumTime >= waitTime)
        {
            LeaveGame();

        }
    }
    #endregion


    #endregion




}

