using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logging;
using Users;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;
using System;
using Decks;
using nsSettings;
using Databases;
using static Users.User;
using PopupBox;
using AppManagement.Loading;
#if UNITY_EDITOR
using ParrelSync;
#endif

public class App 
{
    #region Instance Properties

    public static readonly string ManagerAsset = "AppManager";
    private static App _Instance = null;
    public static App Instance
    {
        get
        {
            if (_Instance == null) { _Instance = new App(); }
            return _Instance;
        }
    }

    private static bool _IsReady = false;
    public static bool IsReady { get { return _IsReady; } }
    public static void SetReady(bool ready)
    {
        _IsReady = ready;
    }


    #region User
    private static User _account = null;
    public static User Account
    {
        get
        {
            if (_account == null)
            {
                
                LogFatal("There is no account in use.");
            }
            return _account;
        }
    }
    public static bool AccountExists
    {
        get
        {
            return _account != null;
        }
    }
    public static void LoadAccount(User user)
    {
        _account = user;
        ChangeScene(MainScene.SceneName);

        
    }
    public static void LoadGuest()
    {
        LoadAccount(User.Guest());
    }
   
    public static void LoadUserAccount()
    {
        LoadAccount(User.AccountOnFile);
    }
    #endregion

    public static Decklist ActiveDeck
    {
        get
        {
            if (_account == null)
            {
                LogFatal("There is no account in use.");
                return null;
            }
            int activeDeck = SettingsManager.Account.Settings.ActiveDeck;
            Decklist deck = Account.DeckLists[activeDeck];
            return deck;
        }
    }

    #region Functions
    public static bool isConnected
    {
        get
        {
            return AppManager.Instance != null;
            //if (_Instance == null) { return false; }
            //return ConnectionManager.IsConnected();
        }
    }


    #endregion

    #endregion


    /// <summary>
    /// Figure this out later. 
    /// </summary>
    #region Verion Management

    private static readonly string AppUrl = "https://drive.google.com/file/d/1OyFb76IbbfpFjb0d0q4xds2TtVOJZ2pH/view?usp=sharing";
    /// <summary>
    ///  BE SURE TO CHANGE THESE WHEN BUILDING APP
    /// </summary>
    public static readonly string AppVersionKey = "tts-002";
    public static readonly int BuildIndex = 0;
    private static VersionData _currentVersion = null;
    public static VersionData CurrentVersion
    {
        get
        {
            if (_currentVersion == null)
            {
                _currentVersion = VersionService.GetVersion(AppVersionKey);
            }
            return _currentVersion;
        }
    }

    public static bool IsVersionLatest()
    {
        return CurrentVersion.IsLatestVersion();
    }
    public static void UpdateToNewestVersion()
    {
        string msg = $"There is a newer version available! This version of the app is no longer in use.";
        App.DisplayError(msg, ConfirmAppUpdate);
    }
    private static void ConfirmAppUpdate()
    {
        Application.OpenURL(AppUrl);
    }
    #endregion


    #region Closing
    public static void Close()
    {
        if (isConnected)
        {
            if (_account != null && _account.IsDirty) { Account.Save(); }
            ConnectionManager.Disconnect();
            if (_Instance != null)
            {
                _Instance = null;
            }

        }
    }
    #endregion

    #region Logging
    public static void Log(string msg) { LogController.Log(msg); }
    public static void Log(string[] msg) { LogController.Log(msg); }
    public static void LogWarning(string msg) { LogController.Warning(msg); }
    public static void LogError(string msg) { LogController.Error(msg); }
    public static bool LogFatal(string msg) { return LogController.Fatal(msg); }
    #endregion

    #region Scene Management
    private static string pendingScene;
    public static void TryChangeScene(string scene)
    {
        pendingScene = scene;
        AskYesNo($"Do you want leave this Scene and go Back?", ConfirmChangeScene);
    }
    private static void ConfirmChangeScene(bool change)
    {
        string scene = pendingScene;
        pendingScene = "";
        
        if (change)
        {
            ChangeScene(scene);
        }
        else
        {

            DisplayManager.AddAction(TryChangeScene, scene);
        }
    }
    public static void ChangeScene(string scene)
    {
        //if (ScreenManager.Instance)
        //{
        //    ScreenManager.Instance.ShowRandomScreen(.5f);
        //}
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
    public static void ChangeScene(int scene)
    {
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
    #endregion

    #region Object Management
    public static void ShowObject(GameObject obj, int sortLayerValue)
    {
        //do something with object stacking eventually
        AppManager.Instance.ShowObject(obj, sortLayerValue);
    }
    #endregion

    #region Popup Boxes

    #region Message Display Only
    protected static PopupManager popUp { get { return PopupManager.Instance; } }
    
    public static DisplayBox ShowWaitingMessage(string msg, Action callback = null)
    {
        if (PopupManager.ActivePopup == null)
        {
            popUp.DisplayMessage(msg, callback, false, false);
            return popUp.Message;
        }
        return null;
    }

    public static void ShowMessage(string msg, Action callback = null)
    {
        if (PopupManager.ActivePopup == null)
        {
            popUp.DisplayMessage(msg, callback, true, false);
        }
        
    }
    public static void ShowTimedMessage(string msg, float time, Action callback = null)
    {
        if (PopupManager.ActivePopup == null)
        {
            popUp.DisplayTimedMessage(msg, callback, time);
        }
    }

    public static bool DisplayError(string msg, Action callback = null, bool log = false)
    {
        if (PopupManager.ActivePopup == null)
        {
            popUp.DisplayMessage(msg, callback, true, false);
            
        }
        else
        {
            popUp.StopAllCoroutines();
            popUp.DisplayNewMessage(msg, callback, true, false);
        }

        if (log)
        {
            App.LogError(msg);
        }
        return false;

    }

    #endregion

    #region User Input
    public static void AskYesNo(string msg, Action<bool> callback)
    {
        if (PopupManager.ActivePopup == null)
        {
            popUp.AskYesNo(msg, callback);
        }
        else
        {
            popUp.AskYesNo(msg, callback, true);
        }
        
    }
    public static void AskYesNoCancel(string msg, Action<PopupResponse> callback)
    {
        if (PopupManager.ActivePopup == null)
        {
            popUp.AskYesNoCancel(msg, callback);
        }
        else
        {
            popUp.AskYesNoCancel(msg, callback, true);
        }

    }
    public static void ShowDropdown(string msg, List<string> options, Action<string> callback, bool closeOnBack = true)
    {
        if (PopupManager.ActivePopup == null)
        {
            popUp.ShowDropdown(msg, options, callback, closeOnBack);
        }
    }
    public static void ShowDropdown<T>(string msg, List<T> options, Action<T> callback, string propName, bool closeOnBack = true)
    {
        if (PopupManager.ActivePopup == null)
        {
            popUp.ShowDropdown(msg, options, callback, propName, closeOnBack);
        }
    }

    
    #endregion

    #endregion

    #region Global Functions
    public static string WhoAmI { get { return Account.Id; } }

    public static double DateTimeToUnixTimestamp(DateTime dateTime)
    {
        DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        long unixTimeStampInTicks = (dateTime.ToUniversalTime() - unixStart).Ticks;
        return (double)unixTimeStampInTicks / TimeSpan.TicksPerSecond;
    }
    public static DateTime UnixTimestampToDateTime(double unixTime)
    {
        DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        long unixTimeStampInTicks = (long)(unixTime * TimeSpan.TicksPerSecond);
        return new DateTime(unixStart.Ticks + unixTimeStampInTicks, System.DateTimeKind.Utc);
    }
    #endregion
}
