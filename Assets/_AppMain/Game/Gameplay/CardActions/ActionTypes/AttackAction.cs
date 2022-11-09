using System.Collections;
using System.Collections.Generic;
using Gameplay.CardActions;
using UnityEngine;
using UnityEngine.UIElements;

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
        protected override ActionCategory GetCategory()
        {
            return ActionCategory.Attack;
        }

        protected override string LocalActionMessage
        {
            get
            {
                if (!IsDirectAttack)
                {
                    return $"{sourceCard.cardStats.title} attacks {targetSlot.SlotTitle}!"; ;
                }
                return $"{sourceCard.cardStats.title} attacks {sourceCard.Owner.Opponent.userId} directly!"; ;

            }
        }
        protected override string LocalDeclareMessage
        {
            get
            {
                if (!IsDirectAttack)
                {
                    return $"{sourceCard.cardStats.title} is Targetting {targetSlot.SlotTitle} for an Attack!";
                }
                return $"{sourceCard.cardStats.title} is Targetting {sourceCard.Owner.Opponent.userId} for a Direct Attack!";
            }
        }

        protected override CardActionData GetActionData()
        {
            //can tell it's a direct attack on the import based on the slot. if it's the spirit deck slot, then it's direct.
            CardActionData data = new CardActionData(this);
            data.SetPlayer(player);
            data.AddData("attacker", sourceCard.cardId);
            data.AddData("defend_slot", targetSlot.slotId);
            data.AddData("attack_outcome", (int)attackResult);
            data.AddData("attack_damage", damageDealt);
            data.AddData("result", (int)actionResult);

            return data;
        }
        #region Initialization
        public static AttackAction FromData(CardActionData data)
        {
            return new AttackAction(data);
        }
        protected AttackAction(CardActionData data) : base(data)
        {

        }
        protected override void ParseData(CardActionData data)
        {
            base.ParseData(data);
            player = Game.FindPlayer(data.Value<string>(CardActionData.PlayerKey));
            sourceCard = Game.FindCard(data.Value<string>(CardActionData.SourceKey));
            targetSlot = Game.FindSlot(data.Value<string>("defend_slot"));
            attackResult = (AttackResult)data.Value<int>("attack_outcome");
            damageDealt = data.Value<int>("attack_damage");
            actionResult = data.GetResult();
            SetDetails();
        }
        protected void SetDetails()
        {
            IsDirectAttack = targetSlot.slotType == CardLocation.SpiritDeck;
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


        protected IEnumerator DoAttack(GameCard card, CardSlot to, float time = 1.2f)
        {
           
            Vector3 startPos = card.cardObject.transform.position;


            float acumTime = 0f;
            float percentMove = 0f;
            float percentDone = 0f;

            this.Freeze();
            do
            {
                
                percentDone = (acumTime / time);
                

                if (percentDone <= .5f)
                {
                    percentMove = percentDone * 2f;
                    card.SetPosition(Vector2.Lerp(card.CurrentSlot.Position, to.Position, percentMove));

                }
                else
                {
                    percentMove = (percentDone * 2f) - 1f;
                    card.SetPosition(Vector2.Lerp(to.Position, card.CurrentSlot.Position, percentMove));
                }

                Debug.Log(percentMove);
                yield return new WaitForEndOfFrame();
                acumTime += Time.deltaTime;

            } while (Validate(acumTime, time));
            card.SetPosition(startPos);
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
                if (attacker.cardStats.attack > 0) { damage = attacker.EnchantingSpiritTypes.Count; } else { damage = 0; }
                attackResult = AttackResult.Succeed;
            }
            else
            {
                attackResult = GetAttackResult(attacker, defender.MainCard);
                if (attackResult == AttackResult.Succeed)
                {
                    damage = Mathf.Clamp(attacker.EnchantingSpiritTypes.Count - defender.MainCard.EnchantingSpiritTypes.Count, 0, 9999);
                }
            }

            damageDealt = damage;
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

