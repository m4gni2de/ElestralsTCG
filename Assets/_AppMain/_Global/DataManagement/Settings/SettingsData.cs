using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Databases
{
    public struct SettingsData
    {
        public string settingsKey { get; set; }
        public string settingsValue { get; set; }

        public static SettingsData Create<T>(string key, T source)
        {
            SettingsData sett = new SettingsData
            {
                settingsKey = key,
                settingsValue = JsonUtility.ToJson(source)
            };
            return sett;   
        }

        public bool Exists
        {
            get
            {
                return !string.IsNullOrEmpty(settingsKey);
            }
        }
       
        public static SettingsData Empty
        {
            get
            {
                return new SettingsData
                {
                    settingsKey = "",
                    settingsValue = ""
                };
            }
        }
        
       
        public static implicit operator SettingsDTO(SettingsData data)
        {
            SettingsDTO dto = new SettingsDTO
            {
                settingsKey = data.settingsKey,
                settingsValue = data.settingsValue
            };
            return dto;
        }

        public static implicit operator SettingsData(SettingsDTO dto)
        {
            return new SettingsData
            {
                settingsKey = dto.settingsKey,
                settingsValue = dto.settingsValue
            };
        }
    }
}

