using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardViewDisplay : CardView
{



    #region Overides
    public StoneBottom Bottom
    {
        get
        {
            if (IsFullArt) { return faBottom; } return defaultBottom;
        }
           
    }

   
    public override bool IsFullArt
    {
        get { return _isFullArt; }
        set
        {
            DoArtChange(value);
            _isFullArt = value;
        }
    }
    public override CardConfig CurrentConfig
    {
        get
        {
            if (IsFullArt) { return FullArt; }
            return Default;
        }
    }
    protected override void DoArtChange(bool isFullArt)
    {

        DefaultConfig.Toggle(!isFullArt);
        FullArtConfig.Toggle(isFullArt);

    }
    public override void LoadCard(Card card = null, bool flip = false)
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

            //Texts.SetBlank();
            //Bottom.SetBlank();
            //glowControls.SetBlank();
            
            gameObject.SetActive(false);
        }


        Show();
    }

    public override void Flip(bool toBack = false)
    {
        if (toBack)
        {
            //sp.MainSprite = AssetPipeline.ByKey<Sprite>("cardbackSp");
            //CurrentConfig.ToggleDisplay(false);
            //Bottom.gameObject.SetActive(false);
        }
        else
        {
            //sp.MainSprite = CardLibrary.GetFullCard(ActiveCard);
            //CurrentConfig.ToggleDisplay(true);
            //Texts.SetTexts(ActiveCard);
            //Bottom.SetStone(ActiveCard);
            //Bottom.gameObject.SetActive(true);
            //glowControls.Set(ActiveCard);
            //LoadSprites();
        }

        IsFaceUp = !toBack;
    }
    #endregion


    #region Properties
    [SerializeField]
    protected CardConfig Default;
    [SerializeField]
    protected StoneBottom defaultBottom;
    [SerializeField]
    protected CardConfig FullArt;
    [SerializeField]
    protected StoneBottom faBottom;
    #endregion
}
