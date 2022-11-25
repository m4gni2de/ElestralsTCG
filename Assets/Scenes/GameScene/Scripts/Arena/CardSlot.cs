using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameplay.Menus;
using UnityEngine.Events;
using Gameplay.Menus.Popup;
using System;
using UnityEngine.UI;
using static Gameplay.Menus.CardBrowseMenu;
using Gameplay.CardActions;
using Gameplay.Turns;
using UnityEngine.UIElements;

namespace Gameplay
{
    public class CardSlot : MonoBehaviour, iScaleCard
    {

        #region Enums
        public enum CardFacing
        {
            FaceUp = 0,
            FaceDown = 1,
            Both = 2
        }
        public enum Orientation
        {
            Vertical = 0,
            Horizontal = 1,
            Both = 2
        }

        #endregion

        #region Interface
        [SerializeField]
        protected Vector2 _cardScale = new Vector2(8f, 8f);
        public Vector2 CardScale { get { return _cardScale; } }
        
        [SerializeField]
        protected string _SortLayer = "Card";
        public string SortLayer { get { return _SortLayer; } }

        public void ClickSlot()
        {
            GameManager.Instance.cardSlotMenu.Close();
            if (GameManager.Instance.IsSelecting)
            {
                if (GameManager.Instance.currentSelector.TargetSlots.Contains(this))
                {
                    GameManager.Instance.currentSelector.SelectSlot(this);
                }
            }
            else
            {
                OpenPopMenu();
            }
        }
        #endregion

       
        #region Slot Info
        public GameCard MainCard { get; set; }
        public string SlotTitle
        {
            get
            {
                if (MainCard == null)
                {
                    string title = $"{Owner.userId}'s {slotType.ToString()}";
                    return title;
                }
                else
                {
                    return $"{Owner.userId}'s {MainCard.cardStats.title}";
                }
            }
        }
        public string SlotLocationName { get; private set; }
        public void SetLocationName(string st)
        {
            SlotLocationName = st;
            
        }

             
        protected virtual void SetMainCard(GameCard card)
        {
            App.LogFatal($"Card Slot {name} is not of iMainCard interface, so it cannot accept a MainCard.");
        }
        #endregion



        #region Properties
        public int index;
        private string _slotId = "";
        public string slotId
        {
            get { return _slotId; }
            set { _slotId = value; }
        }
        protected List<GameCard> _cards = null;
        public List<GameCard> cards { get { _cards ??= new List<GameCard>(); return _cards; } }
        public bool isBlocked { get; private set; }

        private SpriteRenderer _sp { get; set; }
        public Vector2 Position { get { return transform.position; } }
        protected Player _Owner = null;
        public Player Owner { get { if (_Owner == null) { _Owner = GameManager.Instance.arena.GetSlotOwner(this); } return _Owner; } }
        public bool IsYours { get { return Owner == GameManager.ActiveGame.You; } }

        public CardLocation slotType;
        public CardFacing facing;
        public Orientation orientation;
        protected int SpiritSortOrderBase = 0;
        protected int NonSpiritSortOrderBase = 0;
        private GameCard _SelectedCard = null;
        public GameCard SelectedCard
        {
            get
            {
                return _SelectedCard;
            }
            set
            {
                _SelectedCard = value;
                
            }
        }

        public RectTransform rect { get; set; }

        private Vector2 _SlotSize = Vector2.zero;
        public Vector2 SlotSize
        {
            get
            {
                if (_SlotSize == Vector2.zero) { _SlotSize = GetSlotSize(); }
                return _SlotSize;
            }
        }
        protected virtual Vector2 GetSlotSize() { return rect.sizeDelta; }
        #endregion

        #region Functions/Commands
        public bool IsOpen { get => GetIsOpen(); }
        protected virtual bool GetIsOpen() { return !isBlocked && MainCard == null; }
        public bool Validate { get { return GetClickValidation(); } }
        public bool ValidatePlayer()
        {
            Field f = GameManager.Instance.arena.GetPlayerField(GameManager.ActiveGame.You);
            return f.cardSlots.Contains(this);
        }
        protected virtual bool GetClickValidation()
        {
            //if (ValidatePlayer()) return true;
            //if (App.WhoAmI != Owner.userId) { return false; }
            //if (GameManager.Instance.popupMenu.isOpen) { return false; }
            return true;
        }

