using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Sockets;
using Cards;
using CardsUI.Filtering;
using Databases;
using Decks;
using Gameplay.Decks;
using Gameplay.Menus;
using nsSettings;
using PopupBox;
using TMPro;
using TouchControls;
#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEditor.Rendering.LookDev;
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


    public static void EditDeck(Decklist deck)
    {
        if (Instance != null)
        {
            Instance.LoadSpecificDeck(App.Account.DeckLists, deck);
        }
    }
    #endregion

    #region Properties

    #region Card Properties
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

            if (IsRemoteDeck || IsLocked) { canAdd = false; canRemove = false; }
            SetQtyButtons(canAdd, canRemove);
        }
    }
    #endregion
    public TMP_Dropdown deckSelector;

   
    private Canvas canvas;
    [SerializeField] private string canvasSortingLayer;
    [SerializeField] private MagicButton btnUndo, btnOpenCatalog, btnSave, btnLock, btnDownload, btnUpload, btnDelete;
    [SerializeField] private MagicTextBox spiritTxt, mainTxt, totalTxt;
    [Header("Canvas Sorting")]
    [SerializeField] private int _catalogSortId = 2;
    [SerializeField] private int _catalogSortOrder = 0;


    public CardScroll deckScroller;
    [SerializeField] private CardView DisplayCard;
    #region Catalog Section
    public CardCatalog catalog;
    [SerializeField] private CustomScroll deckCardScroll;
    [SerializeField] private DeckImporter deckImporter;

    


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


    private bool _isRemoteDeck = false;
    public bool IsRemoteDeck
    {
        get
        {
            return _isRemoteDeck;
        }
        set
        {
            _isRemoteDeck = value;
        }

    }

    public bool IsLocked { get; set; }
    #endregion

    #region Deck Selecting Properties
    [SerializeField] private GameObject selectorPanel;
    [SerializeField] private DeckDownloader downloader;
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
    [SerializeField] private MagicTextBox uploadCodeText;

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
                btnUndo.CanClick = History.Count > 0;
            }
            _isDirty = value;

            btnSave.CanClick = value;
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
        downloader.Hide();
        Decklist.Clear();
        ActiveDeck = null;
        IsRemoteDeck = false;
        IsLocked = false;

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
        downloader.OnSearchComplete += DownloaderSearchComplete;
        downloader.OnDisplayChanged += OnCodeSearchDisplay;
    }

    

    private void OnDestroy()
    {
        //CardStack.OnQuantityChanged -= SetCardQuantity;
        CardView.OnCardClicked -= SelectCard;
        CardView.OnCardHeld -= StartCardHold;
        qtyChanger.OnValueChanged -= ChangeCardQtyInDeck;
        downloader.OnSearchComplete -= DownloaderSearchComplete;
        downloader.OnDisplayChanged -= OnCodeSearchDisplay;
        ActiveDeck = null;
        ActiveCard = null;
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
    public void LoadSpecificDeck(List<Decklist> list, Decklist deck)
    {
        Refresh();
        _deckList = new List<Decklist>();
        for (int i = 0; i < list.Count; i++)
        {
            Decklist.Add(list[i]);

        }

        deckSelector.AddOptions(DeckOptions);
        deckSelector.value = 0;
        SetActiveDeck(deck);

        int index = -1;
        for (int i = 0; i < Decklist.Count; i++)
        {
            if (Decklist[i].DeckKey.Trim().ToLower() == deck.DeckKey.Trim().ToLower()) { index = i; break; }
        }
        deckIndex = index;
    }

    #endregion

    #region Deck Selection
    public void ToggleSelector(bool isOn)
    {
        selectorPanel.SetActive(isOn);
        showSelectorBtn.gameObject.SetActive(!isOn);
        deckScroller.ToggleCanvas(!isOn);
        if (isOn)
        {
            downloader.Toggle(false);
        }
    }
    public void ToggleDownloder(bool isOn)
    {
        selectorPanel.SetActive(!isOn);
        showSelectorBtn.gameObject.SetActive(!isOn);
        downloader.Toggle(isOn);
    }



    #region Deck Importer
    public void LoadImporter()
    {
        deckImporter.OnDisplayChanged += OnImporterDisplayChange;
        deckImporter.Show();
        
    }
    private void OnImporterDisplayChange(bool isShowing)
    {
        if (!isShowing)
        {
            deckImporter.OnDisplayChanged -= OnImporterDisplayChange;
            deckScroller.ToggleCanvas(true);
            if (ActiveDeck == null || DeckCards.Count == 0)
            {
                selectorPanel.SetActive(true);
            }
        }
        else
        {
            selectorPanel.SetActive(false);
            deckScroller.ToggleCanvas(false);
            downloader.Toggle(false);
        }
    }
    #endregion



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

        string saveMsg = $"There are unsaved changes to the deck. Do you wish to Save them before changing Decks?";
        if (IsRemoteDeck) { saveMsg = $"This deck has not been saved to your device. Do you wish to save it locally before changing Decks?"; IsDirty = true; }

        if (IsDirty || deckNameText.IsContentChanged())
        {
            
            App.AskYesNoCancel(saveMsg, SaveDeckChanges);
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

        IsRemoteDeck = !deck.IsSavedLocally;
        IsLocked = deck.isLocked;

        
        ToggleSelector(false);
        SetDeckInfo(deck);
        SetWatchers(true);
        ToggleCatalog(false);

        
        
    }

    public void LockMode(Decklist deck)
    {
        deckNameText.Refresh();
        deckNameText.SetText(deck.DeckName);
        if (IsLocked)
        {
            deckNameText.ToggleInputEnabled(false);
            btnUndo.CanClick = false;
            btnDelete.CanClick = false;
            App.ShowMessage("Deck is currently locked! To make changes to this Deck, it must be Unlocked by pushing the Unlock Button, or, if the deck is uploaded, remove it from the Database.");
        }
        else
        {
            deckNameText.AddTextChangeListener(() => SetDeckName(deckNameText.Content));
            deckNameText.ToggleInputEnabled(true);
            SetActiveToggle(deck);
            btnUndo.CanClick = History.Count > 0;
            btnDelete.CanClick = !IsRemoteDeck;
        }
    }


    #endregion

    #region Deck Information 
    private void SetDeckInfo(Decklist deck)
    {
        LockMode(deck);

        if (IsRemoteDeck)
        {
            activeToggle.OnToggleChanged -= OnActiveDeckChanged;
            activeToggle.Interactable = false;
            activeToggle.Toggle(false);
            activeToggle.SetText("Deck not yet downloaded.");
            deckNameText.ToggleInputEnabled(false);
        }

        uploadCodeText.Refresh();
        if (deck.IsUploaded)
        {
            uploadCodeText.gameObject.SetActive(true);
            uploadCodeText.SetText(deck.UploadCode);
        }
        else
        {
            uploadCodeText.SetText("");
            uploadCodeText.gameObject.SetActive(false);
        }

       
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
            int index = App.Account.DeckIndex(ActiveDeck.DeckKey);
            SettingsManager.Account.Settings.ActiveDeck = index;
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
        btnUndo.CanClick = true;
        btnSave.CanClick = true;
    }
    private void UndoChange(bool undo)
    {
        DoThawAtInteveral(.5f);
        if (!undo) { return; }
        if (History.Count == 0) { IsDirty = false;  btnUndo.CanClick = false; return; }
       
        CardHistory mostRecent = History.LastItem;

        int changeVal = mostRecent.oldQty - mostRecent.qty;
        History.RemoveLatest();

        SetActiveCardFromKey(mostRecent.cardKey, true);
        ActiveCardQuantityChange(mostRecent.oldQty, false);

       
        if (History.Count == 0)
        {
            btnUndo.CanClick = false;
            btnSave.CanClick = !Validate();
            
        }

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


   
    #region Card Qty Constraints
    private void SetQuantityTexts()
    {
        spiritTxt.SetText($"Spirits:  {SpiritCount}");
        mainTxt.SetText($"Main Deck:  {MainDeckCount}");
        totalTxt.SetText($"Total:  {DeckCardCount}");

        btnOpenCatalog.CanClick = DeckCardCount < MaxDeckCount || IsRemoteDeck;
    }
    private void SetQtyButtons(bool canAdd, bool canRemove)
    {
        qtyChanger.ToggleUpButton(canAdd);
        qtyChanger.ToggleDownButton(canRemove);
    }

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
        if (IsRemoteDeck) { AddError($"Deck has not been saved locally."); };
        if (deckNameText.IsContentChanged()) { AddError($"Deck Name has un-saved changes."); }
        if (History.Count > 0) { AddError($"Deck has un-saved card changes."); }
        if (IsDeckChanged()) { AddError($"Quantities in deck and in editor don't match."); }
        return ErrorList.Count == 0;
    }

    public bool IsDeckChanged()
    {
        if (ActiveDeck == null) { return false; }
        if (DeckCards.Count == 0) { return false; }
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
                App.ShowMessage("Deck saved!");
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
    protected void DoThawAtInteveral(float seconds)
    {
        Invoke("DoThaw", seconds);
    }
    #endregion


    #region Upload/Downloading
    private void DownloaderSearchComplete(Decklist obj)
    {
        bool containsDeck = false;
        //for (int i = 0; i < Decklist.Count; i++)
        //{
        //    Decklist d = Decklist[i];
        //    if (d.UploadCode.ToLower() == obj.DeckKey.ToLower()) { containsDeck = true; break; }
        //}
        if (!containsDeck)
        {
            Decklist.Add(obj);
            int index = Decklist.Count - 1;
            TryChangeDeck(index);
        }
    }

    public void DeckUploadButton()
    {
        if (ActiveDeck.IsUploaded)
        {
            string msg = $"This deck is already uploaded with code '{ActiveDeck.UploadCode}'.";
            if (!IsRemoteDeck)
            {
                msg += " Do you want to remove it from the server?";
                App.AskYesNoCancel(msg, AwaitDeckRemove);
                return;
            }
            else
            {
                App.DisplayError(msg);
            }

        }
        else
        {
            string msg = $"Uploading a deck to the public server will allow it to be shared with the rest of the world. Do you want to Upload it?";
            App.AskYesNoCancel(msg, AwaitDeckUpload);
        }
    }

    private async void AwaitDeckUpload(PopupResponse response)
    {
        switch (response)
        {
            case PopupResponse.Cancel:
                break;
            case PopupResponse.Yes:
                bool uploaded = await ActiveDeck.UploadDeck();
                if (uploaded)
                {
                    string msg = $"Deck uploaded with code {ActiveDeck.UploadCode}";
                    ActiveDeck.UploadCode.CopyToClipboard(false);
                    App.ShowMessage(msg);
                    SetActiveDeck(ActiveDeck);
                }
                break;
            case PopupResponse.No:
                break;
        }
    }

    private async void AwaitDeckRemove(PopupResponse response)
    {
        switch (response)
        {
            case PopupResponse.Cancel:
                break;
            case PopupResponse.Yes:
                bool removed = await ActiveDeck.RemoveUpload();
                if (removed)
                {
                    string msg = $"Deck removed from server!";
                    App.ShowMessage(msg);
                    SetActiveDeck(ActiveDeck);
                }
                break;
            case PopupResponse.No:
                SetActiveDeck(ActiveDeck);
                break;
        }
    }
    #endregion

    #region Buttons
    public void CopyCodeButton()
    {
        if (!uploadCodeText.Content.IsEmpty())
        {
            uploadCodeText.Content.CopyToClipboard(true);
        }
    }
    public void ExportDecklistButton()
    {
        if (ActiveDeck == null || DeckCards.Count == 0) { return; }

        if (DeckCards.Count > 0)
        {
            string msg = $"Decklist has been copied to your clipboard!";
            ActiveDeck.GetCardList.CopyToClipboard(msg);
        }
    }
    public void SaveDeckButton()
    {
        string saveMsg = $"Do you wish to save your deck?";
        if (IsRemoteDeck) { saveMsg = $"This deck has not been saved to your device. Do you wish to save it locally?"; IsDirty = true; }

        if (IsDirty || deckNameText.IsContentChanged())
        {
            _pendingDeckIndex = deckIndex;
            App.AskYesNoCancel(saveMsg, SaveDeckChanges);
        }
    }
    public void LockButton()
    {
        if (IsRemoteDeck) { App.DisplayError("$Deck must be saved locally before being able to edit it."); return; }
        bool locked = ActiveDeck.isLocked;

        if (!locked)
        {
            App.AskYesNoCancel($"In order to lock a deck, it needs to be saved. Do you wish to save and lock this deck?", ToggleDeckLocked);
        }
        else
        {
            ActiveDeck.Lock(false);
            IsDirty = true;
            IsLocked = false;
            LockMode(ActiveDeck);
            App.ShowMessage("Deck unlocked!");
        }
        
        

    }

    private void ToggleDeckLocked(PopupResponse response)
    {
        switch (response)
        {
            case PopupResponse.Cancel:
                break;
            case PopupResponse.Yes:
                ActiveDeck.Lock(true);
                App.ShowMessage("Deck locked!");
                SaveActiveDeck();
                SetActiveDeck(ActiveDeck);
                break;
            case PopupResponse.No:
                SetActiveDeck(ActiveDeck);
                break;
        }

    }
    
    public void DeleteDeckButton()
    {
        if (App.Account.DeckLists.Count == 1) { App.DisplayError("You cannot delete your only remaining deck!"); return; }
        if (ActiveDeck == null) { App.DisplayError("A deck must be selected in order to delete it!"); return; }
        if (ActiveDeck.IsUploaded) { App.DisplayError("A deck uploaded to the server cannot be deleted. Remove it from the server if you wish to delete this deck."); return; }

        string question = $"Do you want to delete '{ActiveDeck.DeckName}'? This action is permanent and cannot be undone. Selecting 'yes' will permanently remove thie deck from your device.";
        App.AskYesNo(question, DoDeleteDeck);
    }
    private void DoDeleteDeck(bool delete)
    {
        if (delete)
        {
            string msg = $"Deck '{ActiveDeck.DeckName}' has been deleted!";
            App.Account.DeleteDeck(ActiveDeck);
            Decklist newActive = App.Account.DeckLists[SettingsManager.Account.Settings.ActiveDeck];
            LoadSpecificDeck(App.Account.DeckLists, newActive);
            App.ShowMessage(msg);

        }
    }
    #endregion

    #region Misc Event Watching
    private void OnCodeSearchDisplay(bool isVisible)
    {
        if (isVisible) { DoFreeze(); } else { DoThaw(); }
    }
    #endregion

}
