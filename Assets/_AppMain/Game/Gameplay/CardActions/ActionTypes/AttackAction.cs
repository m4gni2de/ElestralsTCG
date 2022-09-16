using System.Collections;
using System.Collections.Generic;
using Gameplay.CardActions;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static Gameplay.CardActions.DrawAction;

namespace Gameplay
{
    public class AttackAction : CardAction
    {
        public enum AttackResult
        {
            Succeed = 0,
            Failure = 1,
            Draw = 2,

        }
        protected bool IsDirectAttack = false;
        public CardSlot targetSlot;
        public AttackResult attackResult;
        public int damageDealt = 0;

        protected override CardActionData GetActionData()
        {
            //can tell it's a direct attack on the import based on the slot. if it's the spirit deck slot, then it's direct.
            CardActionData data = new CardActionData(this);
            data.AddData("player", player.userId);
            data.AddData("attacker", sourceCard.cardId);
            data.AddData("action_type", AttackType);
            data.AddData("defend_slot", targetSlot.slotId);
            data.AddData("attack_outcome", (int)attackResult);
            data.AddData("attack_damage", damageDealt);
            data.AddData("result", (int)actionResult);

            return data;
        }
        #region Initialization
        protected AttackAction(CardActionData data) : base(data)
        {

        }
        protected override void ParseData(CardActionData data)
        {
            base.ParseData(data);
            player = Game.FindPlayer(data.Value<string>("player"));
            sourceCard = Game.FindCard(data.Value<string>("attacker"));
            targetSlot = Game.FindSlot(data.Value<string>("defend_slot"));
            attackResult = (AttackResult)data.Value<int>("attack_outcome");
            damageDealt = data.Value<int>("attack_damage");
            actionResult = (ActionResult)data.Value<int>("result");
            SetDetails();
        }
        protected void SetDetails()
        {
            IsDirectAttack = targetSlot.slotType == CardLocation.SpiritDeck;
            _declaredMessage = $"{sourceCard.cardStats.title} targets {targetSlot.SlotTitle} for an Attack!";
            _actionMessage = $"{sourceCard.cardStats.title} attacks {targetSlot.SlotTitle}!";
        }
        public AttackAction(Player p, GameCard attacker, CardSlot defender, ActionResult ac) : base(p, attacker, ac)
        {
            targetSlot = defender;
            SetDetails();
            
        }

        public static AttackAction ElestralAttack(GameCard attacker, CardSlot defender)
        {
            return new AttackAction(attacker.Owner, attacker, defender, ActionResult.Pending);
        }


        #endregion

        public override IEnumerator PerformAction()
        {
           
            yield return DoAttack(sourceCard, targetSlot);

        }


        protected IEnumerator DoAttack(GameCard card, CardSlot to, float time = .65f)
        {
            Vector3 direction = GetDirection(card, to);
            Vector3 startPos = card.cardObject.transform.position;

            
            float acumTime = 0f;
            do
            {
                this.Freeze();
                yield return new WaitForEndOfFrame();
                if (acumTime < time / 2f)
                {
                    card.cardObject.transform.position += direction * (Time.deltaTime * 2f);
                }
                else
                {
                    card.cardObject.transform.position -= direction * (Time.deltaTime * 2f);
                }
                
                acumTime += Time.deltaTime;
            } while (Validate(acumTime, time));
            card.cardObject.transform.position = startPos;
            this.Thaw();
            
            End(ActionResult.Succeed);

        }

       
        protected override void ResolveAction(ActionResult result)
        {
            base.ResolveAction(result);

        }

        public void CalculateAttack(GameCard attacker, CardSlot defender)
        {
            int damage = 0;
            if (defender.slotType == CardLocation.SpiritDeck)
            {
                if (attacker.cardStats.attack > 0) { damage = attacker.EnchantingSpirits.Count; } else { damage = 0; }
                attackResult = AttackResult.Succeed;
            }
            else
            {
                attackResult = GetAttackResult(attacker, defender.MainCard);
                if (attackResult == AttackResult.Succeed)
                {
                    damage = attacker.EnchantingSpirits.Count - defender.MainCard.EnchantingSpirits.Count;
                }
            }
        }

        protected AttackResult GetAttackResult(GameCard attacker, GameCard defender)
        {
            int attackStat = attacker.cardStats.attack;
            int defendStat = defender.cardStats.attack;
            if (defender.mode == CardMode.Defense)
            {
                defendStat = defender.cardStats.defense;
            }
            if (attackStat > defendStat)
            {
                return AttackResult.Succeed;
            }
            if (attackStat < defendStat) { return AttackResult.Failure; }
            return AttackResult.Draw;
        }

       
    }
}