        protected virtual void SetSelectedCard(GameCard view = null)
        {
            
            if (view == null) 
            { 
                SelectedCard = null;
            }
            else
            {
                SelectedCard = view;
            }

            

        }
       
        public List<PopupCommand> ButtonCommands { get { return GetSlotCommands(); } }

        
        protected virtual List<PopupCommand> GetSlotCommands()
        {
            App.LogFatal($"{name} has no Commands Set!");
            return new List<PopupCommand>();
        }

        public CardBrowseMenu BrowseMenu { get { return GameManager.Instance.browseMenu; } }
        #endregion

       

        #region Initialization

        protected void Awake()
        {
            
            name = $"{slotType}{index}";
            rect = GetComponent<RectTransform>();
            GetSpriteRenderer();
            WakeSlot();

        }
        protected void Start()
        {
            StartSlot();
        }


        protected virtual void GetSpriteRenderer()
        {
            if (_sp == null)
            {
                if (GetComponentInChildren<SpriteRenderer>())
                {
                    _sp = GetComponentInChildren<SpriteRenderer>();
                }
                
            }

        }
        protected virtual void WakeSlot()
        {
           
        }
        protected virtual void StartSlot()
        {

        }



        public virtual void SetPlayer(Player owner, int count)
        {
            index = count;
            name = $"{slotType}{count}";
            string indexString = index.ToString();
            if (index < 10)
            {
                indexString = $"0{index}";
            }
            slotId = $"{owner.lobbyId}{indexString}";
        }
        public void SetIndex(int count)
        {
            index = count;
            name = $"{slotType}{count}";
            
        }
        public void SetId(string ownerId)
        {
            string indexString = index.ToString();
            if (index < 10)
            {
                indexString = $"0{index}";
            }
            slotId = $"{ownerId}{indexString}";
            
            
        }

       
        #endregion

        #region Drag Validation
        public virtual void SetValidate(bool isValid)
        {
            if (isValid)
            {
                _sp.color = new Color(Color.green.r, Color.green.g, Color.green.g, .45f);

            }
            else
            {
                _sp.color = new Color(Color.red.r, Color.red.g, Color.red.g, .45f);

            }
        }
        public virtual void ClearValidation()
        {
            _sp.color = Color.clear;

        }

        public virtual bool ValidateCard(GameCard card)
        {
            return false;
           
        }
        #endregion

        #region Drag Event Watchers
       
        #endregion

        #region Card Management
        public virtual void RemoveCard(GameCard card)
        {
            cards.Remove(card);
            TouchObject to = card.cardObject.touch;
            to.ClearClick();
            to.ClearHold();

        }

    public virtual void AllocateTo(GameCard card, bool sendToServer = true)
    {
       
        card.RemoveFromSlot();
        cards.Add(card);
        card.AllocateTo(this, sendToServer);

        DisplayCardObject(card);
        SetCommands(card);

        

    }
    
    
    public void RefreshAtSlot(GameCard card)
    {
        SetCommands(card);
    }
    protected virtual void SetCommands(GameCard card)
    {
        TouchObject to = card.cardObject.touch;
        to.AddClickListener(() => ClickCard(card));
            //if (IsYours)
            //{
            //    to.AddHoldListener(() => DragCard(card));
            //}
            to.AddHoldListener(() => DragCard(card));
        }

    protected virtual void DragCard(GameCard card)
    {
        GameManager.SelectedCard = card;
        GameManager.Instance.DragCard(card, this);
    }

    public void ReAddCard(GameCard card, bool reAddCommands)
    {
        if (reAddCommands)
        {
            SetCommands(card);
        }
        DisplayCardObject(card);
        NetworkPipeline.SendNewCardSlot(card.cardId, slotId, card.mode);
        
        
    }
    protected virtual void DisplayCardObject(GameCard card)
    {
        card.cardObject.Show();
        CardView c = card.cardObject;
        c.SetAsChild(transform, CardScale, SortLayer);
        card.rect.sizeDelta = rect.sizeDelta;
        c.transform.localPosition = new Vector2(0f, 0f);
        c.Flip(facing == CardFacing.FaceDown);
        c.Rotate(orientation == Orientation.Horizontal);
    }

