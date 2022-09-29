using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nsSettings
{
    [System.Serializable]
    public class AccountSettings : ISettingsType<AccountSettings>
    {
        public int ActiveDeck;

        public AccountSettings Default
        {
            get
            {
                return new AccountSettings
                {
                    ActiveDeck = 0
                };
            }
        }
    }
}
