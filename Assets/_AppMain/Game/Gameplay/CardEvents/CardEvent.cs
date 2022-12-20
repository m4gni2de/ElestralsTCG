using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEvents;
using Gameplay;

public class CardEvent<T> : GameEvent<T> 
{
   public CardEvent(string eventKey) : base(eventKey) { }
}
