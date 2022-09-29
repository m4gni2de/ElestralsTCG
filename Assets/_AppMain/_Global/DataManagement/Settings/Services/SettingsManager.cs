using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nsSettings
{
    public static class SettingsManager
    {
        public static readonly string AdvancedSettingsName = "advSettings";
        public static readonly string AudioSettingsName = "audioSettings";
        public static readonly string AccountSettingsName = "acctSettings";

        private static GameSettings<AdvancedSettings> _Advanced = null;
        public static GameSettings<AdvancedSettings> Advanced
        {
            get
            {
                _Advanced ??= new GameSettings<AdvancedSettings>(AdvancedSettingsName);
                return _Advanced;
            }
        }

        private static GameSettings<AudioSettings> _Audio = null;
        public static GameSettings<AudioSettings> Audio
        {
            get
            {
                _Audio ??= new GameSettings<AudioSettings>(AudioSettingsName);
                return _Audio;
            }
        }

        private static GameSettings<AccountSettings> _Account = null;
        public static GameSettings<AccountSettings> Account
        {
            get
            {
                _Account ??= new GameSettings<AccountSettings>(AccountSettingsName);
                return _Account;
            }
        }




    }
}
    


