using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Users;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using SimpleSQL;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets.ResourceLocators;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceLocations;
using Databases;
using Gameplay;

using UnityEngine.Networking;
using System.Security.Permissions;
using System.Net.Sockets;
using AppManagement;
using UnityEngine.Events;
using Cards.Collection;
#if UNITY_EDITOR
using ParrelSync;
#endif

public class AppManager : MonoBehaviour
{

    #region Initialization & Startup
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            CheckAssets();
        }

      
    }

    #region Assets
    private void CheckAssets()
    {
        Addressables.InitializeAsync().Completed += LoadApp;
    }

    private async void LoadApp(AsyncOperationHandle<IResourceLocator> obj)
    {
        Addressables.InitializeAsync().Completed -= LoadApp;

        bool hasDb = await dbConnector.ConnectAsync();

        var data = await AssetPipeline.CheckForCatalogUpdate();
        bool hasUpdate = data != null;
        if (data != null)
        {

            List<string> items = data.Item1;
            double size = data.Item2;
            if (size > 0)
            {
                Instance.ShowLoadingBar("Items Downloaded", 0f, items.Count);
                //AssetPipeline.DoRedownloadAllCards();

            }

        }


        //AssetPipeline.DoRedownloadAllCards();
        if (hasDb)
        {
            CardCollection.GenerateCollection();
            SetupManager();
            //AssetPipeline.DoRedownloadAllCards();
            CheckForAccount();

        }

    }
    #endregion


    private void Start()
    {

    }



    #endregion

    #region Instance
    public static readonly string ManagerAsset = "AppManager";
    public static AppManager Instance { get; private set; }
    #endregion



    #region User Account
    //public User Account;

    protected void CheckForAccount()
    {
#if UNITY_EDITOR
        CheckForAccountEditor();
#else
CheckForAccountDevice();
#endif
    }


    protected void CheckForAccountEditor()
    {
#if UNITY_EDITOR
        if (ClonesManager.IsClone())
        {
            if (!App.AccountExists)
            {
                App.LoadGuest();

                return;
            }
        }
        if (!App.AccountExists)
        {
            bool exists = User.ExistsOnFile;
            if (exists)
            {
                if (ClonesManager.IsClone())
                {
                    App.LoadGuest();

                    return;
                }
                else
                {
                    App.LoadUserAccount();

                    return;
                }

            }
            else
            {

                CreateAccount();
            }
        }
#endif
    }

    protected void CheckForAccountDevice()
    {
        if (!App.AccountExists)
        {
            bool exists = User.ExistsOnFile;
            if (exists)
            {
                App.LoadUserAccount();

            }
            else
            {
                CreateAccount();
            }
        }
    }

    //first, ask if they want to create an account, or just load up a guest account.
    protected static void CreateAccount()
    {
        string message = $"There is no account on this device! Would you like to enter a username? (Selecting 'No' will create a temporary account that will last until the app closes.";
        App.AskYesNo(message, AskCreateAccount);
    }

    //handle the question above
    private static void AskCreateAccount(bool create)
    {
        if (!create)
        {
            App.LoadGuest();
        }
        else
        {
            TextInput.Load("Type desired Username", ConfirmCreateAccount);
        }
    }

    //before confirming their username, just ask to make sure that's what they want.
    private static void ConfirmCreateAccount(string username)
    {
        TextInput.Instance.Hide();
        App.AskYesNo($"Desired Username is: '{username}'. Proceed with this Username?", FinalizeCreateAccount);
    }

    //if they decide they want that name, create the account and write it to the DB
    private static void FinalizeCreateAccount(bool create)
    {
        TextInput.Instance.Show();
        if (!create)
        {
            TextInput.Reload("Type desired Username.", ConfirmCreateAccount);
        }
        else
        {
            string username = TextInput.Instance.Value;
            User u = User.Create(username);
            App.LoadAccount(u);
        }
    }
    #endregion

    #region Properties
    private DateTime _SessionStart = DateTime.MinValue;
    public DateTime SessionStart { get { return _SessionStart; } }


    private DateTime _SessionEnd = DateTime.MinValue;
    public DateTime SessionEnd { get { return _SessionEnd; } }

    protected DbConnector lastConnected = null;
    [SerializeField]
    private DbConnector _dbConn;
    public DbConnector dbConnector
    {
        get
        {
            return _dbConn;
        }
    }

    [SerializeField]
    private DbConnector _playerDb;
    public DbConnector dbPlayer
    {
        get
        {
            return _playerDb;
        }
    }

    #region Pause/Play Management
    public static float TimeScale { get { return Time.timeScale; } set { Time.timeScale = value; } }
    public static bool IsPlaying { get { return TimeScale > 0; } }
    public static void Pause() { TimeScale = 0f; }
    public static void Resume() { TimeScale = 1f; }
    #endregion
    #endregion

    #region UI Properties

    #region Global Objects
    private GlobalObject _worldCanvas = null;
    public GlobalObject worldCanvas { get { return _worldCanvas; } set { _worldCanvas = value; } }

    [SerializeField]
    private Canvas appCanvas;
    public Blocker appBlocker;
    public LoadingBar loadingBar;
    public DisplayManager displayManager;

    private static bool _IsFrozen = false;
    public static bool IsFrozen { get { return _IsFrozen; } }
    #endregion
    public int GetManagerLayer()
    {
        return SortingLayer.GetLayerValueFromName("AppManager");
    }
    #endregion

    #region Events
    //public static event Action ChangeLoadingText;
    #endregion

    #region Click Management
    public static event Action<bool> OnTapped;
    private static bool _tap = false;
    public static bool tap
    {
        get
        {
            return _tap;
        }
        set
        {
            if (_tap == value) { return; }
            _tap = value;
            OnTapped?.Invoke(value);
        }
    }
    #region Game Time/Freeze Management
    [SerializeField]
    private GameLog _freezeLog = null;
    public GameLog FreezeLog { get { _freezeLog ??= GameLog.Create("FreezeLog", true); return _freezeLog; } }
    protected static List<iFreeze> _FreezeObjects = null;
    public static List<iFreeze> FreezeObjects { get { _FreezeObjects ??= new List<iFreeze>(); return _FreezeObjects; } }
    public static void Freeze(bool isFreeze = true)
    {
        _IsFrozen = isFreeze;
        if (CameraMotion.main != null)
        {
            CameraMotion.main.Freeze(_IsFrozen);
        }

    }
    public static void Freeze(iFreeze obj)
    {
        if (!FreezeObjects.Contains(obj))
        {
            FreezeObjects.Add(obj);
            string logMsg = $"{obj} was added as a Freeze Object. There are now {FreezeObjects.Count} Freeze Objects.";
            if (Instance)
            {
                Instance.FreezeLog.AddLog(logMsg);
            }
        }

        Freeze(true);
    }
    public static void Thaw(iFreeze obj)
    {
        if (FreezeObjects.Contains(obj))
        {
            FreezeObjects.Remove(obj);
            string logMsg = $"{obj} was removed as a Freeze Object. There are now {FreezeObjects.Count} Freeze Objects.";
            if (Instance)
            {
                Instance.FreezeLog.AddLog(logMsg);
            }

        }
        if (FreezeObjects.Count == 0)
        {
            Freeze(false);
        }
    }
    public static void ThawOnRelease(iFreeze obj)
    {
        if (Input.touchCount > 0 || Input.GetMouseButton(0))
        {
            Instance.StartCoroutine(Instance.AwaitRelease(obj));
        }
        else
        {
            Thaw(obj);
        }
    }
    private IEnumerator AwaitRelease(iFreeze obj)
    {
        do
        {
            yield return null;
        } while (true && Input.GetMouseButton(0));

        if (obj != null)
        {
            Thaw(obj);
        }
            

    }
    #endregion

    #endregion


    private void OnEnable()
    {
        appCanvas.overrideSorting = true;
        appCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        appCanvas.worldCamera = Camera.main;
        appCanvas.sortingLayerName = "AppManager";
        appCanvas.sortingOrder = 1;
    }

    protected void SetupManager()
    {
        //_SessionStart = DateTime.Now;
        SceneManager.activeSceneChanged += OnSceneChange;
        name = "AppManager";
        appBlocker.SetBlocker("AppManager", -1);
        appBlocker.HideBlocker();
    }

    
    private void OnSceneChange(Scene arg0, Scene arg1)
    {
        
        SetUI();
        
    }

    protected void SetUI()
    {
        worldCanvas = WorldCanvas.Instance;
        appCanvas.worldCamera = Camera.main;
        
        PopupManager.SetActivePopup();
        
    }

    #region Update
    private void Update()
    {
        if (!tap)
        {
            if (Input.GetMouseButtonDown(0))
            {
                tap = true;
            }
        }
        else
        {
            if (!Input.GetMouseButton(0)) { tap = false; }
        }

    }
    #endregion

    #region Quitting


    public void TryQuit()
    {
        App.AskYesNo($"Do you want to quit the app?", ConfirmQuit);
    }

    private void ConfirmQuit(bool quit)
    {
        if (quit)
        {
            Application.Quit();
        }
    }
    private void OnApplicationQuit()
    {
        if (Instance != null)
        {
            End();
        }
    }
    private void OnDestroy()
    {
        if (Instance != null)
        {
            End();
        }
        //_SessionEnd = DateTime.Now;
        //LogSession();

    }
    public void End()
    {
        SceneManager.activeSceneChanged -= OnSceneChange;
        if (Instance != null)
        {
            dbConnector.Flush();
            Instance = null;
        }



    }


    private void LogSession()
    {
        string[] lines = new string[3];
        lines[0] = $"Start time: {SessionStart}";
        lines[1] = $"End time: {SessionEnd}";

        TimeSpan total = SessionEnd - SessionStart;
        lines[2] = $"Total Time: {total}";
        App.Log(lines);
    }
    #endregion

    #region Object Management
    public void ShowObject(GameObject obj, int sortLayer)
    {
        int highestSort = GetManagerLayer();
        if (sortLayer > highestSort) { sortLayer = highestSort; }

        obj.transform.SetParent(transform);
    }

    public void ShowLoadingBar(string msg, float startVal, float maxVal, float minVal = 0)
    {
        loadingBar.Display(msg, startVal, maxVal, minVal);
    }
    public static async Task<bool> AwaitLoading(LoadingBar bar, Action ac)
    {

        ac.Invoke();
        do
        {
            await Task.Delay(1);
        } while (true && !bar.LoadComplete);

        bar.Hide();
        return true;
    }


    public void DoAssetLoad<T>(string key)
    {
        StartCoroutine(AwaitAssetLoad<T>(key));
    }

    private IEnumerator AwaitAssetLoad<T>(string key)
    {
        loadingBar.Display($"Initializing {key}", 0f, 1f, 0f);
        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);

        do
        {
            yield return new WaitForEndOfFrame();
            if (loadingBar != null)
            {
                loadingBar.SetSlider(handle.PercentComplete);
            }

        } while (true && !handle.IsDone);
        yield return handle;

    }


    public static event Action AssetsLoadComplete;
    public void DoMultipleAssetLoad<T>(List<string> keys)
    {
        StartCoroutine(AwaitMulitpleAssets<T>(keys));
    }

    private IEnumerator AwaitMulitpleAssets<T>(List<string> keys)
    {
        int count = keys.Count;
        loadingBar.Display($"Initializing assets", 0f, count, 0f);


        int doneCount = 0;
        do
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(keys[doneCount]);
            do
            {
                loadingBar.SetText($"{keys[doneCount]} - {handle.PercentComplete * 100f}%");
                //yield return new WaitForEndOfFrame();
                yield return null;
                loadingBar.SetText($"{keys[doneCount]} - {handle.PercentComplete * 100f}%");
            } while (!handle.IsDone);

            yield return handle;
            loadingBar.MoveSlider(1f);
            doneCount += 1;


        } while (true && doneCount < count);


        loadingBar.Hide();
        AssetsLoadComplete?.Invoke();

    }
    #endregion

   


    #region Outbound Connections

    public static async Task<string> DoPostRequestWithPayload(string url, WWWForm payload)
    {
        using (var www = UnityWebRequest.Post(url, payload))
        {
            www.timeout = 3;
            www.SendWebRequest();
            byte[] by = www.uploadHandler.data;

            string c = Convert.ToBase64String(by);


            while (!www.isDone)
                await Task.Delay(100);
            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                string msg = www.error.ToString();
                App.DisplayError($"Error: {msg}", null, true);
                return "error";
            }
            else
            {
                if (www.responseCode == 200)
                {
                    var data = www.downloadHandler.text; //You can process text - bytes - etc...
                    return data; //Processes the downloaded information
                }
                else
                {
                    string msg = www.responseCode.ToString();
                    App.DisplayError($"Error: {msg}", null, true);
                    return "error";
                }
            }

        }
    }


    public static async Task<string> DoGetRequest(string url, Dictionary<string, string> headers)
    {

        using (var www = UnityWebRequest.Get(url))
        {
            if (headers != null)
            {
                foreach (KeyValuePair<string, string> header in headers)
                {
                    www.SetRequestHeader(header.Key, header.Value);
                }
            }

            www.SendWebRequest();
            while (!www.isDone)
                await Task.Delay(100);
            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                return www.error;
            }
            else
            {
                if (www.responseCode == 200)
                {
                    var data = www.downloadHandler.text; //You can process text - bytes - etc...

                    return data; //Processes the downloaded information
                }
                else
                {
                    //return www.error;
                    return www.error;
                }
            }

        }
    }
    #endregion



   
}
