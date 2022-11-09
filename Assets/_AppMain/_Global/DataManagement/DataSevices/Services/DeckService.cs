using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Decks;
using UnityEngine.TextCore.LowLevel;

namespace Databases
{
    public class DeckService : DataService
    {
        public static readonly string LocalOwner = "local";

        public static readonly int CardLimit = 60;
        public static readonly int SpiritLimit = 20;
        public static readonly int CopyLimit = 3;

        public static readonly string DeckTable = "DeckDTO";
        public static readonly string DeckCardTable = "uDeckCardDTO";
        public static readonly string viewDecklist = "qDecklist";
        public static readonly string DeckListTable = "DeckListDTO";

        public static DeckDTO LoadDeck(string deckKey)
        {
            string query = $"deckKey = '{deckKey}'";
            DeckDTO deck = GetFirstWhere<DeckDTO>(DeckTable, query);
            return deck;
        }

        public static List<DeckDTO> GetDecksByOwner(string userKey = "")
        {
            if (string.IsNullOrWhiteSpace(userKey))
            {
                userKey = LocalOwner;
            }
            string query = $"owner = '{userKey}'";
            List<DeckDTO> list = GetAllWhere<DeckDTO>(DeckTable, query);
            return list;
        }

        public static List<qDeckList> LoadCards(string deckKey)
        {
            string query = $"deckKey = '{deckKey}'";
            List<qDeckList> cards = GetAllWhere<qDeckList>(viewDecklist, query);
            return cards;
        }

       
    }
}

