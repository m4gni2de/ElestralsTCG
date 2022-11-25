using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.GameCommands
{
    public class CardEvent
    {
        public enum EventType
        {
            DrawCard = 0,
            MillCard = 1,
            Discard = 2,
            SendBack = 3,
            Destroyed = 4,
            EffectProc = 5,
            NegateEffect = 6,
            DisEnchant = 7,
            Expend = 8,
            Enchant = 9,
            DeclareAttack = 10,
            Battle = 11,

        }

        public EventType eventType;
        public string id;
        public string playerId;
        public string sourceCard;

        public CardEvent(string source, string player, EventType ty)
        {
            id = UniqueString.Create("cve", 6);
            eventType = ty;
            sourceCard = source;
            playerId = player;
        }

        

        public virtual void Declare()
        {

        }
        public virtual void Do()
        {

        }
    }
}