        #endregion

        #region Slot Menus
       
        protected IEnumerator AwaitBrowseMenuClose()
        {
            yield return new WaitUntil(() => !GameManager.Instance.browseMenu.IsOpen);
        }
        protected virtual void ClickCard(GameCard card)
        {
            GameManager.SelectedCard = card;
        }
        

        public virtual void OpenPopMenu()
        {
            if (Validate)
            {
                GameManager.Instance.popupMenu.LoadMenu(this);
            }
        }

        public void CloseMenu(bool keepSelected = false)
        {
            ClosePopMenu(keepSelected);
        }
        protected virtual void ClosePopMenu(bool keepSelected = false)
        {
            GameManager.Instance.popupMenu.CloseMenu();
            if (!keepSelected)
            {
                GameManager.SelectedCard = null;
            }
            
        }
        #endregion
        private void Update()
        {
            
        }

        #region Base Commands
        protected virtual void Refresh()
        {
           
        }

        protected void CloseCommand(bool keepSelected = false)
        {
            ClosePopMenu(keepSelected);
        }

        #region Browse
        protected void BrowseCards(List<GameCard> cards, string title, bool faceUp, int minSelectable, int maxSelectable)
        {
            GameManager.Instance.browseMenu.LoadCards(cards, title, faceUp, minSelectable, maxSelectable);
            ClosePopMenu();
        }
        #endregion

        #region Manage
        protected void ManageCards(List<GameCard> cards, string title, bool faceUp, int minSelectable, int maxSelectable)
        {
            BrowseCards(cards, title, faceUp, minSelectable, maxSelectable);
            GameManager.Instance.browseMenu.OnClosed += AwaitManage;
            ClosePopMenu();
        }
        protected virtual void AwaitManage(BrowseArgs args)
        {
            GameManager.Instance.browseMenu.OnClosed -= AwaitManage;
        }

        #endregion

        #region Cast Commands

        public void BaseCastCommand()
        {
            StartCastCommand();
        }

        /// <summary>
        /// Start the action of Casting a card. Game calculates first available slot for casting this way, since only the card is selected to initiate the cast.
        /// </summary>
        #region Cast to Field
        protected void StartCastCommand()
        {
            int castCount = SelectedCard.card.SpiritsReq.Count;
            List<GameCard> toShow = Owner.gameField.SpiritDeckSlot.cards;

            string title = $"Select {CardUI.AnySpiritUnicode(castCount)}    to Cast {SelectedCard.card.cardData.cardName}";
            GameManager.Instance.browseMenu.LoadCards(toShow, title, true, castCount, castCount);
            GameManager.Instance.browseMenu.CastMode(SelectedCard);
            ClosePopMenu(true);
            GameManager.Instance.browseMenu.OnCastClose += CastToField;
        }
        protected virtual void CastToField(List<GameCard> selectedCards, CardMode cMode)
        {
            GameManager.Instance.browseMenu.OnCastClose -= CastToField;
            if (cMode == CardMode.None) { return; }

            Field f = GameManager.Instance.arena.GetPlayerField(Owner);
            List<GameCard> cardsList = new List<GameCard>();
            for (int i = 0; i < selectedCards.Count; i++)
            {
                cardsList.Add(selectedCards[i]);
            }
            GameCard Selected = SelectedCard;
            CardSlot slot = f.ElestralSlot(0, true);

            if (SelectedCard.CardType == CardType.Rune)
            {
                slot = f.RuneSlot(0, true);

                if (SelectedCard.cardStats.Tags.Contains(CardTag.Stadium))
                {
                    slot = f.StadiumSlot;
                }
                RuneCastClose(SelectedCard, slot, cardsList, cMode);

            }
            else
            {
                GameManager.Instance.Cast(Owner, Selected, cardsList, slot, cMode);
                Refresh();
            }



        }

