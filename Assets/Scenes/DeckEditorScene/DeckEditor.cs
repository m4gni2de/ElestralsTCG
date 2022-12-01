using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Decks;
using CardsUI.Filtering;
using static Decks.Decklist;
using System;
using PopupBox;
using Cards;
using UnityEngine.UI;
using Gameplay.Menus;
using nsSettings;

public class DeckEditor : ValidationObject
{
    #region Deck Constraints
    public static readonly int MaxSpiritCount = 20;
    public static readonly int MinDeckCount = 60;
    public static readonly int MaxDeckCount = 60;
    #endregion

    #region Instance
    public static DeckEditor Instance { get; private set; }
    public static bool HasChanges
    {
        get
        {
            if (Instance == null) { return false; }
            //if (Instance.IsDirty) { return true; }
            return !Instance.Validate();
        }
    }


    #endregion

    #region Properties
    private CardView _selectedCard = null;
    public CardView SelectedCard
    {
        get
        {
            return _selectedCard;
        }
        private set
        {
            bool didChange = value != _selectedCard;
            if (didChange)
            {
                qtyChanger.Hide();
            }
            _selectedCard = value;
        }
    }
    public CardScroll deckScroll;
    public TMP_Dropdown deckSelector;

   
    private Canvas canvas;
    [SerializeField] private string canvasSortingLayer;
    [SerializeField] private Button btnUndo, btnOpenCatalog;
    [SerializeField] private MagicTextBox spiritTxt, mainTxt, totalTxt;
    [Header("Canvas Sorting")]
    [SerializeField] private int _catalogSortId = 2;
    [SerializeField] private int _catalogSortOrder = 0;

    #region Catalog Section
    public CardCatalog catalog;
    [SerializeField] private CustomScroll deckCardScroll;


    public void ToggleCatalog(bool isOn)
    {
       
        if (isOn)
        {
            GridSettings sett = GridSettings.Create(5, 3, 70, new Vector2(4f, 12f));
            catalog.Open(sett, SortingLayer.IDToName(_catalogSortId), _catalogSortOrder);

            GridSettings deckListSettings = GridSettings.Create(1, 0, -1, new Vector2(0f, 0f));
            deckCardScroll.Initialize(deckListSettings, SetDeckCard);
            deckCardScroll.SetDataContext(deckScroll.Cards);
        }
        
        catalog.Toggle(isOn);
        deckCardScroll.Toggle(isOn);
        deckScroll.Toggle(!isOn);
        
    }

    private void SetDeckCard(iGridCell obj, object data)
    {
        CardQtyCell cell = (CardQtyCell)obj;
        cell.SetClickListener(() => catalog.JumpToCard(cell.ConnectedCard.ActiveCard.cardData.cardKey));
    }

    #endregion

   
    #endregion

    #region Deck Selecting Properties
    [SerializeField] private GameObject selectorPanel;
    [SerializeField] private Button showSelectorBtn;
    [SerializeField] private Button hideSelectorBtn;

    private List<Decklist> _deckList = null;
    public List<Decklist> Decklist
    {
        get
        {
            _deckList ??= new List<Decklist>();
            return _deckList;
        }
    }
    private Decklist _activeDeck = null;
    public Decklist ActiveDeck
    {
        get
        {
            _activeDeck ??= App.ActiveDeck;
            return _activeDeck;
        }
        set
        {
            _activeDeck = value;
        }
    }
    #endregion

    #region Deck Editing Properties


    [SerializeField] private GameObject DeckInfo;
    [SerializeField] private MagicInputText deckNameText;
    [SerializeField] private MagicToggle activeToggle;

    private Archive<CardHistory> _history = null;
    public Archive<CardHistory> History
    {
        get
        {
            _history ??= new Archive<CardHistory>();
            return _history;
        }
    }
    private Dictionary<string, int> _cardCounts = null;
    public Dictionary<string, int> CardCounts
    {
        get
        {
            _cardCounts ??= new Dictionary<string, int>();
            return _cardCounts;
        }
    }


    private int _deckindex = -1;
    private int deckIndex { get { return _deckindex; } set { _deckindex = value; } }
    private int _pendingDeckIndex = -1;
    protected List<TMP_Dropdown.OptionData> DeckOptions
    {
        get
        {
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            for (int i = 0; i < Decklist.Count; i++)
            {
                TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(Decklist[i].DeckName);
                options.Add(optionData);
            }
            return options;
        }
    }

