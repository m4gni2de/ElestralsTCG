using System.Collections.Generic;
using System.Threading.Tasks;
using Cards;
using CardsUI.Filtering;
using Databases;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using nsSettings;
using System.Reflection;
using System;

public enum CatalogMode
{
    ViewMode = 0,
    EditMode = 1,
}
public class CardCatalog : MonoBehaviour, iScaleCard
{

    #region Interface
    private Vector2 m_Scale = Vector2.zero;
    public Vector2 CardScale { get { if (m_Scale == Vector2.zero) { m_Scale = new Vector2(10f, 10f); } return m_Scale; } }

    private string m_sortLayer = "Card";
    public string SortLayer => m_sortLayer;


    #endregion

    #region Properties

    private CatalogSettings _settCatalog = null;
    public CatalogSettings settCatalog
    {
        get
        {
            _settCatalog ??= SettingsManager.Catalog.Settings;
            return _settCatalog;
        }
        set
        {
            _settCatalog = value;
        }
    }

    #region UI
    public CardView cardDisplay;
    public ScrollRect CardScroll;

    [SerializeField]
    private Scrollbar scrollBar;

    [SerializeField]
    private GameObject PagesObject;

    private Canvas _pagesCanvas = null;
    public Canvas pagesCanvas
    {
        get
        {
            _pagesCanvas ??= PagesObject.GetComponent<Canvas>();
            return _pagesCanvas;
        }
    }
    [SerializeField] private List<GameObject> BorderObjects = new List<GameObject>();

    private Transform Content { get { return CardScroll.content; } }
    private GridLayoutGroup Grid { get { return Content.GetComponent<GridLayoutGroup>(); } }
    private Canvas _canvas = null;
    private Canvas canvas { get { _canvas ??= GetComponent<Canvas>(); return _canvas; } }

    public CanvasGroup filtersCanvasGroup;
    [SerializeField]
    private Canvas nonCardCanvas;

    private GridSettings _gridSettings = GridSettings.Empty;
    protected GridSettings settings
    {
        get
        {
            if (_gridSettings.IsEmpty)
            {
                _gridSettings = GridSettings.CatalogDefault();
            }
            return _gridSettings;
        }
        set
        {
            _gridSettings = value;
        }
    }

    private bool _isLoaded = false;
    public bool IsLoaded { get { return _isLoaded; } }

    private bool _isOpen = false;
    public bool IsOpen { get { return _isOpen; } }

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
        if (cardCount <= settings.itemsPerPage) { return 1; }
        if (cardCount % settings.itemsPerPage > 0)
        {
            return (cardCount / settings.itemsPerPage) + 1;
        }
        return cardCount / settings.itemsPerPage;
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

