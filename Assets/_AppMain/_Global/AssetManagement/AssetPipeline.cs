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
using System.Drawing;

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

    public static async Task<List<string>> GetPreloadList<T>(string locationName)
    {
        AsyncOperationHandle<IList<IResourceLocation>> locations = Addressables.LoadResourceLocationsAsync(locationName);
        await locations.Task;
        int expectedCount = locations.Result.Count;

        List<string> list = new List<string>();
        foreach (IResourceLocation location in locations.Result)
        {
            if (location.ResourceType == typeof(T))
            {
                list.Add(location.PrimaryKey);
            }


        }
        return list;
    }

    public static async Task<List<string>> GetAssetList<T>(params string[] assetTags)
    {
        List<string> keys = new List<string>();
        for (int i = 0; i < assetTags.Length; i++)
        {
            string locationName = assetTags[i];

            AsyncOperationHandle<IList<IResourceLocation>> locations = Addressables.LoadResourceLocationsAsync(locationName);
            await locations.Task;
            int expectedCount = locations.Result.Count;

            
            foreach (IResourceLocation location in locations.Result)
            {
                if (location.ResourceType == typeof(T))
                {
                    keys.Add(location.PrimaryKey);
                }


            }
        }
        return keys;
    }

    public static async void DownloadAllCards()
    {
        
        if (!isDownloading)
        {
            List<string> keys = new List<string>();
            keys.Add("CardArt");
            //keys.Add("FullCard");
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

    public static async Task<T> AwaitDownload<T>(AsyncOperationHandle<T> handle)
    {
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

   

    public static void DoRedownloadAllCards()
    {

        List<qUniqueCard> allCards = DataService.GetAllWhere<qUniqueCard>(Cards.CardService.qUniqueCardView, "setName is not null");
        isDownloading = true;
        LoadingBar.Instance.Display("Initializing Card Assets", 0f, allCards.Count, 0f);


        List<string> cardKeys = new List<string>();
        foreach (qUniqueCard card in allCards)
        {
            string imageKey = card.image.ToLower();

            cardKeys.Add(imageKey);
            //AsyncOperationHandle<long> size = Addressables.GetDownloadSizeAsync(imageKey);
            //await size.Task;
            //if (size.Result > 0f && size.Status == AsyncOperationStatus.Succeeded)
            //{
            //    LoadingBar.Instance.Display($"Initializing Card Assets", count, 1f, 0f);

            //    cardKeys.Add(imageKey);

            //    AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(imageKey);
            //    handle.Completed += SpriteHandle_Completed;
            //    Sprite s = await AwaitDownload<Sprite>(Addressables.LoadAssetAsync<Sprite>(imageKey));
            //    if (LoadingBar.Instance == null) { return; }
            //    if (!CardLibrary.CardArt.ContainsKey(imageKey)) { CardLibrary.CardArt.Add(imageKey, s); }
            //    LoadingBar.Instance.Display($"Completing {imageKey}", count, allCards.Count, 0f);
            //    count += 1;
            //    LoadingBar.Instance.MoveSlider(1f);

            //}
            //else
            //{
            //    //Sprite sp = await ByKeyAsync<Sprite>(imageKey, CardLibrary.DefaultCardKey);
            //    LoadingBar.Instance.MoveSlider(1f);
            //    count += 1;

            //}

            //if (count >= allCards.Count - 1)
            //{
            //    DownloadComplete();
            //    isDownloading = false;
            //}

        }

        AppManager.Instance.DoMultipleAssetLoad<Sprite>(cardKeys);
       
    }

    private static void SpriteHandle_Completed(AsyncOperationHandle<Sprite> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Sprite result = handle.Result;
            LoadingBar.Instance.SetText($"Completing {result.name}");
            LoadingBar.Instance.MoveSlider(1f);
            // The texture is ready for use.
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

   
    public static T ByKey<T>(string key, string fallback = "")
    {
        UnityEngine.ResourceManagement.ResourceManager.ExceptionHandler = (op, ex) => SetDefaultImage(op, ex);
        LoadingBar.Instance.Display($"Downloading {key}", 0f, 1f, 0f);


        
        var op = Addressables.LoadAssetAsync<T>(key);
        //op.Completed += OnCompleted;
        T go = op.WaitForCompletion();
        //op.Completed -= OnCompleted;

        if (op.Status == AsyncOperationStatus.Failed && !string.IsNullOrEmpty(fallback))
        {
           
            op = Addressables.LoadAssetAsync<T>(fallback);
            go = op.WaitForCompletion();
        }

        LoadingBar.Instance.SetSlider(1f);
        LoadingBar.Instance.gameObject.SetActive(false);
        UnityEngine.ResourceManagement.ResourceManager.ExceptionHandler = (op, ex) => throw ex;
        return go;
    }

    private static void OnCompleted<T>(T obj)
    {
        LoadingBar.Instance.Display($"Downloading Complete", 0f, 0f, 0f);
    }

    public static async Task<T> ByKeyAsync<T>(string key, string fallback = "")
    {
        UnityEngine.ResourceManagement.ResourceManager.ExceptionHandler = (op, ex) => SetDefaultImage(op, ex);
        var op = Addressables.LoadAssetAsync<T>(key);
        T go = await op.Task;

        if (op.Status == AsyncOperationStatus.Failed && !string.IsNullOrEmpty(fallback))
        {
           
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

   
    public static T OfGameObject<T>(string key)
    {
        LoadingBar.Instance.Display($"Downloading {key}", 0f, 1f, 0f);
        GameObject go = ByKey<GameObject>(key);
        return go.GetComponent<T>();
    }
    public static async Task<T> OfGameObjectAsync<T>(string key)
    {
        LoadingBar.Instance.Display($"Downloading {key}", 0f, 1f, 0f);
        GameObject go = await ByKeyAsync<GameObject>(key);
        return go.GetComponent<T>();
    }

    public static GameObject GameObjectClone(string key, Transform parent)
    {
        GameObject go = ByKey<GameObject>(key);
        GameObject clone = MonoBehaviour.Instantiate(go, parent);
        clone.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
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
