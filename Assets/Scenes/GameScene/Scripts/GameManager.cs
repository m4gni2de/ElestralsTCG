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

public class GameManager : MonoBehaviour, iFreeze
{
    public static readonly string SceneName = "GameScene";

    //#region Networking
    //[SerializeField]
    //private ClientManager _clientManager;
    //public ClientManager ClientManager { get { return _clientManager; } }

    //[SerializeField]
    //private ServerManager _serverManager;
    //public ServerManager ServerManager { get { return _serverManager; } }
    
    //public static void HostGame()
    //{
    //    OnGameLoaded += StartOnlineGame;
    //    App.ChangeScene(SceneName);
    //}
    //private static void StartOnlineGame()
    //{
    //    OnGameLoaded -= StartOnlineGame;
    //    Instance.ServerManager.Create();
    //    Game.ServerGame = Game.NewGame(Instance.ServerManager.myAddressGlobal, Instance.ServerManager.Server.Port);
    //    Instance.ClientManager.ConnectAsHost(Instance.ServerManager.Server.Port);
    //}

    //public static void SendPlayer()
    //{
    //    Message message = Message.Create(MessageSendMode.reliable, (ushort)c2s.registerPlayer);
    //    message.AddString(App.Account.Id);
    //    int activeDeck = SettingsManager.Account.Settings.ActiveDeck;
    //    message.AddString(App.Account.DeckLists[activeDeck].AsJson);

    //    ClientManager.Instance.Client.Send(message);
    //    //ties out to OnlineGame.PlayerToServer();
    //}

    //#endregion


    #region Client Only Networking
    public static void LoadConnectedGame(string id)
    {
        ActiveGame = Game.ConnectTo(id);
        Instance.turnManager.LoadGame(ActiveGame);
    }
    public static void AddPlayer(Player player, string fieldId)
    {
        ActiveGame.AddPlayer(player);
        Instance.arena.SetPlayer(player, fieldId);
        
        
    }
    public static Player ByNetworkId(ushort id)
    {
        for (int i = 0; i < ActiveGame.players.Count; i++)
        {
            if (ActiveGame.players[i].lobbyId == id)
            {
                return ActiveGame.players[i];
            }
        }
        return null;
    }
    #endregion






















    #region Instance 
    public static Game ActiveGame { get; protected set; }
    public static GameManager Instance { get; protected set; }
   
    public Player ActivePlayer { get { return turnManager.ActiveTurn.ActivePlayer; } }
    public Turn ActiveTurn { get { return turnManager.ActiveTurn; } }
    
    public virtual void StartNewGame()
    {
        if (ActiveGame != null) { App.LogError("There is already an active game."); }


        ActiveGame = Game.RandomGame();
        turnManager.LoadGame(ActiveGame);
        gameLog = GameLog.Create(ActiveGame.gameId, false);
        gameLog.AddLog($"Game '{ActiveGame.gameId}' has been started.");
        ActiveGame.AddPlayer(App.Account.Id, "1", true);
        SetGameWatchers();
        
        
    }

    #endregion

    #region Properties
    public GameLog gameLog;
    public int ExpectedPlayers = 2;
    [SerializeField]
    private Arena _arena;
    public Arena arena { get { return _arena; } }
    public List<Player> _players = new List<Player>();

