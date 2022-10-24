using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Databases;
using Cards;
using System;
using Gameplay.Decks;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Decks
{
    [System.Serializable]
    public class Decklist 
    {
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

        private bool _isDirty = false;
        public bool IsDirty
        {
            get => _isDirty;
        }

        private List<DeckCard> _cards = null;
        public List<DeckCard> Cards { get { _cards ??= new List<DeckCard>(); return _cards; } }
        #region Properties
        private string _deckName;
        public string Name { get { return _deckName; } }

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

        
        protected DeckDTO GetDTO
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

        public bool IsUploaded
        {
            get => !string.IsNullOrEmpty(UploadCode);
        }
        #endregion
        #endregion

        #region Static Functions
        
        public static Decklist Load(string deckKey)
        {
            DeckDTO dto = DeckService.LoadDeck(deckKey);
            List<qDeckList> cards = DeckService.LoadCards(deckKey);
            return new Decklist(dto, cards);

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
        public static Decklist Load(DeckDTO deck)
        {
            return Load(deck.deckKey);
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


        Decklist(string owner, string uploadKey)
        {
            _key = uploadKey;
            _owner = owner;
            _whenCreated = DateTime.Now;
            _deckName = $"{owner}'s Uploaded Deck";
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
        }


        public void Save()
        {
            DeckDTO dto = GetDTO;
            DeckService.Save<DeckDTO>(dto, DeckService.DeckTable, "deckKey", dto.deckKey);
        }

        #region Adding Cards
        protected static DeckCard NewCard(string key, int cardType, int indexOfCopy)
        {
            return new DeckCard { key = key, cardType = (CardType)cardType, copy = indexOfCopy };
        }
       
       
        protected void AddCard(DeckCard c)
        {
            Cards.Add(c);
        }
        #endregion

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

    }
}

