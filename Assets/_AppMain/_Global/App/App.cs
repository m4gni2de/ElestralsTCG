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
            if (_Instance == null) { return false; }
            return ConnectionManager.IsConnected();
        }
    }

   
    #endregion

    #endregion

    #region Initialization


    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void StartApp()
    {
        CheckForAccount();
        //if (_Instance == null)
        //{
        //    await AssetPipeline.WorldObjectClone(ManagerAsset);
        //    //Manager.LoadManager();
        //    if (CheckDatabase())
        //    {
        //        CheckForAccount();
        //        //ChangeScene("SampleScene");
        //    }
        //}
    }

    
   
    public static bool CheckDatabase()
    {
        if (!isConnected)
        {
            _Instance = new App();

            //ConnectionManager.Connect();
        }
        return isConnected;
    }


    protected static void CheckForAccount()
    {
        //eventually create account stuff here, for now just use it locally
        if (_account == null)
        {
            bool exists = User.ExistsOnFile;
            if (exists) { _account = User.AccountOnFile; } else { _account = User.Create(); }

            AppManager.Instance.Account = _account;
        }


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
    public static void LogFatal(string msg) { LogController.Fatal(msg); }
    #endregion

    #region Scene Management
    public static void ChangeScene(string scene)
    {
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
    protected static PopupManager popUp { get { return PopupManager.Instance; } }
    public static void ShowMessage(string msg, Action callback = null)
    {
        if (PopupManager.ActivePopup == null)
        {
            popUp.DisplayMessage(msg, callback);
        }

    }
    public static void AskYesNo(string msg, Action<bool> callback)
    {
        if (PopupManager.ActivePopup == null)
        {
            popUp.AskYesNo(msg, callback);
        }
        
    }
    #endregion

    #region Global Functions
    public static string WhoAmI { get { return Account.Id; } }
    #endregion
}
