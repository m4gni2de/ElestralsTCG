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
        //OnDownloadComplete?.Invoke();
        LoadingBar.Instance.SetSlider(LoadingBar.Instance.slider.maxValue);
        LoadingBar.Instance.Hide();
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

            LoadingBar.Instance.Display("Downloading Assets", 0f, (float)downloadExpected, 0f);
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
                            LoadingBar.Instance.MoveSlider(1f);
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
                            LoadingBar.Instance.MoveSlider(1f);
                            AppManager.Instance.loadingBar.SetText("Initializing Card Images");

                        }
                    }

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

    public static async Task<T> AwaitDownload<T>(AsyncOperationHandle<T> handle, float size)
    {
        LoadingBar.Instance.Display($"Downloading Card Assets", 0f, 1f, 0f);
        do
        {
            await Task.Delay(1);
            if (LoadingBar.Instance != null)
            {
                LoadingBar.Instance.SetSlider(handle.PercentComplete);
            }
            
        } while (true && !handle.IsDone);

        await handle.Task;
        return handle.Result;
    }

    public static async void DoRedownloadAllCards()
    {

        List<qCards> allCards = DataService.GetAllWhere<qCards>(Cards.CardService.CardsByImageTable, "image is not null");
        isDownloading = true;
        LoadingBar.Instance.Display("Initializing Card Assets", 0f, allCards.Count, 0f);

        int count = 0;
        foreach (qCards card in allCards)
        { 
            string imageKey = card.image.ToLower() + "_c";
            AsyncOperationHandle<long> size = Addressables.GetDownloadSizeAsync(imageKey);
            await size.Task;
            if (size.Result > 0f)
            {
                Sprite s = await AwaitDownload<Sprite>(Addressables.LoadAssetAsync<Sprite>(imageKey), size.Result);
                if (LoadingBar.Instance == null) { return; }
                if (!CardLibrary.CardArt.ContainsKey(imageKey)) { CardLibrary.CardArt.Add(imageKey, s); }
                LoadingBar.Instance.Display("Initializing Card Assets", count, allCards.Count, 0f);
                count += 1;
                LoadingBar.Instance.MoveSlider(1f);

            }
            else
            {
                Sprite sp = await ByKeyAsync<Sprite>(imageKey, CardLibrary.DefaultCardKey);
                LoadingBar.Instance.MoveSlider(1f);
                count += 1;

            }

            if (count >= allCards.Count - 1)
            {
                DownloadComplete();
                isDownloading = false;
            }

        }
       
    }

    public static async void PreloadFullCards()
    {

        AsyncOperationHandle<IList<IResourceLocation>> locations = Addressables.LoadResourceLocationsAsync("FullCard");

        await locations.Task;

        int expectedCount = locations.Result.Count;
        LoadingBar.Instance.Display("Downloading Assets", 0f, (float)expectedCount, 0f);

        int count = 0;
        foreach (IResourceLocation location in locations.Result)
        {
            if (location.ResourceType == typeof(Sprite))
            {
                AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(location);
                await handle.Task;
                count += 1;
                LoadingBar.Instance.MoveSlider(1f);

                
            }
        }
        LoadingBar.Instance.Hide();
        DownloadComplete();

    }
    #endregion

    protected static void SetDefaultImage(AsyncOperationHandle handle, Exception ex)
    {
       
    }

    #region Download Managing
    public static AsyncOperationHandle _activeHandle { get; set; }
    public static event Action<float> OnAssetDownloading;
    public static void Downloading(float val)
    {
        OnAssetDownloading.Invoke(val);
    }

  
#endregion
    public static T ByKey<T>(string key, string fallback = "")
    {
        UnityEngine.ResourceManagement.ResourceManager.ExceptionHandler = (op, ex) => SetDefaultImage(op, ex);
        LoadingBar.Instance.Display("Downloading Assets", 0f, 1f, 0f);
        var op = Addressables.LoadAssetAsync<T>(key);
        T go = op.WaitForCompletion();

        if (op.Status == AsyncOperationStatus.Failed && !string.IsNullOrEmpty(fallback))
        {
            //Debug.Log($"Asset {key} not found");
            op = Addressables.LoadAssetAsync<T>(fallback);
            go = op.WaitForCompletion();
        }

        LoadingBar.Instance.SetSlider(1f);
        LoadingBar.Instance.gameObject.SetActive(false);
        UnityEngine.ResourceManagement.ResourceManager.ExceptionHandler = (op, ex) => throw ex;
        return go;
    }

    public static async Task<T> ByKeyAsync<T>(string key, string fallback = "")
    {
        UnityEngine.ResourceManagement.ResourceManager.ExceptionHandler = (op, ex) => SetDefaultImage(op, ex);
        var op = Addressables.LoadAssetAsync<T>(key);
        T go = await op.Task;

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

   
    public static GameObject GameObjectClone(string key, Transform parent)
    {
        GameObject go = ByKey<GameObject>(key);
        GameObject clone = MonoBehaviour.Instantiate(go, parent);
        return clone;

    }
    public static GameObject WorldObjectClone(string key)
    {
        GameObject go = ByKey<GameObject>(key);
        GameObject clone = MonoBehaviour.Instantiate(go);
        return clone;

    }


    //public static GameObject BattleObjectClone(string key, Transform parent, )


}
