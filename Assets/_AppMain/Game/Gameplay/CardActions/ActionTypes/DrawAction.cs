using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace Gameplay.CardActions
{
    public class DrawAction : CardAction
    {
        protected CardSlot fromSlot, toSlot;
        public DrawType drawType;

        bool isMainDeck
        {
            get
            {
                if (fromSlot.slotType == CardLocation.Deck) { return true; } return false;
            }
        }

        public enum DrawType
        {
            GameStart = 0,
            TurnStart = 1,
            Mill = 2,
            FromEffect = 3,
        }

        protected override CardActionData GetActionData()
        {
            CardActionData data = new CardActionData(this);
            data.AddData("player", player.userId);
            data.AddData("card", sourceCard.cardId);
            data.AddData("action_type", "draw");
            data.AddData("draw_type", (int)drawType);
            data.AddData("slot_from", fromSlot.slotId);
            data.AddData("slot_to", toSlot.slotId);
            data.AddData("result", actionResult);

            return data;
        }

        public DrawAction(Player player, GameCard source, CardSlot from, CardSlot to, DrawType drawType) : base(player, source)
        {
            fromSlot = from;
            toSlot = to;
            actionTime = .65f;
            this.drawType = drawType;
        }

        #region Static Constructors
        public static DrawAction TurnStart(Player p)
        {
            GameCard toDraw = p.deck.Top;
            CardSlot from = p.gameField.DeckSlot;
            CardSlot to = p.gameField.HandSlot;
            DrawType t = DrawType.TurnStart;

            DrawAction ac = new DrawAction(p, toDraw, from, to, t);
            return ac;
        }
        #endregion

        public override IEnumerator PerformAction()
        {
            yield return DoMove(sourceCard, toSlot);

        }

        
        protected override IEnumerator DoMove(GameCard card, CardSlot to, float time = .65f)
        {
            yield return base.DoMove(card, to, time);
            GameDeck deck = player.deck;
            Decks.Deck sourceDeck = deck.MainDeck;
            if (!isMainDeck) { sourceDeck = deck.SpiritDeck; }
            player.SendCardDraw(card);
            deck.RemoveCard(card, sourceDeck);
            to.AllocateTo(card);
            End(Result.Succeed);
        }

        protected bool IsValid(float acumTime, float time)
        {
            if (GameManager.Instance == true && acumTime < time) { return true; }
            return false;
        }


        protected override void ResolveAction(Result result)
        {
            base.ResolveAction(result);

        }
    }
}

