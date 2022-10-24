using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Databases;
using System;
using Decks;
using System.Threading.Tasks;
using Mono.Data.Sqlite;
using UnityEditor;
using System.IO;
#if UNITY_EDITOR
using ParrelSync;
#endif

namespace Users
{
    [System.Serializable]
    public class User
    {
        [System.Serializable]
        public struct UserData
        {
            public string id;
            public string name;
            public string email;
            public DateTime whenCreated;

            public UserDTO data
            {
                get
                {
                    return new UserDTO
                    {
                        userKey = id,
                        username = name,
                        email = email,
                        whenCreated = whenCreated,
                    };
                }
            }
            
            public UserData(UserDTO dto, bool isNew)
            {
                id = dto.userKey;
                name = dto.username;
                email = dto.email;
                if (isNew)
                {
                    whenCreated = DateTime.Now;
                }
                else
                {
                    whenCreated = dto.whenCreated;
                }
            }

            

        }


        #region Properties
        [SerializeField]
        private UserData data;
        public string Id { get { return data.id; } }

        public string Name { get { return data.name; } }

        public string Email { get { return data.email; } }

        private bool _IsDirty = false;
        public bool IsDirty { get { return _IsDirty; } }

        [SerializeField]
        private List<Decklist> _deckLists = null;
        public List<Decklist> DeckLists { get { _deckLists ??= new List<Decklist>(); return _deckLists; }}
        #endregion

        #region Static Functions
        public static bool ExistsOnFile { get { return UserService.AccountExists(); } }
        public static User AccountOnFile
        {
            get
            {
                if (UserService.AccountExists(out UserDTO dto))
                {
                    return Load(dto.userKey);
                }
                return null;

            }
        }


        public static User Guest()
        {
            UserDTO dto = new UserDTO
            {
                userKey = UniqueString.Create("usr", 7),
                username = "clone",
                whenCreated = System.DateTime.Now,
                email = ""
            };
            UserData data = new UserData(dto, true);
            return new User(data, true);
        }
        public static User Create(string username = "admin")
        {
            UserDTO dto = UserService.CreateUser(username);
            UserData data = new UserData(dto, true);
            return new User(data);
        }
        public static User Load(string key)
        {
            UserDTO dto = UserService.LoadUser(key);
            UserData data = new UserData(dto, false);
            return new User(data);
        }
        #endregion

        #region Initialization and Loading
        User(UserData dto, bool isGuest = false)
        {
            data = dto;
            LoadAllDecks();


        }
        //protected void LoadDecks()
        //{
        //    DeckLists.Clear();

        //    List<DeckCardDTO> deckCards = UserService.LocalDecklists();
        //    List<DeckDTO> deckKeys = UserService.LocalUserDeckDTOs();
        //    DeckService.CopyFromLocal(deckCards, deckKeys);


        //    for (int i = 0; i < deckKeys.Count; i++)
        //    {
        //        Decklist deck = Decklist.Load(deckKeys[i]);
        //        DeckLists.Add(deck);
        //    }
            
            
        //}

        protected void LoadAllDecks()
        {
            DeckLists.Clear();

            List<DeckCardDTO> deckCards = UserService.LocalDecklists();
            List<DeckDTO> deckKeys = UserService.LocalUserDeckDTOs();
            List<Decklist> decks = Decklist.LoadAllLocalDecks(deckKeys, deckCards);
            DeckLists.AddRange(decks);
        }
        #endregion

        #region Saving
        public void ToggleDirty(bool dirty)
        {
            _IsDirty = dirty;
        }

        public void SetNewUserID()
        {
            string oldId = data.id;
            data.id = UniqueString.Create("usr", 7);

            string query = $"UPDATE uUserDTO SET userKey = '{data.id}';";
            UserService.DoQuery(query);
            
        }

        public void Save()
        {
            UserService.SaveUser(data.data);
        }

        public static void Reset()
        {

        }
        #endregion


        #region Network Properties
        
        #endregion

    }
}

