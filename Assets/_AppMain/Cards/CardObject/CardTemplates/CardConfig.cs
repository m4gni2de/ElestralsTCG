using System.Collections;
using System.Collections.Generic;
using CardsUI;
using CardsUI.Glowing;
using UnityEngine;
using UnityEngine.Profiling;

public class CardConfig  : MonoBehaviour 
{

    #region Static Configs
    private static readonly string _elestralRegularConfig = "ElestralCard";
    private static readonly string _runeRegularConfig = "RuneCard";
    private static readonly string _spiritRegularConfig = "SpiritCard";
    private static readonly string _elestralFAConfig = "ElestralCardFA";
    private static readonly string _runeFAConfig = "RuneCardFA";
    private static readonly string _spiritFAConfig = "SpiritCardFA";

    private static CardConfig _elestralReg = null;
    public static CardConfig ElestralRegular { get => _elestralReg; }
    private static CardConfig _runeReg = null;
    public static CardConfig RuneRegular { get => _runeReg; }
    private static CardConfig _spiritReg = null;
    public static CardConfig SpiritRegular { get => _spiritReg; }

    private static CardConfig _elestralFA = null;
    public static CardConfig ElestralFA { get => _elestralFA; }
    private static CardConfig _runeFA = null;
    public static CardConfig RuneFA { get => _runeFA; }
    private static CardConfig _spiritFA = null;
    public static CardConfig SpiritFA { get => _spiritFA; }

    public static void LoadConfigs()
    {
        _elestralReg = AssetPipeline.OfGameObject<CardConfig>(_elestralRegularConfig);
        _runeReg = AssetPipeline.OfGameObject<CardConfig>(_runeRegularConfig);
        _spiritReg = AssetPipeline.OfGameObject<CardConfig>(_spiritRegularConfig);
        _elestralFA = AssetPipeline.OfGameObject<CardConfig>(_elestralFAConfig);
        _runeFA = AssetPipeline.OfGameObject<CardConfig>(_runeFAConfig);
        _spiritFA = AssetPipeline.OfGameObject<CardConfig>(_spiritFAConfig);

    }
    public static CardConfig GetConfig(Card card)
    {
        CardType ct = card.CardType;

        switch (ct)
        {
            case CardType.None:
                return ElestralRegular;
            case CardType.Spirit:
                if (card.isFullArt) { return SpiritFA; }return SpiritRegular;
            case CardType.Elestral:
                if (card.isFullArt) { return ElestralFA; } return ElestralRegular;
            case CardType.Rune:
                if (card.isFullArt) { return RuneFA; } return RuneRegular;
            default:
                return null;
        }
    }
    #endregion


    #region Propeprties
    public SpriteRenderer CardImageSp;
    public SpriteRenderer BackgroundSp;
    public GameObject cardFrame;
    public CardTexts Texts;
    public GlowControls glowControls;

    [Tooltip("Only prefab CardConfigs have this. If it's a card display that changes configs, this is kept null.")]
    [SerializeField]
    private StoneVariant _stoneBottom;
    public StoneVariant StoneVariant { get { return _stoneBottom; } }
   

    [SerializeField]
    private SortMapping sortMap;
    public SortMapping SortMap { get { return sortMap; } }

    protected bool isWatching = false;
    #endregion

    #region Functions
    public int BaseSortOrder { get { return sortMap.BaseImage.SortOrder; } }
    public string BaseSortLayer { get { return sortMap.BaseImage.SortLayerName; } }
    #endregion

    private void Awake()
    {

    }

   
    public void ToggleDisplay(bool show)
    {
        Texts.gameObject.SetActive(show);
        glowControls.gameObject.SetActive(show);
        CardImageSp.gameObject.SetActive(show);
        BackgroundSp.gameObject.SetActive(show);
        cardFrame.gameObject.SetActive(show);
    }




    public void SetSortOrder(int newOrder)
    {
        sortMap.UpdateBaseOrder(newOrder);
        Texts.SetSortOrder(newOrder + 5);
    }
    public void SetSortLayer(string layer)
    {
        sortMap.UpdateBaseLayer(layer);
        Texts.SetSortLayer(layer);
    }


    public void SetWatchers(CardView view)
    {
        isWatching = true;

       
    }
    
}
