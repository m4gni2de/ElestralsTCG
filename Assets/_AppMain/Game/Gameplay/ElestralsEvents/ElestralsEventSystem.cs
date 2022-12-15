using System.Collections;
using System.Collections.Generic;
using GameEvents;
using Gameplay.CardActions;
using UnityEngine;

namespace Gameplay
{
    [System.Serializable]
    public class ElestralsEventSystem
    {
        protected static GameEvent<GameCard, GameCard> _OnElestralEmpowered;
        public static GameEvent<GameCard, GameCard> OnElestralEmpowered { get { _OnElestralEmpowered ??=  GameEvent.Create<GameCard, GameCard>("Elestral", "Rune"); return _OnElestralEmpowered; } }
    }
}

