using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;
using CardsUI;
using Gameplay.GameCommands;

namespace Gameplay
{
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
    public enum CardMode
    {
        None = 0,
        Attack = 1,
        Defense = 2,
    }

    [System.Serializable]
    public class GameCard
    {
        public string name;
        public CardStats cardStats;
        #region Visual Info
        public struct VisualInfo
        {
            public Transform transform { get; set; }
            public Vector2 scale { get; set; }
            public string sortLayer { get; set; }
            public int childIndex { get; set; }
            public Vector2 position { get; set; }
            public bool isFaceUp { get; set; }
            public CardLocation location { get; set; }

           VisualInfo(GameCard card)
            {
                transform = card.cardObject.transform.parent;
                scale = card.cardObject.transform.localScale;
                sortLayer = card.cardObject.sp.SortLayerName;
                childIndex = card.cardObject.transform.GetSiblingIndex();
                position = card.cardObject.transform.localPosition;
                isFaceUp = card.IsFaceUp;
                location = card.location;
            }

            public static VisualInfo GetInfo(GameCard card)
            {
                return new VisualInfo(card);
            }
        }

        #region Visual Properties
        public VisualInfo m_VisualInfo
        {
            get
            {
                return VisualInfo.GetInfo(this);
            }
            set
            {

                SetCard(value);
            }
        }
        public bool IsFaceUp { get { return cardObject.IsFaceUp; } }

        protected void SetCard(VisualInfo value)
        {
            cardObject.SetAsChild(value.transform, value.scale, value.sortLayer, value.childIndex);
            cardObject.transform.localPosition = value.position;
            cardObject.Flip(!value.isFaceUp);

        }
        #endregion
        #endregion

        #region Static Constructors
        public static GameCard Spirit(CardData data, int copy)
        {
            Spirit spirit = new Spirit(data);
            return new GameCard(spirit, copy);
        }
        public static GameCard Elestral(ElestralData data, int copy)
        {
            Elestral card = new Elestral(data);
            return new GameCard(card, copy);
        }
        public static GameCard Rune(RuneData data, int copy)
        {
            Rune card = new Rune(data);
            return new GameCard(card, copy);
        }
        #endregion

        #region Properties
        private Card _card = null;
        public Card card { get { return _card; } }
        public CardLocation location;
        public CardMode mode;
        public int deckPosition;
        public string cardId;

        private int _slotIndex = -1;
        public int slotIndex { get { return _slotIndex; } }

       

        public CardView cardObject { get; set; }
        private RectTransform _rect = null;
        public RectTransform rect
        {
            get
            {
                if (cardObject == null) { return null; }
                if (_rect == null)
                {
                    _rect = cardObject.GetComponent<RectTransform>();
                }
                return _rect;
            }
        }

        private List<GameCard> _enchantingSpirits = null;
        public List<GameCard> EnchantingSpirits
        {
            get
            {
                _enchantingSpirits ??= new List<GameCard>();
                return _enchantingSpirits;
            }
        }
        #endregion

        #region Functions
        public Player Owner
        {
            get
            {
                for (int i = 0; i < GameManager.ActiveGame.players.Count; i++)
                {
                    Player p = GameManager.ActiveGame.players[i];
                    if (p.deck.Cards.Contains(this))
                    {
                        return p;
                    }
                }
                return null;
            }
        }

        private CardSlot _CurrentSlot = null;
        public CardSlot CurrentSlot
        {
            get { return _CurrentSlot; }
        }

        #endregion


       
        GameCard(Card data, int copy)
        {
            _card = data;
            location = CardLocation.Deck;
            cardId = UniqueString.GetShortId("car");
            name = $"{_card.cardData.cardName} - {copy}";
            cardStats = new CardStats(this);
        }

       
        public void SetObject(CardView obj)
        {
            //do something to tie the carddata of the Object to the GameCard
            cardObject = obj;
        }


        public void SetDeckPosition(int index)
        {
            deckPosition = index;
        }
        public void SetCardMode(CardMode cardMode)
        {
            mode = cardMode;
            if (CardType == CardType.Elestral)
            {
                cardObject.Rotate(mode == CardMode.Defense);
            }
           
        }
        

        #region Slot Management
        public void AllocateTo(CardSlot slot)
        {
            _CurrentSlot = slot;
            location = slot.slotType;
            _slotIndex = slot.index;
        }
       
        public void Enchant(GameCard card)
        {
            if (this.CardType == CardType.Spirit) { return; }
            if (card.CardType != CardType.Spirit) { return; }

            EnchantingSpirits.Add(card);
            
        }
        public void DisEnchant(GameCard card)
        {
            if (EnchantingSpirits.Contains(card))
            {
                EnchantingSpirits.Remove(card);
            }
        }

        public void RemoveFromSlot()
        {
            if (CurrentSlot == null) { return; }
            CurrentSlot.RemoveCard(this);
        }
        public void ReAddToSlot(bool reAddCommands)
        {
            if (CurrentSlot == null) { return; }
            CurrentSlot.ReAddCard(this, reAddCommands);
        }
        public void RefreshAtSlot()
        {
            if (CurrentSlot == null) { return; }
            CurrentSlot.RefreshAtSlot(this);
        }
       

        public void SelectCard(bool toggle)
        {
            cardObject.SelectCard(toggle);
        }
        #endregion

        #region Information/Stats Management
        public CardType CardType { get { return cardStats.cardType; } }
        public CardType DefaultCardType { get { return card.CardType; } }
        #endregion

        #region Card Actions
        //public void ChangeCardMode(CardMode cardMode)
        //{
        //    CardMode current = mode;
        //    if (cardMode == CardMode.None || cardMode == current) { return; }
        //    SetCardMode(cardMode);
        //    if (CardType == CardType.Elestral)
        //    {
        //        cardObject.Rotate(cardMode == CardMode.Defense);

        //    }
        //    if (CardType == CardType.Rune)
        //    {
        //        if (cardMode == CardMode.Attack)
        //        {

        //        }

        //    }
        //}
        #endregion

        #region Event Watching
        //protected void SetWatchers()
        //{
        //    Game.OnNewTargetParams += OnNewTargetParams;
        //}

        //private void OnNewTargetParams(TargetArgs obj)
        //{
        //    if (obj == null)
        //    {
        //        cardObject.touch.RemoveOverrideClick();
        //        cardObject.touch.CheckFreeze();
        //    }
        //    else
        //    {
        //        if (obj.Validate(this))
        //        {
        //            cardObject.touch.OverrideClick(() => obj.SelectTarget(this));
        //        }
        //        else
        //        {
        //            cardObject.touch.FreezeClick();
        //        }
        //    }
        //}

        //protected void RemoveWatchers()
        //{
        //    Game.OnNewTargetParams -= OnNewTargetParams;
        //}
        #endregion

    }


}