    private bool _isDirty = false;
    public bool IsDirty
    {
        get
        {
            return _isDirty;
        }
        private set
        {
            if (_isDirty != value)
            {
                btnUndo.interactable = History.Count > 0;
            }
            _isDirty = value;
        }
    }

    //private void ToggleDirty(bool setDirty)
    //{
    //    IsDirty = setDirty;
    //}



    #endregion

    #region Initialization

    private void Refresh()
    {
        deckSelector.ClearOptions();
        ToggleSelector(true);
        ToggleCatalog(false);
        qtyChanger.Hide();
        Decklist.Clear();
        ActiveDeck = null;

    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            canvas = GetComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;

            string layerName = "Card";
            if (!string.IsNullOrEmpty(canvasSortingLayer)) { layerName = canvasSortingLayer; }
            canvas.sortingLayerName = layerName;
            
        }

    }
    private void Start()
    {

    }
    private void OnDestroy()
    {
        CardStack.OnQuantityChanged -= SetCardQuantity;
        CardView.OnCardClicked -= CardClick;
        if (Instance != null)
        {
            Instance = null;
        }

    }
    public void LoadDecks(List<Decklist> list)
    {
        Refresh();
        _deckList = new List<Decklist>();
        for (int i = 0; i < list.Count; i++)
        {
            Decklist.Add(list[i]);

        }

        deckSelector.AddOptions(DeckOptions);
        deckSelector.value = 0;

    }

    #endregion
    #region Deck Selection
    public void ToggleSelector(bool isOn)
    {
       
        selectorPanel.SetActive(isOn);
        
        showSelectorBtn.gameObject.SetActive(!isOn);
        if (isOn)
        {
            deckScroll.ToggleCanvas(false);

        }
        else
        {
            deckScroll.ToggleCanvas(true);
        }
    }

   

    public void SelectDeck()
    {
        int index = deckSelector.value;
        if (index != deckIndex || ActiveDeck == null)
        {
            //do some save validation stuff here before changing decks
            
            TryChangeDeck(index);


        }
    }
    private void TryChangeDeck(int newIndex)
    {
        _pendingDeckIndex = newIndex;
        if (IsDirty || deckNameText.IsContentChanged())
        {
            string msg = $"There are unsaved changes to the deck. Do you wish to Save them before changing Decks?";
            App.AskYesNoCancel(msg, SaveDeckChanges);
        }
        else
        {
            SetActiveDeck(Decklist[newIndex]);
            deckIndex = newIndex;
            _pendingDeckIndex = -1;
        }

    }

    public void OpenDeckEditor()
    {
        GridSettings sett = GridSettings.Create(8, 8, 60, new Vector2(5f, 5f));
        deckScroll.Setup(sett, false);
        deckScroll.Settings.stackDuplicates = true;
    }
    
    private void UpdateDeckEditor(Decklist deck)
    {
        IsDirty = false;
        History.Clear();
        

        CardCounts.Clear();
        foreach (var item in deck.Quantities)
        {
            CardCounts.Add(item.Key, item.Value);
        }
        deckScroll.LoadDeck(CardCounts);
        SetDeckInfo(deck);
        SetQuantityTexts();

    }

    private void SetWatchers(bool turnOn)
    {
        if (turnOn)
        {
            CardStack.OnQuantityChanged += SetCardQuantity;
            CardView.OnCardClicked += CardClick;
            CardView.OnCardHeld += StartCardHold;
        }
        else
        {
            CardStack.OnQuantityChanged -= SetCardQuantity;
            CardView.OnCardClicked -= CardClick;
            CardView.OnCardHeld -= StartCardHold;
        }
    }
    private void SetActiveDeck(Decklist deck)
    {
        SetWatchers(false);
        IsDirty = false;
        History.Clear();

        if (deckIndex < 0)
        {
            OpenDeckEditor();
        }
        ActiveDeck = deck;
        
        CardCounts.Clear();
        foreach (var item in deck.Quantities)
        {
            CardCounts.Add(item.Key, item.Value);
        }
        deckScroll.LoadDeck(CardCounts);
       
        ToggleSelector(false);
        SetDeckInfo(deck);
        SetWatchers(true);
        ToggleCatalog(false);
        
    }




    #endregion

    #region Deck Information
    private void SetDeckInfo(Decklist deck)
    {
        deckNameText.Refresh();
        deckNameText.SetText(deck.DeckName);
        deckNameText.AddTextChangeListener(() => SetDeckName(deckNameText.Content));

        SetActiveToggle(deck);

    }

    private void SetDeckName(string newName)
    {
        if (ActiveDeck != null) { ActiveDeck.DeckName = newName; SaveActiveDeck(); }
    }

    #region Active Status Toggle
    private void SetActiveToggle(Decklist deck)
    {
        bool isActiveDeck = deck.IsActiveDeck;
        activeToggle.OnToggleChanged -= OnActiveDeckChanged;
        if (isActiveDeck)
        {
            activeToggle.Toggle(true);
            activeToggle.Interactable = false;
            activeToggle.SetText("Already active.");
        }
        else
        {
            activeToggle.Interactable = true;
            activeToggle.Toggle(false);
            activeToggle.SetText("Make active deck?");
            activeToggle.OnToggleChanged += OnActiveDeckChanged;
        }
    }

    private void OnActiveDeckChanged(MagicToggle toggle)
    {
        if (toggle.IsOn)
        {
            int currentUserActive = SettingsManager.Account.Settings.ActiveDeck;
            App.AskYesNo($"Would you like to change your Active Deck from {App.Account.DeckLists[currentUserActive].DeckName} to {ActiveDeck.DeckName}?", ConfirmActiveSelection);
        }
    }

    private void ConfirmActiveSelection(bool confirm)
    {
        if (confirm)
        {
            SettingsManager.Account.Settings.ActiveDeck = int.Parse(ActiveDeck.DeckKey);
            SetActiveToggle(ActiveDeck);
        }
        else
        {
            activeToggle.Toggle(false);
        }
    }
    #endregion

   
    #endregion

    #region Undo
    public void UndoButtonClick()
    {
        App.AskYesNo($"Do you want to Undo the last change?", UndoChange);
    }
    #endregion

    #region Deck Functions
    public List<CardView> CardsWithSharedBase(Card card, List<CardView> cardsToSearch)
    {
        List<CardView> cards = new List<CardView>();
        List<string> allDupes = card.AltArts;
        for (int i = 0; i < cardsToSearch.Count; i++)
        {
            CardView view = cardsToSearch[i];
            if (view.ActiveCard != null)
            {
                if (allDupes.Contains(view.CardKey))
                {
                    cards.Add(view);
                }
            }
        }
        return cards;

    }

    public int SpiritCount
    {
        get
        {
            int count = 0;
            for (int i = 0; i < deckScroll.Cards.Count; i++)
            {
                CardStack card = (CardStack)deckScroll.Cards[i];
                if (card.ActiveCard != null)
                {
                    if (card.ActiveCard.CardType == CardType.Spirit)
                    {
                        count += card.quantity;
                    }
                }
            }
            return count;
        }
    }
    public int MainDeckCount
    {
        get
        {
            int count = 0;
            for (int i = 0; i < deckScroll.Cards.Count; i++)
            {
                CardStack card = (CardStack)deckScroll.Cards[i];
                if (card.ActiveCard != null)
                {
                    if (card.ActiveCard.CardType != CardType.Spirit)
                    {
                        count += card.quantity;
                    }
                }
            }
            return count;
        }
    }
    public int DeckCardCount
    {
        get
        {
            int count = 0;
            foreach (var item in CardCounts)
            {
                count += item.Value;
            }
            return count;
        }
    }

    public int QuantityInDeck(CardView view)
    {

        List<string> alts = CardService.GetAllDuplicateKeys(view.ActiveCard.cardData);
        if (!alts.Contains(view.CardKey)) { alts.Add(view.CardKey); }
        int cardQty = 0;
        for (int i = 0; i < alts.Count; i++)
        {
            string cardKey = alts[i];
            if (CardCounts.ContainsKey(cardKey))
            {
                cardQty += CardCounts[cardKey];
            }
        }
        return cardQty;
    }
    #endregion

    #region Card Quantity 
    [SerializeField] private IncrementChanger qtyChanger;
    private void AddHistory(string cardKey, int oldQty, int newQty)
    {
        CardHistory h = new CardHistory(cardKey, oldQty, newQty);
        History.Add(h);
    }
    private void UndoChange(bool undo)
    {
        if (!undo) { return; }
        if (History.Count == 0) { IsDirty = false;  return; }
       
        CardHistory mostRecent = History.LastItem;
        History.RemoveLatest();
        if (mostRecent.qty > 0)
        {
            bool cardObjCreated;
            AddCard(mostRecent.cardKey, mostRecent.oldQty, false, out cardObjCreated);

        }
        else
        {
            RemoveCard(mostRecent.cardKey, false);
        }
        IsDirty = History.Count > 0;

        
    }
    //public void ChangeQuantityButton(float qty)
    //{
    //    CardStack stack = SelectedCard as CardStack;
    //    stack.SetQuantity((int)qty, true);
    //}

    /// <summary>
    /// This is called when changing the qty of a card in the deck, but when selecting a card from another scroll or window. Finds the corresponding card in the deck, then sets/changes the qty.
    /// </summary>
    /// <param name="qty"></param>
    private void ChangeQuantityInDeck(float quantity)
    {
        CardView selected = SelectedCard;
        string cardKey = selected.CardKey;

        bool addedNewCard;
        AddCard(cardKey, (int)quantity, true, out addedNewCard);
        if (addedNewCard)
        {
            deckCardScroll.AddData(deckScroll.Cards.Last());
        }
    }

    /// <summary>
    /// The Up and Down Qty Buttons on the template card in the card scroll must call this
    /// </summary>
    /// <param name="setKey"></param>
    /// <param name="qty"></param>
    public void SetCardQuantity(CardView card, int qty, bool addHistory)
    {
        string setKey = card.CardKey;
        if (CardCounts.ContainsKey(setKey))
        {
            int oldQty = CardCounts[setKey];
            CardCounts[setKey] = qty;
            if (addHistory)
            {
                AddHistory(setKey, oldQty, qty);
            }
        }
        else
        {
            CardCounts.Add(setKey, qty);
            if (addHistory)
            {
                AddHistory(setKey, 0, qty);
            }
           
        }

        IsDirty = true;
        CheckCardQuantities();
       
    }

    #region Card Qty Constraints
    private void SetQuantityTexts()
    {
        spiritTxt.SetText($"Spirits:  {SpiritCount}");
        mainTxt.SetText($"Main Deck:  {MainDeckCount}");
        totalTxt.SetText($"Total:  {DeckCardCount}");

        btnOpenCatalog.interactable = DeckCardCount < MaxDeckCount;
    }
    private void CheckCardQuantities()
    {
        if (SelectedCard != null)
        {
           
            CardType type = SelectedCard.ActiveCard.CardType;
            if (SelectedCard is CardStack)
            {
                if (type == CardType.Spirit)
                {
                    if (SpiritCount >= MaxSpiritCount) { SetQtyButtons(false, true); }
                    if (SpiritCount <= 0) { SetQtyButtons(true, false); }
                    if (SpiritCount > 0 && SpiritCount < MaxSpiritCount) { SetQtyButtons(true, true); }
                }
                else
                {
                    CheckCardLimits(SelectedCard as CardStack);
                    
                }
            }


        }
    }

    private void SetQtyButtons(bool canAdd, bool canRemove)
    {
        qtyChanger.ToggleUpButton(canAdd);
        qtyChanger.ToggleDownButton(canRemove);
    }
    private void CheckCardLimits(CardStack view)
    {
        bool canAdd = true;
        bool canRemove = true;

        List<CardView> alts = CardsWithSharedBase(view.ActiveCard, deckScroll.Cards);
        string cardKey = view.ActiveCard.cardData.cardKey;
        int qty = view.quantity;

        int altCount = 0;
        for (int i = 0; i < alts.Count; i++)
        {
            CardStack card = (CardStack)alts[i];
            altCount += card.quantity;
        }
        int totalCopies = qty + altCount;
        if (totalCopies >= CardService.DeckLimit(cardKey))
        {
            canAdd = false;
            canRemove = true;
        }
        else
        {
            canAdd = true;
            canRemove = totalCopies > 0;
        }
        SetQtyButtons(canAdd, canRemove);
    }

    #endregion


    #endregion
    #region Card Adding/Removing
    public bool CanAddCard(CardView view)
    {
        float max = 3f;
        if (view.ActiveCard.CardType == CardType.Spirit) { max = 20f; }
        int deckQty = QuantityInDeck(view);
        return deckQty < max;
    }
    public void CatalogToggleButton(bool turnOn)
    {
        ToggleCatalog(turnOn);
    }
    public void AddCard(string setKey, int qty, bool addHistory, out bool wasCreated)
    {
        wasCreated = false;
        CardStack stack = (CardStack)deckScroll.FindCard(setKey);
        if (stack != null)
        {
            stack.ChangeQuantity(qty, addHistory);
        }
        else
        {
            deckScroll.AddCardByKey(setKey, addHistory, qty);
            wasCreated = true;
        }

    }
    public void RemoveCard(string setKey, bool addHistory)
    {
        CardStack stack = (CardStack)deckScroll.FindCard(setKey);
        if (stack != null)
        {
            stack.ChangeQuantity(0, addHistory);
        }
    }
    #endregion

    #region Card Click/Hold
    private Coroutine _cardHeld = null;
    protected Coroutine CardHeld { get { return _cardHeld; } set { _cardHeld = value; } }

    /// <summary>
    /// CardStacks in the DeckScroll and CardViews in the Catalog are set with this
    /// </summary>
    /// <param name="view"></param>
    private void CardClick(CardView view)
    {
        if (_selectedCard != null && _selectedCard == view)
        {
            if (qtyChanger.gameObject.activeSelf)
            {
                qtyChanger.Hide();
                return;
            }
        }
        SelectedCard = view;
        if (view is CardStack)
        {
            ClickCardStack(view as CardStack);
        }
        else
        {
            ClickCatalogCard(view);
        }
    }
    private void ClickCardStack(CardStack view)
    {
        float max = 3f;
        if (view.ActiveCard.CardType == CardType.Spirit) { max = 20f; }
        qtyChanger.Load(view.DisplayName, view.quantity, 1f, 0f, max);
        qtyChanger.AddValueListener(view.SetQuantity);
    }
    private void ClickCatalogCard(CardView view)
    {
        float max = 3f;
        if (view.ActiveCard.CardType == CardType.Spirit) { max = 20f; }

        int deckQty = QuantityInDeck(view);
        qtyChanger.Load(view.DisplayName, deckQty, 1f, 0f, max);
        qtyChanger.AddValueListener(ChangeQuantityInDeck);
    }

    public void StartCardHold(CardView view)
    {
        if (CardHeld != null) { StopCoroutine(CardHeld); }
        //CardHeld = StartCoroutine(DoCardHold(view));
    }

    private void HoldCardStack(CardStack view)
    {
        float max = 3f;
        if (view.ActiveCard.CardType == CardType.Spirit) { max = 20f; }
        qtyChanger.Load(view.DisplayName, view.quantity, 1f, 0f, max);
        qtyChanger.AddValueListener(view.SetQuantity);
    }
    private void HoldCardView(CardView view)
    {
        float max = 3f;
        if (view.ActiveCard.CardType == CardType.Spirit) { max = 20f; }

        int deckQty = QuantityInDeck(view);
        qtyChanger.Load(view.DisplayName, deckQty, 1f, 0f, max);
        qtyChanger.AddValueListener(ChangeQuantityInDeck);
    }

   
    #endregion









    #region Validation/Saving
    public override bool Validate()
    {
        ErrorList.Clear();
        if (deckNameText.IsContentChanged()) { AddError($"Deck Name has un-saved changes."); }
        if (History.Count > 0) { AddError($"Deck has un-saved card changes."); }
        if (IsDeckChanged()) { AddError($"Quantities in deck and in editor don't match."); }
        return ErrorList.Count == 0;
    }

    public bool IsDeckChanged()
    {
        if (ActiveDeck == null) { return false; }

        Dictionary<string, int> originalQty = ActiveDeck.Quantities;
        foreach (var item in CardCounts)
        {
            if (originalQty.ContainsKey(item.Key))
            {
                int startQty = originalQty[item.Key];
                if (startQty != item.Value) { continue; }
            }
            else
            {
                return true;
            }
        }

        return false;
    }
    private void SaveDeckChanges(PopupResponse response)
    {

        switch (response)
        {
            case PopupResponse.Cancel:
                break;
            case PopupResponse.Yes:
                SaveActiveDeck();
                SetActiveDeck(Decklist[_pendingDeckIndex]);
                deckIndex = _pendingDeckIndex;
                break;
            case PopupResponse.No:
                SetActiveDeck(Decklist[_pendingDeckIndex]);
                deckIndex = _pendingDeckIndex;
                break;
        }

        _pendingDeckIndex = -1;

    }
    public void SaveActiveDeck()
    {
        ActiveDeck.Save(CardCounts);
        UpdateDeckEditor(ActiveDeck);
    }
    #endregion

}
