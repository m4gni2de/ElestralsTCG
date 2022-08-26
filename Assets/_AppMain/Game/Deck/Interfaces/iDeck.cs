using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Decks;
using Cards;
using Gameplay;

public interface iDeck
{
    void Shuffle();
    void Draw(int count);
    void Discard();

    List<GameCard> Cards { get; }
    
}
