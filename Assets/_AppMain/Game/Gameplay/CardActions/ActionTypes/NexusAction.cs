using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using UnityEngine;

namespace Gameplay.CardActions
{
    public class NexusAction : CardAction
    {
        protected CardSlot toSlot;
        protected GameCard targetCard;
        private List<GameCard> _spirits = null;
        protected List<GameCard> spirits { get { _spirits ??= new List<GameCard>(); return _spirits; } }

        protected override ActionCategory GetCategory()
        {
            return ActionCategory.Nexus;
        }

        protected override string LocalActionMessage
        {
            get
            {
                
               
                return $"{sourceCard.cardStats.title} sends {spirits.SpiritUnicode()} to {targetCard.cardStats.title}!";
            }
        }
        protected override string LocalDeclareMessage { get { return $"{sourceCard.cardStats.title} Nexus {spirits.SpiritUnicode()} to {targetCard.cardStats.title}!"; } }

        protected override CardActionData GetActionData()
        {
            int spiritCount = spirits.Count;

            CardActionData data = new CardActionData(this);
            data.SetPlayer(player);
            data.SetSourceCard(sourceCard);
            data.AddData("nexus_target", targetCard.cardId);
            data.AddData("slot_to", toSlot.slotId);
            for (int i = 0; i < spirits.Count; i++)
            {
                data.SetSpirit(i + 1, spirits[i].cardId);
            }
            data.AddData("result", actionResult);

            return data;
        }

        public override void ForceCompleteAction()
        {
            base.ForceCompleteAction();
        }

        #region Initialization

        #region Import from Data
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
            player = Game.FindPlayer(data.Value<string>(CardActionData.PlayerKey));
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

        #endregion
        protected void SetDetails(Player player)
        {
            actionTime = .65f;
            
        }
        NexusAction(Player p, GameCard source, GameCard target, List<GameCard> spiritsMoving) : base(p, source)
        {
            toSlot = target.CurrentSlot;
            targetCard = target;
            spirits.AddRange(spiritsMoving);
            SetDetails(player);

        }

        public static NexusAction Create(Player p, GameCard source, GameCard target, List<GameCard> spiritsMoving)
        {
            return new NexusAction(p, source, target, spiritsMoving);
        }
        #endregion

        public override IEnumerator PerformAction()
        {
            Movements.Clear();
            
            yield return DoStaggeredMove(spirits, toSlot, actionTime, .04f);
        }

        protected override IEnumerator DoStaggeredMove(List<GameCard> cards, CardSlot to, float time, float staggerTime = .04f)
        {
            yield return base.DoStaggeredMove(cards, to, time);
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
