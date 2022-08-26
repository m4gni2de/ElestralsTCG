using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using Databases;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.ResourceLocations;

public class AssetPipeline
{
    private static bool _AssetsLoaded = false;
    public static bool AssetsLoaded { get { return _AssetsLoaded; } }

    public static event Action<float> OnItemDownloaded;
    protected static void ItemDownloaded(float val)
    {
        OnItemDownloaded.Invoke(val);
    }
    public static event Action OnDownloadComplete;
    protected static void DownloadComplete()
    {
        OnDownloadComplete.Invoke();
    }
    #region Asset Management
    public static void LoadAssets()
    {
#if UNITY_EDITOR

        AssetsComplete();
#else
        _AssetsLoaded = false;
        Addressables.InitializeAsync().Completed += AssetsComplete;
#endif


    }
    private static void AssetsComplete(AsyncOperationHandle<UnityEngine.AddressableAssets.ResourceLocators.IResourceLocator> obj)
    {
        AssetsComplete();
        
    }
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    private static async void AssetsComplete()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        _AssetsLoaded = true;
#if UNITY_EDITOR

#else
GameObject app = await AssetPipeline.WorldObjectClone(AppManager.ManagerAsset);
        bool dbReady = await WaitForDatabase();
#endif
    }
    #endregion

    private static async Task<bool> WaitForDatabase()
    {
        do
        {
            await Task.Delay(1);
        } while (true && AppManager.Instance.dbManager._conn == null);
        return true;
    }

    public static async Task<bool> LoadAllAssets(AsyncOperationHandle<IResourceLocator> obj)
    {
        int count = 0;
        foreach (var item in obj.Result.Keys)
        {
            
            if (item.GetType() == typeof(Sprite))
            {
                AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(item);
                await handle.Task;
                count += 1;
                Debug.Log($"Downloaded {count} Sprites!");
            }
           
        }
        return true;
    }


    #region Asset Downloading

    #region Properties
    private static bool isDownloading = false;
    private static bool checkingCatalog = false;

    #endregion
    public static async Task<Tuple<List<string>, double>> CheckForCatalogUpdate()
    {
        Tuple<List<string>, double> dataInfo = null;
        if (!isDownloading)
        {
            checkingCatalog = true;
            AsyncOperationHandle<List<string>> newAssets = Addressables.CheckForCatalogUpdates();
            await newAssets.Task;

            if (newAssets.IsValid())
            {
                dataInfo = await GetNewAssets(newAssets);
            }
            
        }


        checkingCatalog = false;
        return dataInfo;

    }

    private static async Task<Tuple<List<string>, double>> GetNewAssets(AsyncOperationHandle<List<string>> newAssets)
    {
        List<string> assets = new List<string>();
        double dataSize = 0f;
        if (newAssets.Task.IsCompleted && newAssets.Status == AsyncOperationStatus.Succeeded)
        {
            if (newAssets.Task.Result.Count > 0)
            {
                for (int i = 0; i < newAssets.Task.Result.Count; i++)
                {
                    AsyncOperationHandle<long> size = Addressables.GetDownloadSizeAsync(newAssets.Task.Result[i]);
                    assets.Add(newAssets.Task.Result[i]);
                    await size.Task;
                    dataSize += (size.Task.Result / 1000000f);
                }
            }
        }

        return new Tuple<List<string>, double>(assets, dataSize);
    }

    public static async void DownloadAllCards()
    {
        
        if (!isDownloading)
        {
            List<string> keys = new List<string>();
            keys.Add("Card");
            keys.Add("FullCard");
            AsyncOperationHandle<IList<IResourceLocation>> locations = Addressables.LoadResourceLocationsAsync(keys);
            await locations.Task;
            int downloadExpected = locations.Result.Count;
            int count = 0;
            if (downloadExpected > 0)
            {

                foreach (IResourceLocation location in locations.Result)
                {
                    AsyncOperationHandle<long> size = Addressables.GetDownloadSizeAsync(location);
                    await size.Task;

                    if (size.Result != 0)
                    {
                        AppManager.Instance.loadingBar.SetText("Downloading Card Image");
                        if (location.ResourceType == typeof(Sprite))
                        {
                            isDownloading = true;

                            AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(location);
                            await handle.Task;

                            //if (handle.Status == AsyncOperationStatus.Succeeded && handle.IsDone)
                            //{
                                
                            //}
                            ItemDownloaded(1f);
                            isDownloading = false;
                            count += 1;
                            if (count >= locations.Result.Count - 1)
                            {
                                DownloadComplete();
                            }

                        }

                    }
                    else
                    {
                        if (location.ResourceType == typeof(Sprite))
                        {
                           //ItemDownloaded(1f);
                            AppManager.Instance.loadingBar.SetText("Initializing Card Images");

                        }
                    }
                        ItemDownloaded(1f);
                    if (count >= locations.Result.Count - 1)
                    {
                        DownloadComplete();
                    }

                }
            }
            else
            {
                DownloadComplete();
            }

        }

    }

    public static async void DoRedownloadAllCards()
    {
        List<string> keys = new List<string>();
        keys.Add("Card");
        keys.Add("FullCard");
        AsyncOperationHandle<IList<IResourceLocation>> locations = Addressables.LoadResourceLocationsAsync(keys);
        await locations.Task;
        isDownloading = false;
        Addressables.Release(locations);
        Addressables.ClearDependencyCacheAsync(locations.Result);
        DownloadAllCards();
    }

    public static async void PreloadFullCards()
    {

        AsyncOperationHandle<IList<IResourceLocation>> locations = Addressables.LoadResourceLocationsAsync("FullCard");

        await locations.Task;

        int count = 0;
        foreach (IResourceLocation location in locations.Result)
        {
            if (location.ResourceType == typeof(Sprite))
            {
                AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(location);
                await handle.Task;
                count += 1;
                ItemDownloaded(1f);
            }
        }
        DownloadComplete();

    }
    #endregion

    protected static void SetDefaultImage(AsyncOperationHandle handle, Exception ex)
    {
       
    }

    public static async Task<T> ByKeyAsync<T>(string key, string fallback = "")
    {
        //await GameManager.Instance.downloadManager.ShowDownloadStatus(items);

        UnityEngine.ResourceManagement.ResourceManager.ExceptionHandler = (op, ex) => SetDefaultImage(op, ex);
        AsyncOperationHandle<T> sp = Addressables.LoadAssetAsync<T>(key);
        T go = await sp.Task;

        if (sp.Status == AsyncOperationStatus.Failed && !string.IsNullOrEmpty(fallback))
        {
            //Debug.Log($"Asset {key} not found");
            sp = Addressables.LoadAssetAsync<T>(key);
            go = await sp.Task;

            UnityEngine.ResourceManagement.ResourceManager.ExceptionHandler = (op, ex) => throw ex;
            return go;
        }

       
        if (sp.Status == AsyncOperationStatus.Succeeded && sp.IsDone)
        {
            return sp.Result;
        }
        else
        {
            return default(T);
        }
    }
    public static T ByKey<T>(string key, string fallback = "")
    {
        UnityEngine.ResourceManagement.ResourceManager.ExceptionHandler = (op, ex) => SetDefaultImage(op, ex);
        var op = Addressables.LoadAssetAsync<T>(key);
        T go = op.WaitForCompletion();

        if (op.Status == AsyncOperationStatus.Failed && !string.IsNullOrEmpty(fallback))
        {
            //Debug.Log($"Asset {key} not found");
            op = Addressables.LoadAssetAsync<T>(fallback);
            go = op.WaitForCompletion();
        }

        UnityEngine.ResourceManagement.ResourceManager.ExceptionHandler = (op, ex) => throw ex;
        return go;
    }



    public static void Release<T>(T obj)
    {
        Addressables.Release(obj);
    }

    public static async Task<GameObject> GameObjectCloneAsync(string key, Transform parent)
    {
        GameObject go = await ByKeyAsync<GameObject>(key);
        GameObject clone = MonoBehaviour.Instantiate(go, parent);
        return clone;

    }

    public static GameObject GameObjectClone(string key, Transform parent)
    {
        GameObject go = ByKey<GameObject>(key);
        GameObject clone = MonoBehaviour.Instantiate(go, parent);
        return clone;

    }

    public static async Task<GameObject> WorldObjectClone(string key)
    {
        GameObject go = await ByKeyAsync<GameObject>(key);
        GameObject clone = MonoBehaviour.Instantiate(go);
        return clone;

    }

   

    //public static GameObject BattleObjectClone(string key, Transform parent, )


}
