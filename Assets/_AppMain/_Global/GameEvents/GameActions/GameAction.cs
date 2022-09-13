using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;

namespace GameActions
{
    public class GameAction
    {
        public enum ActionCode
        {
            Location = 0,
            DeckPosition = 1,
            Orientation = 2,

        }

        public enum ActionType
        {
            DrawCard = 0,
            MillCard = 1,
            Discard = 2,
            SendBack = 3,
            Destroyed = 4,
            ActivateEffect = 5,
            NegateEffect = 6,
            DisEnchant = 7,
            Expend = 8,
            Enchant = 9,
            DeclareAttack = 10,
            Battle = 11,

        }
        public string cardId;
        public int index;
        public ActionType actionType;
        public ActionCode actionCode;


        public GameAction(string cardIdCode, ActionType ty)
        {
            cardId = cardIdCode;
            actionType = ty;
        }

        //public static GameAction MoveAction(string cardId, ActionType ty, string prevSlot, string newSlot)
        //{
        //    GameAction ac = new GameAction(cardId, ty);
        //}

    }
}

