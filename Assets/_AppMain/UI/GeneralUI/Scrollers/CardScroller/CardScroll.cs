using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Cards;
using CardsUI.Filtering;
using Databases;
using GameActions;
using UnityEngine;
using GlobalUtilities;
using Decks;

public class CardScroll : CustomScroll
{
    #region ScrollSettings
    public class ScrollSettings
    {
        public bool stackDuplicates = false;

    }
    #endregion

    #region Properties
    private CardView templateCard { get { return As<CardView>(templateItem); } }
    public CardView displayCard;
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

    protected List<CardView> cardCells { get { return CellsAs<CardView>(); } }
    private List<Card> cardResults
    {
        get
        {
            List<Card> list = new List<Card>();
            for (int i = 0; i < DataContext.Count; i++)
            {
                list.Add((Card)DataContext[i]);
            }
            return list;
        }
    }
    #endregion

    #region Filtering
    [SerializeField]
    protected FiltersMenu filterMenu;
    protected static readonly string DefaultQueryWhere = " setName is not null;";
    protected string _queryWhere = "";
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

    #region Sorting
    //private List<CardView> SortedCards(List<CardView> toSort)
    //{
    //    List<CardView> sorted = new List<CardView>();

    //    for (int i = 0; i < toSort.Count; i++)
    //    {
    //        toSort.Sort();
    //    }
    //}
    #endregion

    public CardView FindCard(string setKey)
    {
        for (int i = 0; i < cardCells.Count; i++)
        {
            CardView view = cardCells[i];
            if (view.IsCard(setKey)) { return view; }
        }
        return null;
    }

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
        if (cardCount <= perPage || perPage == 0) { return 1; }
        if (cardCount % perPage > 0)
        {
            return (cardCount / perPage) + 1;
        }
        return cardCount / perPage;
    }

    #endregion

    #region Touch Interaction
    public void ToggleCanvas(bool canUse)
    {
        if (!canUse)
        {
            canvasGroup.interactable = false;
            canvasGroup.alpha = .5f;
            EnableScroll(false);
            
           
        }
        else
        {
            canvasGroup.interactable = true;
            canvasGroup.alpha = 1f;
            EnableScroll(true);
        }
    }
   
    #endregion

    #region Initialization
    private void Awake()
    {
        filterMenu.Toggle(false);
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void InitializeCards(List<Card> cards = null)
    {
        if (cards == null)
        {

            cards = Results(QueryWhere);
        }

        SetDataContext(cards);
        PageNumber = 1;
        TotalPages = GetTotalPages(DataContext.Count);

        int loadCount = gridSettings.itemsPerPage;
        if (gridSettings.InfiniteMode || TotalPages == 1) { loadCount = DataContext.Count; }


        LoadData(0, loadCount);
    }

    #region Overrides

    public override void Initialize(GridSettings sett, Action<iGridCell, object> setAction = null)
    {
        displayCard.Hide();
        cardResults.Clear();
        cardCells.Clear();
        templateCard.LoadCard();
        templateCard.Hide();

        LoadGridSetings(sett);

        IsLoaded = true;


        if (setAction != null)
        {
            OnLoadCell = GameAction.Create(setAction);
        }

        DataSorter typeSort = new DataSorter(SortBy.CardType, SortDirection.ASC);
        DataSorter qtySort = new DataSorter(SortBy.Quantity, SortDirection.DESC);
        DataSorter costSort = new DataSorter(SortBy.Cost, SortDirection.DESC);

        Sorter = new Sorter(typeSort, qtySort, costSort);
    }

    protected override void LoadGridSetings(GridSettings sett)
    {
        gridSettings = sett;
        float freeWidth = Scroll.GetComponent<RectTransform>().FreeWidth(Scroll.verticalScrollbar.handleRect, gridSettings.TotalPadding());
        gridSettings.SetGridSize(Grid, freeWidth);
        gridSettings.OnSettingsUpdate -= UpdateGrid;
        gridSettings.OnSettingsUpdate += UpdateGrid;
        templateCard.MatchSize(Grid.cellSize);
    }

    protected override void Refresh()
    {
        scrollBar.value = .99f;
        for (int i = 0; i < cardCells.Count; i++)
        {
            cardCells[i].Clear();
        }

        DataContext.Clear();
        _isDirty = false;
    }
    public override void Toggle(bool isOn)
    {
        filterMenu.Toggle(false);
        Scroll.gameObject.SetActive(isOn);

        if (!isOn)
        {
            for (int i = 0; i < cardCells.Count; i++)
            {
                cardCells[i].touch.Interactable = false;
            }
        }
        else
        {
            CheckVisibleCells();
        }
        
        
    }



    

    public override void SetDataContext<T>(List<T> items)
    {
        Refresh();
        _dataType = typeof(T);

        List<T> sorted = Sorter.SortItems(items);
        for (int i = 0; i < items.Count; i++)
        {
            DataContext.Add(sorted[i]);
            LoadCell(i);
        }

        scrollBar.value = 1f;
        _isDirty = true;
    }

    protected override void ScrollValueChange(Vector2 pos)
    {
       
        CheckVisibleCells();
    }


    #endregion

    #endregion

    #region Card Data Loading


    protected override void LoadCell(int index)
    {
        if (Cells.Count <= index)
        {
            SpawnCell();
        }
        iGridCell cell = Cells[index];
        object data = DataContext[index];
        cell.LoadData(data, index);

        ToggleCelVisibility(cell);
        OnLoadCell?.Invoke(cell, data);
    }
    protected override void LoadCell(int cellIndex, int dataIndex)
    {
        if (Cells.Count < cellIndex)
        {
            SpawnCell();
        }
        CardView cell = cardCells[cellIndex];
        cell.LoadCard(cardResults[dataIndex]);
        ToggleCelVisibility(cell);
        OnLoadCell?.Invoke(cell, cardResults[dataIndex]);
    }

    protected override iGridCell SpawnCell()
    {
        iGridCell cell = base.SpawnCell();
        CardView view = (CardView)cell;
        if (!view.touch.HasGroup || view.touch.Group != touchGroup) { view.touch.RemoveFromGroup(); touchGroup.Add(view.touch); }
        return cell;
    }

    protected void LoadData(int startIndex, int loadCount)
    {
        for (int i = 0; i < loadCount; i++)
        {
            int index = startIndex + i;
            LoadCell(i, index);
        }
    }





    #endregion
}