        protected void RuneCastClose(GameCard Selected, CardSlot slot, List<GameCard> cardsList, CardMode cMode)
        {

            if (cMode == CardMode.Defense)
            {
                GameManager.Instance.SetCast(Owner, Selected, slot);
                Refresh();
                return;
            }
            else
            {

                if (Selected.cardStats.Tags.Contains(CardTag.Artifact))
                {
                    CastAction ac = CastAction.Cast(Owner, Selected, cardsList, slot, cMode);
                    TurnManager.SetCrafingAction(ac.ActionData);
                    ChooseEmpoweredElestral();
                }
                else
                {
                    GameManager.Instance.Cast(Owner, Selected, cardsList, slot, cMode);
                    Refresh();

                }
            }
        }

        #endregion


        #region Empowering
        protected void ChooseEmpoweredElestral()
        {

            List<CardSlot> targets = new List<CardSlot>();
            targets.AddRange(Owner.gameField.ElestralSlots(false));
            targets.AddRange(Owner.Opponent.gameField.ElestralSlots(false));

            SlotSelector sourceSelect = SlotSelector.Create("Select Elestral to Empower", "Empowered Elestral", targets, 1);
            GameManager.Instance.SetSelector(sourceSelect);
            sourceSelect.OnSelectionHandled += AwaitEmpowerSource;
            ClosePopMenu(true);
        }

       
        protected virtual void AwaitEmpowerSource(bool isConfirm, SlotSelector sel)
        {
            sel.OnSelectionHandled -= AwaitEmpowerSource;
           
            if (isConfirm)
            {
                
                EmpowerAction empower = CastAction.FromData(TurnManager.Instance.CraftingAction) + sel.SelectedSlots[0].MainCard;
                GameManager.Instance.DoCast(empower);
                sel.SelectedSlots[0].MainCard.SelectCard(false);
                TurnManager.SetCrafingAction();
                GameManager.Instance.SetSelector();
                Refresh();
            }
            else
            {
                GameCard source = TurnManager.Instance.CraftingAction.FindSourceCard();
                GameManager.Instance.SetSelector();
                TurnManager.SetCrafingAction();
                source.ReAddToSlot(false);
                Refresh();
                
            }
            

        }

        #endregion




        #endregion

        #endregion


        #region Networking
        /// <summary>
        /// This is called simply for organization while I am still determining to use this or not. That way, I know exactly where the call for a remote card being allocated comes from.
        /// </summary>
        /// <param name="card"></param>
        public void GetRemoteAllocateTo(GameCard card)
        {
            AllocateTo(card, false);
        }


        #endregion


       
    }
}

[System.Serializable]
#region Slot Data
public class CardSlotData
{
    public enum ZoneIndex
    {
        Stadium = 0,
        Elestral1 = 1,
        Elestral2 = 2,
        Elestral3 = 3,
        Elestral4 = 4,
        Underworld = 5,
        SpiritDeck = 6,
        Rune1 = 7,
        Rune2 = 8,
        Rune3 = 9,
        Rune4 = 10,
        Deck = 11,
        Hand = 12

    }

    public int index;
    public List<string> cards;
    public string slotId;
    public CardLocation slotType;
    public ZoneIndex zoneIndex;
    public enum CardLocation
    {
        removed = -1,
        Elestral = 0,
        Rune = 1,
        Stadium = 2,
        Underworld = 3,
        Deck = 4,
        SpiritDeck = 5,
        Hand = 6
    }



    public CardSlotData(int id)
    {
        cards = new List<string>();
        zoneIndex = (ZoneIndex)id;

        if (zoneIndex == ZoneIndex.Stadium)
        {
            slotType = CardLocation.Stadium;
        }
        this.index = id;
    }
    public void SetId(string id)
    {
        string indexString = index.ToString();
        if (index < 10)
        {
            indexString = $"0{index}";
        }
        this.slotId = $"{id}{indexString}";


    }

    public void SetOrder(string[] cardOrder)
    {
        cards.Clear();
        for (int i = 0; i < cardOrder.Length; i++)
        {
            cards.Add(cardOrder[i]);

        }
    }

    public void AddCard(string id)
    {
        if (!cards.Contains(id))
        {
            cards.Add(id);
        }
    }
    public void RemoveCard(string id)
    {
        if (cards.Contains(id))
        {
            cards.Remove(id);
        }
    }

    public void SetSlotType(int location)
    {
        slotType = (CardLocation)location;
    }
}
#endregion

