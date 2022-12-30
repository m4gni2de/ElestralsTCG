using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using AppManagement.Loading;
using CardsUI.Stones;
using Mono.Cecil;
using Mono.Cecil.Cil;
using nsSettings;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.U2D;
using static CardsUI.TypeStones;
using static Elestral;

public class CardFactory : MonoBehaviour
{



    public static CardFactory Instance { get; private set; }


    public CardView templateCard;


    public Sprite defaultFrame, frameShadow;
    
    [Header("Stone Bottoms")]
    public Sprite elestralStone;
    public Sprite runeStone;
    public Sprite spiritStone;
    public Sprite faElestralStone;
    public Sprite faRuneStone;
   
    [Header("Glows")]
    public Sprite fireGlowL;
    public Sprite earthGlowL;
    public Sprite waterGlowL;
    public Sprite thunderGlowL;
    public Sprite windGlowL;
    public Sprite darkGlowL;
    public Sprite lightGlowL;
    public Sprite rainbowGlowL;
    public Sprite spiritGlowL;
    public Sprite spiritGlowR;

    public Sprite swordSp;
    public Sprite shieldSp;

   
    public static Dictionary<SubClass, Sprite> SubClassSprites = new Dictionary<SubClass, Sprite>();
    public static Dictionary<ElementCode, Sprite> BackgroundSprites = new Dictionary<ElementCode, Sprite>();
    public static Dictionary<ElementCode, Sprite> TypeStoneSprites = new Dictionary<ElementCode, Sprite>();
    public static Dictionary<ElementCode, Sprite> LargeStoneSprites = new Dictionary<ElementCode, Sprite>();
    public static Dictionary<string, Sprite> GlowSprites = new Dictionary<string, Sprite>();
    public static Dictionary<Rarity, Sprite> RaritySprites = new Dictionary<Rarity, Sprite>();
    public static Dictionary<Rune.RuneType, Sprite> RuneSprites = new Dictionary<Rune.RuneType, Sprite>();
    public static Dictionary<string, Sprite> SetStampSprites = new Dictionary<string, Sprite>();

    #region Sleeves
    public static readonly string DefaultSleeves = "sleeves0";

    private static Sprite _cardBackSp = null;
    public static Sprite cardBackSp
    {
        get
        {
            if (_cardBackSp == null)
            {
                
                int sleeveIndex = SettingsManager.Account.Settings.Sleeves;
                string asset = $"sleeves{sleeveIndex}";
                _cardBackSp = AssetPipeline.ByKey<Sprite>(asset, DefaultSleeves);
            }
            return _cardBackSp;
        }
    }

    private static Sprite _DefaultCardBack = null;
    public static Sprite DefaultCardBack
    {
        get
        {
            if (_DefaultCardBack == null)
            {
                _DefaultCardBack = AssetPipeline.ByKey<Sprite>(DefaultSleeves, DefaultSleeves);
            }
            return _DefaultCardBack;
        }
    }

    public static Sprite CardSleeveSprite(int sleeveIndex)
    {
        string asset = $"sleeves{sleeveIndex}";
        return AssetPipeline.ByKey<Sprite>(asset, DefaultSleeves);
    }
    #endregion
    #region Playmatt
    private static readonly string PlayMattAsset = "playmatt";
    private static readonly string PlaymattFallback = "playmatt0";
    public static Sprite PlaymattSprite(int matIndex)
    {
        string assetString = $"{PlayMattAsset}{matIndex}";
        Sprite sp = AssetPipeline.ByKey<Sprite>(assetString, PlaymattFallback);
        return sp;
    }
    #endregion

    private static Sprite _borderSp = null;
    
