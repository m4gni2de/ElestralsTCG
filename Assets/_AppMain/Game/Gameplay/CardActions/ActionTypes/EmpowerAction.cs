using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.CardActions
{
    public class EmpowerAction : EnchantAction
    {

        protected GameCard empoweredElestral = null;

        #region Operators
       
       
        #endregion
        #region Overrides
        protected override ActionCategory GetCategory()
        {
            return ActionCategory.Empower;
        }
        protected override CardActionData GetActionData()
        {
            int spiritCount = spirits.Count;

            CardActionData data = new CardActionData(this);
            data.SetPlayer(player);
            data.SetSourceCard(sourceCard);
            data.AddData("enchant_type", (int)enchantType);
            data.AddData("card_mode", (int)CardMode.Attack);
            data.AddData("empowered_elestral", empoweredElestral.cardId);
            data.AddData("slot_to", toSlot.slotId);
            for (int i = 0; i < spiritCount; i++)
            {
                data.SetSpirit(i + 1, spirits[i].cardId);
            }
            data.AddData("result", (int)actionResult);

            return data;
        }
        protected override void ParseData(CardActionData data)
        {
            base.ParseData(data);
            player = Game.FindPlayer(data.Value<string>(CardActionData.PlayerKey));
            sourceCard = Game.FindCard(data.Value<string>(CardActionData.SourceKey));
            enchantType = (EnchantActionType)data.Value<int>("enchant_type");
            cardMode = (CardMode)data.Value<int>("card_mode");
            toSlot = Game.FindSlot(data.Value<string>("slot_to"));
            actionResult = data.GetResult();
            empoweredElestral = Game.FindCard(data.Value<string>("empowered_elestral"));
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
            SetDetails();

        }
        public static new EmpowerAction FromData(CardActionData data)
        {
            return new EmpowerAction(data);
        }

        public override IEnumerator DisplayRemoteAction()
        {
            Game.Empower(sourceCard, empoweredElestral);
            return base.DisplayRemoteAction();

        }


        public override IEnumerator PerformAction()
        {
            Movements.Clear();

            for (int i = 0; i < spirits.Count; i++)
            {
                Movements.Add(DoMove(spirits[i], toSlot));
            }

            if (enchantType == EnchantActionType.FromFaceDown)
            {
                Movements.Add(DoFlip(sourceCard, CardMode.Attack));
            }

            if (doesSourceMove)
            {
                Movements.Add(DoMove(sourceCard, toSlot));

            }


            yield return DoMovements();
            Game.Empower(sourceCard, empoweredElestral);
            


        }
        #endregion

        #region Initialization
        protected EmpowerAction(CardActionData data) : base(data)
        {

        }

        //EmpowerAction(Player p, GameCard rune, CardSlot to, GameCard[] spiritsUsed, GameCard elestralEmpowered, EnchantActionType ty) : base(p, rune, to, ty, spiritsUsed, CardMode.Attack)
        //{
        //    empoweredElestral = elestralEmpowered;
        //}

        protected override void SetDetails()
        {
            actionTime = .65f;
            if (enchantType == EnchantActionType.FromFaceDown || enchantType == EnchantActionType.DisEnchant) { doesSourceMove = false; } else { doesSourceMove = true; }
            _declaredMessage = $"Enchant {sourceCard.cardStats.title} with {SpiritString}";
            _actionMessage = $"{empoweredElestral.cardStats.title} is Empowered by {sourceCard.cardStats.title}!";
            
        }

        public static EmpowerAction FromEnchant(EnchantAction ac, GameCard elestral)
        {
            CardActionData data = new CardActionData(ActionCategory.Empower);
            data.SetPlayer(ac.player);
            data.SetSourceCard(ac.sourceCard);
            data.AddData("enchant_type", (int)ac.enchantType);
            data.AddData("card_mode", (int)CardMode.Attack);
            data.AddData("empowered_elestral", elestral.cardId);
            data.AddData("slot_to", ac.enchantingSlot.slotId);
            for (int i = 0; i < ac.spirits.Count; i++)
            {
                data.SetSpirit(i + 1, ac.spirits[i].cardId);
            }
            data.AddData("result", (int)ActionResult.Pending);
            return new EmpowerAction(data);
        }
        public static EmpowerAction EmpowerElestral(Player p, GameCard source, CardSlot to,  List<GameCard> spirits, GameCard elestral)
        {
            EnchantActionType en = EnchantActionType.Normal;
            CardActionData data = new CardActionData(ActionCategory.Empower);
            data.SetPlayer(p);
            data.SetSourceCard(source);
            data.AddData("enchant_type", (int)en);
            data.AddData("card_mode", (int)CardMode.Attack);
            data.AddData("empowered_elestral", elestral.cardId);
            data.AddData("slot_to", to.slotId);
            for (int i = 0; i < spirits.Count; i++)
            {
                data.SetSpirit(i + 1, spirits[i].cardId);
            }
            data.AddData("result", (int)ActionResult.Pending);
            return new EmpowerAction(data);
        }
        #endregion
    }
}
