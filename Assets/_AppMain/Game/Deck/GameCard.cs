using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;
using CardsUI;
using Gameplay.GameCommands;
using static Gameplay.GameCommands.MoveCommand;

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

    public class GameCard
    {
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
                scale = card.cardObject.Container.transform.localScale;
                sortLayer = card.cardObject.CardImageSp.sortingLayerName;
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
        public bool IsFaceUp { get { return cardObject.CardBack.activeSelf == false; } }

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
        public int deckPosition;
        public string cardId;

        private int _slotIndex = -1;
        public int slotIndex { get { return _slotIndex; } }

        public string name;

        public CardObject cardObject { get; set; }
        public CardView cardView { get; set; }
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

        public CardSlot CurrentSlot
        {
            get
            {
                Field f = GameManager.Instance.arena.GetPlayerField(Owner);
                return f.ByIndex(slotIndex);
            }
        }

        #endregion

        GameCard(Card data, int copy)
        {
            _card = data;
            location = CardLocation.Deck;
            cardId = UniqueString.GetTempId("car");
            name = $"{_card.cardData.cardName} - {copy}";
        }

        public void SetObject(CardObject obj)
        {
            //do something to tie the carddata of the Object to the GameCard
            cardObject = obj;
        }

       
        public void SetDeckPosition(int index)
        {
            deckPosition = index;
        }

        #region Slot Management
        public void SetSlot(int index)
        {
            _slotIndex = index;
        }

        public void AllocateTo(CardLocation loc)
        {
            location = loc;
        }

        public void RemoveFromSlot()
        {
            if (CurrentSlot == null) { return; }
            CurrentSlot.RemoveCard(this);
        }
        public void ReAddToSlot()
        {
            if (CurrentSlot == null) { return; }
            CurrentSlot.ReAddCard(this);
        }
       
        #endregion

        #region Command Management
        
        #endregion

    }


}

