using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.CardActions
{
    public class PreparedAction 
    {
        public int turnIndex { get; set; }
        public CardAction cardAction { get; set; }
        public int priority { get; set; }

        public PreparedAction(CardAction ac, int turnIndex, int priority)
        {
            this.cardAction = ac;
            this.turnIndex = turnIndex;
            this.priority = priority;
        }
    }
}
