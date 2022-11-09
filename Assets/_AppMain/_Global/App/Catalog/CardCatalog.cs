using System.Collections.Generic;
using System.Threading.Tasks;
using Cards;
using CardsUI.Filtering;
using Databases;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardCatalog : MonoBehaviour, iScaleCard
{
    #region Interface
    private Vector2 m_Scale = Vector2.zero;
    public Vector2 CardScale { get { if (m_Scale == Vector2.zero) { m_Scale = new Vector2(10f, 10f); } return m_Scale; } }

    private string m_sortLayer = "Card";
    public string SortLayer => m_sortLayer;
    #endregion
    
    #region Instance
    private static CardCatalog _Instance = null;
    private static CardCatalog Instance
    {
        get
        {
            if (_Instance == null)
            {
                App.LogFatal("Card Catalog not loaded. Please call CardCatalog.Open to create it.");
            }
            return _Instance;
        }
    }
    private static readonly string AssetName = "CardCatalog";
    #endregion

    #region Properties

    #region UI
    public CardView cardDisplay;
    public ScrollRect CardScroll;

    private Transform Content { get { return CardScroll.content; } }
    private GridLayoutGroup Grid { get { return Content.GetComponent<GridLayoutGroup>(); } }
    private Canvas _canvas = null;
    private Canvas canvas { get { _canvas ??= GetComponent<Canvas>(); return _canvas; } }

    public CanvasGroup filtersCanvasGroup;

    protected GridSettings settings { get; set; }

   
    #endregion

    #region Cards and Pages
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

    private List<CardView> _CardsList = null;
    public List<CardView> CardsList { get { _CardsList ??= new List<CardView>(); return _CardsList; } }
    public CardView _templateCard;

    private int _PageNumber;
    public int PageNumber
    {
        get { return _PageNumber; }
        set
        {
            _PageNumber = value;
            cPageText.text = $"Page {_PageNumber} of {TotalPages}";
            
        }
    }

    private int _TotalPages;
    public int TotalPages
    {
        get { return _TotalPages; }
        set
        {
            _TotalPages = value;
            cPageText.text = $"Page {_PageNumber} of {_TotalPages}";

        }
    }

    protected int GetTotalPages(int cardCount)
    {
        if (cardCount <= settings.cardsPerPage) { return 1; }
        if (cardCount % settings.cardsPerPage > 0)
        {
            return (cardCount / settings.cardsPerPage) + 1;
        }
        return cardCount / settings.cardsPerPage;
    }
    private List<Card> _results = null;
    private List<Card> results { get { _results ??= new List<Card>(); return _results; } }

    protected List<CardView> _visibleCards
    {
        get
        {
            List<CardView> list = new List<CardView>();
            for (int i = 0; i < CardsList.Count; i++)
            {
                if (CardsList[i].gameObject.activeSelf)
                {
                    list.Add(CardsList[i]);
                }
            }
            return list;
        }
    }
    protected int GetVisibleIndex(Card view)
    {
        for (int i = 0; i < _visibleCards.Count; i++)
        {
            if (_visibleCards[i].ActiveCard == view)
            {
                return i;
            }
        }
        return -1;
    }
    protected CardView GetCardAtVisibleIndex(int index)
    {
        return _visibleCards[index];
    }
    #endregion

    #region Filters
    public FiltersMenu FilterMenu;
    public Button openFiltersBtn, closeFiltersBtn;
    #endregion

    public TMP_Text cPageText;
    public TMP_Text totalText;
    #endregion

    #region Filters
    protected List<Card> Results(string queryWhere)
    {
        List<Card> list = new List<Card>();
        results.Clear();

        List<qUniqueCard> dtos = CardService.ListByQuery<qUniqueCard>(CardService.qUniqueCardView, queryWhere);

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

    protected void ToggleFilterMenu(bool open)
    {
        if (open)
        {
            FilterMenu.Toggle(true);
            closeFiltersBtn.gameObject.SetActive(true);
            openFiltersBtn.gameObject.SetActive(false);
            CardScroll.gameObject.SetActive(false);

        }
        else
        {
            FilterMenu.Toggle(false);
            closeFiltersBtn.gameObject.SetActive(false);
            openFiltersBtn.gameObject.SetActive(true);
            CardScroll.gameObject.SetActive(true);
        }

    }

   
    #endregion

    #region Initialization
    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
        }
        settings = GridSettings.CatalogDefault();
        cardDisplay.gameObject.SetActive(false);
        name = "CardCatalog";


       
        //ScrollSize = new Vector2(GetComponent<RectTransform>().rect.width * .9f, GetComponent<RectTransform>().rect.height * .75f);
        //Vector2 newScale = (Grid.cellSize.x / defaultWidth) * _templateCard.sp.GetComponent<Transform>().localScale;
        //_templateCard.sp.GetComponent<Transform>().localScale = newScale;

        FilterMenu.SetFilter<qBaseCard>();
    }

    private void SetLayout(float freeWidth)
    {
        float cellWidth = freeWidth / (float)settings.cardsPerRow;
        float cellHeight = (cellWidth / settings.cardRatio);

        Grid.constraintCount = settings.cardsPerRow;

        Grid.cellSize = new Vector2(cellWidth, cellHeight);
        Grid.spacing = settings.GridSpacing;
        Grid.padding.left = settings.sidePadding;
        Grid.padding.right = settings.sidePadding;


    }


    public void Open()
    {
        if (_Instance != null)
        {
            SetupCatalog();
        }
        
    }

   
    private void SetupCatalog()
    {
        float freeWidth = CardScroll.GetComponent<RectTransform>().FreeWidth(CardScroll.verticalScrollbar.handleRect, settings.TotalPadding());

        settings.UpdateGrid(Grid, freeWidth);
       
        //SetLayout(freeWidth);

        closeFiltersBtn.gameObject.SetActive(false);
        openFiltersBtn.gameObject.SetActive(true);
        CardsList.Add(_templateCard);
        _templateCard.MatchSize(Grid.cellSize);

        for (int i = 1; i < settings.cardsPerPage; i++)
        {
            CardView g = Instantiate(_templateCard, Content.transform);
            
            CardsList.Add(g);
            
        }

        InitializeCards();
    }

  
    private void InitializeCards()
    {
        Refresh();
        _results = Results(QueryWhere);
        PageNumber = 1;
        TotalPages = GetTotalPages(results.Count);


        int loadCount = settings.cardsPerPage;
        if (TotalPages == 1) { loadCount = results.Count; }

        
        for (int i = 0; i < loadCount; i++)
        {
            int index = i;
            Card c = results[index];
            CardsList[i].LoadCard(c);
            //CardsList[i].SetSortingLayer(Card.CardLayer1);
            totalText.text = $"1 - {i + 1} of {_results.Count}";
        }
    }

    #endregion

    #region Populating/Navigating Catalog

    private void Refresh()
    {
        for (int i = 0; i < CardsList.Count; i++)
        {

            CardsList[i].Hide();

        }

        CardScroll.verticalScrollbar.value = 1f;
    }

    private void LoadCatalog()
    {

        Refresh();
        _results = Results(QueryWhere);
        PageNumber = 1;
        TotalPages = GetTotalPages(results.Count);


        int loadCount = settings.cardsPerPage;
        if (TotalPages == 1) { loadCount = results.Count; }

        LoadCards(0, loadCount);
    }

   
    protected void LoadCards(int startIndex, int loadCount)
    {
        Refresh();
        for (int i = 0; i < loadCount; i++)
        {
            int index = startIndex + i;
            CardsList[i].LoadCard(results[index]);
            totalText.text = $"{startIndex} - {index + 1} of {_results.Count}";
        }
    }

    #region Page Movement
    protected void PageUp(int steps)
    {
        int newPage = PageNumber + steps;
        if (newPage <= TotalPages) { JumpToPage(newPage); } else { JumpToPage(1); }
    }
    protected void PageDown(int steps)
    {
        int newPage = PageNumber - steps;
        if (newPage > 0) { JumpToPage(newPage); } else { JumpToPage(TotalPages); }
    }

    protected void JumpToPage(int newPage)
    {
        PageNumber = newPage;
        int loadCount = settings.cardsPerPage;
        int startIndex = (PageNumber - 1) * settings.cardsPerPage;
        if (results.Count - startIndex < settings.cardsPerPage) { loadCount = results.Count - startIndex; }
        
        LoadCards(startIndex, loadCount);

    }

    public void NextPageButton()
    {
        PageUp(1);
    }
    public void PreviousPageButton()
    {
        PageDown(1);
    }
    public void NextCardButton()
    {
        Card c = cardDisplay.ActiveCard;
        int visIndex = GetVisibleIndex(c);

        int nextIndex = visIndex + 1;
        if (nextIndex > _visibleCards.Count - 1)
        {
            nextIndex = 0;
        }

        CardView v = GetCardAtVisibleIndex(nextIndex);
        DisplayCard(v);
        
    }
    public void PrevCardButton()
    {
        Card c = cardDisplay.ActiveCard;
        int visIndex = GetVisibleIndex(c);

        int nextIndex = visIndex - 1;
        if (nextIndex < 0)
        {
            nextIndex = _visibleCards.Count - 1;
        }

        CardView v = GetCardAtVisibleIndex(nextIndex);
        DisplayCard(v);
    }
    #endregion

    public void DisplayCard(CardView card)
    {
        if (card.ActiveCard != null)
        {
            cardDisplay.LoadCard(card.ActiveCard);
            CardScroll.gameObject.SetActive(false);
            //DisplayManager.SetAction(() => HideDisplay());
            DisplayManager.AddAction(HideDisplay);
        }

    }
    public void HideDisplay()
    {
        if (cardDisplay.gameObject.activeSelf == true)
        {
            //DisplayManager.RemoveAction(() => HideDisplay());
            DisplayManager.RemoveAction(HideDisplay);
            cardDisplay.gameObject.SetActive(false);
            CardScroll.gameObject.SetActive(true);
        }
        
    }
    #endregion

    #region Filters Menu
    public void OpenFilters()
    {
        ToggleFilterMenu(true);
        DisplayManager.AddAction(CloseFilters);
    }
    public void CloseFilters()
    {
        if (FilterMenu.Validate())
        {
            DisplayManager.RemoveAction(CloseFilters);

            string qWhere = FilterMenu.GenerateQuery();
            string qSort = 
            _queryWhere = FilterMenu.GenerateQuery();
            ToggleFilterMenu(false);
            LoadCatalog();
        }
       

    }
    
    
    #endregion
}
