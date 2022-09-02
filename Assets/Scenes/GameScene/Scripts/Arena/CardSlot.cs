using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameplay.Menus;
using UnityEngine.Events;

namespace Gameplay
{
    public class CardSlot : MonoBehaviour, iScaleCard
    {
        #region Interface
        [SerializeField]
        protected Vector2 _cardScale = new Vector2(8f, 8f);
        public Vector2 CardScale { get { return _cardScale; } }
        
        [SerializeField]
        protected string _SortLayer = "Card";
        public string SortLayer { get { return _SortLayer; } }
        #endregion
       

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
                if (GetComponent<RectTransform>()) { return GetComponent<RectTransform>().anchoredPosition; }
                return transform.position;
            }
        }
       
        protected List<CardObject> atSlot = new List<CardObject>();
        public CardLocation slotType;
        public CardFacing facing;
        public Orientation orientation;

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

        public bool Validate()
        {
            //if (ValidatePlayer()) return true;
            if (App.WhoAmI != Owner.userId) { return false; }
            if (GameManager.Instance.popupMenu.isOpen) { return false; }
            return true;
        }
       
        public Dictionary<string, UnityAction> ButtonCommands
        {
            get
            {
                return GetButtonCommands();
            }
        }
        protected virtual Dictionary<string, UnityAction> GetButtonCommands()
        {
            App.LogFatal($"{name} has no Commands Set!");
            return new Dictionary<string, UnityAction>();
        }
        #endregion

        protected void Awake()
        {
            
            name = $"{slotType}{index}";
            slotId = UniqueString.Create("csl", 5);
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
        }

        protected void Start()
        {
            
        }

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
        public void RemoveCard(GameCard card)
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
            TouchObject to = card.cardObject.touch;
            to.ClearClick();
            to.ClearHold();
            card.SetSlot(index);
            card.AllocateTo(slotType);

           
            DisplayCardObject(card);
            SetCommands(card);


        }

        protected virtual void SetCommands(GameCard card)
        {
            TouchObject to = card.cardObject.touch;
            to.OnClickEvent.AddListener(() => ClickCard(card));
            to.OnHoldEvent.AddListener(() => GameManager.Instance.DragCard(card, this));
        }

        public void ReAddCard(GameCard card)
        {
            DisplayCardObject(card);
        }
        protected virtual void DisplayCardObject(GameCard card)
        {
            CardObject c = card.cardObject;
            c.transform.SetParent(transform);
            card.rect.sizeDelta = rect.sizeDelta;
            if (!atSlot.Contains(c))
            {
                atSlot.Add(c);
            }
            c.transform.localPosition = new Vector2(0f, 0f);
            c.SetScale(CardScale);
            c.Flip(facing == CardFacing.FaceDown);
        }


        #endregion

       
        protected virtual void ClickCard(GameCard card)
        {
            GameManager.Instance.SelectCard(card);
        }

        #region Slot Menus
        public virtual void OpenSlotMenu()
        {
            if (Validate())
            {

                GameManager.Instance.popupMenu.LoadMenu(this);
            }
        }

        protected virtual void CloseSlotMenu()
        {
            GameManager.Instance.popupMenu.CloseMenu();
        }
        #endregion
        private void Update()
        {
            
        }
    }
}

