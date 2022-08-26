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
    public SpriteRenderer sp;
    public TouchObject touch;

    public static event Action<float> OnCardLoaded;
    public static void CardLoaded(float val)
    {
        OnCardLoaded.Invoke(val);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }

    
    public void LoadCard(Card card = null)
    {
        if (card != null)
        {
            ActiveCard = card;
            sp.sprite = CardLibrary.GetFullCard(card);
        }
        else
        {
            ActiveCard = null;
            sp.sprite = AssetPipeline.ByKey<Sprite>("cardbackSp");
        }

        Show();
        

    }
    public async Task<bool> LoadCardAsync(Card card = null)
    {
        if (card != null)
        {
            ActiveCard = card;
            sp.sprite = await CardLibrary.GetFullCardAsync(ActiveCard);
        }
        else
        {
            ActiveCard = null;
            sp.sprite = await AssetPipeline.ByKeyAsync<Sprite>("cardbackSp");
        }

        
        Show();
        return true;
    }

    
}
