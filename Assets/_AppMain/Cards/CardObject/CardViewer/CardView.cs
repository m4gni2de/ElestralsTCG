using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cards;
using CardsUI;
using System;
using System.Threading.Tasks;
using Gameplay;
using static Decks.Decklist;
using CardsUI.Glowing;
using Databases;

public class CardView : MonoBehaviour, iRemoteAsset
{
    public static string AssetName { get { return RemoteAssetHelpers.GetAssetName<CardView>(); } }
    public static string BorderMapping = "Border";
    public static readonly Vector2 GameSize = new Vector2(65f, 90f);

    #region Properties
    public string CardName;
    public string CardSessionId;
    public Card ActiveCard;
    public TouchObject touch;
    public int cardIndex;
    #endregion


    #region Card Building
    [Header("Card Building")]
    [SerializeField]
    protected GameObject Container;
    [Tooltip("This is used to display a single sprite across the entire card, such as the card back, or a masking.")]
    [SerializeField]
    protected SpriteDisplay FlatImage;

    [SerializeField]
    protected CardConfig _cardConfig = null;
    public virtual CardConfig CurrentConfig
    {
        get
        {
           
            return _cardConfig;
        }
    }

    private MultiImage _images = null;
    public MultiImage Images { get { _images ??= GetComponent<MultiImage>(); return _images; } }

    public SpriteDisplay borderSp
    {
        get
        {
            return Images.FromKey(BorderMapping);
        }
    }

    protected bool _isFullArt = false;
    public virtual bool IsFullArt
    {
        get { return _isFullArt; }
        set
        {
            _isFullArt = value;
        }
    }

    //public CardConfig CurrentConfig
    //{
    //    get
    //    {
    //        if (IsFullArt) { return FullArt; } return Default;
    //    }
    //}

    protected virtual void DoArtChange(bool isFullArt)
    {

        //Default.gameObject.SetActive(!isFullArt);
        //FullArt.gameObject.SetActive(isFullArt);
    }

    public SpriteRenderer CardImageSp { get { return CurrentConfig.CardImageSp; } }

    public SpriteRenderer BackgroundSp { get { return CurrentConfig.BackgroundSp; } }

    public GlowControls glowControls { get { return CurrentConfig.glowControls; } }
    public CardTexts Texts { get { return CurrentConfig.Texts; } }

    //public StoneBottom Bottom { get { return CurrentConfig.Bottom; } }


    #endregion

    #region static Functions
    public static CardView GenerateCard(CardView template, Transform parent, Card card = null, bool flip = false)
    {
        if (card == null)
        {
            return Empty(template, parent);
        }

        CardConfig config = CardConfig.GetConfig(card);
        CardView cv = Instantiate(template, parent);
        CardConfig clone = Instantiate(config, cv.Container.transform);
        cv.SetCardConfig(clone);
        cv.LoadCard(card, flip);
        clone.name = card.cardData.cardKey + "_Config";
        return cv;

    }

    private static CardView Empty(CardView template, Transform parent)
    {
        CardConfig config = CardConfig.ElestralRegular;
        CardView cv = Instantiate(template, parent);
        CardConfig clone = Instantiate(config, cv.Container.transform);
        cv.SetCardConfig(clone);
        cv.LoadCard();
        clone.name = "EmptyConfig";
        return cv;
    }
    #endregion

    public bool isDragging = false;

    public bool IsFaceUp { get; set; }
    public bool IsVertical
    {
        get
        {
            return transform.localEulerAngles.z == 0f;
        }
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }

    protected void SetCardConfig(CardConfig config)
    {
        _cardConfig = config;
        config.SetWatchers(this);
    }
    public virtual void LoadCard(Card card = null, bool flip = false)
    {
       
        if (card != null)
        {
            //ActiveCard = card;
            //sp.MainSprite = CardLibrary.GetFullCard(card);
            //IsFaceUp = true;

            

            gameObject.SetActive(true);
            ActiveCard = card;
            name = $"{card.cardData.cardKey} - {card.cardData.cardName}";
            IsFullArt = card.isFullArt;
            Flip(flip);
        }
        else
        {
            //ActiveCard = null;
            //sp.MainSprite = AssetPipeline.ByKey<Sprite>("cardbackSp");


            ActiveCard = null;
            name = "empty";
            _isFullArt = false;
            Container.SetActive(false);
            FlatImage.Clear();

            Texts.SetBlank();
            CurrentConfig.StoneVariant.SetBlank();
            glowControls.SetBlank();
            ClearSprites();
            gameObject.SetActive(false);
        }

      
        Show();
        

    }

    #region Card Building
  
