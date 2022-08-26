using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;
using UnityEngine.AddressableAssets;
using Databases;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class CardLibrary 
{
    #region Properties
    public static readonly string DefaultCardKey = "defaultcard";
    private static Dictionary<string, Sprite> _CardArt = null;
    public static Dictionary<string, Sprite> CardArt
    {
        get
        {
            _CardArt ??= new Dictionary<string, Sprite>();
            return _CardArt;
        }
    }

    private static Dictionary<string, Sprite> _CardBackgrounds = null;
    public static Dictionary<string, Sprite> CardBackgrounds
    {
        get
        {
            _CardBackgrounds ??= new Dictionary<string, Sprite>();
            return _CardBackgrounds;
        }
    }

    #endregion

    #region Events

    #endregion

    

    public static async Task<Sprite> GetCardArtAsync(Card card)
    {
        string cardKey = ArtImageText(card).ToLower();
        if (CardArt.ContainsKey(cardKey)) { return CardArt[cardKey]; }
        Sprite sp = await AssetPipeline.ByKeyAsync<Sprite>(cardKey, DefaultCardKey);
        if (!CardArt.ContainsKey(cardKey)) { CardArt.Add(cardKey, sp); }
        return sp;
    }

    public static Sprite GetCardArt(Card card)
    {
        string cardKey = ArtImageText(card).ToLower();
        if (CardArt.ContainsKey(cardKey)) { return CardArt[cardKey]; }
        Sprite sp = AssetPipeline.ByKey<Sprite>(cardKey, DefaultCardKey);
        if (!CardArt.ContainsKey(cardKey)) { CardArt.Add(cardKey, sp); }
        return sp;
    }

    #region Full Card Sprites
    public static Sprite GetFullCard(Card card)
    {
        string cardKey = ArtImageText(card).ToLower() + "_c"; ;
        if (CardArt.ContainsKey(cardKey)) { return CardArt[cardKey]; }
        Sprite sp = AssetPipeline.ByKey<Sprite>(cardKey, DefaultCardKey);
        if (!CardArt.ContainsKey(cardKey)) { CardArt.Add(cardKey, sp); }
        return sp;
    }
    public static async Task<Sprite> GetFullCardAsync(Card card)
    {
        string cardKey = ArtImageText(card).ToLower() + "_c"; ;
        if (CardArt.ContainsKey(cardKey)) { return CardArt[cardKey]; }
        Sprite sp = await AssetPipeline.ByKeyAsync<Sprite>(cardKey, DefaultCardKey);
        if (!CardArt.ContainsKey(cardKey)) { CardArt.Add(cardKey, sp); }
        return sp;
    }

    public static async Task PreloadFullCards()
    {
        string key = "FullCard";
        AsyncOperationHandle<IList<IResourceLocation>> locations = Addressables.LoadResourceLocationsAsync(key);
        await locations.Task;
        AppManager.Instance.ShowLoadingBar("Items Downloaded", 0f, locations.Result.Count);
        AssetPipeline.OnItemDownloaded += AppManager.Instance.loadingBar.MoveSlider;
        AssetPipeline.OnDownloadComplete += AppManager.Instance.loadingBar.CompleteLoad;
        bool complete = await AppManager.AwaitLoading(AppManager.Instance.loadingBar, AssetPipeline.PreloadFullCards);
        AssetPipeline.OnItemDownloaded -= AppManager.Instance.loadingBar.MoveSlider;
        AssetPipeline.OnDownloadComplete -= AppManager.Instance.loadingBar.CompleteLoad;

    }
    #endregion


    protected static string ArtImageText(Card card)
    {
        string cardKey = card.cardData.cardKey;
        string artCode = CardService.CardArtString(cardKey);
        return artCode;
    }

    public static async Task<Sprite> GetBackgroundAsync(Card card)
    {
        string fallback = "bg_rainbow";
        string assetName = BackgroundText(card.SpiritsReq[0].Code);

        if (CardBackgrounds.ContainsKey(assetName)) { return CardBackgrounds[assetName]; }
        Sprite sp = await AssetPipeline.ByKeyAsync<Sprite>(assetName, fallback);
        if (!CardBackgrounds.ContainsKey(assetName)) { CardBackgrounds.Add(assetName, sp); }
        return sp;
    }

    public static Sprite GetBackground(Card card)
    {
        string fallback = "bg_rainbow";
        string assetName = BackgroundText(card.SpiritsReq[0].Code);

        if (CardBackgrounds.ContainsKey(assetName)) { return CardBackgrounds[assetName]; }
        Sprite sp = AssetPipeline.ByKey<Sprite>(assetName, fallback);
        if (!CardBackgrounds.ContainsKey(assetName)) { CardBackgrounds.Add(assetName, sp); }
        return sp;
    }

    protected static string BackgroundText(ElementCode code)
    {
        string st = $"bg_{code.ToString().ToLower()}";
        return st;
    }

    public static void OnCardDownloaded()
    {

    }

   
}
