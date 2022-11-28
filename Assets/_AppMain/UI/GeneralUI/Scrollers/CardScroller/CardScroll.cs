using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cards;
using CardsUI.Filtering;
using Databases;
using Decks;
using Gameplay;
using Newtonsoft.Json.Bson;
using UnityEngine;
using UnityEngine.UI;

public class CardScroll : MonoBehaviour
{

    #region ScrollSettings
    public class ScrollSettings
    {
        public bool stackDuplicates = false;

    }
    #endregion

    #region Properties
    public CardView templateCard;
    public CardView displayCard;
    
    private List<CardView> cardCells = new List<CardView>();
    public List<CardView> Cards { get { return cardCells; }}
    private List<Card> cardResults = new List<Card>();


    [Header("Scroll Properties")]
    public Transform ScrollContent;
    public ScrollRect Scroll;
    [SerializeField]
    private GridLayoutGroup Grid;
    private GridSettings gridSettings;
    [SerializeField] private CanvasGroup canvasGroup;
    private ScrollSettings _settings = null;
    public ScrollSettings Settings
    {
        get
        {
            _settings ??= new ScrollSettings();
            return _settings;
        }
    }

    private bool _isLoaded = false;
    public bool IsLoaded { get { return _isLoaded; } }
    #region Filtering
    [SerializeField]
    private FiltersMenu filterMenu;
    private static readonly string DefaultQueryWhere = " setName is not null;";
    private string _queryWhere = "";
    protected string QueryWhere
    {
        get
        {
            if (string.IsNullOrEmpty(_queryWhere)) { _queryWhere = DefaultQueryWhere; }
            return _queryWhere;
        }
        set
        {
            _queryWhere = value;
        }
    }

    protected List<Card> Results(string queryWhere)
    {
        List<Card> list = new List<Card>();
        List<Card> uniqueList = new List<Card>();
       

        List<qUniqueCard> dtos = new List<qUniqueCard>();
        dtos = CardService.ListByQuery<qUniqueCard>(CardService.qUniqueCardView, queryWhere);

        for (int i = 0; i < dtos.Count; i++)
        {
            qUniqueCard dto = dtos[i];
            CardData data;
            if (dto.cardClass == (int)CardType.Elestral) { data = new ElestralData(dto); Elestral e = new Elestral((ElestralData)data); list.Add(e); }
            if (dto.cardClass == (int)CardType.Rune) { data = new RuneData(dto); Rune r = new Rune((RuneData)data); list.Add(r); }
            if (dto.cardClass == (int)CardType.Spirit) { data = new CardData(dto); Spirit s = new Spirit(data); list.Add(s); }


        }

        return list;

    }
    #endregion

    #endregion
    #region Functions
    public List<CardView> VisibleCards
    {
        get
        {
            List<CardView> list = new List<CardView>();
            for (int i = 0; i < cardCells.Count; i++)
            {
                if (cardCells[i].isActiveAndEnabled) { list.Add(cardCells[i]); }
            }
            return list;
        }
    }
    public CardView FindCard(string setKey)
    {
        for (int i = 0; i < cardCells.Count; i++)
        {
            CardView view = cardCells[i];
            if (view.IsCard(setKey)) { return view; } 
        }
        return null;
    }
    #endregion
    #region Pages
    private int _PageNumber;
    public int PageNumber
    {
        get { return _PageNumber; }
        set
        {
            _PageNumber = value;

        }
    }

    private int _TotalPages;
    public int TotalPages
    {
        get { return _TotalPages; }
        set
        {
            _TotalPages = value;

        }
    }

    protected int GetTotalPages(int cardCount)
    {
        int perPage = gridSettings.itemsPerPage;
        if (gridSettings.InfiniteMode) { perPage = cardCount; }
        if (cardCount <= perPage) { return 1; }
        if (cardCount % perPage > 0)
        {
            return (cardCount / perPage) + 1;
        }
        return cardCount / perPage;
    }

    #endregion

    #region Card Data Loading
    private void SetCard(CardView card, bool addToCells)
    {
        card.touch.ClearAll();
        card.touch.RemoveFromGroup();
        if (addToCells)
        {
            cardCells.Add(card);
        }
        

    }
    #endregion

    #region Touch Interaction
    public void ToggleCanvas(bool canUse)
    {
        if (!canUse)
        {
            canvasGroup.interactable = false;
            canvasGroup.alpha = .5f;
        }
        else
        {
            canvasGroup.interactable = true;
            canvasGroup.alpha = 1f;
        }
    }
    #endregion



