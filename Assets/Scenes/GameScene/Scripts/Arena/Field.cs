using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameplay.Decks;
using System.Threading.Tasks;

namespace Gameplay
{
    public class Field : MonoBehaviour
    {
        [SerializeField]
        public string fieldId { get; set; }
        public int baseIndex;
        private SpriteRenderer spMat;

        public List<CardSlot> cardSlots = new List<CardSlot>();

        #region Player
        public Player _player { get; set; }
        protected GameDeck _deck { get { return _player.deck; } }
        #endregion

        #region Functions
        public CardSlot DeckSlot
        {
            get
            {
                for (int i = 0; i < cardSlots.Count; i++)
                {
                    if (cardSlots[i].slotType == CardLocation.Deck) { return cardSlots[i]; }
                }
                App.LogFatal("There is no Deck Slot marked.");
                return null;
            }
        }
        public CardSlot SpiritDeckSlot
        {
            get
            {
                for (int i = 0; i < cardSlots.Count; i++)
                {
                    if (cardSlots[i].slotType == CardLocation.SpiritDeck) { return cardSlots[i]; }
                }
                App.LogFatal("There is no Spirit Deck Slot marked.");
                return null;
            }
        }
        public CardSlot HandSlot
        {
            get
            {
                for (int i = 0; i < cardSlots.Count; i++)
                {
                    if (cardSlots[i].slotType == CardLocation.Hand) { return cardSlots[i]; }
                }
                App.LogFatal("There is no Hand Slot marked.");
                return null;
            }
        }

        public List<CardSlot> ElestralSlots
        {
            get
            {
                List<CardSlot> list = new List<CardSlot>();
                for (int i = 0; i < cardSlots.Count; i++)
                {
                    if (cardSlots[i].slotType == CardLocation.Elestral)
                    {
                        list.Add(cardSlots[i]);
                    }
                }
                return list;
            }
        }
        protected List<CardSlot> RuneSlots
        {
            get
            {
                List<CardSlot> list = new List<CardSlot>();
                for (int i = 0; i < cardSlots.Count; i++)
                {
                    if (cardSlots[i].slotType == CardLocation.Rune)
                    {
                        list.Add(cardSlots[i]);
                    }
                }
                return list;
            }
        }

        private CardSlot _UnderWorldSlot = null;
        public CardSlot UnderworldSlot
        {
            get
            {
                if (_UnderWorldSlot == null)
                {
                    bool hasSlot = false;
                    for (int i = 0; i < cardSlots.Count; i++)
                    {
                        if (cardSlots[i].slotType == CardLocation.Underworld) { _UnderWorldSlot =  cardSlots[i]; hasSlot = true; }
                    }
                    if (!hasSlot)
                    {
                        App.LogFatal("There is no Underworld Slot marked.");
                        return null;
                    }
                   
                }
                return _UnderWorldSlot;
               
            }
        }

        public CardSlot ByIndex(int index)
        {
            for (int i = 0; i < cardSlots.Count; i++)
            {
                if (cardSlots[i].index == index) { return cardSlots[i]; }
            }
            return null;
        }

        public CardSlot ElestralSlot(int index, bool onlyOpenSlots)
        {
            List<CardSlot> slots = ElestralSlots;
            return GetSlotAt(slots, index, onlyOpenSlots);
        }
        public CardSlot RuneSlot(int index, bool onlyOpenSlots)
        {
            List<CardSlot> slots = RuneSlots;
            return GetSlotAt(slots, index, onlyOpenSlots);
        }

        protected CardSlot GetSlotAt(List<CardSlot> slots, int index, bool onlyOpenSlots)
        {
            if (!onlyOpenSlots) { return slots[index]; }
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].cards.Count == 0) { return slots[i]; }
            }
            return null;
        }

        public CardSlot FirstOpenElestralSlot
        {
            get
            {
                List<CardSlot> slots = ElestralSlots;

                for (int i = 0; i < slots.Count; i++)
                {
                    if (slots[i].cards.Count == 0) { return slots[i]; }
                }
                return null;
            }
        }

        #endregion

        private void Awake()
        {

        }

        public string Register()
        {
            fieldId = UniqueString.GetShortId("fld");
            for (int i = 0; i < cardSlots.Count; i++)
            {
                cardSlots[i].SetIndex(baseIndex + i);
            }
            return fieldId;
        }
       
        public void SetPlayer(Player p)
        {
            _player = p;
            for (int i = 0; i < cardSlots.Count; i++)
            {
                cardSlots[i].SetId(p.userId);
            }
            AllocateCards();
        }
        private void AllocateCards()
        {
            Deck sp = _deck.SpiritDeck;
            for (int i = 0; i < sp.Cards.Count; i++)
            {
                GameCard g = sp.Cards[i];
                CardView co = SpawnCard(g, SpiritDeckSlot);
                g.SetObject(co);
                SpiritDeckSlot.AllocateTo(g);

            }

            Deck de = _deck.MainDeck;
            for (int i = 0; i < de.InOrder.Count; i++)
            {
                GameCard g = de.InOrder[i];
                CardView co = SpawnCard(g, DeckSlot);
                g.SetObject(co);
                DeckSlot.AllocateTo(g);

            }
            GameManager.Instance.ReadyPlayer(_player);
        }

       
        protected CardView SpawnCard(GameCard card, CardSlot slot)
        {
            GameObject go = Instantiate(GameManager.Instance.cardTemplate, transform).gameObject;
            CardView c = go.GetComponent<CardView>();
            bool displayBack = slot.facing == CardSlot.CardFacing.FaceDown;
            c.LoadCard(card.card);
            c.name = card.name;
            return c;
        }



        #region Card Slot Interactions
        private CardSlot _SelectedSlot = null;
        public CardSlot SelectedSlot { get { return _SelectedSlot; } set { _SelectedSlot = value; } }

        public void SetSlot(bool isValid = true, CardSlot slot = null)
        {
            if (slot == null)
            {
                if (_SelectedSlot != null)
                {
                    _SelectedSlot.ClearValidation();
                }
            }
            else
            {
                if (_SelectedSlot != null)
                {
                    if (slot != _SelectedSlot)
                    {
                        _SelectedSlot.ClearValidation();
                    }

                }
                slot.SetValidate(isValid);
            }

            SelectedSlot = slot;
        }
        public void ValidateSlots(GameCard card)
        {
            bool isSelected = false;
            int selected = -1;
            for (int i = 0; i < cardSlots.Count; i++)
            {
                if (card.rect.DoesIntersect(cardSlots[i].rect))
                {
                    if (cardSlots[i].ValidateCard(card))
                    {
                        SetSlot(true, cardSlots[i]);
                        isSelected = true;
                        break;
                    }
                    else
                    {
                        SetSlot(false, cardSlots[i]);
                        isSelected = true;
                        break;
                    }
                }

            }

            if (isSelected == false)
            {
               SetSlot();
            }
            
        }
        #endregion
    }
}

