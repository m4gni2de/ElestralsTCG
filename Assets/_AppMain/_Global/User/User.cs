using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Databases;
using System;
using Decks;
using System.Threading.Tasks;

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
        User(UserData dto)
        {
            data = dto;
            LoadDecks();
            
        }
        protected void LoadDecks()
        {
            DeckLists.Clear();
            List<string> deckKeys = UserService.UserDecks(Id);

            for (int i = 0; i < deckKeys.Count; i++)
            {
                Decklist deck = Decklist.Load(deckKeys[i]);
                DeckLists.Add(deck);
            }
        }
        #endregion

        #region Saving
        public void ToggleDirty(bool dirty)
        {
            _IsDirty = dirty;
        }

        public void Save()
        {
            UserService.SaveUser(data.data);
        }

        #endregion


        #region Network Properties
        
        #endregion

    }
}

