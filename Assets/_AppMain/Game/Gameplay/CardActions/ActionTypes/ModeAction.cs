using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Profiling;

namespace Gameplay.CardActions
{
    public class ModeAction : CardAction
    {
        public CardMode newCardMode;
        public CardMode oldCardMode;

        protected override ActionCategory GetCategory()
        {
            return ActionCategory.Mode;
        }


        protected override string LocalActionMessage
        {
            get
            {
                return $"{sourceCard.cardStats.title} is changed to {newCardMode} Mode!";
            }
        }
         protected override string LocalDeclareMessage { get { return $"Change {sourceCard.cardStats.title} to {newCardMode} Mode!"; } }

        protected override CardActionData GetActionData()
        {

            CardActionData data = new CardActionData(this);
            data.SetPlayer(player);
            data.SetSourceCard(sourceCard);
            data.AddData("old_mode", (int)oldCardMode);
            data.AddData("new_mode", (int)newCardMode);
            data.SetResult(actionResult);
            return data;
        }

        public static ModeAction FromData(CardActionData data)
        {
            return new ModeAction(data);
        }

        protected ModeAction(CardActionData data) : base(data)
        {

        }
        protected override void ParseData(CardActionData data)
        {
            base.ParseData(data);
            player = Game.FindPlayer(data.Value<string>(CardActionData.PlayerKey));
            sourceCard = Game.FindCard(data.Value<string>(CardActionData.SourceKey));
            oldCardMode = (CardMode)data.Value<int>("old_mode");
            newCardMode = (CardMode)data.Value<int>("new_mode");
            actionResult = data.GetResult();
            

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
                sourceCard.NetworkCard.SendRotation();
                yield return new WaitForEndOfFrame();
                acumTime += Time.deltaTime;

            } while (Validate(acumTime, actionTime) || IsValid(goingUp, sourceCard.cardObject.transform.localEulerAngles.z, targetRotation.z));
            sourceCard.SetCardMode(newCardMode);
            sourceCard.NetworkCard.SendRotation();
            End(ActionResult.Succeed);
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


        protected override void ResolveAction(ActionResult result)
        {
            base.ResolveAction(result);

        }
    }
}
