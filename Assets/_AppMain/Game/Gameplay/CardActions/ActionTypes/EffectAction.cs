using System.Collections;
using System.Collections.Generic;
using Gameplay.Menus;
using System.Drawing.Drawing2D;
using UnityEngine;

namespace Gameplay.CardActions
{
    public class EffectAction : CardAction
    {
        protected CardEffect cardEffect;
        private List<GameCard> _costSpirits = null;
        protected List<GameCard> costSpirits { get { _costSpirits ??= new List<GameCard>(); return _costSpirits; } }

        private List<CardAction> _effectActions = null;
        public List<CardAction> effectActions { get { _effectActions ??= new List<CardAction>(); return _effectActions; } }

        protected override ActionCategory GetCategory()
        {
            return ActionCategory.Effect;
        }

        protected override string LocalActionMessage
        {
            get
            {


                return $"{sourceCard.cardStats.title} activates it's Effect!";
            }
        }
        protected override string LocalDeclareMessage { get { return $"{sourceCard.cardStats.title} activates it's Effect!"; } }

        protected override CardActionData GetActionData()
        {
            int spiritCount = costSpirits.Count;

            CardActionData data = new CardActionData(this);
            data.SetPlayer(player);
            data.SetSourceCard(sourceCard);
            for (int i = 0; i < costSpirits.Count; i++)
            {
                data.SetSpirit(i + 1, costSpirits[i].cardId);
            }
            for (int i = 0; i < effectActions.Count; i++)
            {
                string actionTitle = $"effAction_{i}";
                data.SetData(actionTitle, effectActions[i].ActionData.Print);
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
        public static EffectAction FromData(CardActionData data)
        {
            return new EffectAction(data);
        }
        protected EffectAction(CardActionData data) : base(data)
        {

        }
        protected override void ParseData(CardActionData data)
        {
            base.ParseData(data);
            player = Game.FindPlayer(data.Value<string>(CardActionData.PlayerKey));
            sourceCard = Game.FindCard(data.Value<string>(CardActionData.SourceKey));
            actionResult = (ActionResult)data.Value<int>("result");
            cardEffect = sourceCard.Effect;

            int spiritCount = data.CountOfSpiritFields();
            for (int i = 0; i < spiritCount; i++)
            {
                string fieldName = $"spirit_{i + 1}";
                string spirit = data.Value<string>(fieldName);
                if (!string.IsNullOrEmpty(spirit))
                {
                    GameCard spiritCard = Game.FindCard(spirit);
                    costSpirits.Add(spiritCard);
                }
            }

            int effCount = data.CountOfBaseField("effAction_");
            for (int i = 0; i < effCount; i++)
            {
                string fieldName = $"effAction_{i}";
                string cardData = data.Value<string>(fieldName);
                CardActionData effData = CardActionData.FromData(cardData);
                CardAction ac = CardActionData.ParseData(effData);
                effectActions.Add(ac);
            }
            SetDetails(player);

        }

        #endregion
        protected void SetDetails(Player player)
        {
            actionTime = .65f;

        }

        public EffectAction(Player p, GameCard source, CardEffect effect) : base(p, source)
        {
            cardEffect = effect;
            SetDetails(player);
        }
        public void AddSpiritsCost(List<GameCard> spiritsCost)
        {
            costSpirits.Clear();
            costSpirits.AddRange(spiritsCost);
        }
        public void AddAction(CardAction action)
        {
            effectActions.Add(action);
        }

        #endregion

        #region Performing
        protected override IEnumerator TryAction(float waitTime = .1f)
        {
            float acumTime = 0f;
            _isRunning = true;
            GameMessage message = GameMessage.FromAction(GetDeclaredMessage(false), this, true, -1f);
            GameManager.Instance.messageControl.ShowMessage(message);

            GameManager.Instance.DeclareCardAction(this);

            yield return PayCost();
            do
            {

                acumTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            } while (acumTime <= waitTime && actionResult == ActionResult.Pending);

            //figure out how to respond to actions here, but for now, just allow players to undo an action if it's been countered
            actionResult = ActionResult.Succeed;
        }

        private IEnumerator PayCost()
        {
            Movements.Clear();

            for (int i = 0; i < costSpirits.Count; i++)
            {
                GameCard c = costSpirits[i];
                c.isBlackout = false;
                if (c.CurrentSlot != player.gameField.UnderworldSlot)
                {
                    Movements.Add(DoMove(c, player.gameField.UnderworldSlot, .45f));
                }
            }
            yield return DoMovements();
        }
        public override IEnumerator PerformAction()
        {
            Movements.Clear();

            for (int i = 0; i < costSpirits.Count; i++)
            {
                GameCard c = costSpirits[i];
                c.isBlackout = false;
                if (c.CurrentSlot != player.gameField.UnderworldSlot)
                {
                    Movements.Add(DoMove(c, player.gameField.UnderworldSlot, .45f));
                }
            }
            yield return DoMovements();
            End(ActionResult.Succeed);

        }

        protected override IEnumerator DoMove(GameCard card, CardSlot to, float time = .65f)
        {
            yield return base.DoMove(card, to, time);
            to.AllocateTo(card);
        }
        #endregion
    }
}