    public void MatchSize(Vector2 rectSize)
    {
        Vector2 sizeRatio = rectSize / GameSize;

        Vector2 newScale = Container.transform.localScale *= sizeRatio;
        SetScale(newScale);
    }
    public virtual void SetScale(Vector2 newScale)
    {
        //sp.m_Transform.localScale = newScale;
        Container.transform.localScale = newScale;
        FlatImage.transform.localScale = newScale;
        transform.position = new Vector3(transform.position.x, transform.position.y, -2f);
        
    }


    protected void LoadSprites()
    {
        BackgroundSp.sprite = CardLibrary.GetBackground(ActiveCard);

        Sprite cardArt = CardLibrary.GetCardArt(ActiveCard);
        if (cardArt != null)
        {
            CardImageSp.sprite = cardArt;
        }
        else
        {
            CardImageSp.sprite = null;
        }

    }

    protected void ClearSprites()
    {
        BackgroundSp.sprite = null;
        CardImageSp.sprite = null;
    }

    #endregion

    public virtual void Flip(bool toBack = false)
    {
        if (toBack)
        {
            //sp.MainSprite = AssetPipeline.ByKey<Sprite>("cardbackSp");
            CurrentConfig.ToggleDisplay(false);
            CurrentConfig.StoneVariant.Hide();
            FlatImage.gameObject.SetActive(true);
            FlatImage.SetSprite(AssetPipeline.ByKey<Sprite>("cardbackSp"));
        }
        else
        {
            //sp.MainSprite = CardLibrary.GetFullCard(ActiveCard);
            CurrentConfig.ToggleDisplay(true);
            CurrentConfig.StoneVariant.Set(ActiveCard);
            CurrentConfig.StoneVariant.Show();
            FlatImage.Clear();
            Texts.SetTexts(ActiveCard);
            glowControls.Set(ActiveCard);
            LoadSprites();
        }

        IsFaceUp = !toBack;
    }
    public Vector3 GetScale()
    {
        //return sp.m_Transform.localScale;
        return Container.transform.localScale;
    }
    public virtual void Rotate(bool isTapped)
    {
        if (isTapped)
        {
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 90f);
            
        }
        else
        {
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0f);
        }
    }

    public virtual void SetAsChild(Transform tf, Vector2 scale, string sortLayer = "", int childIndex = -1)
    {
        transform.SetParent(tf);
        
        SetScale(scale);
        if (!string.IsNullOrEmpty(sortLayer)) { SetSortingLayer(sortLayer); }
        if (childIndex > -1)
        {
            transform.SetSiblingIndex(childIndex);
        }
    }


    public event Action<string> OnSortLayerChange;
    public virtual void SetSortingLayer(string sortLayer)
    {
        //sp.SetSortLayer(sortLayer);
        //borderSp.SetSortLayer(sortLayer);

        //Renderer[] rends = GetComponentsInChildren<Renderer>(true);
        //Bottom.SetSortingLayer(sortLayer);

        //for (int i = 0; i < rends.Length; i++)
        //{
        //    rends[i].sortingLayerName = sortLayer;
        //}

        FlatImage.SetSortLayer(sortLayer);
        CurrentConfig.SetSortLayer(sortLayer);
        OnSortLayerChange?.Invoke(sortLayer);
        
    }

    public event Action<int> OnSortOrderChange;
    public virtual void SetSortingOrder(int order)
    {

        //sp.SetSortOrder(order);
        //borderSp.SetSortOrder(sp.SortOrder + 1);


        FlatImage.SetSortOrder(order);
        if (_cardConfig != null)
        {
            CurrentConfig.SetSortOrder(order);
        }
        OnSortOrderChange?.Invoke(order);

        //Renderer[] rends = GetComponentsInChildren<Renderer>(true);
        //Bottom.SetSortingOrder(order + 1);

        //for (int i = 0; i < rends.Length; i++)
        //{
        //    rends[i].sortingOrder = order;
        //}
    }

    public virtual void AddToSortingOrder(int order)
    {
        Renderer[] rends = GetComponentsInChildren<Renderer>(true);

        for (int i = 0; i < rends.Length; i++)
        {
            rends[i].sortingOrder += order;
        }

        
    }

    public void SelectCard(bool toggle, Color col)
    {
        if (toggle)
        {
            SetColor("Border", col);
            Images.ShowSprite("Border");
        }
        else
        {
            SetColor("Border", col);
            Images.HideSprite("Border");
        }
    }

    public void SetColor(string spriteKey, Color color)
    {
        Images.SetColor(spriteKey, color);
    }
    public void MaskCard(Color col)
    {
        FlatImage.SetSprite(AssetPipeline.ByKey<Sprite>(CardUI.BlankSprite));
        FlatImage.SetColor(col);
    }



    #region Network Sync
    public void SendNetworkTransform()
    {
        Vector3 scale = transform.localScale;
        Vector3 localPos = transform.localPosition;
        
    }


    #endregion


    


}
