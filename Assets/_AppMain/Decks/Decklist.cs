using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Databases;
using Cards;
using System;
using Gameplay.Decks;
using System.Threading.Tasks;
using nsSettings;

namespace Decks
{
    [System.Serializable]
    public class Decklist : iDto<DeckDTO>
    {
        #region Deck Card
        [System.Serializable]
        public class DeckCard
        {
            public string key;
            public CardType cardType;
            public int copy;

            #region Initialization
            public static DeckCard Empty
            {
                get
                {
                    return new DeckCard("", CardType.None, 1);
                }
            }

            public DeckCard(string cardKey, CardType cType, int copyIndex)
            {
                key = cardKey;
                cardType = cType;
                copy = copyIndex;
            }

            public static DeckCard FromDTO(DeckCardDTO dto)
            {

                return CardService.DeckCardFromDTO(dto);
            }
            public static DeckCard ByCardKey(string key)
            {

                return CardService.DeckCardFromDownload(key);
            }
            #endregion

            #region Operators
            public static List<DeckCard> FromCardList(List<string> cardKeys)
            {
                List<DeckCard> cards = new List<DeckCard>();
                Dictionary<string, int> counts = new Dictionary<string, int>();

                for (int i = 0; i < cardKeys.Count; i++)
                {
                    string key = cardKeys[i];
                    if (!counts.ContainsKey(key))
                    {
                        counts.Add(key, 1);
                    }
                    else
                    {
                        counts[key] += 1;
                    }
                }

                foreach (var item in counts)
                {
                    int qty = item.Value;
                    string cardKey = item.Key;

                    for (int i = 1; i <= qty; i++)
                    {
                        DeckCard card = DeckCard.ByCardKey(cardKey);
                        card.copy = i;
                        cards.Add(card);
                    }
                }

                return cards;
            }
            #endregion

        }

        public bool ContainsCard(string cardKey, out DeckCard card)
        {
            for (int i = 0; i < Cards.Count; i++)
            {
                if (Cards[i].key.ToLower() == cardKey.ToLower()) { card = Cards[i]; return true; }
            }
            card = DeckCard.Empty;
            return false;
        }
        private List<DeckCard> CopiesOfCard(string cardKey)
        {
            List<DeckCard> cards = new List<DeckCard>();
            for (int i = 0; i < Cards.Count; i++)
            {
                if (Cards[i].key.ToLower() == cardKey.ToLower())
                {
                    cards.Add(Cards[i]);
                }
            }
            return cards;
        }

        
        #endregion

        #region Operators
        public static implicit operator Decklist(UploadedDeckDTO dto)
        {
            Decklist deck = new Decklist(dto.title, dto.deckKey);
            List<DeckCard> cards = DeckCard.FromCardList(dto.deck);
            deck.Cards.AddRange(cards);
            return deck;


            //List<DeckCard> cards = new List<DeckCard>();
            //for (int i = 0; i < dto.deck.Count; i++)
            //{
            //    string cardKey = dto.deck[i];
            //    //DeckCard card = DeckCard.Empty;

            //    bool hasCard = false;
            //    int countOf = 1;
            //    for (int j = 0; j < cards.Count; j++)
            //    {
            //        DeckCard d = cards[j];

            //        if (d.key.ToLower() == cardKey.ToLower())
            //        {
            //            hasCard = true;
            //            countOf += 1;
            //        }
            //    }

            //    DeckCard card = CardService.DeckCardFromDownload(cardKey);
            //    if (hasCard)
            //    {
            //        card.copy = countOf;
            //    }
            //    cards.Add(card);
            //}

        }
        #endregion
       
        #region Interface
        private DeckDTO _zeroDto = null;
        public DeckDTO ZeroDTO
        {
            get
            {
                return _zeroDto;
            }
        }
        public void SetZeroDTO(DeckDTO dto)
        {
            _zeroDto = dto;
        }
        public DeckDTO GetDTO
        {
            get
            {
                DeckDTO dto = new DeckDTO
                {
                    title = _deckName,
                    deckKey = _key,
                    owner = _owner,
                    whenCreated = _whenCreated,
                    uploadCode = _uploadCode,
                    sDeckKey = _sideDeckKey,
                };
                return dto;
            }
        }
        #endregion
      
        #region Static Functions

        public static Decklist Load(string deckKey)
        {
            DeckDTO deck = DeckService.LoadDeck(deckKey);
            List<qDeckList> cards = DeckService.LoadCards(deckKey);
            return new Decklist(deck, cards);

        }

