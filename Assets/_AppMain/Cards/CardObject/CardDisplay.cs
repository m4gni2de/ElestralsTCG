using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Image image;
    public TouchObject touch;
    public Card ActiveCard;
    public int cardIndex;

    public void LoadCard(CardView card)
    {
        ActiveCard = card.ActiveCard;
        gameObject.SetActive(true);
        image.sprite = CardLibrary.GetFullCard(ActiveCard);
        cardIndex = card.cardIndex;
    }

    public void LoadBack()
    {
        image.sprite = AssetPipeline.ByKey<Sprite>(CardLibrary.DefaultCardKey);
    }
    public void SetSortingLayer(string sortLayer)
    {
        Canvas c = GetComponent<Canvas>();
        c.overrideSorting = true;
        c.sortingLayerName = sortLayer;
    }
    


    
}
