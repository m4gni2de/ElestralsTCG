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

public class GameManager : MonoBehaviour, iFreeze
{
    #region Instance 
    
    public static Game ActiveGame { get; private set; }
    public static GameManager Instance { get; private set; }
   
    public Player ActivePlayer { get { return turnManager.ActiveTurn.ActivePlayer; } }
    public Turn ActiveTurn { get { return turnManager.ActiveTurn; } }
    
    public void StartNewGame()
    {
        if (ActiveGame != null) { App.LogError("There is already an active game."); }


        ActiveGame = Game.RandomGame();
        turnManager.LoadGame(ActiveGame);
        gameLog = GameLog.Create(ActiveGame.gameId, false);
        gameLog.AddLog($"Game '{ActiveGame.gameId}' has been started.");
        ActiveGame.AddPlayer(App.Account.Id, "1");
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
    //private TargetArgs _ActiveTargetArgs = null;
    //public TargetArgs ActiveTargetParams
    //{
    //    get
    //    {
    //        return _ActiveTargetArgs;
    //    }
    //    set
    //    {
    //        _ActiveTargetArgs = value;
    //        Game.SetTargetParams(value);
    //    }
    //}
    //public void SetTargetModeArgs(TargetArgs args)
    //{
    //    if (args != null) { ActiveTargetParams = args; } else { ActiveTargetParams = null; }
       
    //}
    //public List<GameCard> TargetModeScope(TargetArgs args)
    //{
    //    List<GameCard> cards = new List<GameCard>();

    //    for (int i = 0; i < _players.Count; i++)
    //    {
    //        if (args.PlayerScope.Contains(_players[i]))
    //        {
    //            List<GameCard> playerScope = _players[i].GetTargets(args);
    //            cards.AddRange(playerScope);
    //        }
           
    //    }
    //    return cards;
    //}
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

        if (ActiveGame != null)
        {
            RegisterFields();
        }

    }

    private void RegisterFields()
    {
        string nearId = arena.NearField.Register();
        gameLog.AddLog($"Near Field registered with ID of {nearId}");
        string farId = arena.FarField.Register();
        gameLog.AddLog($"Far Field registered with ID of {farId}");
    }

    private void Start()
    {
        if (ActiveGame != null)
        {
            SetPlayerFields();

        }
        else
        {
            StartNewGame();
            RegisterFields();
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

    public void ReadyPlayer(Player p)
    {
        _players.Add(p);

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

    protected void AddAction(CardAction ac)
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
    public static void DeclareCardAction(CardAction ac)
    {
        Instance.OnActionDeclared?.Invoke(ac);
    }
   
    //public void PlayerDraw(Player p, GameCard c, CardSlot from, CardSlot to, DrawAction.DrawActionType drawType)
    //{
    //    DrawAction draw = new DrawAction(p, c, from, to, drawType);
    //    AddAction(draw);
    //}
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
    
    public void SelectCard(GameCard card)
    {
       
        Debug.Log(card.card.cardData.cardName);
    }
    #endregion

    #region Card Dragging
    public void ClickCard(GameCard card)
    {
        GameManager.Instance.SelectCard(card);
    }

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