        public static List<Decklist> LoadAllLocalDecks(List<DeckDTO> dto, List<DeckCardDTO> cards)
        {
            List<Decklist> list = new List<Decklist>();
            for (int i = 0; i < dto.Count; i++)
            {
                Decklist deck = new Decklist(dto[i].owner, dto[i].deckKey, dto[i].title);
                list.Add(deck);
            }

            for (int i = 0; i < list.Count; i++)
            {
                Decklist d = list[i];
                for (int j = 0; j < cards.Count; j++)
                {
                    DeckCardDTO card = cards[j];
                    int qty = card.qty;
                    if (card.deckKey == d.DeckKey)
                    {
                        for (int c = 1; c <= qty; c++)
                        {
                            DeckCardDTO copy = new DeckCardDTO { deckKey = card.deckKey, setKey = card.setKey, qty = c };
                            DeckCard dCard = DeckCard.FromDTO(copy);
                            d.AddCard(dCard);
                        }
                    }

                }
            }

            return list;
        }

        #region Empty Deck
       
        #endregion
        #endregion

        #region Functions
        public string GetCardList
        {
            get
            {
                List<string> list = new List<string>();
                for (int i = 0; i < Cards.Count; i++)
                {
                    list.Add(Cards[i].key);
                }

                return list.ToJson();
            }
        }

        protected List<DeckCardDTO> GetDeckCards(Dictionary<string, int> cardList)
        {
            List<DeckCardDTO> list = new List<DeckCardDTO>();
            foreach (var item in cardList)
            {
                DeckCardDTO dto = new DeckCardDTO { deckKey = _key, setKey = item.Key, qty = item.Value };
                list.Add(dto);
            }
            return list;
        }

      
        public bool IsActiveDeck
        {
            get
            {

                int activeIndex = SettingsManager.Account.Settings.ActiveDeck;
                return activeIndex.ToString() == _key;
            }
        }


        public Dictionary<string, int> Quantities
        {
            get
            {
                Dictionary<string, int> result = new Dictionary<string, int>();
                for (int i = 0; i < Cards.Count; i++)
                {
                    DeckCard c = Cards[i];
                    if (!result.ContainsKey(c.key))
                    {
                        result.Add(c.key, 1);
                    }
                    else
                    {
                        result[c.key] += 1;
                    }
                }
                return result;
            }
        }

        public Dictionary<string, int> SideDeckQuantities
        {
            get
            {
                Dictionary<string, int> result = new Dictionary<string, int>();
                for (int i = 0; i < SideDeck.Count; i++)
                {
                    DeckCard c = Cards[i];
                    if (!result.ContainsKey(c.key))
                    {
                        result.Add(c.key, 1);
                    }
                    else
                    {
                        result[c.key] += 1;
                    }
                }
                return result;
            }
        }
        #endregion

        #region Properties


        #region Card Lists
        private List<DeckCard> _cards = null;
        public List<DeckCard> Cards { get { _cards ??= new List<DeckCard>(); return _cards; } }

        private List<DeckCard> _sideDeck = null;
        public List<DeckCard> SideDeck
        {
            get
            {
                _sideDeck ??= new List<DeckCard>();
                return _sideDeck;
            }
        }

        private List<string> _cardList = null;
        public List<string> CardList
        {
            get
            {
                _cardList ??= new List<string>();
                return _cardList;
            }
        }
        #endregion



        private string _deckName;
        public string DeckName { get { return _deckName; } set { _deckName = value; } }

        private string _key;
        public string DeckKey { get { return _key; } }

        private DateTime _whenCreated = DateTime.MinValue;
        public DateTime WhenCreated { get {return _whenCreated; } }

        private string _owner;
        public string Owner { get { return _owner; } }

        private string _uploadCode;
        public string UploadCode { get { return _uploadCode; } }
        private string _sideDeckKey;
        public string sideDeckKey { get { return _sideDeckKey; } }

       




        public bool IsUploaded
        {
            get => !string.IsNullOrEmpty(UploadCode);
        }
        #endregion

        #region DeckCard Creating
        public DeckCard AddCard(string setKey)
        {
            //DeckCard card = DeckCard.Empty;
            //card = CardService.DeckCardFromDownload(setKey);
            DeckCard card = DeckCard.ByCardKey(setKey);
            AddCard(card);
            return card;
        }
        //public DeckCard AddCard(DeckCardDTO dto)
        //{
        //    //DeckCard card = DeckCard.Empty;
        //    //card = CardService.DeckCardFromDTO(dto);
        //    DeckCard card = DeckCard.FromDTO(dto);
        //    AddCard(card);
        //    return card;
        //}
        #endregion

