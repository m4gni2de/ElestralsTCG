using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameplay.Decks;
using System.Threading.Tasks;
using System;

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

        
        public List<CardSlot> ElestralSlots(bool includeEmpty)
        {
            List<CardSlot> list = new List<CardSlot>();
            for (int i = 0; i < cardSlots.Count; i++)
            {
                if (cardSlots[i].slotType == CardLocation.Elestral)
                {
                    if (!includeEmpty)
                    {
                        if (cardSlots[i].MainCard != null) { list.Add(cardSlots[i]); }
                    }
                    else
                    {
                        list.Add(cardSlots[i]);
                    }
                    
                }
            }
            return list;
        }
       
        public List<CardSlot> RuneSlots(bool includeEmpty)
        {
            List<CardSlot> list = new List<CardSlot>();
            for (int i = 0; i < cardSlots.Count; i++)
            {
                if (cardSlots[i].slotType == CardLocation.Rune)
                {
                    if (!includeEmpty)
                    {
                        if (cardSlots[i].MainCard != null) { list.Add(cardSlots[i]); }
                    }
                    else
                    {
                        list.Add(cardSlots[i]);
                    }
                        
                }
            }
            return list;
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
            List<CardSlot> slots = ElestralSlots(true);
            return GetSlotAt(slots, index, onlyOpenSlots);
        }
        public CardSlot RuneSlot(int index, bool onlyOpenSlots)
        {
            List<CardSlot> slots = RuneSlots(true);
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
                List<CardSlot> slots = ElestralSlots(false);

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
      
        public void Register()
        {
            
            int runeCount = 0;
            int elestralCount = 0;
            for (int i = 0; i < cardSlots.Count; i++)
            {
                if (cardSlots[i].slotType == CardLocation.Rune)
                {
                    runeCount += 1;
                    cardSlots[i].SetLocationName($"Rune Slot {runeCount}");
                }
                else if (cardSlots[i].slotType == CardLocation.Elestral)
                {
                    elestralCount += 1;
                    cardSlots[i].SetLocationName($"Elestral Slot {elestralCount}");
                }
                else
                {
                    string s = $"{cardSlots[i].slotType}";
                    cardSlots[i].SetLocationName(s);
                }

                cardSlots[i].SetIndex(baseIndex + i);
            }
            
        }

        public void SetPlayer(Player p)
        {
            _player = p;
            Register();
            for (int i = 0; i < cardSlots.Count; i++)
            {
                cardSlots[i].SetId(p.userId);
            }
            AllocateCards();
        }

        public void SetPlayer(Player p, string id)
        {
            fieldId = id;
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
            //NetworkPipeline.SpawnNewCard(card.NetworkId, slot.index);
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
            for (int i = 0; i < cardSlots.Count; i++)
            {
                if (card.rect.DoesIntersect(cardSlots[i].rect))
                {
                    if (cardSlots[i].ValidateCard(card))
                    {
                        SetSlot(true, cardSlots[i]);
                        card.cardObject.SetColor(Color.green);
                        isSelected = true;
                        break;
                    }
                    else
                    {
                        SetSlot(false, cardSlots[i]);
                        card.cardObject.SetColor(Color.red);
                        isSelected = true;
                        break;
                    }
                }

            }

            if (isSelected == false)
            {
               SetSlot();
               card.cardObject.SetColor(Color.white);
            }
            
        }
        
        #endregion
    }
}

