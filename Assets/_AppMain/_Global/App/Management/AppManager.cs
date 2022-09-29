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
using RiptideNetworking;
using UnityEngine.Networking;

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
               
            }
            
        }
        
        if (hasDb)
        {
            SetupManager();
            App.ChangeScene(1);
            
        }
       
    }




    #endregion

    #region Instance
    public static readonly string ManagerAsset = "AppManager";
    public static AppManager Instance { get; private set; }
    #endregion

   

    #region User
    public User Account;
    #endregion

    #region Properties
    private DateTime _SessionStart = DateTime.MinValue;
    public DateTime SessionStart { get { return _SessionStart; } }


    private DateTime _SessionEnd = DateTime.MinValue;
    public DateTime SessionEnd { get { return _SessionEnd; } }

    [SerializeField]
    private DbConnector _dbConn;
    public DbConnector dbConnector { get { return _dbConn; } }

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
    

    private static bool _IsFrozen = false;
    public static bool IsFrozen { get { return _IsFrozen; } }
    #endregion
    public int GetManagerLayer()
    {
        return SortingLayer.GetLayerValueFromName("AppManager");
    }
    #endregion

    #region Events
    public static event Action ChangeLoadingText;
    #endregion

    #region Click Management
    public static iHold ActiveHoldObject { get; set; }
    protected bool IsClicked = false;
    public static void SetHoldObject(iHold hold = null)
    {
        ActiveHoldObject = hold;
    }



    #region Game Time/Freeze Management
    [SerializeField]
    private GameLog _freezeLog = null;
    public GameLog FreezeLog { get { _freezeLog ??= GameLog.Create("FreezeLog", false); return _freezeLog; } }
    protected static List<iFreeze> _FreezeObjects = null;
    public static List<iFreeze> FreezeObjects { get { _FreezeObjects ??= new List<iFreeze>(); return _FreezeObjects; } }
    public static void Freeze(bool isFreeze = true)
    {
        _IsFrozen = isFreeze;
        CameraMotion.main.Freeze(_IsFrozen);
    }
    public static void Freeze(iFreeze obj)
    {
        if (!FreezeObjects.Contains(obj))
        {
            FreezeObjects.Add(obj);
            string logMsg = $"{obj} was added as a Freeze Object. There are now {FreezeObjects.Count} Freeze Objects.";
            Instance.FreezeLog.AddLog(logMsg);
        }
        
        Freeze(true);
    }
    public static void Thaw(iFreeze obj)
    {
        if (FreezeObjects.Contains(obj))
        {
            FreezeObjects.Remove(obj);
            string logMsg = $"{obj} was removed as a Freeze Object. There are now {FreezeObjects.Count} Freeze Objects.";
            Instance.FreezeLog.AddLog(logMsg);
        }
        if (FreezeObjects.Count == 0)
        {
            Freeze(false);
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
        appCanvas.sortingOrder = 0;
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
    }

    #region Update
    private void Update()
    {
        //if (!IsClicked)
        //{
        //    if (Input.GetMouseButton(0))
        //    {
        //        IsClicked = Input.GetMouseButton(0);
        //        //var worldMousePosition =
        //        //       Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
        //        //var x = worldMousePosition.x;
        //        //var y = worldMousePosition.y;

        //        TouchButton.CheckTouch();
        //    }
        //}
        //else
        //{
        //    if (!Input.GetMouseButton(0)) { IsClicked = false; }
        //}
        
    }
    #endregion

    #region Quitting

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
                string title = "Error";
                string msg = www.error.ToString();
                //GameManager.Instance.ShowMessage(title, msg);
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
                    string title = "Error";
                    string msg = www.responseCode.ToString();
                    Debug.Log(msg);
                    //GameManager.Instance.ShowMessage(title, msg);
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
