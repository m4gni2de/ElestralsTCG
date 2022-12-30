using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameplay.Decks;
using System.Threading.Tasks;
using System;
using TMPro;

namespace Gameplay
{
    public class Field : MonoBehaviour
    {
        #region Properties
        public string fieldId { get; set; }
        public int baseIndex;
        [SerializeField] private SpriteDisplay spMat;

        public List<CardSlot> cardSlots = new List<CardSlot>();
        

        #region Player
        public Player _player { get; set; }
        protected GameDeck _deck { get { return _player.deck; } }

        #region Playmatt
        private static readonly string PlayMattAsset = "playmatt";
        private static readonly string PlaymattFallback = "playmatt0";
        public async void SetPlaymatt(int matIndex)
        {
            string assetString = $"{PlayMattAsset}{matIndex}";
            Sprite sp = await AssetPipeline.ByKeyAsync<Sprite>(assetString, PlaymattFallback);
            if (sp != null)
            {
                spMat.SetSprite(sp);
            }
        }
        #endregion

            #endregion

            #endregion


            #region Functions

            #region Explicit Slots
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

        public CardSlot StadiumSlot
        {
            get
            {
                for (int i = 0; i < cardSlots.Count; i++)
                {
                    if (cardSlots[i].slotType == CardLocation.Stadium) { return cardSlots[i]; }
                }
                App.LogFatal("There is no Stadium Slot marked.");
                return null;
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
                        if (cardSlots[i].slotType == CardLocation.Underworld) { _UnderWorldSlot = cardSlots[i]; hasSlot = true; }
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
        #endregion

        #region Slot Searching
        public List<CardSlot> SlotsOfTypes(params CardLocation[] locs)
        {
            List<CardSlot> slots = new List<CardSlot>();
            for (int i = 0; i < locs.Length; i++)
            {
                foreach (var item in cardSlots)
                {
                    if (item.slotType == locs[i])
                    {
                        if (!slots.Contains(item))
                        {
                            slots.Add(item);
                        }
                       
                    }
                }
            }

            return slots;
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
        

       

        public CardSlot SlotById(string id)
        {
            for (int i = 0; i < cardSlots.Count; i++)
            {
                if (cardSlots[i].slotId.ToLower() == id.ToLower())
                {
                    return cardSlots[i];
                    
                }
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

        public List<CardSlot> ValidSlots(GameCard card)
        {
            List<CardSlot> slots = new List<CardSlot>();
            for (int i = 0; i < cardSlots.Count; i++)
            {
                if (cardSlots[i].ValidateCard(card))
                {
                    slots.Add(cardSlots[i]);
                }
            }
            return slots;
        }

        #endregion

        #region Field Info
        public int OpenRuneSlots()
        {
            int count = 0;
            List<CardSlot> slots = RuneSlots(true);
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].IsOpen) { count += 1; }
            }
            return count;
        }
        public int OpenElestralSlots()
        {
            int count = 0;
            List<CardSlot> slots = ElestralSlots(true);
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].IsOpen) { count += 1; }
            }
            return count;
        }
        #endregion

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

                cardSlots[i].SetPlayer(_player, i);
            }
            
        }

        public void SetPlayer(Player p)
        {
            _player = p;
            fieldId = p.lobbyId.ToString();
            //name = _player.lobbyId + "_Field";
            name = _player.userId + "_Field";
            SetPlaymatt(p.playmatt);

            Register();

            
        }

        
        public void AllocateCards(bool sendToServer = true)
        {
            Deck sp = _deck.SpiritDeck;
            for (int i = 0; i < sp.Cards.Count; i++)
            {
                GameCard g = sp.Cards[i];
                CardView co = SpawnCard(g, SpiritDeckSlot);
                g.SetObject(co);
                SpiritDeckSlot.AllocateTo(g, sendToServer);
               
            }

            Deck de = _deck.MainDeck;
            for (int i = 0; i < de.InOrder.Count; i++)
            {
                GameCard g = de.InOrder[i];
                CardView co = SpawnCard(g, DeckSlot);
                g.SetObject(co);
                DeckSlot.AllocateTo(g, sendToServer);

            }

            GameManager.Instance.ReadyPlayer(_player);
        }

        //public void SpawnCard(GameCard card)
        //{
        //    if (card.CardType == CardType.Spirit)
        //    {
        //        CardView co = SpawnCard(card, SpiritDeckSlot);
        //        card.SetObject(co);
        //        SpiritDeckSlot.AllocateTo(card);
        //    }
        //    else
        //    {
        //        CardView co = SpawnCard(card, DeckSlot);
        //        card.SetObject(co);
        //        DeckSlot.AllocateTo(card);
        //    }
        //}



        protected CardView SpawnCard(GameCard card, CardSlot slot)
        {
            bool displayBack = slot.facing == CardSlot.CardFacing.FaceDown;
            Sprite sleeveSp = _player.SleevesSp;
            CardView c = CardFactory.CreateCard(transform, card.card, displayBack);
            c.ChangeSleeves(sleeveSp);
            c.name = card.cardName;
            c.gameObject.AddComponent<NetworkCard>();
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
                        card.cardObject.MaskCard(Color.green);
                        isSelected = true;
                        break;
                    }
                    else
                    {
                        SetSlot(false, cardSlots[i]);
                        card.cardObject.MaskCard(Color.red);
                        isSelected = true;
                        break;
                    }
                }

            }

            if (isSelected == false)
            {
               SetSlot();
               card.cardObject.ResetColors();
            }
            
        }
        
        #endregion
    }
}

