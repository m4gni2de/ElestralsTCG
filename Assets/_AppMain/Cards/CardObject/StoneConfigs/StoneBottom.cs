using System.Collections;
using System.Collections.Generic;
using CardsUI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CardsUI.Glowing;

public class StoneBottom : MonoBehaviour
{
   
    #region Variants
    public StoneVariant stoneSpirit, stoneElestral, stoneRune;
    protected List<StoneVariant> _variants
    {
        get
        {
            List<StoneVariant> list = new List<StoneVariant>();
            list.Add(stoneSpirit);
            list.Add(stoneElestral);
            list.Add(stoneRune);
            return list;
        }
    }

    private StoneVariant _ActiveVariant = null;
    public StoneVariant ActiveVariant { get { return _ActiveVariant; } }
    #endregion

    protected StoneConfig _config;
    public StoneConfig Config { get { return _config; } }




    public void SetStone(Card card)
    {
        if (_ActiveVariant != null)
        {
            StoneVariant active = _ActiveVariant;
        }
        
        _ActiveVariant = GetActiveVariant(card.CardType);

        SetVariant(card);
        
    } 

    public void SetBlank()
    {
        if (ActiveVariant != null)
        {
            ActiveVariant.SetBlank();
        }
        _ActiveVariant = null;
    }

    private StoneVariant GetActiveVariant(CardType cardType)
    {
        switch (cardType)
        {
            case CardType.Spirit:
                return stoneSpirit;
            case CardType.Elestral:
                return stoneElestral;
            case CardType.Rune:
                return stoneRune;
            default:
                return _variants[0];
        }
    }

    protected void SetVariant(Card card)
    {
        for (int i = 0; i < _variants.Count; i++)
        {
            if (_variants[i] != _ActiveVariant)
            {
                _variants[i].Hide();
            }
        }

        ActiveVariant.Set(card);
    }

    public void SetSortingLayer(string sortLayer)
    {
        Renderer[] rends = GetComponentsInChildren<Renderer>(true);

        for (int i = 0; i < rends.Length; i++)
        {
            rends[i].sortingLayerName = sortLayer;
        }
    }

    
}
