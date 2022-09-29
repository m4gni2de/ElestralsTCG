using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Users;



namespace Databases
{
    public class UserService : DataService
    {
        protected static readonly string UserTable = "uUserDTO";
        protected static readonly string KeyColumn = "userKey";

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

        public static bool SaveUser(UserDTO dto)
        {
            
            Save<UserDTO>(dto, UserTable, KeyColumn, dto.userKey);
            return true;
        }


        #region Other table Management
        public static List<string> UserDecks(string userKey)
        {
            List<string> list = new List<string>();
            List<DeckDTO> decks = DeckService.GetUserDecklist(userKey);
            for (int i = 0; i < decks.Count; i++)
            {
                list.Add(decks[i].deckKey);
            }
            return list;
        }
        #endregion

    }
}

