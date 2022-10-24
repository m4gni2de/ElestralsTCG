using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CardsUI;
using CardsUI.Glowing;
using Cards;
using System.Threading.Tasks;

public class CardObject : MonoBehaviour, iHold
{
    #region Static Functions

    public static readonly string CardKey = "CardObject";
   

    #endregion


    #region Art Types
    public CardConfig Default, FullArt;
    public StoneBottom defaultBottom, faBottom;

    private bool _isFullArt = false;
    public bool IsFullArt
    {
        get { return _isFullArt; }
        set
        {
            DoArtChange(value);
            _isFullArt = value;
        }
    }

    protected void DoArtChange(bool isFullArt)
    {
        Default.gameObject.SetActive(!isFullArt);
        FullArt.gameObject.SetActive(isFullArt);
    }
   

    public SpriteRenderer CardImageSp
    {
        get
        {
            if (IsFullArt) { return FullArt.CardImageSp; } return Default.CardImageSp;
        }
    }
    public SpriteRenderer BackgroundSp
    {
        get
        {
            if (IsFullArt) { return FullArt.BackgroundSp; }
            return Default.BackgroundSp;
        }
    }
    public GlowControls glowControls { get { if (IsFullArt) { return FullArt.glowControls; } return Default.glowControls; } }
    public CardTexts Texts
    {
        get
        {
            if (IsFullArt) { return FullArt.Texts; } return Default.Texts; 
        }
    }
    public StoneBottom Bottom { get { if (IsFullArt) { return faBottom; } return defaultBottom; } }
    //public StoneVariant Bottom { get { if (IsFullArt) { return FullArt.StoneVariant; } return Default.StoneVariant; } }


    #endregion

    public GameObject Container, CardBack;

    private TouchObject _touch = null;
    public TouchObject touch { get { _touch ??= GetComponent<TouchObject>(); return _touch; } }
    #region Properties

    protected Card _Card = null;
    public Card ActiveCard { get { return _Card; } }

    public bool isHeld { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public bool isClicked { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public CardType GetCardType()
    {
        if (_Card == null) { return CardType.None; }
        return ActiveCard.CardType;
    }
    

    #endregion

    #region UI Properties
   
    public void SetAsChild(Transform tf, Vector2 scale, string sortLayer = "", int childIndex = -1)
    {
        transform.SetParent(tf);
        Container.transform.localScale = scale;
        if (!string.IsNullOrEmpty(sortLayer)) { SetSortingLayer(sortLayer); }
        if (childIndex > -1)
        {
            transform.SetSiblingIndex(childIndex);
        }
    }
    public void SetScale(Vector2 scale)
    {
        Vector2 currScale = Container.transform.localScale;
        Vector2 scaleDiff = scale / currScale;

        Container.transform.localScale = scale;
        CardBack.transform.localScale *= scaleDiff;
    }
    public void DisplayBack(string assetString = "cardbackSp")
    {
        Container.SetActive(false);
        CardBack.SetActive(true);
        Sprite sp = AssetPipeline.ByKey<Sprite>(assetString, "cardbackSp");
        CardBack.GetComponent<SpriteRenderer>().sprite = sp;
    }
    public virtual void SetSortingLayer(string sortLayer)
    {
        Renderer[] rends = GetComponentsInChildren<Renderer>(true);
        //Bottom.SetSortingLayer(sortLayer);

        for (int i = 0; i < rends.Length; i++)
        {
            rends[i].sortingLayerName = sortLayer;
        }

        
    }
   
    #endregion

    #region Functions
    public ElementCode ElementRequired(int indexOf)
    {
        List<Element> elements = ActiveCard.SpiritsReq;
        if (indexOf > elements.Count) { indexOf = elements.Count - 1; }
        return elements[indexOf].Code;
    }
    #endregion

    //public void RandomElestral()
    //{

    //}

    

    #region Initialization

    private void Start()
    {

        //Texts.title.text = $"Card Name ";
        //CustomFont.Format(CustomFont.ThunderSymbol, Texts.title);

        //CardBase card = CardBase.FindElestral("bs_28");
        //LoadCard(card);

    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    
   
    #region Loading Card

    public void SetBlank()
    {
        LoadCard(null);
        touch.ClearAll();

    }
    public void LoadCard(Card card = null, bool displayBack = false)
    {
        if (card != null)
        {
            gameObject.SetActive(true);
            _Card = card;
            name = $"{card.cardData.cardKey} - {card.cardData.cardName}";
            IsFullArt = card.isFullArt;

            if (displayBack)
            {
                DisplayBack();
            }
            else
            {
                Container.SetActive(true);
                CardBack.SetActive(false);
                Texts.SetTexts(_Card);
                Bottom.SetStone(_Card);
                glowControls.Set(card);
                LoadSprites();
            }
        }
        else
        {
            _Card = null;
            name = "empty";
            _isFullArt = false;
            Container.SetActive(false);
            CardBack.SetActive(true);

            Texts.SetBlank();
            Bottom.SetBlank();
            glowControls.SetBlank();
            ClearSprites();
            gameObject.SetActive(false);
            
        }
        
    }

    public void Flip(bool toBack = false)
    {
        if (toBack)
        {
            Container.SetActive(false);
            CardBack.SetActive(true);
        }
        else
        {
            Container.SetActive(true);
            CardBack.SetActive(false);
        }
    }
    public void Rotate(bool isTapped)
    {
        if (isTapped)
        {
            Container.transform.localEulerAngles = new Vector3(Container.transform.localEulerAngles.x, Container.transform.localEulerAngles.y, 180f);
        }
        else
        {
            Container.transform.localEulerAngles = new Vector3(Container.transform.localEulerAngles.x, Container.transform.localEulerAngles.y, 0f);
        }
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    protected async void LoadSprites()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        BackgroundSp.sprite = CardLibrary.GetBackground(_Card);

        Sprite cardArt = CardLibrary.GetCardArt(_Card);
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

    
    #endregion


    #region Interface
    public virtual void HoldCallback(bool startHold)
    {
        if (startHold)
        {
            
        }
    }
  
    #endregion
}
