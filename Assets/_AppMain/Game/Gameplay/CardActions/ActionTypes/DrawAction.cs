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

        protected override string LocalActionMessage
        {
            get
            {
                string msg = "";
                switch (drawType)
                {
                    case DrawActionType.GameStart:
                    case DrawActionType.TurnStart:
                        msg = $"{player.username} draws a card!";
                        break;
                    case DrawActionType.Mill:
                        msg = $"{sourceCard.card.cardData.cardName} is sent to {toSlot.SlotTitle} from {fromSlot.SlotTitle}";
                        break;
                    case DrawActionType.FromEffect:
                        msg = $"{sourceCard.card.cardData.cardName} is taken from {fromSlot.SlotTitle} to {toSlot.SlotTitle}.";
                        break;
                }
                return msg;
            }
        }
        protected override string RemoteActionMessage
        {
            get
            {
                string msg = "";
                switch (drawType)
                {
                    case DrawActionType.GameStart: case DrawActionType.TurnStart:
                        msg = $"{player.username} draws a card!";
                        break;
                    case DrawActionType.Mill:
                        msg = $"A card from {fromSlot.SlotTitle} is sent to {toSlot.SlotTitle}.";
                        break;
                    case DrawActionType.FromEffect:
                        msg = $"{player.username} mills a card from {fromSlot.SlotTitle}.";
                        break;
                }
                return msg;
            }
        }
        protected override string LocalDeclareMessage { get { return LocalActionMessage; } }
        protected override string RemoteDeclareMessage { get { return RemoteActionMessage; } }

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
            GameDeck deck = player.deck;
            Decks.Deck sourceDeck = deck.MainDeck;
            if (!isMainDeck) { sourceDeck = deck.SpiritDeck; }
            player.SendCardDraw(sourceCard);
            deck.RemoveCard(sourceCard, sourceDeck);
            toSlot.AllocateTo(sourceCard);
            End(ActionResult.Succeed);
        }

        
        protected override IEnumerator DoMove(GameCard card, CardSlot to, float time = .65f)
        {
            yield return base.DoMove(card, to, time);
            
        }

        

        protected override void ResolveAction(ActionResult result)
        {
            base.ResolveAction(result);

        }
    }
}

