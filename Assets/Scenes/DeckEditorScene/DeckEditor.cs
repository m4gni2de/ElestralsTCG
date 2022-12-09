using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Cards;
using CardsUI.Filtering;
using Databases;
using Decks;
using Gameplay.Menus;
using nsSettings;
using PopupBox;
using TMPro;
using TouchControls;
#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
#endif

using UnityEngine;
using UnityEngine.UI;
using static Decks.Decklist;

public class DeckEditor : ValidationObject, iFreeze
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

    private Card _activeCard = null;
    public Card ActiveCard
    {
        get
        {
            return _activeCard;
        }
        set
        {
            _activeCard = value;
        }
    }
    private CardView _selectedCard = null;
    public CardView SelectedCard
    {
        get
        {
            return _selectedCard;
        }
        set
        {
            if (_selectedCard == value) { return; }
            if (_selectedCard != null)
            {
                _selectedCard.ResetColors();
            }
            value.Highlight(value.cardBorder, Color.yellow);
            _selectedCard = value;
        }
    }

    

    private void SetActiveCard(Card card = null)
    {
        if (card == null)
        {
            ActiveCard = null;
            SelectedCard = null;
            qtyChanger.Hide();
        }
        else
        {
            ActiveCard = card;
            int qty = QuantityInDeck(card);
            float max = 3f;
            if (card.CardType == CardType.Spirit) { max = 20f; }
            qtyChanger.Load(card.DisplayName, qty, 1f, 0f, max);

            bool canAdd = CanAddCard(ActiveCard.CardType, qty);
            bool canRemove = qty > 0;
            SetQtyButtons(canAdd, canRemove);
        }
    }

    public TMP_Dropdown deckSelector;

   
    private Canvas canvas;
    [SerializeField] private string canvasSortingLayer;
    [SerializeField] private Button btnUndo, btnOpenCatalog;
    [SerializeField] private MagicTextBox spiritTxt, mainTxt, totalTxt;
    [Header("Canvas Sorting")]
    [SerializeField] private int _catalogSortId = 2;
    [SerializeField] private int _catalogSortOrder = 0;


    public CardScroll deckScroller;
    [SerializeField] private CardView DisplayCard;
    #region Catalog Section
    public CardCatalog catalog;
    [SerializeField] private CustomScroll deckCardScroll;

    


    public void ToggleCatalog(bool isOn)
    {
      
        if (isOn)
        {
            if (!deckCardScroll.IsLoaded)
            {
                GridSettings sett = GridSettings.Create(5, 3, 70, new Vector2(4f, 12f));
                catalog.Open(sett, SortingLayer.IDToName(_catalogSortId), _catalogSortOrder);

                GridSettings deckListSettings = GridSettings.Create(1, 0, -1, new Vector2(0f, 0f));
                deckCardScroll.Initialize(deckListSettings, SetDeckCard);
                
            }
               deckCardScroll.SetDataContext(DeckCards);
            catalog.Toggle(true);
            deckCardScroll.Toggle(true);
            deckScroller.Toggle(false);
        }
        else
        {
            catalog.Toggle(false);
            deckCardScroll.Toggle(false);
            deckScroller.Toggle(true);
        }  
    }

    #region Scroll Cell Data Overrides
    private void SetDeckCard(iGridCell obj, object data)
    {
        CardQtyCell cell = (CardQtyCell)obj;
        cell.SetClickListener(() => SelectDeckQtyCard(cell));
    }
    private void SelectDeckQtyCard(CardQtyCell cell)
    {
        catalog.JumpToCard(cell.ConnectedCard.key);
        SetActiveCard(cell.ActiveCard);
    }

   
    #endregion

    #endregion


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

    public List<DeckCard> CardsWithSharedBase(Card card, List<DeckCard> cardsToSearch)
    {
        List<DeckCard> cards = new List<DeckCard>();
        List<string> allDupes = card.AltArts;
        for (int i = 0; i < cardsToSearch.Count; i++)
        {
            DeckCard dc = cardsToSearch[i];
            if (allDupes.Contains(dc.key))
            {
                cards.Add(dc);
            }

        }
        return cards;

    }
    public int IndexInDeck(string setKey)
    {
        for (int i = 0; i < DeckCards.Count; i++)
        {
            if (DeckCards[i].key == setKey) { return i; }
        }
        return -1;
    }
    public DeckCard FindCardInDeck(string setKey)
    {
        for (int i = 0; i < DeckCards.Count; i++)
        {
            if (DeckCards[i].key == setKey) { return DeckCards[i]; }
        }
        return null;
    }

    public int SpiritCount
    {
        get
        {
            int count = 0;
            for (int i = 0; i < DeckCards.Count; i++)
            {
                if (DeckCards[i].cardType == CardType.Spirit)
                {
                    count += DeckCards[i].copy;
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
            for (int i = 0; i < DeckCards.Count; i++)
            {
                if (DeckCards[i].cardType != CardType.Spirit)
                {
                    count += DeckCards[i].copy;
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
            foreach (var item in DeckCards)
            {
                count += item.copy;
            }
            return count;
        }
    }

   
    public int QuantityInDeck(Card card)
    {

        List<string> alts = CardService.GetAllDuplicateKeys(card.cardData);
        int cardQty = 0;
        for (int i = 0; i < alts.Count; i++)
        {
            string cardKey = alts[i];
            DeckCard c = FindCardInDeck(cardKey);
            if (c != null)
            {
                cardQty += c.copy;
            }
        }


        return cardQty;
    }
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
    
    private List<DeckCard> _deckCards = null;
    public List<DeckCard> DeckCards
    {
        get
        {
            _deckCards ??= new List<DeckCard>();
            return _deckCards;
        }
    }

    public List<Card> Cards
    {
        get
        {
            List<Card> card = new List<Card>();
            for (int i = 0; i < DeckCards.Count; i++)
            {
                Card c = DeckCards[i];
                card.Add(c);
            }
            return card;
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
        qtyChanger.OnValueChanged += ChangeCardQtyInDeck;
    }
    private void OnDestroy()
    {
        //CardStack.OnQuantityChanged -= SetCardQuantity;
        CardView.OnCardClicked -= SelectCard;
        CardView.OnCardHeld -= StartCardHold;
        qtyChanger.OnValueChanged -= ChangeCardQtyInDeck;
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
        deckScroller.ToggleCanvas(!isOn);
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
        deckScroller.Initialize(sett);
        deckScroller.Settings.stackDuplicates = true;
    }
   
    private void UpdateDeckEditor(Decklist deck)
    {
        SetWatchers(false);
        IsDirty = false;
        History.Clear();

        DeckCards.Clear();
        DeckCards.AddRange(deck.GetCardQuantities);
        deckScroller.SetDataContext(Cards);

        SetDeckInfo(deck);
        SetWatchers(true);
        

    }

    private void SetWatchers(bool turnOn)
    {
        if (turnOn)
        {
            //CardStack.OnQuantityChanged += SetCardQuantity;
            CardView.OnCardClicked += SelectCard;
            CardView.OnCardHeld += StartCardHold;
            
        }
        else
        {
            //CardStack.OnQuantityChanged -= SetCardQuantity;
            CardView.OnCardClicked -= SelectCard;
            CardView.OnCardHeld -= StartCardHold;
        }
    }
    private void SetActiveDeck(Decklist deck)
    {
        SetWatchers(false);
        IsDirty = false;
        History.Clear();

        //SelectedCard = null;
        ActiveCard = null;

        if (deckIndex < 0)
        {
            OpenDeckEditor();
        }
        ActiveDeck = deck;

        DeckCards.Clear();
        DeckCards.AddRange(deck.GetCardQuantities);
        deckScroller.SetDataContext(Cards);
       
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
        SetQuantityTexts();

    }

    private void SetDeckName(string newName)
    {
        if (ActiveDeck != null) { ActiveDeck.DeckName = newName; SaveActiveDeck(); UpdateDeckEditor(ActiveDeck); }
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
            if (DeckCardCount < MaxDeckCount)
            {
                App.DisplayError($"Deck must have 60 cards to be used in-game. Please select a deck with 60 cards to be your Active Deck.");
                activeToggle.Toggle(false);
            }

            if (DeckCardCount == MaxDeckCount)
            {
                int currentUserActive = SettingsManager.Account.Settings.ActiveDeck;
                App.AskYesNo($"Would you like to change your Active Deck from {App.Account.DeckLists[currentUserActive].DeckName} to {ActiveDeck.DeckName}?", ConfirmActiveSelection);
            }
            
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
        DoFreeze();
        App.AskYesNo($"Do you want to Undo the last change?", UndoChange);
    }
    #endregion

   

    #region Card Quantity 
    [SerializeField] private IncrementChanger qtyChanger;
    private void AddHistory(string cardKey, int oldQty, int newQty)
    {
        CardHistory h = new CardHistory(cardKey, oldQty, newQty);
        History.Add(h);
        btnUndo.interactable = true;
    }
    private void UndoChange(bool undo)
    {
        Invoke("DoThaw", .5f);
        if (!undo) { return; }
        if (History.Count == 0) { IsDirty = false;  btnUndo.interactable = false; return; }
       
        CardHistory mostRecent = History.LastItem;

        int changeVal = mostRecent.oldQty - mostRecent.qty;
        History.RemoveLatest();

        SetActiveCardFromKey(mostRecent.cardKey, true);
        ActiveCardQuantityChange(mostRecent.oldQty, false);

       
        if (History.Count == 0) { btnUndo.interactable = false; }  
    }


    #region Active Card instead of SelectedCard
    public void SelectCard(CardView view)
    {
        SelectedCard = view;
        if (view.ActiveCard != null)
        {
            
            SetActiveCard(view.ActiveCard);

        }
        else
        {
            SetActiveCard();
        }
    }
    private void SetActiveCardFromKey(string cardKey, bool doSilent = false)
    {
        qUniqueCard dto = CardService.ByKey<qUniqueCard>(CardService.qUniqueCardView, "setKey", cardKey);
        Card c = dto;
        if (doSilent)
        {
            ActiveCard = c;
        }
        else
        {
            SetActiveCard(c);
        }
        


    }
    private void ChangeCardQtyInDeck(float quantity)
    {
        ActiveCardQuantityChange((int)quantity, true);

    }

    public void ActiveCardQuantityChange(int qty, bool addHistory)
    {
        string cardKey = ActiveCard.cardData.cardKey;
        int index = IndexInDeck(cardKey);
        if (index > -1)
        {
            DeckCard inDeck = FindCardInDeck(cardKey);
            int oldQty = inDeck.copy;
            inDeck.copy = qty;
            if (addHistory)
            {
                AddHistory(cardKey, oldQty, qty);
            }


            if (qty <= 0)
            {
                DeckCards.Remove(inDeck);
                
            }

            deckScroller.SetDataContext(Cards);
            if (deckCardScroll.gameObject.activeSelf == true)
            {
                deckCardScroll.SetDataContext(DeckCards);
            }
        }
        else
        {
            DeckCard newCard = new DeckCard(cardKey, ActiveCard.CardType, qty);
            DeckCards.Add(newCard);
            if (addHistory)
            {
                AddHistory(cardKey, 0, qty);
            }
            Card c = newCard;
            deckScroller.AddData(c);
            if (deckCardScroll.gameObject.activeSelf == true)
            {
                deckCardScroll.AddData(newCard);
            }
        }


        IsDirty = true;

        int newQty = QuantityInDeck(ActiveCard);
        bool canAdd = CanAddCard(ActiveCard.CardType, newQty);
        bool canRemove = newQty > 0;
        SetQtyButtons(canAdd, canRemove);

        
        float max = 3f;
        if (ActiveCard.CardType == CardType.Spirit) { max = 20f; }
        qtyChanger.Load(ActiveCard.DisplayName, newQty, 1f, 0f, max);

        SetQuantityTexts();


    }


    public bool CanAddCard(CardType type, int deckQty)
    {

        float max = 3f;
        if (type == CardType.Spirit)
        {
            max = 20f;
            if (SpiritCount >= MaxSpiritCount) { return false; }

        }
        else
        {
            if (MainDeckCount >= (MaxDeckCount - MaxSpiritCount)) { return false; }
        }

        if (DeckCardCount >= MaxDeckCount) { return false; }
        if (deckQty >= max) { return false; }


        return true;
    }
    #endregion


    #region SELECTED CARD DEPRECIATED
   
    #region Card Qty Constraints
    private void SetQuantityTexts()
    {
        spiritTxt.SetText($"Spirits:  {SpiritCount}");
        mainTxt.SetText($"Main Deck:  {MainDeckCount}");
        totalTxt.SetText($"Total:  {DeckCardCount}");

        btnOpenCatalog.interactable = DeckCardCount < MaxDeckCount;
    }
    private void SetQtyButtons(bool canAdd, bool canRemove)
    {
        qtyChanger.ToggleUpButton(canAdd);
        qtyChanger.ToggleDownButton(canRemove);
    }

    #endregion


    #endregion
    #region Card Adding/Removing
   
    public void CatalogToggleButton(bool turnOn)
    {
        ToggleCatalog(turnOn);
    }

  
    public void RemoveCard(string setKey, bool addHistory)
    {
        CardStack stack = (CardStack)deckScroller.FindCard(setKey);
        if (stack != null)
        {
            stack.SetQuantity(0, addHistory);
        }
    }

    #endregion
    #endregion

    #region Card Click/Hold
    private Coroutine _cardHeld = null;
    protected Coroutine CardHeld { get { return _cardHeld; } set { _cardHeld = value; } }

    public void StartCardHold(CardView view)
    {
        if (CardHeld != null) { StopCoroutine(CardHeld); }
        SelectedCard = view;
        CardHeld = StartCoroutine(DoCardHold(view.ActiveCard));
    }
    private IEnumerator DoCardHold(Card card)
    {
        deckScroller.ToggleCanvas(false);
        deckCardScroll.EnableScroll(false);
        catalog.CardScroll.enabled = false;
        SetDisplayCard(card);
        do
        {
            yield return null;

        } while (true && Input.GetMouseButton(0));
        SetDisplayCard();
        deckScroller.ToggleCanvas(true);
        catalog.CardScroll.enabled = true;
        deckCardScroll.EnableScroll(true);

    }

    private void SetDisplayCard(Card card = null)
    {
        if (card != null)
        {
            DisplayCard.LoadCard(card);
        }
        else
        {
            DisplayCard.Clear();
        }
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

        List<DeckCard> originalCards = ActiveDeck.GetCardQuantities;


        for (int i = 0; i < originalCards.Count; i++)
        {

            bool containsCard = false;
            DeckCard card = originalCards[i];
            int expectedQty = card.copy;

            for (int j = 0; j < DeckCards.Count; j++)
            {
                if (DeckCards[j].key == card.key)
                {
                    containsCard = true;
                    if (expectedQty == DeckCards[j].copy)
                    {
                        break;
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            if (!containsCard) { return true; }
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
        //ActiveDeck.Save(CardCounts);
        ActiveDeck.Save(DeckCards);
        
    }
    #endregion

    #region Touch Freezing
    protected void DoFreeze()
    {
        this.Freeze();
    }

    protected void DoThaw()
    {
        this.Thaw();
    }
    #endregion

}