    public CardView cardTemplate;
    public Canvas UICanvas;
    public Blocker m_Blocker;
    public HandMenu handMenu;
    public CardBrowseMenu browseMenu;
    public PopupMenu popupMenu;
    public TurnMenu turnMenu;
    public CardSlotMenu cardSlotMenu;
    public MessageController messageControl;
    public LocationMenu locationMenu;
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
                //DoFreeze();
            }
            else
            {
                _isSelecting = false;
                //DoThaw();
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
    #endregion

    
    #region Setup
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

        //if (ActiveGame != null)
        //{
        //    RegisterFields();
        //}

    }

    

    public static event Action OnGameLoaded;
    private void Start()
    {
        //arena.NearField.Register();
        //arena.FarField.Register();
        OnGameLoaded?.Invoke();
        LoadGame();
        

    }

    protected virtual void LoadGame()
    {
        if (ActiveGame != null)
        {
            SetPlayerFields();

        }
        else
        {
            StartNewGame();
            SetPlayerFields();
        }
    }


    void SetPlayerFields()
    {
        for (int i = 0; i < ActiveGame.players.Count; i++)
        {
            Player p = ActiveGame.players[i];
            _arena.SetPlayer(p);
        }

        //Go();
    }

    public virtual void ReadyPlayer(Player p)
    {
        _players.Add(p);

        CardSlot deck = p.gameField.DeckSlot;
        NetworkPipeline.SendDeckOrder(deck.index, p.deck.DeckOrder.ToList(), p.deck.NetworkDeckOrder.ToList());
        CardSlot spiritDeck = p.gameField.SpiritDeckSlot;
        NetworkPipeline.SendDeckOrder(spiritDeck.index, p.deck.SpiritOrder.ToList(),p.deck.NetworkSpiritOrder.ToList());
        //DO A NETWORK CALL HERE TO MAKE SURE EVERYONE IS READY

        if (_players.Count == ExpectedPlayers)
        {
            turnManager.StartGame();
        }


    }

    #endregion

    #region Update
    private void Update()
    {

    }
    #endregion



    #region Card Actions

    public void AddAction(CardAction ac)
    {
        if (!ActiveGame.isOnline)
        {
            CardActions.Add(ac);
            if (CardActions.Count == 1)
            {
                StartCoroutine(DoActions());
            }
        }
        else
        {

            TryNetworkAction(ac);

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

    private void TryNetworkAction(CardAction ac)
    {

        CardAction activeAction = ac;
        DeclareNetworkAction(activeAction);
    }
    //once action is sent by client, it is confirmed once the server recieves it, and sends it back to both players
    public void DeclareNetworkAction(CardAction ac = null)
    {
        if (ac != null)
        {
            DoFreeze();
            CardActions.Add(ac);
            ActiveAction = ac;
            ac.SendAction(true);
            //allow the server to wait for this. if after X seconds, the server will automatically send a response. client response will be sent to server, and then sent back to both clients at once to keep sync.
            //this only changes from server response
        }
        else
        {
            //do some sort of visual indicator that the action is about to happen. hourglass icon stops turning, etc
            //as of now, actions will not complete because only 1 player is confiring them. 
        }
    }
    public void ConfirmActiveAction(ActionResult result)
    {
        ActiveAction.ConfirmAttempt(result);
        StartCoroutine(DoNetworkAction());
    }
    private IEnumerator DoNetworkAction()
    {
        yield return StartCoroutine(ActiveAction.Do());
        NetworkPipeline.SendActionEnd(ActiveAction.ActionData);
        gameLog.LogAction(ActiveAction);
        DoThaw();
    }
   

    public static void DeclareCardAction(CardAction ac)
    {
        CardAction declaredAction = ac;
        Instance.OnActionDeclared?.Invoke(declaredAction);
    }

    public void DecideOnRemoteAction(CardAction ac = null)
    {
        if (ac != null)
        {
            DoFreeze();
            ActiveAction = ac;
            ac.SendAction(false);
            //allow the server to wait for this. if after X seconds, the server will automatically send a response. client response will be sent to server, and then sent back to both clients at once to keep sync.
            //this only changes from server response
        }
    }
   

    public void AddRemoteAction(CardAction ac)
    {
        AddAction(ac);
    }
    public void PlayerDraw(DrawAction draw)
    {
        AddAction(draw);
    }

    #region Enchant Actions
    public void NormalEnchant(Player p, GameCard source, List<GameCard> spirits, CardSlot to, CardMode cMode)
    {
        EnchantAction enchant = EnchantAction.Normal(p, source, spirits, to, cMode);
        AddAction(enchant);
    }
    public void ReEnchant(Player p, GameCard source, List<GameCard> spirits)
    {
        EnchantAction enchant = EnchantAction.ReEnchant(p, source, spirits);
        AddAction(enchant);
    }
    public void SetEnchant(Player p, GameCard source, CardSlot to)
    {
        EnchantAction enchant = EnchantAction.Set(p, source, to);
        AddAction(enchant);
    }
    public void DisEnchant(Player p, GameCard source, List<GameCard> spirits, CardSlot to)
    {
        EnchantAction enchant = EnchantAction.DisEnchant(p, source, spirits, to);
        AddAction(enchant);
    }
    public void FaceDownRuneEnchant(Player p, GameCard source, List<GameCard> spirits)
    {
        EnchantAction enchant = EnchantAction.FromFaceDown(p, source, spirits);
        AddAction(enchant);
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

    public static iHold HeldObject
    {
        get { return AppManager.ActiveHoldObject; }
    }
    public static bool HasHoldObject { get { return HeldObject != null; } }

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
                newSlot.AllocateTo(card);
            }
        }));
    }

    protected IEnumerator DoDragCard(GameCard card, CardSlot from, System.Action<CardSlot> callBack)
    {
        Field f = arena.GetPlayerField(ActiveGame.You);

       // card.cardObject.transform.SetParent(f.transform);
        Vector2 newScale = new Vector3(8f, 8f, 1f);
        card.cardObject.SetScale(newScale);

        card.cardObject.SetAsChild(f.transform, newScale);
        do
        {
            DoFreeze();
            yield return new WaitForEndOfFrame();
            var newPos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            card.cardObject.transform.position = new Vector3(newPos.x, newPos.y, -2f);
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
    #endregion

    #region Game Freezing
    protected void DoFreeze()
    {
        this.Freeze();
    }

    protected void DoThaw()
    {
        this.Thaw();
    }
    #endregion

    #region Event Watching
    protected void SetGameWatchers()
    {
        //Game.OnNewTargetParams += TargetModeWatcher;
    }
    protected void RemoveGameWatchers()
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





}

