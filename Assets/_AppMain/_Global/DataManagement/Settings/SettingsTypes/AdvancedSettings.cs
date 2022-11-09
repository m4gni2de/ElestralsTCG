using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nsSettings
{
    [System.Serializable]
    public class AdvancedSettings : ISettingsType<AdvancedSettings>
    {


        public int LogLevel;
        public int PreloadCards;

        public AdvancedSettings Default
        {
            get
            {
                return new AdvancedSettings
                {
                    LogLevel = 1,
                    PreloadCards = 1
                };
            }
        }
        //public AdvancedSettings()
        //{
        //    this = Default();
        //}


        


    }
}

