using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace nsSettings
{
    [System.Serializable]
    public class AudioSettings : ISettingsType<AudioSettings>
    {
        public float masterVol;
        public float fxVol;
        public float musicVol;

        public AudioSettings Default
        {
            get
            {
                return new AudioSettings
                {
                    masterVol = 1f,
                    fxVol = 1f,
                    musicVol = 1f,
                };
            }
        }
    }
}

