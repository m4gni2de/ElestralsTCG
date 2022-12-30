using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Databases;
using Cards;
using System;
using Gameplay.Decks;
using System.Threading.Tasks;
using nsSettings;
using static Decks.Decklist;

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

        }
        public static implicit operator Decklist(DownloadedDeckDTO dto)
        {
            Decklist deck = new Decklist(dto);
            List<DeckCard> cards = DeckCard.FromCardList(dto.deck);
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
                    sDeckKey = _sideDeckKey,
                    isLocked = _isLocked.BoolToInt(),
                    
                    
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
                Decklist deck = new Decklist(dto[i]);
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
        protected List<DeckCardDTO> GetDeckCards(List<DeckCard> cardKeys)
        {
            List<DeckCardDTO> cards = new List<DeckCardDTO>();
            Dictionary<string, int> counts = new Dictionary<string, int>();

            for (int i = 0; i < cardKeys.Count; i++)
            {
                DeckCard key = cardKeys[i];
                DeckCardDTO newCard = new DeckCardDTO { deckKey = _key, setKey = key.key, qty = key.copy };
                cards.Add(newCard);

            }

            return cards;
        }


        public bool IsActiveDeck
        {
            get
            {

                int activeIndex = SettingsManager.Account.Settings.ActiveDeck;

                int deckIndex = App.Account.DeckIndex(DeckKey);
                return activeIndex == deckIndex;
            }
        }


        

        /// <summary>
        /// Return a list of DeckCards(CardKey, CardType, Qty) that corresponnds to 1 Object for each card in the deck. 
        /// The Copy property in this list is equal to the amount of copies the deck has of that card.
        /// </summary>
        public List<DeckCard> GetCardQuantities
        {
            get
            {
                Dictionary<DeckCard, int> result = new Dictionary<DeckCard, int>();
                List<DeckCard> cards = new List<DeckCard>();

                for (int i = 0; i < Cards.Count; i++)
                {
                    DeckCard c = Cards[i];
                    bool hasCard = false;

                    foreach (var item in result)
                    {
                        if (item.Key.key == Cards[i].key)
                        {
                            DeckCard inResult = item.Key;
                            result[inResult] += 1;
                            hasCard = true;
                            break;
                        }
                    }

                    if (!hasCard)
                    {
                        DeckCard dc = new DeckCard(c.key, c.cardType, 1);
                        result.Add(dc, 1);
                    }
                }


                foreach (var item in result)
                {
                    DeckCard d = item.Key;
                    d.copy = item.Value;
                    cards.Add(d);
                }

                return cards;
               
            }
        }

        public bool IsSavedLocally
        {
            get
            {
                return DeckKey.ToLower() != UploadCode.ToLower();
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
        private bool _isLocked;
        public bool isLocked { get { return _isLocked; } }

        private int _downloads = 0;
        public int downloads { get { return _downloads; } }

        public int CardCount
        {
            get
            {
                int count = 0;
                foreach (var item in GetCardQuantities)
                {
                    count += item.copy;
                }
                return count;
            }
        }


        public bool IsUploaded
        {
            get => !string.IsNullOrEmpty(UploadCode);
        }
        #endregion

        #region DeckCard Creating
        public DeckCard AddCard(string setKey)
        {
            DeckCard card = DeckCard.ByCardKey(setKey);
            AddCard(card);
            return card;
        }
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
            _isLocked = false;

            SetZeroDTO(GetDTO);
        }
        Decklist(DeckDTO dto)
        {
            _key = dto.deckKey;
            _owner = dto.owner;
            _whenCreated = dto.whenCreated;
            _deckName = dto.title;
            if (string.IsNullOrWhiteSpace(dto.uploadCode))
            {
                _uploadCode = "";
            }
            else
            {
                _uploadCode = dto.uploadCode;
            }
            
            _sideDeckKey = dto.sDeckKey;
            _isLocked = dto.isLocked.IntToBool();

            SetZeroDTO(GetDTO);
        }

        Decklist(string title, string deckKey)
        {
            _key = deckKey;
            _owner = title;
            _whenCreated = DateTime.Now;
            _deckName = title;
            _sideDeckKey = $"sd_{deckKey}";
            _isLocked = false;
            _uploadCode= "";
            
            SetZeroDTO(GetDTO);
        }
        Decklist(DownloadedDeckDTO dto)
        {
            _key = dto.deckKey;
            _owner = dto.owner;
            _whenCreated = dto.whenUpload;
            _deckName = dto.title;
            _sideDeckKey = "";
            _isLocked = true;
            _uploadCode = dto.deckKey;
            _downloads = dto.downloads;
            SetZeroDTO(GetDTO);
        }
        Decklist(DeckDTO deck, List<qDeckList> cards)
        {
            _deckName = deck.title;
            _key = deck.deckKey;
            _owner = deck.owner;
            _whenCreated = deck.whenCreated;
            if (string.IsNullOrWhiteSpace(deck.uploadCode))
            {
                _uploadCode = "";
            }
            else
            {
                _uploadCode = deck.uploadCode;
            }
            _sideDeckKey = deck.sDeckKey;
            _isLocked = deck.isLocked.IntToBool();

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
            if (string.IsNullOrWhiteSpace(deck.uploadCode))
            {
                _uploadCode = "";
            }
            else
            {
                _uploadCode = deck.uploadCode;
            }
            _sideDeckKey = deck.sDeckKey;
            _isLocked = deck.isLocked.IntToBool();
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
        }


        #endregion


        #region Editing/Saving Deck

        #region Property Editing
        public void Lock(bool doLock)
        {
            _isLocked = doLock;
        }
        public void SetUploadCode(string code)
        {
            _uploadCode = code;
        }
        #endregion
       
        public void Save(List<DeckCard> cardList = null)
        {
            if (!IsSavedLocally)
            {
                string prefix = UniqueString.RandomLetters(3);
                _key = UniqueString.CreateId(6, prefix);
                _uploadCode = "";
                _isLocked = false;
               
            }
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


        /// <summary>
        /// Use this only when adding a deck directly from an import
        /// </summary>
        public void AddAndSave()
        {
            DeckDTO dto = GetDTO;
            UserService.Save<DeckDTO>(dto, UserService.UserDeckTable, "deckKey", dto.deckKey);
            UserService.SaveDeckList(_key, GetDeckCards(GetCardQuantities));

            ReloadDeck();
        }

        
        protected async Task<string> DoUpload()
        {

            if (IsUploaded) { return UploadCode; }
            string upCode = UniqueString.Create("dk", 9);
            SetUploadCode(upCode);
            Lock(true);
            bool uploaded = await RemoteData.AddDeckToRemoteDB(this);
            if (uploaded) { return _uploadCode; }

            return "";

        }

        protected async Task<bool> DoRemove()
        {

            if (!IsUploaded) { return false; }
            bool removed = await RemoteData.RemoveDeckFromRemoteDB(this);
            if (removed)
            {
                SetUploadCode("");
                Lock(false);
                return true;
            }
            return false;

        }
        #endregion

        #region Uploading/Download Deck
        public async Task<bool> UploadDeck()
        {
            string code = await DoUpload();
            if (code.IsEmpty()) { SetUploadCode("") ; Lock(false); return false; }
            Save();
            return true;
        }

        public async Task<bool> RemoveUpload()
        {
            bool removed = await DoRemove();
            if (removed) { Save(); return true; } return false;
        }
        #endregion

    }
}

