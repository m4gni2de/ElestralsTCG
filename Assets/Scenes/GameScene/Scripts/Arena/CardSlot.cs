using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameplay.Menus;
using UnityEngine.Events;
using Gameplay.Menus.Popup;
using System;
using UnityEngine.UI;

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

        private SpriteRenderer _sp { get; set; }
        public Vector2 Position
        {
            get
            {
                return transform.position;
            }
        }
       
        public CardLocation slotType;
        public CardFacing facing;
        public Orientation orientation;
        protected int SpiritSortOrderBase = 0;
        protected int NonSpiritSortOrderBase = 0;
        public GameCard SelectedCard { get; set; }

        public RectTransform rect { get; set; }
       
        #endregion

        #region Functions/Commands
        protected Player _Owner = null;
        protected Player Owner
        {
            get
            {
                if (_Owner == null) { _Owner = GameManager.Instance.arena.GetSlotOwner(this); }return _Owner;
            }
        }
        public bool ValidatePlayer()
        {
            Field f = GameManager.Instance.arena.GetPlayerField(GameManager.ActiveGame.You);
            return f.cardSlots.Contains(this);
        }

        public bool Validate
        {
            get
            {
                return GetClickValidation();
            }
        }
        protected virtual bool GetClickValidation()
        {
            //if (ValidatePlayer()) return true;
            //if (App.WhoAmI != Owner.userId) { return false; }
            if (GameManager.Instance.popupMenu.isOpen) { return false; }
            return true;
        }

        protected void SetSelectedCard(GameCard view = null)
        {
            if (view == null) { SelectedCard = null; } else { SelectedCard = view; }

        }
        protected virtual void ToggleSelect(bool isSelected)
        {

        }
       
        public List<PopupCommand> ButtonCommands
        {
            get
            {
                return GetSlotCommands();
            }
        }
        
        protected virtual List<PopupCommand> GetSlotCommands()
        {
            App.LogFatal($"{name} has no Commands Set!");
            return new List<PopupCommand>();
        }

        #endregion

        #region Initialization

        protected void Awake()
        {
            
            name = $"{slotType}{index}";
            rect = GetComponent<RectTransform>();
            GetSpriteRenderer();
            SetSlot();

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
        protected virtual void SetSlot()
        {
           
        }

        public void SetIndex(int newIndex)
        {
            index = newIndex;
            name = $"{slotType}{index}";
        }
        public void SetId(string ownerId)
        {
            slotId = $"{ownerId}-{name}";
        }

        protected void Start()
        {
            
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

       #region Card Management
    public virtual void RemoveCard(GameCard card)
    {
        cards.Remove(card);
        TouchObject to = card.cardObject.touch;
        to.ClearClick();
        to.ClearHold();

    }

    public virtual void AllocateTo(GameCard card)
    {
        card.RemoveFromSlot();
        cards.Add(card);
        card.AllocateTo(this);

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
        to.AddHoldListener(() => GameManager.Instance.DragCard(card, this));
    }

    public void ReAddCard(GameCard card, bool reAddCommands)
    {
        if (reAddCommands)
        {
            SetCommands(card);
        }
        DisplayCardObject(card);
    }
    protected virtual void DisplayCardObject(GameCard card)
    {
        CardView c = card.cardObject;
        c.SetAsChild(transform, CardScale, SortLayer);
        card.rect.sizeDelta = rect.sizeDelta;
        c.transform.localPosition = new Vector2(0f, 0f);
        c.Flip(facing == CardFacing.FaceDown);
        c.Rotate(orientation == Orientation.Horizontal);
    }

        #endregion

        #region Slot Menus
        protected virtual void ClickCard(GameCard card)
        {
            GameManager.Instance.SelectCard(card);
        }
        public void ClickSlot()
        {
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

        public virtual void OpenPopMenu()
        {
            if (Validate)
            {
                GameManager.Instance.popupMenu.LoadMenu(this);
            }
        }

        protected virtual void ClosePopMenu()
        {
            GameManager.Instance.popupMenu.CloseMenu();
        }
        #endregion
        private void Update()
        {
            
        }
    }
}

