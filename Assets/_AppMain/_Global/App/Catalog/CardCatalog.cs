using System.Collections.Generic;
using System.Threading.Tasks;
using Cards;
using CardsUI.Filtering;
using Databases;
using UnityEngine;
using UnityEngine.UI;

public class CardCatalog : MonoBehaviour, iScaleCard
{
    #region Interface
    private Vector2 m_Scale = Vector2.zero;
    public Vector2 CardScale { get { if (m_Scale == Vector2.zero) { m_Scale = new Vector2(10f, 10f); } return m_Scale; } }

    private string m_sortLayer = "Card";
    public string SortLayer => m_sortLayer;
    #endregion
    public struct CatalogSettings
    {
        public int cardsPerRow { get; set; }
        public float cardSpace { get; set; }
        public int sidePadding { get; set; }       
        public float cardRatio { get; set; }
        public int cardsPerPage { get; set; }


        public static CatalogSettings Default()
        {
            CatalogSettings settings = new CatalogSettings();
            settings.cardsPerRow = 5;
            settings.cardSpace = 1f;
            settings.sidePadding = 8;
            settings.cardRatio = .75f;
            settings.cardsPerPage = 100;
            return settings;
        }

        public float TotalPadding()
        {
            return (cardSpace * (float)cardsPerRow) + ((float)sidePadding * 2f);
        }

        public Vector2 GridSpacing()
        {
            return new Vector2(cardSpace, cardSpace);
        }



    }
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
    public ScrollRect CardScroll;

    private Transform Content { get { return CardScroll.content; } }
    private GridLayoutGroup Grid { get { return Content.GetComponent<GridLayoutGroup>(); } }
    private Canvas _canvas = null;
    private Canvas canvas { get { _canvas ??= GetComponent<Canvas>(); return _canvas; } }

    public CanvasGroup mainCanvasGroup, filtersCanvasGroup;

    protected CatalogSettings settings { get; set; }

    private Vector2 _size = Vector2.zero;
    protected Vector2 ScrollSize
    {
        get
        {
            if (_size == Vector2.zero)
            {
                _size = CardScroll.GetComponent<RectTransform>().sizeDelta;
            }
            return _size;
        }
        set
        {
            CardScroll.GetComponent<RectTransform>().sizeDelta = value;

            float totalPadding = settings.TotalPadding();

            float freeWidth = CardScroll.GetComponent<RectTransform>().FreeWidth(CardScroll.verticalScrollbar.GetComponent<RectTransform>(), totalPadding);
            SetLayout(freeWidth);
            
        }
    }
    public FiltersMenu FilterMenu;
    #endregion

    #region Cards and Pages
    private string _queryWhere = "";
    protected string QueryWhere { get { return _queryWhere; } }

    private List<CardView> _CardsList = null;
    public List<CardView> CardsList { get { _CardsList ??= new List<CardView>(); return _CardsList; } }
    public CardView _templateCard;

    public int PageNumber { get; set; }
    public int TotalPages { get; set; }
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
    #endregion

    public GameObject Display;
    private CardObject _CardDisplay { get; set; }
    #endregion

    #region Filters
    protected List<Card> Results(string queryWhere)
    {
        List<Card> list = new List<Card>();
        results.Clear();

        Debug.Log(queryWhere);
        List<CardDTO> dtos = CardService.ListByQuery<CardDTO>(CardService.CardTable, queryWhere);

        for (int i = 0; i < dtos.Count; i++)
        {
            CardDTO dto = dtos[i];
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
            CardScroll.gameObject.SetActive(false);

        }
        else
        {
            FilterMenu.Toggle(false);
            CardScroll.gameObject.SetActive(true);
        }

        SetCanvasGroups();

    }

    private void SetCanvasGroups()
    {
        
        mainCanvasGroup.interactable = !FilterMenu.IsOpen;
        mainCanvasGroup.alpha = FilterMenu.IsOpen == true ? 0f : 1f;

        filtersCanvasGroup.interactable = FilterMenu.IsOpen;
        filtersCanvasGroup.alpha = FilterMenu.IsOpen == true ? 1f : 0f;
    }
    #endregion

    #region Initialization
    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
        }
        settings = CatalogSettings.Default();
        canvas.overrideSorting = true;
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;
        canvas.sortingLayerName = "Table";

        _CardDisplay = Display.GetComponentInChildren<CardObject>();
        _CardDisplay.SetScale(new Vector2(72f, 72f));
        _CardDisplay.touch.OnClickEvent.AddListener(() => HideDisplay());
        _CardDisplay.GetComponent<RectTransform>().sizeDelta = WorldCanvas.Instance.GetComponent<RectTransform>().sizeDelta;
        Display.SetActive(false);
        name = "CardCatalog";

        //ScrollSize = new Vector2(GetComponent<RectTransform>().rect.width * .9f, GetComponent<RectTransform>().rect.height * .75f);

       
        FilterMenu.SetFilter<CardDTO>();
    }

    private void SetLayout(float freeWidth)
    {
        float cellWidth = freeWidth / (float)settings.cardsPerRow;
        float cellHeight = (cellWidth / settings.cardRatio);

        Grid.constraintCount = settings.cardsPerRow;

        Grid.cellSize = new Vector2(cellWidth, cellHeight);
        Grid.spacing = settings.GridSpacing();
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
       

        CardsList.Add(_templateCard);
        for (int i = 1; i < settings.cardsPerPage; i++)
        {
            CardView g = Instantiate(_templateCard, Content.transform);
            CardsList.Add(g);
            
        }

        _queryWhere = $"";
        CreateCatalog();
    }

   
    
    private async void CreateCatalog()
    {
        
        AppManager.Instance.ShowLoadingBar("Loading Catalog", 0f, settings.cardsPerPage);
        CardView.OnCardLoaded += LoadingBar.Instance.MoveSlider;
        bool complete = await AppManager.AwaitLoading(LoadingBar.Instance, InitializeCards);
        CardView.OnCardLoaded -= LoadingBar.Instance.MoveSlider;

    }
    private async void InitializeCards()
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
            await CardsList[i].LoadCardAsync(results[index]);
            CardView.CardLoaded(1f);
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
        }
    }

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
    
    public void DisplayCard(CardView card)
    {
        if (card.ActiveCard != null)
        {
            Display.gameObject.SetActive(true);
            _CardDisplay.LoadCard(card.ActiveCard);
            CardScroll.gameObject.SetActive(false);
        }

    }
    public void HideDisplay()
    {
        if (Display.gameObject.activeSelf == true)
        {
            Display.gameObject.SetActive(false);
            CardScroll.gameObject.SetActive(true);
        }
        
    }
    #endregion

    #region Filters Menu
    public void OpenFilters()
    {
        ToggleFilterMenu(true);
    }
    public void CloseFilters()
    {
        _queryWhere = FilterMenu.GenerateQuery();
        ToggleFilterMenu(false);
        LoadCatalog();

    }
    public void ToggleFilters()
    {
        if (FilterMenu.IsOpen)
        {
            CloseFilters();
        }
        else
        {
            OpenFilters();
        }
    }
    #endregion
}
