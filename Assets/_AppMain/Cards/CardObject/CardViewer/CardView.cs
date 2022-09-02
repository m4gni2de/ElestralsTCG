using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cards;
using CardsUI;
using System;
using System.Threading.Tasks;

public class CardView : MonoBehaviour
{
    public Card ActiveCard;
    public SpriteDisplay sp;
    public TouchObject touch;
    public int cardIndex;

    
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }

    
    public void LoadCard( Card card = null)
    {
        if (card != null)
        {
            ActiveCard = card;
            sp.MainSprite = CardLibrary.GetFullCard(card);
        }
        else
        {
            ActiveCard = null;
            sp.MainSprite = AssetPipeline.ByKey<Sprite>("cardbackSp");
        }

        Show();
        

    }

    public async Task<bool> LoadCardAsync(Card card = null)
    {
        if (card != null)
        {
            ActiveCard = card;
            sp.MainSprite = await CardLibrary.GetFullCardAsync(card);
        }
        else
        {
            ActiveCard = null;
            sp.MainSprite = AssetPipeline.ByKey<Sprite>("cardbackSp");
        }

        Show();
        return true;


    }


}
