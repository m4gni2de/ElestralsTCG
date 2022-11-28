using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Users;



namespace Databases
{
    public class UserService : PlayerService
    {
        protected static readonly string UserTable = "uUserDTO";
        protected static readonly string KeyColumn = "userKey";
        protected static readonly string DeckCardTable = "DeckCardDTO";
        public static readonly string UserDeckTable = "uDeckDTO";

        public static UserDTO CreateUser(string username)
        {
            UserDTO dto = new UserDTO
            {
                userKey = UniqueString.Create("usr", 7),
                username = username,
                whenCreated = System.DateTime.Now,
                email = ""
            };

            

            SaveUser(dto);
            return dto;
        }

        public static UserDTO LoadUser(string userKey)
        {
            UserDTO dto = ByKey<UserDTO>(UserTable, KeyColumn, userKey);
            if (dto != null)
            {
                return dto;
            }
            App.LogFatal("User with that key does not exist");
            return null;
        }

        public static bool AccountExists(out UserDTO dto)
        {
            
            List<UserDTO> list = GetAll<UserDTO>(UserTable);
            if (list.Count > 0) { dto = list[0]; } else { dto = null; }
            return list.Count > 0;
        }

        public static bool AccountExists()
        {

            List<UserDTO> list = GetAll<UserDTO>(UserTable);
            return list.Count > 0;
        }

        public static bool SaveUser(UserDTO dto, bool isNew = false)
        {
            
            Save<UserDTO>(dto, UserTable, KeyColumn, dto.userKey);
            return true;
        }


        #region Other table Management

        public static List<DeckCardDTO> LoadDecklist(string deckKey)
        {
            string query = $"deckKey = '{deckKey}'";
            List<DeckCardDTO> cards = GetAllWhere<DeckCardDTO>(DeckCardTable, query);
            return cards;

        }
        public static List<DeckCardDTO> LocalDecklists()
        {
            List<DeckCardDTO> cards = GetAll<DeckCardDTO>(DeckCardTable);
            return cards;

        }
        public static DeckDTO LoadDeck(string deckKey)
        {
            string query = $"deckKey = '{deckKey}'";
            DeckDTO deck = GetFirstWhere<DeckDTO>(UserDeckTable, query);
            return deck;
        }

        public static List<DeckDTO> LocalUserDeckDTOs()
        {
            List<DeckDTO> cards = GetAll<DeckDTO>(UserDeckTable);
            return cards;

        }
        
        public static void UpdateLocalDecksOwner(string newName, string oldName)
        {
            
            string query = $"Update {UserDeckTable} SET owner = '{newName}', whenCreated = '{DateTime.Now}' WHERE owner is null OR owner = '{oldName}'";
            db.QueryGeneric(query);
            db.Commit();
        }

        public static void SaveDeckList(string deckKey, List<DeckCardDTO> cards)
        {
            string queryWhere = $"deckKey = '{deckKey}'";
            if (Delete(DeckCardTable, queryWhere))
            {
                for (int i = 0; i < cards.Count; i++)
                {
                    db.Insert(cards[i], DeckCardTable);
                }
                db.Commit();
            }
        }
        #endregion

    }
}

