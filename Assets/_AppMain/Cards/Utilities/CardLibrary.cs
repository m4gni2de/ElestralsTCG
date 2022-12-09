using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;
using UnityEngine.AddressableAssets;
using Databases;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Packs;

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

    public static readonly string StampDefault = "prototype";
    private static Dictionary<string, GameSetDTO> _GameSets = null;
    public static Dictionary<string, GameSetDTO> GameSets
    {
        get
        {
            if (_GameSets == null)
            {
                _GameSets ??= new Dictionary<string, GameSetDTO>();
                List<GameSetDTO> list = DataService.GetAll<GameSetDTO>("GameSetDTO");
                foreach (var item in list)
                {
                    _GameSets.Add(item.setAbbr, item);
                }
            }
            
            return _GameSets;
        }
    }

    #endregion

    #region Events

    #endregion




    public static Sprite GetCardArt(Card card)
    {
        //string cardKey = ArtImageText(card).ToLower();
        string cardKey = card.cardData.image.ToLower();
        Sprite sp = AssetPipeline.ByKey<Sprite>(cardKey, DefaultCardKey);
        //if (CardArt.ContainsKey(cardKey)) { return CardArt[cardKey]; }
        //Sprite sp = AssetPipeline.ByKey<Sprite>(cardKey, DefaultCardKey);
        //if (!CardArt.ContainsKey(cardKey)) { CardArt.Add(cardKey, sp); }
        return sp;
    }

    #region Full Card Sprites
    public static Sprite GetFullCard(Card card)
    {
        string cardKey = card.cardData.image.ToLower() + "_c"; ;
        if (CardArt.ContainsKey(cardKey)) { return CardArt[cardKey]; }
        Sprite sp = AssetPipeline.ByKey<Sprite>(cardKey, DefaultCardKey);
        if (!CardArt.ContainsKey(cardKey)) { CardArt.Add(cardKey, sp); }
        return sp;
    }

    public static async Task<Sprite> GetFullCardAsync(Card card)
    {
        string cardKey = card.cardData.image.ToLower() + "_c"; ;
        if (CardArt.ContainsKey(cardKey)) { return CardArt[cardKey]; }
        Sprite sp = await AssetPipeline.ByKeyAsync<Sprite>(cardKey, DefaultCardKey);
        if (!CardArt.ContainsKey(cardKey)) { CardArt.Add(cardKey, sp); }
        return sp;
    }


    public static string GetCardSetStamp(string setName)
    {
        if (GameSets.ContainsKey(setName))
        {
            return GameSets[setName].stamp + "Stamp";
        }
        return StampDefault + "Stamp";
    }
    public static int SetCount(string setName)
    {
        if (GameSets.ContainsKey(setName))
        {
            return GameSets[setName].cardCount;
        }
        return 0;
    }
    public static GameSetDTO GetGameSet(string setName)
    {
        if (GameSets.ContainsKey(setName))
        {
            return GameSets[setName];
        }
        return null;
    }

    #endregion





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

   
    public static string CostString(string costInt)
    {
        int cost = int.Parse(costInt);
        string costString = "";
        if (cost == 0)
        {
            return "(cardClass = 0)";
        }
        if (cost == 1)
        {
            return "(cost1 > -1) And (cost2 is null or cost2 = -1) and (cost3 is null or cost3 = -1) and (cardClass <> 0)";
        }
        if (cost == 2)
        {
            return "(cost1 > -1) And (cost2 > -1) and (cost3 is null or cost3 = -1) and (cardClass <> 0)";
        }
        if (cost == 3)
        {
            return "(cost1 > -1) And (cost2 > -1) and (cost3 > -1) and (cardClass <> 0)";
        }
        return costString;
    }
}
