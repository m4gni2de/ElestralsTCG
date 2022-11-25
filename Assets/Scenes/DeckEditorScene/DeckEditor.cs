using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Decks;
using CardsUI.Filtering;
using static Decks.Decklist;
using System;
using PopupBox;
using UnityEditor.Rendering.LookDev;
using Newtonsoft.Json.Bson;
using Cards;

public class DeckEditor : ValidationObject
{
    #region Deck Rules
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
            return Instance.IsDirty;
        }
    }


    #endregion
    #region Properties
    public CardScroll deckScroll;
    public TMP_Dropdown deckSelector;

    private Canvas canvas;
    [SerializeField]
    private string canvasSortingLayer;
    #endregion

    #region Deck Management
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

    #region Deck Editing
    private Dictionary<string, int> _cardCounts = null;
    public Dictionary<string, int> CardCounts
    {
        get
        {
            _cardCounts ??= new Dictionary<string, int>();
            return _cardCounts;
        }
    }

    public List<CardStack> CardsWithSharedBase(Card card)
    {
        List<CardStack> cards = new List<CardStack>();
        List<string> altArts = card.AltArts;
        for (int i = 0; i < deckScroll.Cards.Count; i++)
        {
            CardStack stack = (CardStack)deckScroll.Cards[i];
            if (stack.ActiveCard != null)
            {
                if (altArts.Contains(stack.ActiveCard.cardData.cardKey))
                {
                    cards.Add(stack);
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
                TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData(Decklist[i].Name);
                options.Add(optionData);
            }
            return options;
        }
    }

    private bool _isDirty = false;
    public bool IsDirty
    {
        get { return _isDirty; }
    }
    #endregion


    #region Initialization
    
    private void Refresh()
    {
        deckSelector.ClearOptions();
        deckSelector.gameObject.SetActive(true);
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
    public void OpenDeckEditor()
    {
        deckScroll.Setup(false);
        deckScroll.Settings.stackDuplicates = true;
    }
    private void ReloadDeckEditor()
    {

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
        if (IsDirty)
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

    private void SaveDeckChanges(PopupResposne response)
    {

        switch (response)
        {
            case PopupResposne.Cancel:
                break;
            case PopupResposne.Yes:
                SaveActiveDeck();
                SetActiveDeck(Decklist[_pendingDeckIndex]);
                deckIndex = _pendingDeckIndex;
                break;
            case PopupResposne.No:
                SetActiveDeck(Decklist[_pendingDeckIndex]);
                deckIndex = _pendingDeckIndex;
                break;
        }

        _pendingDeckIndex = -1;

    }
   
    private void SetActiveDeck(Decklist deck)
    {
        CardStack.OnQuantityChanged -= SetCardQuantity;
        if (deckIndex < 0)
        {
            OpenDeckEditor();
        }
        else
        {
            ReloadDeckEditor();
        }
        ActiveDeck = deck;
        CardCounts.Clear();
        foreach (var item in deck.CardCounts)
        {
            CardCounts.Add(item.Key, item.Value);
        }
        deckScroll.LoadDeck(CardCounts);
        _isDirty = false;
        deckSelector.gameObject.SetActive(false);
        CardStack.OnQuantityChanged += SetCardQuantity;
    }

    public void SaveActiveDeck()
    {
        ActiveDeck.SetCardQuantities(CardCounts);
    }
    #endregion

    #region Validation/Saving
    public override bool Validate()
    {
        return true;
    }
    #endregion



    #region Card Quantity 
   
    /// <summary>
    /// The Up and Down Qty Buttons on the template card in the card scroll must call this
    /// </summary>
    /// <param name="setKey"></param>
    /// <param name="qty"></param>
    public void SetCardQuantity(CardView card, int qty)
    {
        
        string setKey = card.ActiveCard.cardData.cardKey;
        if (CardCounts.ContainsKey(setKey))
        {
            CardCounts[setKey] = qty;
        }
        else
        {
            CardCounts.Add(setKey, qty);
        }

        _isDirty = true;
        CheckCardConstraints((CardStack)card, qty);

    }

    private void CheckCardConstraints(CardStack card, int qty)
    {
        bool canAdd = true;
        bool canRemove = true;
        CardType type = card.ActiveCard.CardType;
        string cardKey = card.ActiveCard.cardData.cardKey;
        if (type == CardType.Spirit)
        {
            CheckSpiritConstraints(card);
            return;
        }
        if (DeckCardCount >= MaxDeckCount)
        {
            canAdd = false;
            canRemove = true;
            SetMainDeckButtons(canAdd, canRemove);
            return;
        }
        else
        {
            canAdd = true;
            canRemove = true;
            SetMainDeckButtons(canAdd, canRemove, card);
        }

        List<CardStack> alts = CardsWithSharedBase(card.ActiveCard);
        int altCount = 0;
        for (int i = 0; i < alts.Count; i++)
        {
            altCount += alts[i].quantity;
        }
        int totalCopies = qty + altCount;
        if (totalCopies >= CardService.DeckLimit(cardKey))
        {
            canAdd = false;
            canRemove = true;
            for (int i = 0; i < alts.Count; i++)
            {
                SetCardButtons(alts[i], canAdd, canRemove);
            }
        }
        else
        {
            canAdd = true;
            canRemove = totalCopies > 0;
            for (int i = 0; i < alts.Count; i++)
            {
                SetCardButtons(alts[i], canAdd, canRemove);
            }
        }

        SetCardButtons(card, canAdd, canRemove);
    }
    private void CheckSpiritConstraints(CardView card)
    {
        if (SpiritCount >= MaxSpiritCount) { SetSpiritButtons(false, true); }
        if (SpiritCount <= 0) { SetSpiritButtons(true, false); }
        if (SpiritCount > 0 && SpiritCount < MaxSpiritCount) { SetSpiritButtons(true, true); }
    }
    private void SetCardButtons(CardStack card, bool canAdd, bool canRemove)
    {
        card.ToggleUpButton(canAdd);
        card.ToggleDownButton(canRemove);
    }

    private void SetSpiritButtons(bool canAdd, bool canRemove)
    {
        List<CardStack> cards = new List<CardStack>();
        for (int i = 0; i < deckScroll.Cards.Count; i++)
        {
            CardStack card = (CardStack)deckScroll.Cards[i];
            if (card.ActiveCard != null && card.ActiveCard.CardType == CardType.Spirit)
            {
                card.ToggleUpButton(canAdd);
                card.ToggleDownButton(canRemove);
            }
        }
    }
    private void SetMainDeckButtons(bool canAdd, bool canRemove, CardStack exceptCard = null)
    {
        List<CardStack> cards = new List<CardStack>();
        for (int i = 0; i < deckScroll.Cards.Count; i++)
        {
            CardStack card = (CardStack)deckScroll.Cards[i];
            if (card.ActiveCard != null && card.ActiveCard.CardType != CardType.Spirit)
            {
                if (exceptCard != null && exceptCard != card)
                {
                    card.ToggleUpButton(canAdd);
                    card.ToggleDownButton(canRemove);
                }
                
            }
        }
    }

    
    #endregion
}
