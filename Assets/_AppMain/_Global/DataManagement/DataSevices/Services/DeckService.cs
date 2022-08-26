using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Decks;


namespace Databases
{
    public class DeckService : DataService
    {
        public static readonly int CardLimit = 60;
        public static readonly int SpiritLimit = 20;
        public static readonly int CopyLimit = 3;

        public static readonly string DeckTable = "uDeckDTO";
        public static readonly string DeckCardTable = "uDeckCardDTO";
        public static readonly string viewDecklist = "qDecklist";

        public static DeckDTO LoadDeck(string deckKey)
        {
            string query = $"deckKey = '{deckKey}'";
            DeckDTO deck = GetFirstWhere<DeckDTO>(DeckTable, query);
            return deck;
        }

        public static List<DeckDTO> GetUserDecklist(string userKey)
        {
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

