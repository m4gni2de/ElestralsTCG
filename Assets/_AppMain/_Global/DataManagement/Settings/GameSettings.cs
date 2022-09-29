using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Databases;


public class GameSettings<T> where T : ISettingsType<T>, new()
{
      
    public string Key { get; set; }

    private T _Settings { get; set; }
    public T Settings { get; set; }
       
    public bool isDirty
    {
        get
        {
            return LastSaved.settingsValue != Data.settingsValue;
        }
    }

    public SettingsData Data
    {
        get
        {
            return SettingsData.Create(Key, Settings);
        }
    }
    private SettingsData LastSaved { get; set; }

        
        
    public GameSettings(string settKey)
    {
        SettingsData data = SettingsService.FindSettings(settKey);

        bool save = false;
        if (string.IsNullOrEmpty(data.settingsKey))
        {
            ISettingsType<T> obj = new T().Default;
            data.settingsKey = settKey;
            data.settingsValue = JsonUtility.ToJson(obj);
            save = true;
        }

        LastSaved = data;
        Load(data);

        if (save)
        {
            Save();
        }
    }
        
    public void Load(SettingsData data)
    {
        Key = data.settingsKey;
        Settings = JsonUtility.FromJson<T>(data.settingsValue);
            
    }
    public void Save()
    {
        SettingsService.Save(Key, Settings);
    }
        
}


