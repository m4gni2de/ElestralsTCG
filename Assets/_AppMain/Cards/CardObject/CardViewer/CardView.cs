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

public class CardView : MonoBehaviour, iRemoteAsset
{
    public static string AssetName { get { return RemoteAssetHelpers.GetAssetName<CardView>(); } }
    public static string BorderMapping = "Border";

    public string CardName;
    public string CardSessionId;
    public Card ActiveCard;
    public SpriteDisplay sp;
    public TouchObject touch;
    public int cardIndex;

    

    private MultiImage _images = null;
    public MultiImage Images { get { _images ??= GetComponent<MultiImage>(); return _images; } }

    public SpriteDisplay borderSp
    {
        get
        {
            return Images.FromKey(BorderMapping);
        }
    }

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

    
    public void LoadCard(Card card = null)
    {
        if (card != null)
        {
            ActiveCard = card;
            sp.MainSprite = CardLibrary.GetFullCard(card);
            IsFaceUp = true;
            
        }
        else
        {
            ActiveCard = null;
            sp.MainSprite = AssetPipeline.ByKey<Sprite>("cardbackSp");
        }

      
        Show();
        

    }
   

    public virtual void Flip(bool toBack = false)
    {
        if (toBack)
        {
            sp.MainSprite = AssetPipeline.ByKey<Sprite>("cardbackSp");
        }
        else
        {
            sp.MainSprite = CardLibrary.GetFullCard(ActiveCard);
        }

        IsFaceUp = !toBack;
    }
    public virtual void SetScale(Vector2 newScale)
    {
        sp.m_Transform.localScale = newScale;
        transform.position = new Vector3(transform.position.x, transform.position.y, -2f);
        

    }
    public Vector3 GetScale()
    {
        return sp.m_Transform.localScale;
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

    public virtual void SetSortingLayer(string sortLayer)
    {
        sp.SetSortLayer(sortLayer);
        borderSp.SetSortLayer(sortLayer);
    }

    public virtual void SetSortingOrder(int order)
    {
       
        sp.SetSortOrder(order);
        borderSp.SetSortOrder(sp.SortOrder + 1);
    }

    public virtual void AddToSortingOrder(int order)
    {
        Renderer[] rends = GetComponentsInChildren<Renderer>(true);

        for (int i = 0; i < rends.Length; i++)
        {
            rends[i].sortingOrder += order;
        }

        
    }

    public void SelectCard(bool toggle)
    {
        if (toggle)
        {
            sp.SetColor(Color.white);
        }
        else
        {
            Color spColor = sp.GetColor();
            Color newColor = new Color(spColor.r, spColor.g, spColor.b, .5f);
            sp.SetColor(newColor);
        }
    }

    public void SetColor(Color col)
    {
        sp.SetColor(col);
    }



    #region Network Sync
    public void SendNetworkTransform()
    {
        Vector3 scale = transform.localScale;
        Vector3 localPos = transform.localPosition;
        
    }

    
    #endregion


}