    private void Awake()
    {
        filterMenu.Toggle(false);
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Refresh()
    {

        for (int i = 0; i < cardCells.Count; i++)
        {
            cardCells[i].Clear();
        }
        Toggle(true);

    }
    private void UpdateGrid()
    {
        gridSettings.SetGrid(Grid);
    }
    public void Redraw()
    {
        for (int i = 0; i < cardCells.Count; i++)
        {
            cardCells[i].Destroy();
        }
        cardCells.Clear();
    }
    void Start()
    {
        Toggle(false);
        
    }

    public void Toggle(bool turnOn)
    {
        filterMenu.Toggle(false);
        Scroll.gameObject.SetActive(turnOn);
    }


    public void Setup(GridSettings sett, bool autoCreate = true)
    {

        displayCard.Hide();
        cardResults.Clear();
        cardCells.Clear();
        templateCard.LoadCard();
        templateCard.Hide();



        gridSettings = sett;
        float freeWidth = Scroll.GetComponent<RectTransform>().FreeWidth(Scroll.verticalScrollbar.handleRect, gridSettings.TotalPadding());
        gridSettings.SetGridSize(Grid, freeWidth);
        gridSettings.OnSettingsUpdate -= UpdateGrid;
        gridSettings.OnSettingsUpdate += UpdateGrid;
        templateCard.MatchSize(Grid.cellSize);

        _isLoaded = true;

        
        if (autoCreate)
        {
            for (int i = 0; i < gridSettings.itemsPerPage; i++)
            {
                CardView g = Instantiate(templateCard, ScrollContent.transform);
                SetCard(g, true);
            }
        }
       

    }
   
    public void InitializeCards(List<Card> cards = null)
    {
        Refresh();      
        if (cards == null)
        {
            cardResults = Results(QueryWhere);
        }
        else
        {
            cardResults = cards;
        }

        CheckCells();
        //if (cardResults.Count > cardCells.Count)
        //{
        //    int diff = cardResults.Count - cardCells.Count;
        //    for (int i = 0; i < diff; i++)
        //    {
        //        CardView g = Instantiate(templateCard, ScrollContent.transform);
        //        SetCard(g);
        //    }
        //}
        
        PageNumber = 1;
        TotalPages = GetTotalPages(cardResults.Count);


        int loadCount = gridSettings.itemsPerPage;
        if (gridSettings.InfiniteMode || TotalPages == 1) { loadCount = cardResults.Count; }
        //if (TotalPages == 1) { loadCount = cardResults.Count; }

        LoadCards(0, loadCount);
    }

    protected void LoadCards(int startIndex, int loadCount)
    {
        Refresh();
        for (int i = 0; i < loadCount; i++)
        {
            int index = startIndex + i;
            CardView c = cardCells[i];
            c.LoadCard(cardResults[index]);
        }
    }


   
    public void LoadDeck(Dictionary<string, int> cardList)
    {
        Refresh();
        cardResults.Clear();
        Dictionary<Card, int> cardCounts = new Dictionary<Card, int>();
        foreach (var item in cardList)
        {
            if (item.Value > 0)
            {
                qUniqueCard dto = CardService.ByKey<qUniqueCard>(CardService.qUniqueCardView, "setKey", item.Key);
                Card c = dto;
                cardResults.Add(c);
                cardCounts.Add(c, item.Value);
            }
           
        }

        InitializeCards(cardCounts);
    }

    private void InitializeCards(Dictionary<Card, int> cards)
    {
        Refresh();
        int totalObjects = cards.Count;

        if (totalObjects > cardCells.Count)
        {
            int diff = totalObjects - cardCells.Count;
            for (int i = 0; i < diff; i++)
            {
                CardView g = Instantiate(templateCard, ScrollContent.transform);
                SetCard(g, true);
            }
        }

        int count = 0;
        foreach (var item in cards)
        {
            CardView view = cardCells[count];
            view.LoadCard(item.Key);
            SetCard(view, false);


            CardStack stack = (CardStack)view;
           
            if (item.Value > 0)
            {
                stack.ChangeQuantity(item.Value, false);
            }
            count += 1;
        }


        int additionalSpacing = Mathf.Abs((int)templateCard.RenderHeight - (int)Grid.cellSize.y);
        gridSettings.ChangeCellSpacing(new Vector2(Grid.spacing.x * 2f, additionalSpacing));
        PageNumber = 1;
        TotalPages = GetTotalPages(cardCells.Count);


        //int loadCount = gridSettings.cardsPerPage;
        //if (TotalPages == 1) { loadCount = cardCells.Count; }



        //LoadCards(0, loadCount);
    }



    #region Content Changing
    private void CheckCells()
    {
        if (cardResults.Count > cardCells.Count)
        {
            int diff = cardResults.Count - cardCells.Count;
            for (int i = 0; i < diff; i++)
            {
                CardView g = Instantiate(templateCard, ScrollContent.transform);
                SetCard(g, true);
            }
        }
    }
    /// <summary>
    /// If using a CardStack with visible Quantity number, set qty to something >= 0.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="qty"></param>
    public void AddCardByKey(string key, bool addHistory, int qty = -1)
    {
        qUniqueCard dto = CardService.ByKey<qUniqueCard>(CardService.qUniqueCardView, "setKey", key);
        Card c = dto;
        if (c != null)
        {
            cardResults.Add(c);
        }
        CheckCells();
        if (qty >= 0)
        {
            CardView view = cardCells[cardResults.Count - 1];
            view.LoadCard(c);
            SetCard(view, false);
            CardStack stack = (CardStack)view;
            stack.ChangeQuantity(qty, addHistory);
        }
    }
    #endregion

    #region Clean Up
    private void OnDestroy()
    {
        gridSettings.OnSettingsUpdate -= UpdateGrid;
    }
    #endregion

}