    public static Sprite borderSp
    {
        get
        {
            _borderSp ??= AssetPipeline.ByKey<Sprite>("cardBorder");
            return _borderSp;
        }
    }
    private static Sprite _blankCardSp = null;
    public static Sprite blankCardSp
    {
        get
        {
            _blankCardSp ??= AssetPipeline.ByKey<Sprite>("blankCardSp");
            return _blankCardSp;
        }
    }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }
    }

    private void Start()
    {
        DoLoad();
    }



    #region Asset Loading
    //public static event Action OnAssetsLoadComplete;

    public static bool AssetsLoaded = false;

    private async void DoLoad()
    {
        AppManager.AssetsLoadComplete += SetAssetsLoaded;
        List<string> keys = await AssetPipeline.GetAssetList<Sprite>("Preload", "CardArt");
        AppManager.Instance.DoMultipleAssetLoad<Sprite>(keys);
        //List<string> loadList = await AssetPipeline.GetPreloadList<Sprite>("Preload");
        //AppManager.Instance.DoMultipleAssetLoad<Sprite>(loadList);
    }

    private void SetAssetsLoaded()
    {
        AssetsLoaded = true;
        AppManager.AssetsLoadComplete -= SetAssetsLoaded;
    }
    #endregion

   

    
    public static async void LoadSprites()
    {

        
        await LoadSubClassSprites();
        await LoadElementSprites();
        await LoadRaritySprites();
        await LoadRuneSprites();

       
    }

    #region Misc Sprites


    private async Task<Sprite> SetCardSprite(string key)
    {
        return await AssetPipeline.AwaitDownload<Sprite>(Addressables.LoadAssetAsync<Sprite>(key));
    }

    public static Sprite GetSetStamp(string setName)
    {
        
        if (CardLibrary.GameSets.ContainsKey(setName))
        {
            string key = $"{CardLibrary.GameSets[setName].stamp}Stamp";
            return AssetPipeline.ByKey<Sprite>(key);
        }
        return null;
    }


    #endregion

    #region Sub-Class Sprites
    private static async Task LoadSubClassSprites()
    {
        
        int total = Enum.GetNames(typeof(Elestral.SubClass)).Length;
        ScreenManager.Instance.LoadingBarDisplay("Loading Elestral Sprites", 0f, total);
        for (int i = 0; i < total; i++)
        {
            Elestral.SubClass code = (Elestral.SubClass)i;
            if (code != Elestral.SubClass.None)
            {
                string key = code.ToString().ToLower() + "Sp";
                Sprite sp = await AssetPipeline.AwaitDownload<Sprite>(Addressables.LoadAssetAsync<Sprite>(key));
                SubClassSprites.Add(code, sp);
            }
           

        }
    }
    public static Sprite GetSubClassSprite(Elestral.SubClass code)
    {
        if (SubClassSprites.ContainsKey(code)) { return SubClassSprites[code]; }
        string key = code.ToString().ToLower() + "Sp";
        return AssetPipeline.ByKey<Sprite>(key);
    }


    #endregion

    #region Elements

    private static async Task LoadElementSprites()
    {
        int total = Enum.GetNames(typeof(ElementCode)).Length;
        ScreenManager.Instance.LoadingBarDisplay("Loading Element Sprites", 0f, total * 2);
        for (int i = 0; i < total; i++)
        {
            ElementCode code = (ElementCode)i;

            if (code != ElementCode.None)
            {
                string key = $"bg_{code.ToString().ToLower()}";
                Sprite sp = await AssetPipeline.AwaitDownload<Sprite>(Addressables.LoadAssetAsync<Sprite>(key));
                BackgroundSprites.Add(code, sp);
                string stoneKey = code.ToString().ToLower() + "Stone";
                Sprite stoneSp = await AssetPipeline.AwaitDownload<Sprite>(Addressables.LoadAssetAsync<Sprite>(stoneKey));
                TypeStoneSprites.Add(code, stoneSp);
                string glowKey = code.ToString().ToLower() + "L";
                Sprite glowSp = await AssetPipeline.AwaitDownload<Sprite>(Addressables.LoadAssetAsync<Sprite>(glowKey));
                GlowSprites.Add($"{code.ToString()}L", glowSp);

                string glowKeyR = code.ToString().ToLower() + "R";
                Sprite glowSpR = await AssetPipeline.AwaitDownload<Sprite>(Addressables.LoadAssetAsync<Sprite>(glowKeyR));
                GlowSprites.Add($"{code.ToString()}R", glowSpR);


                string largeKey = code.ToString() + "Symbol_Large";
                Sprite largeSp = await AssetPipeline.AwaitDownload<Sprite>(Addressables.LoadAssetAsync<Sprite>(largeKey));
                LargeStoneSprites.Add(code, largeSp);
            }
        }
    }

    public static Sprite GetBackground(ElementCode code)
    {
        if (BackgroundSprites.ContainsKey(code)) { return BackgroundSprites[code]; }
        string key = $"bg_{code.ToString().ToLower()}";
        return AssetPipeline.ByKey<Sprite>(key);
    }
    public static Sprite GetGlowSprite(ElementCode code, bool isLeft)
    {
        string assetCode = $"{code.ToString()}L";
        if (!isLeft)
        {
            assetCode = $"{code.ToString()}R";
        }
        if (GlowSprites.ContainsKey(assetCode)) { return GlowSprites[assetCode]; }
        string glowKey = code.ToString().ToLower() + "L";
        if (!isLeft)
        {
            glowKey = code.ToString().ToLower() + "R";
        }
        Sprite sp = AssetPipeline.ByKey<Sprite>(glowKey);
        if (!GlowSprites.ContainsKey(assetCode)) { GlowSprites.Add(assetCode, sp); }
        return sp;
    }
    public static Sprite GetTypeStoneSprite(ElementCode code)
    {
        if (TypeStoneSprites.ContainsKey(code)) { return TypeStoneSprites[code]; }
        string stoneKey = code.ToString().ToLower() + "Stone";
        Sprite sp = AssetPipeline.ByKey<Sprite>(stoneKey);
        if (!TypeStoneSprites.ContainsKey(code)) { TypeStoneSprites.Add(code, sp); }
        return sp;
    }
    public static Sprite GetLargeStoneSprite(ElementCode code)
    {
        if (LargeStoneSprites.ContainsKey(code)) { return LargeStoneSprites[code]; }
        string stoneKey = code.ToString() + "Symbol_Large";
        Sprite sp = AssetPipeline.ByKey<Sprite>(stoneKey);
        if (!LargeStoneSprites.ContainsKey(code)) { LargeStoneSprites.Add(code, sp); }
        return sp;
    }
    #endregion

    #region Rarity
    private static async Task LoadRaritySprites()
    {
        int total = Enum.GetNames(typeof(Rarity)).Length;
        ScreenManager.Instance.LoadingBarDisplay("Loading Rarity Sprites", 0f, total);
        for (int i = 0; i < total; i++)
        {
            Rarity code = (Rarity)i;
            string key = code.SpriteString();
            if (!string.IsNullOrEmpty(key))
            {
                Sprite sp = await AssetPipeline.AwaitDownload<Sprite>(Addressables.LoadAssetAsync<Sprite>(key));
                RaritySprites.Add(code, sp);
            }
           

        }

    }

    public static Sprite GetRaritySprite(Rarity ty)
    {
        if (RaritySprites.ContainsKey(ty))
        {
            return RaritySprites[ty];
        }
        else
        {
            return AssetPipeline.ByKey<Sprite>(ty.SpriteString());
        }
    }
    #endregion

    #region Runes
    private static async Task LoadRuneSprites()
    {
        int total = Enum.GetNames(typeof(Rune.RuneType)).Length;
        ScreenManager.Instance.LoadingBarDisplay("Loading Rune Sprites", 0f, total);
        for (int i = 0; i < total; i++)
        {
            
            Rune.RuneType code = (Rune.RuneType)i;
            string key = code.RuneSpriteCode();
            if (!string.IsNullOrEmpty(key))
            {
                Sprite sp = await AssetPipeline.AwaitDownload<Sprite>(Addressables.LoadAssetAsync<Sprite>(key));
                if (sp != null)
                {
                    RuneSprites.Add(code, sp);
                }
            }
           
            
        }
    }


    public static Sprite GetRuneSprite(Rune.RuneType ty)
    {
        if (RuneSprites.ContainsKey(ty))
        {
            return RuneSprites[ty];
        }
        else
        {
            return AssetPipeline.ByKey<Sprite>(ty.RuneSpriteCode());
        }
    }

    #endregion

   
    #region Static Lookups

    public static Sprite CardImage(Card card)
    {
        return AssetPipeline.ByKey<Sprite>(card.cardData.image.ToLower(), "defaultcard");
    }
    public static Material CardFontMaterial(ElementCode code, string suffix)
    {
        string assetKey = $"{code.ToString().ToLower()}Font{suffix}";
        switch (code)
        {
            case ElementCode.Wind: 
            case ElementCode.Earth: 
            case ElementCode.Fire: 
            case ElementCode.Thunder: 
            case ElementCode.Water:
                return AssetPipeline.ByKey<Material>(assetKey);
            default:
                return null;
        }
        
    }
#endregion


    public static CardView CreateCard(Transform parent, Card card = null, bool isFaceDown = false)
    {
        CardView view = Instantiate(Instance.templateCard, parent);
        view.LoadCard(card, isFaceDown);
        return view;
    }


    public static CardView Copy(CardView original, Transform parent, Card card)
    {
        CardView co = Instantiate(original, parent);
        co.LoadCard(card);
        return co;
    }


    
}
