using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;
using CardsUI;
using GlobalUtilities;

public interface iCard 
{
    public iCardData cardData { get; }
    public CardType CardType { get; }
    public bool isFullArt { get; }

    int Compare<T>(T x, T y);
}


