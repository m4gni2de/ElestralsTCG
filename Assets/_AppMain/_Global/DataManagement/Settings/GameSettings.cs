using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Databases;
using System.Globalization;


public class GameSettings<T> where T : ISettingsType<T>, new()
{
    #region Properties
    public string Key { get; set; }

    public T Settings { get; set; }

    public object GetValue(string key)
    {
        object val = Settings.GetPropertyOrFieldValue(key);
        if (val == null)
        {
            return App.DisplayError($"Setting {key} does not Exist in {Settings.GetType().Name}!;");
        }
        return Settings.GetPropertyOrFieldValue(key);
    }

    public T this[T val]
    {
        get { return Settings; }
    }

    /// <summary>
    /// Depreciated
    /// </summary>
    public SettingsData Data
    {
        get
        {
            return SettingsData.Create(Key, Settings);
        }
    }


    private SettingsData LastSaved { get; set; }

    #endregion

    #region Initialization
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

    #endregion

    #region Changing Values

    #endregion

    #region Saving
    
    public bool IsDirty
    {
        get
        {
            T zero = JsonUtility.FromJson<T>(LastSaved.settingsValue);
            T current = Settings;

            foreach (var item in zero.GetType().GetProperties())
            {
                object zVal = item.GetValue(zero);
                object currVal = item.GetValue(current);

                if (zVal.CompareTo(currVal) != ComparedTo.EqualTo) { return true; }
            }
            return false;
        }
    }

    public void SetValue<T1>(ref T1 valToChange, T1 newVal)
    {
        valToChange = newVal;
    }

    public void Save()
    {
        SettingsService.Save(Key, Settings);
    }

    public void Rollback()
    {
        Load(LastSaved);
    }
    #endregion

}


