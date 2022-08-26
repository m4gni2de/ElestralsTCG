using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;
using CardsUI;

public interface iCard 
{
    public iCardData cardData { get; }
    public CardType CardType { get; }
    public bool isFullArt { get; }
}
