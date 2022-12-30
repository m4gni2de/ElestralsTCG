using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nsSettings
{
    [System.Serializable]
    public class AccountSettings : ISettingsType<AccountSettings>
    {
        public int ActiveDeck;
        public int Sleeves;
        public int Playmatt;

        public AccountSettings Default
        {
            get
            {
                return new AccountSettings
                {
                    ActiveDeck = 0,
                    Sleeves = 0,
                    Playmatt = 0,
                };
            }
        }
    }
}
