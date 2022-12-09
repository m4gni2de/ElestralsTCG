using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nsSettings
{
    public static class SettingsManager
    {
        private static readonly string AdvancedSettingsName = "advSettings";
        private static readonly string AudioSettingsName = "audioSettings";
        private static readonly string AccountSettingsName = "acctSettings";
        private static readonly string CatalogSettingsName = "catalogSettings";
        private static readonly string GraphicsSettingsName = "graphicsSettings";


        private static GameSettings<AdvancedSettings> _Advanced = null;
        public static GameSettings<AdvancedSettings> Advanced
        {
            get
            {
                _Advanced ??= new GameSettings<AdvancedSettings>(AdvancedSettingsName);
                return _Advanced;
            }
        }
        #region Audio
        private static GameSettings<AudioSettings> _Audio = null;
        public static GameSettings<AudioSettings> Audio
        {
            get
            {
                _Audio ??= new GameSettings<AudioSettings>(AudioSettingsName);
                return _Audio;
            }
        }
        #endregion

        #region Account
        private static GameSettings<AccountSettings> _Account = null;
        public static GameSettings<AccountSettings> Account
        {
            get
            {
                _Account ??= new GameSettings<AccountSettings>(AccountSettingsName);
                return _Account;
            }
        }
        #endregion

        #region Catalog
        private static GameSettings<CatalogSettings> _Catalog = null;
        public static GameSettings<CatalogSettings> Catalog
        {
            get
            {
                _Catalog ??= new GameSettings<CatalogSettings>(CatalogSettingsName);
                return _Catalog;
            }
        }
        #endregion

        #region Graphics
        private static GameSettings<GraphicsSettings> _graphics = null;
        public static GameSettings<GraphicsSettings> Graphics
        {
            get
            {
                _graphics ??= new GameSettings<GraphicsSettings>(GraphicsSettingsName);
                return _graphics;
            }
        }
        #endregion

    }
}
    