    public int GetIndexOfCard(string cardKey)
    {

        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].cardData.cardKey.ToLower() == cardKey.ToLower()) { return i; }
        }
        return -1;
    }
    #endregion

    #region Filters
    public FiltersMenu FilterMenu;
    public Button openFiltersBtn, closeFiltersBtn;
    #endregion

    public TMP_Text cPageText;
    public TMP_Text totalText;

    protected List<Card> Results()
    {
        List<Card> list = new List<Card>();
        List<Card> uniqueList = new List<Card>();
        results.Clear();


        string queryWhere = FilterMenu.GenerateQuery();


        //if (settCatalog.groupAltArts)
        //{
        //    queryWhere += " GROUP BY baseKey";
        //}

        queryWhere += " ORDER BY title ASC;";
        Debug.Log(queryWhere);

        List<qUniqueCard> dtos = new List<qUniqueCard>();
        if (!settCatalog.displayDuplicates) { dtos = CardService.CardsByUniqueArt(queryWhere); } else { dtos = CardService.ListByQuery<qUniqueCard>(CardService.qUniqueCardView, queryWhere); }

        for (int i = 0; i < dtos.Count; i++)
        {
            Card card = dtos[i];
            list.Add(card);
            //if (card.cardData.effect.ToLower().Contains("you can"))
            //{
            //    Debug.Log(card.cardData.effect);
            //}
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

        cardDisplay.gameObject.SetActive(false);
        name = "CardCatalog";
        FilterMenu.SetFilter<qBaseCard>();
        _isLoaded = false;
    }

    public void Toggle(bool isOn)
    {
        _isOpen = isOn;
        gameObject.SetActive(isOn);

    }
    public void Open(GridSettings sett, string sortLayerName = "", int sortOrder = -1)
    {
        settings = sett;

        Toggle(true);
        if (!string.IsNullOrEmpty(sortLayerName) && sortOrder > -1)
        {
            canvas.overrideSorting = true;
            canvas.sortingLayerName = sortLayerName;
            canvas.sortingOrder = sortOrder;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;

            pagesCanvas.overrideSorting = true;

            int order = SortingLayer.GetLayerValueFromName(sortLayerName);
            string sortName = SortingLayer.layers[order + 6].name;
            pagesCanvas.sortingLayerName = sortName;
            pagesCanvas.sortingOrder = canvas.sortingOrder + 500;
            pagesCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            pagesCanvas.worldCamera = Camera.main;

            for (int i = 0; i < BorderObjects.Count; i++)
            {
                SpriteRenderer sp = BorderObjects[i].GetComponent<SpriteRenderer>();
                sp.sortingLayerName = sortName;
                sp.sortingOrder = canvas.sortingOrder + 499;
            }
        }
        SetupCatalog();
        _isLoaded = true;
    }


    private void SetupCatalog()
    {
        float freeWidth = CardScroll.GetComponent<RectTransform>().FreeWidth(scrollBar.handleRect, settings.TotalPadding());

        settings.SetGridSize(Grid, freeWidth);


        closeFiltersBtn.gameObject.SetActive(false);
        openFiltersBtn.gameObject.SetActive(true);




        CardsList.Add(_templateCard);


        for (int i = 1; i < settings.itemsPerPage; i++)
        {
            CardView g = Instantiate(_templateCard, Content.transform);

            CardsList.Add(g);

        }

        InitializeCards();
    }


    private void InitializeCards()
    {
        Refresh();
        _results = Results();
        PageNumber = 1;
        TotalPages = GetTotalPages(results.Count);


        int loadCount = settings.itemsPerPage;
        if (TotalPages == 1) { loadCount = results.Count; }


        for (int i = 0; i < loadCount; i++)
        {
            int index = i;
            Card c = results[index];
            CardsList[i].LoadCard(c);
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

        scrollBar.value = .99f;
    }

    private void LoadCatalog()
    {

        Refresh();
        _results = Results();
        PageNumber = 1;
        TotalPages = GetTotalPages(results.Count);


        int loadCount = settings.itemsPerPage;
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

        CheckCardsInView();
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
        int loadCount = settings.itemsPerPage;
        int startIndex = (PageNumber - 1) * settings.itemsPerPage;
        if (results.Count - startIndex < settings.itemsPerPage) { loadCount = results.Count - startIndex; }

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

    public void JumpToCard(string key)
    {
        int resIndex = -1;
        List<Card> dupes = new List<Card>();
        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].cardData.cardKey.ToLower() == key.ToLower())
            {
                resIndex = i;
                break;
            }

        }
        //check for dupes or alt arts if the actual card is not in the visible collection
        if (resIndex == -1)
        {
            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].DuplicatePrints.Contains(key) || results[i].AltArts.Contains(key))
                {
                    resIndex = i;
                    break;
                }
            }
        }
        if (resIndex > -1)
        {
            var coords = CardScroll.FindCoordinates(resIndex, results.Count, settings);
            JumpToPage(coords.page);
            scrollBar.value = coords.scrollValue;
            CardView card = GetCardAtVisibleIndex(coords.indexOnPage);
            card.Highlight(card.cardBorder, Color.yellow, 3f, 1f);
        }
    }

    public void OnScrollValueChanged(Vector2 pos)
    {
        CheckCardsInView();
    }

    private void CheckCardsInView()
    {
        for (int i = 0; i < CardsList.Count; i++)
        {
            if (CardsList[i].Rect.DoesIntersect(CardScroll.viewport))
            {
                CardsList[i].touch.Interactable = true;
            }
            else
            {
                CardsList[i].touch.Interactable = false;
            }
        }
    }
    #endregion

    public void CardClicked(CardView card)
    {

    }
    public void DisplayCard(CardView card)
    {
        if (card.ActiveCard != null)
        {
            cardDisplay.LoadCard(card.ActiveCard);
            CardScroll.gameObject.SetActive(false);
            nonCardCanvas.gameObject.SetActive(false);
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
            nonCardCanvas.gameObject.SetActive(true);
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
            ToggleFilterMenu(false);
            LoadCatalog();
        }


    }


    #endregion
}
