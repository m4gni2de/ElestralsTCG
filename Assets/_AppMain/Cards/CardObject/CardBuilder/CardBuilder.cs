using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;
using CardsUI;
using UnityEngine.UI;
using TMPro;

public class CardBuilder : MonoBehaviour
{
    public Card ActiveCard { get; private set; }
    public string CardName;
    public string CardSessionId;
    public TouchObject touch;
    public int cardIndex;
    public bool IsFaceUp { get; set; }

    #region Texts
    [Header("Texts")]
    public MagicTextBox NameText;
    public MagicTextBox EffectText;
    public TMP_Text attackText;
    public TMP_Text defenseText;
    #endregion

    #region Sprites
    [Header("Sprites")]
    public SpriteRenderer cardImage;

    #endregion



    public void LoadCard(Card card = null)
    {
        if (card != null)
        {
            ActiveCard = card;
            cardImage.sprite = CardLibrary.GetCardArt(card);
            IsFaceUp = true;

        }
        else
        {
            ActiveCard = null;
            
        }


        //Show();


    }


    #region Elestral Region
    protected void LoadElestral(Card card)
    {
        bool fa = card.isFullArt;
        if (fa)
        {
            FullArtElestral(card);
            return;
        }
    }

    protected void FullArtElestral(Card card)
    {

    }
    #endregion


    public virtual void SetScale(Vector2 newScale)
    {
        transform.localScale = newScale;
        transform.position = new Vector3(transform.position.x, transform.position.y, -2f);


    }
}