        #region Initialization

        #region New Deck
        public static Decklist Empty(string owner, string key, string title)
        {
            return new Decklist(owner, key, title);
        }
        Decklist(string owner, string key, string name)
        {
            _key = key;
            _owner = owner;
            _whenCreated = DateTime.Now;
            _deckName = name;
            _uploadCode = "";
            _sideDeckKey = $"sd_{key}";

            SetZeroDTO(GetDTO);
        }

        Decklist(string owner, string uploadKey)
        {
            _key = uploadKey;
            _owner = owner;
            _whenCreated = DateTime.Now;
            _deckName = $"{owner}'s Uploaded Deck";
            _sideDeckKey = "";

            SetZeroDTO(GetDTO);
        }
        Decklist(DeckDTO deck, List<qDeckList> cards)
        {
            _deckName = deck.title;
            _key = deck.deckKey;
            _owner = deck.owner;
            _whenCreated = deck.whenCreated;
            _uploadCode = deck.uploadCode;
            _sideDeckKey = deck.sDeckKey;

            SetZeroDTO(GetDTO);

            for (int i = 0; i < cards.Count; i++)
            {
                for (int j = 0; j < cards[i].qty; j++)
                {
                    //DeckCard c = NewCard(cards[i].cardKey, cards[i].cardClass, j);
                    DeckCard c = new DeckCard(cards[i].cardKey, (CardType)cards[i].cardClass, j);
                    AddCard(c);
                }
            }

            GetSideDeck(deck.sDeckKey);
        }
        #endregion


        #region Side Deck
        private void GetSideDeck(string sKey)
        {
            SideDeck.Clear();

            if (string.IsNullOrWhiteSpace(sKey)) { return; }
            List<DeckCardDTO> sideDeck = UserService.LoadDecklist(sKey);
            for (int i = 0; i < sideDeck.Count; i++)
            {
                DeckCardDTO dto = sideDeck[i];
                DeckCard c = DeckCard.FromDTO(dto);
                SideDeck.Add(c);
            }
        }
        #endregion

        public void ReloadDeck()
        {
            Cards.Clear();


            string deckKey = _key;
            DeckDTO deck = UserService.LoadDeck(deckKey);
            List<DeckCardDTO> cards = UserService.LoadDecklist(deckKey);

            _deckName = deck.title;
            _key = deck.deckKey;
            _owner = deck.owner;
            _whenCreated = deck.whenCreated;
            _uploadCode = deck.uploadCode;
            _sideDeckKey = deck.sDeckKey;
            SetZeroDTO(deck);

            for (int i = 0; i < cards.Count; i++)
            {
                for (int j = 0; j < cards[i].qty; j++)
                {
                    DeckCardDTO c = cards[i];
                    DeckCard card = DeckCard.FromDTO(c);
                    AddCard(card);
                }
            }
        }
        
       
        protected void AddCard(DeckCard c)
        {
            Cards.Add(c);
            //ChangeCardQuantity(c.key, 1);

        }

        //public void ChangeCardQuantity(string setKey, int changeVal)
        //{
        //    if (changeVal > 0)
        //    {
        //        if (!CardCounts.ContainsKey(setKey))
        //        {
        //            CardCounts.Add(setKey, changeVal);
        //        }
        //        else
        //        {
        //            CardCounts[setKey] += changeVal;
        //        }
        //    }
        //    else if (changeVal < 0)
        //    {
        //        if (CardCounts.ContainsKey(setKey))
        //        {
        //            int newQty = CardCounts[setKey] += changeVal;
        //            if (newQty > 0) { CardCounts[setKey] = newQty; } else { CardCounts.Remove(setKey); }
        //        }
        //    }
        //}

        #endregion


        #region Editing/Saving Deck
        public void Save(Dictionary<string, int> cardList = null)
        {
            if (this.IsDirty())
            {
                DeckDTO dto = GetDTO;
                UserService.Save<DeckDTO>(dto, UserService.UserDeckTable, "deckKey", dto.deckKey);
            }
            if (cardList != null)
            {
                UserService.SaveDeckList(_key, GetDeckCards(cardList));
            }

            ReloadDeck();

        }
       

        public async Task<string> DoUpload()
        {

            if (IsUploaded) { return UploadCode; }


            string upCode = UniqueString.Create("dk", 9);
            _uploadCode = upCode;
            Save();
            bool uploaded = await RemoteData.AddDeckToRemoteDB(this);
            if (uploaded) { return _uploadCode; }

            return "";

        }
        #endregion

    }
}

