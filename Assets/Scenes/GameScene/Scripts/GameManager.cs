using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameplay;
using UnityEngine.SceneManagement;
using Gameplay.Menus;
using Gameplay.GameCommands;

public class GameManager : MonoBehaviour
{
    #region Instance 
    public static Game ActiveGame { get; private set; }
    public static GameManager Instance { get; private set; }

    public static Player ActivePlayer { get { return ActiveGame.ActivePlayer; } }
    public void SetActivePlayer(Player p)
    {
        ActiveGame.ActivePlayer = p;
    }

    public static void StartNewGame()
    {
        if (ActiveGame != null) { App.LogError("There is already an active game."); }
        ActiveGame = Game.NewGame();
        ActiveGame.AddPlayer(App.Account.Name, "1");
        //if (Camera.main.scene.name != "GameScene")
        //{
        //    App.ChangeScene("GameScene");
        //}
        
        
    }
    #endregion

    #region Properties
    
    public int ExpectedPlayers = 1;
    [SerializeField]
    private Arena _arena;
    public Arena arena { get { return _arena; } }
    public List<Player> _players = new List<Player>();

    public Canvas UICanvas;
    public Blocker m_Blocker;
    public HandMenu handMenu;
    public CardBrowseMenu browseMenu;

    #endregion

    #region Global Properties
    public bool IsFrozen = false;
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

    public void Freeze(bool freeze = true)
    {
        CameraMotion.main.Freeze(freeze);
    }
    public IEnumerator ActionEnd
    {
        get
        {
            yield return new WaitForEndOfFrame();
        }
    }

    protected List<IEnumerator> _MovingCards = null;
    public List<IEnumerator> MovingCards { get { _MovingCards ??= new List<IEnumerator>(); return _MovingCards; } }

    public void SelectCard(GameCard card)
    {
        Debug.Log(card.card.cardData.cardName);
    }
    #endregion

    #region Card Moving
    protected IEnumerator DoActions()
    {
        Freeze();
        do
        {

            yield return StartCoroutine(MovingCards[0]);
            MovingCards.RemoveAt(0);
            yield return new WaitForEndOfFrame();

        } while (true && MovingCards.Count > 0);
        Freeze(false);
    }



    public void MoveCard(CardSlot from, GameCard card, CardSlot to)
    {

        IEnumerator move = DoMove(from, card, to);
        MovingCards.Add(move);
        if (MovingCards.Count == 1)
        {
            StartCoroutine(DoActions());
        }

    }
    protected IEnumerator DoMove(CardSlot fromSlot, GameCard card, CardSlot to)
    {
        float time = .65f;

        float acumTime = 0f;

        Vector3 from = Camera.main.ScreenToWorldPoint(card.cardObject.transform.position);
        Vector3 moveTo = Camera.main.ScreenToWorldPoint(to.Position);
        Vector3 direction = (moveTo - from);

        do
        {
            yield return new WaitForEndOfFrame();
            card.cardObject.transform.position += direction * Time.deltaTime;
            acumTime += Time.deltaTime;
        } while (true && acumTime < time);

        fromSlot.RemoveCard(card);
        to.AllocateTo(card);
        yield return ActionEnd;
    }


    #endregion

    #region Card Dragging
    public void ClickCard(GameCard card)
    {
        GameManager.Instance.SelectCard(card);
    }

    public void DragCard(GameCard card, CardSlot from)
    {

        MoveCommand comm = MoveCommand.StartMove(card);
        ActiveGame.AddCommand(comm);

        StartCoroutine(DoDragCard(card, from, comm, newSlot =>
        {
            if (newSlot == from)
            {
                card.ReAddToSlot();


            }
            else
            {
                newSlot.AllocateTo(card);
                comm.Complete(CommandStatus.Success);
            }
        }));
    }

    protected IEnumerator DoDragCard(GameCard card, CardSlot from, MoveCommand comm, System.Action<CardSlot> callBack)
    {
        Field f = arena.GetPlayerField(ActiveGame.You);

       // card.cardObject.transform.SetParent(f.transform);
        Vector2 newScale = new Vector3(8f, 8f, 1f);
        //card.cardObject.SetScale(newScale);

        card.cardObject.SetAsChild(f.transform, newScale);
        do
        {
            Freeze(true);
            yield return new WaitForEndOfFrame();
            var newPos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            card.cardObject.transform.position = new Vector3(newPos.x, newPos.y, -2f);
            f.ValidateSlots(card);

        } while (true && Input.GetMouseButton(0));

        CardSlot slot = f.SelectedSlot;

        if (slot == null)
        {
            callBack(from);
            comm.Complete(CommandStatus.Cancel);
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
                comm.Complete(CommandStatus.Fail);
            }
            
        }
        f.SetSlot();
        Freeze(false);
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

        if (ActiveGame != null)
        {
            RegisterFields();
        }
        
    }

    private void RegisterFields()
    {
        string nearId = arena.NearField.Register();
        ActiveGame.gameLog.Add($"Near Field registered with ID of {nearId}");
        string farId = arena.FarField.Register();
        ActiveGame.gameLog.Add($"Far Field registered with ID of {farId}");
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
            Begin();
        }


    }
    void Begin()
    {
        for (int i = 0; i < _players.Count; i++)
        {
            ActiveGame.PlayerDraw(_players[i], 5);
        }

        SetActivePlayer(_players[0]);

    }

    #endregion

    #region Update
    private void Update()
    {
        
    }
    #endregion


    




}

