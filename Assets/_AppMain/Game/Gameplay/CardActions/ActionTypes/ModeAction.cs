using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.CardActions
{
    public class ModeAction : CardAction
    {
        public CardMode newCardMode;
        public CardMode oldCardMode;


        protected override CardActionData GetActionData()
        {


            CardActionData data = new CardActionData(this);
            data.AddData("player", player.userId);
            data.AddData("card", sourceCard.cardId);
            data.AddData("action_type", "mode");
            data.AddData("old_mode", (int)oldCardMode);
            data.AddData("new_mode", (int)newCardMode);
            return data;
        }

        public ModeAction(Player p, GameCard source, CardMode newMode) : base(p, source)
        {
            newCardMode = newMode;
            oldCardMode = source.mode;
        }

        public static ModeAction DefenseMode(Player p, GameCard source)
        {
            return new ModeAction(p, source, CardMode.Defense);
        }
        public static ModeAction AttackMode(Player p, GameCard source)
        {
            return new ModeAction(p, source, CardMode.Attack);
        }




        public override IEnumerator PerformAction()
        {
            yield return DoChange();

        }


        protected IEnumerator DoChange()
        {
            float acumTime = 0f;
            Vector3 targetRotation = new Vector3(0f, 0f, 90f);
            if (newCardMode == CardMode.Attack) { targetRotation = Vector3.zero; }

            float angleDiff = Mathf.Abs(sourceCard.cardObject.transform.localEulerAngles.z - targetRotation.z);

            bool goingUp = sourceCard.cardObject.transform.localEulerAngles.z < targetRotation.z;
            float anglePerFrame = angleDiff * Time.deltaTime;
            if (!goingUp)
            {
                anglePerFrame *= -1f;
            }
            do
            {
                
                Vector3 angles = sourceCard.cardObject.transform.localEulerAngles;
                sourceCard.cardObject.transform.localEulerAngles = new Vector3(angles.x, angles.y, angles.z + anglePerFrame);
                yield return new WaitForEndOfFrame();
                acumTime += Time.deltaTime;

            } while (acumTime < actionTime || IsValid(goingUp, sourceCard.cardObject.transform.localEulerAngles.z, targetRotation.z));
            sourceCard.SetCardMode(newCardMode);
            End(Result.Succeed);
        }


        protected bool IsValid(bool goingUp, float currVal, float targetVal)
        {
            if (goingUp)
            {
                return currVal < targetVal;
            }
            //if going down, the target value is 0
            return currVal >= 1f;
        }


        protected override void ResolveAction(Result result)
        {
            base.ResolveAction(result);

        }
    }
}
