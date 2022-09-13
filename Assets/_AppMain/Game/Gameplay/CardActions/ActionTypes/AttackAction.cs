using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        
        public AttackAction(Player p, GameCard attacker, CardSlot defender) : base(p, attacker)
        {
            IsDirectAttack = defender.slotType == CardLocation.SpiritDeck;
            targetSlot = defender;
        }

        public static AttackAction ElestralAttack(GameCard attacker, CardSlot defender)
        {
            return new AttackAction(attacker.Owner, attacker, defender);
        }
        
        protected override CardActionData GetActionData()
        {
            //can tell it's a direct attack on the import based on the slot. if it's the spirit deck slot, then it's direct.
            CardActionData data = new CardActionData(this);
            data.AddData("player", player.userId);
            data.AddData("attacker", sourceCard.cardId);
            data.AddData("action_type", "attack");
            data.AddData("defend_slot", targetSlot.slotId);
            data.AddData("attack_outcome", (int)attackResult);
            data.AddData("attack_damage", damageDealt);
            data.AddData("result", actionResult);

            return data;
        }


        public override IEnumerator PerformAction()
        {
            yield return DoAttack(sourceCard, targetSlot);
            //damageDealt = 

        }


        protected IEnumerator DoAttack(GameCard card, CardSlot to, float time = .65f)
        {
            Vector3 direction = GetDirection(card, to);
            Vector3 fromDirection = -direction;

            
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
            } while (IsValid(acumTime, time));
            this.Thaw();
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
                    damage = attacker.cardStats.EnchantingSpirits.Count - defender.MainCard.cardStats.EnchantingSpirits.Count;
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

