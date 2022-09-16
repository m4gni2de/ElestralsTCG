using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using UnityEngine;
using static Gameplay.CardActions.DrawAction;
using static Gameplay.CardActions.EnchantAction;

namespace Gameplay.CardActions
{
    public class NexusAction : CardAction
    {
        protected CardSlot toSlot;
        protected GameCard targetCard;
        private List<GameCard> _spirits = null;
        protected List<GameCard> spirits { get { _spirits ??= new List<GameCard>(); return _spirits; } }

        protected override CardActionData GetActionData()
        {
            int spiritCount = spirits.Count;

            CardActionData data = new CardActionData(this);
            data.AddData("player", player.userId);
            data.AddData("nexus_source", sourceCard.cardId);
            data.AddData("nexus_target", targetCard.cardId);
            data.AddData("action_type", NexusType);
            data.AddData("slot_to", toSlot.slotId);
            data.AddData("spirit_1", "");
            data.AddData("spirit_2", "");
            data.AddData("spirit_3", "");
            data.AddData("spirit_4", "");
            data.AddData("spirit_5", "");
            data.AddData("spirit_6", "");
            if (spiritCount > 0) { data.SetData("spirit_1", spirits[0].cardId); }
            if (spiritCount > 1) { data.SetData("spirit_2", spirits[1].cardId); }
            if (spiritCount > 2) { data.SetData("spirit_3", spirits[2].cardId); }
            if (spiritCount > 3) { data.SetData("spirit_4", spirits[3].cardId); }
            if (spiritCount > 4) { data.SetData("spirit_5", spirits[4].cardId); }
            if (spiritCount > 5) { data.SetData("spirit_6", spirits[5].cardId); }
            data.AddData("result", actionResult);

            return data;
        }

        public override void ForceCompleteAction()
        {
            base.ForceCompleteAction();
        }

        #region Initialization
        public static NexusAction FromData(CardActionData data)
        {
            return new NexusAction(data);
        }
        protected NexusAction(CardActionData data) : base(data)
        {

        }
        protected override void ParseData(CardActionData data)
        {
            base.ParseData(data);
            player = Game.FindPlayer(data.Value<string>("player"));
            sourceCard = Game.FindCard(data.Value<string>("nexus_source"));
            targetCard = Game.FindCard(data.Value<string>("nexus_target"));
            toSlot = Game.FindSlot(data.Value<string>("slot_to"));
            actionResult = (ActionResult)data.Value<int>("result");

            int spiritCount = data.CountOfSpiritFields();
            for (int i = 0; i < spiritCount; i++)
            {
                string fieldName = $"spirit_{i + 1}";
                string spirit = data.Value<string>(fieldName);
                if (!string.IsNullOrEmpty(spirit))
                {
                    GameCard spiritCard = Game.FindCard(spirit);
                    spirits.Add(spiritCard);
                }
            }
            SetDetails(player);

        }
        protected void SetDetails(Player player)
        {
            actionTime = .65f;
            _declaredMessage = $"{sourceCard.cardStats.title} Nexus {spirits.Count} Spirits to {targetCard.cardStats.title}!";
            _actionMessage = $"{sourceCard.cardStats.title} sends {spirits.Count} Spirits to {targetCard.cardStats.title} via Nexus!!";
        }
        NexusAction(Player p, GameCard source, GameCard target, List<GameCard> spiritsMoving) : base(p, source)
        {
            toSlot = target.CurrentSlot;
            targetCard = target;
            spirits.AddRange(spiritsMoving);
            
        }

        public static NexusAction Create(Player p, GameCard source, GameCard target, List<GameCard> spiritsMoving)
        {
            return new NexusAction(p, source, target, spiritsMoving);
        }
        #endregion

        public override IEnumerator PerformAction()
        {
            Movements.Clear();
            
            yield return DoMove(spirits, toSlot, actionTime, .04f);
        }

        protected override IEnumerator DoMove(List<GameCard> cards, CardSlot to, float time, float staggerTime = .04f)
        {
            yield return base.DoMove(cards, to, time);
            for (int i = 0; i < cards.Count; i++)
            {
                GameCard card = cards[i];
                card.CurrentSlot.RemoveCard(card);
                to.AllocateTo(card);
            }
            End(ActionResult.Succeed);
        }

        protected override void ResolveAction(ActionResult result)
        {
            base.ResolveAction(result);

        }

        
    }
}
