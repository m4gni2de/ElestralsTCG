using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Databases;
using Cards;
using System;
using Gameplay.Decks;
using System.Threading.Tasks;

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

            public static DeckCard Empty
            {
                get
                {
                    return new DeckCard { key = "", cardType = CardType.None, copy = 1 };
                }
            }
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
        #region operators
        public static implicit operator Decklist(UploadedDeckDTO dto)
        {
            Decklist deck = new Decklist(dto.title, dto.deckKey);
            List<DeckCard> cards = new List<DeckCard>();
            for (int i = 0; i < dto.deck.Count; i++)
            {
                string cardKey = dto.deck[i];
                //DeckCard card = DeckCard.Empty;

                bool hasCard = false;
                int countOf = 1;
                for (int j = 0; j < cards.Count; j++)
                {
                    DeckCard d = cards[j];
                   
                    if (d.key.ToLower() == cardKey.ToLower())
                    {
                        hasCard = true;
                        countOf += 1;
                    }
                }

                DeckCard card = CardService.DeckCardFromDownload(cardKey);
                if (hasCard)
                {
                    card.copy = countOf;
                }
                cards.Add(card);
            }
            deck.Cards.AddRange(cards);
            return deck;
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
                };
                return dto;
            }
        }
        #endregion

        public string AsJson
        {
            get
            {
                return Cards.ToJson();
            }
        }

        #region Properties
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


        private List<DeckCard> _cards = null;
        public List<DeckCard> Cards { get { _cards ??= new List<DeckCard>(); return _cards; } }
        #region Properties
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

        private List<string> _spiritList = null;
        public List<string> SpiritList { get { _spiritList ??= new List<string>(); return _spiritList; } }

        private List<string> _cardList = null;
        public List<string> CardList
        {
            get
            {
                _cardList ??= new List<string>();
                return _cardList;
            }
        }

        private Dictionary<string, int> _cardCounts = null;
        public Dictionary<string, int> CardCounts
        {
            get
            {
                _cardCounts ??= new Dictionary<string, int>();
                return _cardCounts;
            }
        }
        
        

       

        public bool IsUploaded
        {
            get => !string.IsNullOrEmpty(UploadCode);
        }
        #endregion
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
                            d.AddCard(copy);
                        }
                    }
                    
                }
            }

            return list;
        }
       
        #region Empty Deck
        public static Decklist Empty(string owner, string key, string title)
        {
            return new Decklist(owner, key, title);
        }
        #endregion

        Decklist(string owner, string key, string name)
        {
            _key = key;
            _owner = owner;
            _whenCreated = DateTime.Now;
            _deckName = name;
            _uploadCode = "";

            SetZeroDTO(GetDTO);
        }
        public DeckCard AddCard(string setKey)
        {
            DeckCard card = DeckCard.Empty;
            card = CardService.DeckCardFromDownload(setKey);
            AddCard(card);
            return card;
        }
        public DeckCard AddCard(DeckCardDTO dto)
        {
            DeckCard card = DeckCard.Empty;
            card = CardService.DeckCardFromDTO(dto);
            AddCard(card);
            return card;
        }
        #endregion

        #region Initialization
        Decklist(string owner, string uploadKey)
        {
            _key = uploadKey;
            _owner = owner;
            _whenCreated = DateTime.Now;
            _deckName = $"{owner}'s Uploaded Deck";

            SetZeroDTO(GetDTO);
        }
        Decklist(DeckDTO deck, List<qDeckList> cards)
        {
            _deckName = deck.title;
            _key = deck.deckKey;
            _owner = deck.owner;
            _whenCreated = deck.whenCreated;
            _uploadCode = deck.uploadCode;

            for (int i = 0; i < cards.Count; i++)
            {
                for (int j = 0; j < cards[i].qty; j++)
                {
                    DeckCard c = NewCard(cards[i].cardKey, cards[i].cardClass, j);
                    AddCard(c);
                }
            }
            SetZeroDTO(GetDTO);
        }
        #endregion





        #region Adding Cards
        protected static DeckCard NewCard(string key, int cardType, int indexOfCopy)
        {
            return new DeckCard { key = key, cardType = (CardType)cardType, copy = indexOfCopy };
        }
       
       
        protected void AddCard(DeckCard c)
        {
            Cards.Add(c);
            ChangeCardQuantity(c.key, 1);

        }
        
        #endregion

        public async Task<string> DoUpload()
        {
            
            if (IsUploaded) { return UploadCode; }
            

            string upCode = UniqueString.Create("dk", 9);
            _uploadCode = upCode;
            SaveDeck();
            bool uploaded = await RemoteData.AddDeckToRemoteDB(this);
            if (uploaded) { return _uploadCode; }

            return "";

        }


        #region Deck Editing
       
        public void ChangeCardQuantity(string setKey, int changeVal)
        {
            if (changeVal > 0)
            {
                if (!CardCounts.ContainsKey(setKey))
                {
                    CardCounts.Add(setKey, changeVal);
                }
                else
                {
                    CardCounts[setKey] += changeVal;
                }
            }
            else if (changeVal < 0)
            {
                if (CardCounts.ContainsKey(setKey))
                {
                    int newQty = CardCounts[setKey] += changeVal;
                    if (newQty > 0) { CardCounts[setKey] = newQty; } else { CardCounts.Remove(setKey); }
                }
            }
        }

        
        public void SetCards(Dictionary<string, int> cardList)
        {
            _cardCounts = cardList;
            SaveDeck();
        }

        public void ReloadDeck()
        {
            Cards.Clear();
            CardCounts.Clear();
            

            string deckKey = _key;
            DeckDTO deck = UserService.LoadDeck(deckKey);
            List<DeckCardDTO> cards = UserService.LoadDecklist(deckKey);

            _deckName = deck.title;
            _key = deck.deckKey;
            _owner = deck.owner;
            _whenCreated = deck.whenCreated;
            _uploadCode = deck.uploadCode;
            SetZeroDTO(deck);

            for (int i = 0; i < cards.Count; i++)
            {
                for (int j = 0; j < cards[i].qty; j++)
                {
                    DeckCardDTO c = cards[i];
                    AddCard(c);
                }
            }
        }
        #endregion

        #region Saving
        public void SaveDeck()
        {
            DeckDTO dto = GetDTO;
            UserService.Save<DeckDTO>(dto, UserService.UserDeckTable, "deckKey", dto.deckKey);
            UserService.SaveDeckList(_key, GetDeckCards(CardCounts));
            ReloadDeck();
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
        #endregion

    }
}

