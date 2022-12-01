using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CardView : MonoBehaviour, iRemoteAsset, iCardView
{ 

    #region Interfaces
    public static string AssetName { get { return RemoteAssetHelpers.GetAssetName<CardView>(); } }
    public static string BorderMapping = "Border";
    public static readonly Vector2 GameSize = new Vector2(65f, 90f);

    public virtual void Clear()
    {
        LoadCard();
        Hide();
    }
    public void Destroy()
    {
        Destroy(gameObject);
    }
    public iCard GetCard()
    {
        if (ActiveCard == null) { return null; }
        return ActiveCard;
    }
    
    #endregion

    #region Properties
    public string CardName;
    public string CardSessionId;
    public Card ActiveCard;
    public TouchObject touch;
    public int cardIndex;

    protected Vector2 DefaultPosition { get; set; }
    public string DisplayName
    {
        get
        {
            string st = "";
            if (ActiveCard == null) { return st; }

            st = $"{ActiveCard.cardData.cardName}";
            if (ActiveCard.cardData.artType == ArtType.AltArt)
            {
                st += " - (Alternate Art)";
            }
            else if (ActiveCard.cardData.artType == ArtType.FullArt)
            {
                st += " - (Full Art)";
            }
            if (ActiveCard.cardData.artType == ArtType.Stellar || ActiveCard.cardData.rarity == Rarity.Stellar)
            {
                st = $"Stellar {ActiveCard.cardData.cardName}";
            }
            
                return st;
        }
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

    public float RenderHeight { get { return GetRenderHeight(); } }
   
    protected virtual float GetRenderHeight()
    {
        return GetComponent<RectTransform>().rect.height;
    }

    #region Functions
    public bool IsCard(string cardKey)
    {
        if (CardKey == cardKey) { return true; }
        return false;
    }
    public string CardKey
    {
        get
        {
            
            if (ActiveCard == null) { return ""; }
            return ActiveCard.cardData.cardKey;
        }
    }
    #endregion

    #region Card Building Properties
    [Header("Card Building")]
    [SerializeField]
    protected GameObject Container;
    public CardConfig DefaultConfig, FullArtConfig;

    
    public virtual CardConfig CurrentConfig
    {
        get
        {
            if (IsFullArt) { return FullArtConfig; }
            return DefaultConfig;
        }
    }

 
    protected bool _isFullArt = false;
    public virtual bool IsFullArt
    {
        get { return _isFullArt; }
        set
        {
            _isFullArt = value;
            DoArtChange(value);
        }
    }


    protected virtual void DoArtChange(bool isFullArt)
    {
        DefaultConfig.Toggle(!isFullArt);
        FullArtConfig.Toggle(isFullArt);
       
    }

    #endregion
    #region Built Card Properties
    public SpriteDisplay cardBorder { get { return CurrentConfig.cardBorder; } }
    #endregion

    #region Events
    public event Action<string> OnSortLayerChange;
    public event Action<int> OnSortOrderChange;
    
    #endregion



    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    private void Awake()
    {
        DefaultPosition = transform.position;
       
    }
    

    public virtual void LoadCard(Card card = null, bool flip = false)
    {
        
        if (card != null)
        {
            ActiveCard = card;
            name = $"{card.cardData.cardKey} - {card.cardData.cardName}";
            IsFullArt = card.isFullArt;
            Flip(flip);
            CurrentConfig.LoadCard(ActiveCard);

            
        }
        else
        {
            ActiveCard = null;
            name = "empty";
            _isFullArt = false;
            CurrentConfig.LoadBlank();
            IsFaceUp = true;
        }
        Show();
    }
   
    #region Card Building
  
    public void MatchSize(Vector2 rectSize)
    {
        Vector2 sizeRatio = rectSize / GameSize;

        Vector2 newScale = Container.transform.localScale * sizeRatio;
        SetScale(newScale);
    }
    public virtual void SetScale(Vector2 newScale)
    {
        //sp.m_Transform.localScale = newScale;
        Container.transform.localScale = newScale;
        transform.position = new Vector3(transform.position.x, transform.position.y, -2f);
        
    }

    #endregion

    #region Card Transforming
    public virtual void Flip(bool toBack = false)
    {
        if (toBack)
        {
            //sp.MainSprite = AssetPipeline.ByKey<Sprite>("cardbackSp");

            DefaultConfig.Toggle(false);
            FullArtConfig.Toggle(false);
            CurrentConfig.Flip(true);
        }
        else
        {
            IsFaceUp = true;
            CurrentConfig.Toggle(true);
            CurrentConfig.Flip(false);
           
            
        }

        IsFaceUp = !toBack;
    }
    public Vector3 GetScale()
    {
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
    public void SetChildIndex(int childIndex)
    {
        if (transform.parent == null) { return; }
        transform.SetSiblingIndex(childIndex);
    }

    
    public virtual void SetSortingLayer(string sortLayer)
    {
        string currentLayer = CurrentConfig.BaseSortLayer;
        if (currentLayer != sortLayer)
        {
            CurrentConfig.ChangeSortLayer(sortLayer);
            OnSortLayerChange?.Invoke(sortLayer);
        }
        
        
    }

    
    public virtual void SetSortingOrder(int order)
    {

        CurrentConfig.ChangeSortOrder(order);
        OnSortOrderChange?.Invoke(order);
    }

    public virtual void AddToSortingOrder(int order)
    {
        Renderer[] rends = GetComponentsInChildren<Renderer>(true);

        for (int i = 0; i < rends.Length; i++)
        {
            rends[i].sortingOrder += order;
        }

        
    }
    #endregion

    #region Card Colors/Highlighting
    public void SelectCard(bool toggle, Color col)
    {
        CurrentConfig.Select(col);
    }
    public void MaskCard(Color col)
    {
        CurrentConfig.Mask(col);
    }

    public void SetAlpha(float alpha)
    {
        CurrentConfig.SetAlpha(alpha);
    }
    public void ResetColors()
    {
        CurrentConfig.ResetColors();
    }
    public void Highlight(SpriteDisplay sp, Color col, float time = 0f, float fadeTime = 1f)
    {
        if (time > 0f)
        {
            sp.ChangeToColorForDuration(col, time, fadeTime);
        }
        else
        {
            sp.SetColor(col);
        }
    }
    #endregion

    #region Card Touch Events
    public static event Action<CardView> OnCardClicked;
    public static event Action<CardView> OnCardHeld;
    /// <summary>
    /// Set this as a Persistent Listener for OnCardClicked in the inspector to get the CardView that's clicked by subscribing to the OnCardClicked event
    /// </summary>
    public void ClickCard()
    {
        OnCardClicked?.Invoke(this);
    }
    /// <summary>
    /// Set this as a Persistent Listener for OnCardHeld in the inspector to get the CardView that's Held by subscribing to the OnCardHeld event
    /// </summary>
    public void HoldCard()
    {
        OnCardHeld?.Invoke(this);
    }
    #endregion

    #region Network Sync
    public void SendNetworkTransform()
    {
        Vector3 scale = transform.localScale;
        Vector3 localPos = transform.localPosition;
        
    }


    #endregion


    


}
