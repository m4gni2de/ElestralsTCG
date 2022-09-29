using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Security;

namespace Gameplay.CardActions
{
    public class DrawAction : CardAction
    {
        protected CardSlot fromSlot, toSlot;
        public DrawActionType drawType;
        protected override ActionCategory GetCategory()
        {
            return ActionCategory.Draw;
        }

        bool isMainDeck
        {
            get
            {
                if (fromSlot.slotType == CardLocation.Deck) { return true; } return false;
            }
        }

        public enum DrawActionType
        {
            GameStart = 0,
            TurnStart = 1,
            Mill = 2,
            FromEffect = 3,
        }

        protected override CardActionData GetActionData()
        {
            CardActionData data = new CardActionData(this);
            data.SetPlayer(player);
            data.SetSourceCard(sourceCard);
            data.AddData("draw_type", (int)drawType);
            data.AddData("slot_from", fromSlot.slotId);
            data.AddData("slot_to", toSlot.slotId);
            data.SetResult(actionResult);

            return data;
        }
        public static DrawAction FromData(CardActionData data)
        {
            return new DrawAction(data);
        }
        protected DrawAction(CardActionData data) : base(data)
        {
            
        }
        protected override void ParseData(CardActionData data)
        {
            base.ParseData(data);
            player = Game.FindPlayer(data.Value<string>(CardActionData.PlayerKey));
            sourceCard = Game.FindCard(data.Value<string>(CardActionData.SourceKey));
            drawType = (DrawActionType)data.Value<int>("draw_type");
            fromSlot = Game.FindSlot(data.Value<string>("slot_from"));
            toSlot = Game.FindSlot(data.Value<string>("slot_to"));
            actionResult = (ActionResult)data.Value<int>("result");
            SetDetails(player);

        }
        protected void SetDetails(Player player)
        {
            actionTime = .65f;
            if (this.drawType == DrawActionType.GameStart || this.drawType == DrawActionType.TurnStart) { actionResult = ActionResult.Succeed; }
            string drawString = "draws";
            if (this.drawType == DrawActionType.Mill) { drawString = "mills"; }
            _declaredMessage = $"{player.username} {drawString} a card!";
            _actionMessage = $"{player.username} {drawString} a card from their deck!";
        }

        public DrawAction(Player player, GameCard source, CardSlot from, CardSlot to, DrawActionType drawType, ActionResult ac = ActionResult.Pending) : base(player, source, ac)
        {
            fromSlot = from;
            toSlot = to;
            this.drawType = drawType;
            SetDetails(player);
        }

        #region Static Constructors
        public static DrawAction TurnStart(Player p)
        {
            GameCard toDraw = p.deck.Top;
            CardSlot from = p.gameField.DeckSlot;
            CardSlot to = p.gameField.HandSlot;
            DrawActionType t = DrawActionType.TurnStart;

            DrawAction ac = new DrawAction(p, toDraw, from, to, t, ActionResult.Succeed);
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
            End(ActionResult.Succeed);
        }

        protected override void ResolveAction(ActionResult result)
        {
            base.ResolveAction(result);

        }
    }
}

