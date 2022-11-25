using System.Collections;
using System.Collections.Generic;
using Gameplay.CardActions;
using UnityEngine;
using UnityEngine.Profiling;
using static Gameplay.CardActions.DrawAction;

namespace Gameplay
{
    public class MoveAction : CardAction
    {
        public CardSlot toSlot;

        #region Overrides
        protected override string LocalActionMessage { get { return $"{sourceCard.cardStats.name} sent to {toSlot.SlotTitle}!"; } }
        protected override string LocalDeclareMessage { get { return $"Send {sourceCard.cardStats.name} to {toSlot.SlotTitle}!"; } }

        public override IEnumerator PerformAction()
        {
            yield return DoMove(sourceCard, toSlot);
            

        }

        protected override IEnumerator DoMove(GameCard card, CardSlot to, float time = 0.65F)
        {
            card.cardObject.Show();
            float acumTime = 0f;
            Transform parent = to.transform.parent;
            card.cardObject.SetAsChild(parent, to.CardScale);
            Vector3 direction = GetDirection(card, to);

           
            float intervals = Mathf.Round(time / Time.deltaTime);
            int count = 0;
            do
            {
                this.Freeze();
                yield return new WaitForEndOfFrame();
                float frames = Time.deltaTime / time;
                card.MovePosition((direction * frames));
                count += 1;
                acumTime += Time.deltaTime;
            } while (Validate(acumTime, time, intervals, count));
            this.Thaw();
            sourceCard.CurrentSlot.RemoveCard(sourceCard);
            toSlot.AllocateTo(sourceCard);
            End(ActionResult.Succeed);

        }
        protected bool Validate(float acumTime, float time, float intervals, int count)
        {
            bool valid = (GameManager.Instance == true && acumTime < time && !isResolved && count < intervals);
            return valid;
        }

        protected override ActionCategory GetCategory()
        {
            return ActionCategory.None;
        }

        public static MoveAction FromData(CardActionData data)
        {
            return new MoveAction(data);
        }
        protected MoveAction(CardActionData data) : base(data)
        {

        }
        protected override void ParseData(CardActionData data)
        {
            base.ParseData(data);
            player = Game.FindPlayer(data.Value<string>(CardActionData.PlayerKey));
            sourceCard = Game.FindCard(data.Value<string>(CardActionData.SourceKey));
            toSlot = Game.FindSlot(data.Value<string>("slot_to"));
            actionResult = (ActionResult)data.Value<int>("result");
            SetDetails(sourceCard, toSlot);

        }
        protected override CardActionData GetActionData()
        {


            CardActionData data = new CardActionData(this);
            data.SetPlayer(player);
            data.SetSourceCard(sourceCard);
            data.AddData("slot_to", toSlot.slotId);
            data.SetResult(ActionResult.Succeed);
            return data;
        }
        #endregion


        public MoveAction(Player p, GameCard source, CardSlot to) : base(p, source)
        {
            SetDetails(source, to);
        }
        public void SetDetails(GameCard source, CardSlot to)
        {
            toSlot = to;
            sourceCard = source;
            IsCounterable = false;
        }
        

        
    }
}

