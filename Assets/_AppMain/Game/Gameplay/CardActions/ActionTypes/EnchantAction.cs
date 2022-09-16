using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Gameplay.CardActions.DrawAction;

namespace Gameplay.CardActions
{
    public class EnchantAction : CardAction
    {
        public enum EnchantActionType
        {
            Set = -1,
            Normal = 0,
            Ascend = 1,
            ReEnchant = 2,
            DisEnchant = 3,
            FromFaceDown = 4,
        }

        protected CardSlot toSlot;
        public EnchantActionType enchantType;
        private List<GameCard> _spirits = null;
        protected List<GameCard> spirits { get { _spirits ??= new List<GameCard>(); return _spirits; } }
        protected string SpiritString
        {
            get
            {
                string st = "";
                for (int i = 0; i < spirits.Count; i++)
                {
                    for (int j = 0; j < spirits[i].cardStats.CardElements.Count; j++)
                    {
                        if (i >= spirits.Count - 1 && j >= spirits[i].cardStats.CardElements.Count - 1)
                        {
                            st += $"{spirits[i].cardStats.CardElements[j].ToString()} Spirits.";
                        }
                        else
                        {
                            st += $"{spirits[i].cardStats.CardElements[j].ToString()}, ";
                        }
                        
                    }
                    
                }
                return st;
            }
        }
        protected CardMode cardMode;
        protected bool doesSourceMove = true;

        protected override CardActionData GetActionData()
        {
            int spiritCount = spirits.Count;

            CardActionData data = new CardActionData(this);
            data.AddData("player", player.userId);
            data.AddData("card", sourceCard.cardId);
            data.AddData("action_type", EnchantType);
            data.AddData("enchant_type", (int)enchantType);
            data.AddData("card_mode", (int)cardMode);
            data.AddData("slot_to", toSlot.slotId);
            data.AddData("spirit_1", "");
            data.AddData("spirit_2", "");
            data.AddData("spirit_3", "");
            if (spiritCount > 0) { data.SetData("spirit_1", spirits[0].cardId); }
            if (spiritCount > 1) { data.SetData("spirit_2", spirits[1].cardId); }
            if (spiritCount > 2) { data.SetData("spirit_3", spirits[2].cardId); }
            data.AddData("result", actionResult);

            return data;
        }



        #region Initialization
        public static EnchantAction FromData(CardActionData data)
        {
            return new EnchantAction(data);
        }
        protected EnchantAction(CardActionData data) : base(data)
        {

        }
        protected override void ParseData(CardActionData data)
        {
            base.ParseData(data);
            player = Game.FindPlayer(data.Value<string>("player"));
            sourceCard = Game.FindCard(data.Value<string>("card"));
            enchantType = (EnchantActionType)data.Value<int>("enchant_type");
            cardMode = (CardMode)data.Value<int>("card_mode");
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
            SetDetails();

        }
        protected void SetDetails()
        {
            actionTime = .65f;
            if (enchantType == EnchantActionType.ReEnchant || enchantType == EnchantActionType.FromFaceDown || enchantType == EnchantActionType.DisEnchant) { doesSourceMove = false; } else { doesSourceMove = true; }
            _declaredMessage = $"Enchant {sourceCard.cardStats.title} with {SpiritString}";
            _actionMessage = $"{sourceCard.cardStats.title} is Enchanted with {SpiritString}!";
        }
        protected EnchantAction(Player p, GameCard source, CardSlot to, EnchantActionType enchantType, GameCard[] spiritsUsed, CardMode cMode, ActionResult ac = ActionResult.Pending) : base(p, source, ac)
        {
            toSlot = to;

            for (int i = 0; i < spiritsUsed.Length; i++)
            {
                spirits.Add(spiritsUsed[i]);
            }
            this.enchantType = enchantType;
            cardMode = cMode;

            SetDetails();
        }


        public static EnchantAction Normal(Player p, GameCard source, List<GameCard> spirits, CardSlot to, CardMode cMode)
        {
            EnchantActionType en = EnchantActionType.Normal;
            return new EnchantAction(p, source, to, en, spirits.ToArray(), cMode);
        }
        public static EnchantAction Set(Player p, GameCard source, CardSlot to)
        {
            EnchantActionType en = EnchantActionType.Set;
            return new EnchantAction(p, source, to, en, new GameCard[0], CardMode.Defense);
        }
        public static EnchantAction ReEnchant(Player p, GameCard source, List<GameCard> spirits)
        {
            EnchantActionType en = EnchantActionType.ReEnchant;
            return new EnchantAction(p, source, source.CurrentSlot, en, spirits.ToArray(), source.mode);
        }
        public static EnchantAction DisEnchant(Player p, GameCard source, List<GameCard> spirits, CardSlot to)
        {
            EnchantActionType en = EnchantActionType.DisEnchant;
            return new EnchantAction(p, source, to, en, spirits.ToArray(), source.mode);
        }
        public static EnchantAction FromFaceDown(Player p, GameCard source, List<GameCard> spirits)
        {
            EnchantActionType en = EnchantActionType.FromFaceDown;
            return new EnchantAction(p, source, source.CurrentSlot, en, spirits.ToArray(), CardMode.Attack);
        }

        //public static EnchantAction Ascend(Player p, GameCard newCard, GameCard source, GameCard spirit)
        //{
        //    CardSlot to = source.CurrentSlot;
        //    EnchantType en = EnchantType.Ascend;
        //    return new EnchantAction(p, source, to, en, spirit);
        //}


        #endregion

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

            
            yield return DoEnchant();

        }

        
        protected override IEnumerator DoMove(GameCard card, CardSlot to, float time = .65f)
        {
            yield return base.DoMove(card, to, time);
            card.SetCardMode(cardMode);
            card.CurrentSlot.RemoveCard(card);
            to.AllocateTo(card);
        }
        protected IEnumerator DoFlip(GameCard card, CardMode newMode, float time = .65f)
        {
            float acumTime = 0f;
            bool toFaceDown = false;
            if (newMode == CardMode.Defense) { toFaceDown = true; }
            float targetVal = 0f;
            Vector3 startScale = card.cardObject.GetScale();
            do
            {
                Vector3 cardScale = card.cardObject.GetScale();
                Vector2 newScale = cardScale - new Vector3((startScale.x * (Time.deltaTime * 2f)), 0f, 0f);
                card.cardObject.SetScale(newScale);
                yield return new WaitForEndOfFrame();
                acumTime += time;

            } while (Validate(acumTime, time) || card.cardObject.GetScale().x > targetVal);
            card.SetCardMode(cardMode);
            card.cardObject.SetScale(startScale);
            card.cardObject.Flip(toFaceDown);
        }

        protected IEnumerator DoEnchant()
        {
            do
            {
                IEnumerator move = Movements[0];
                yield return move;
                yield return new WaitForEndOfFrame();
                Movements.Remove(move);


            } while (Movements.Count > 0);
            End(ActionResult.Succeed);
        }

        protected override void ResolveAction(ActionResult result)
        {
            base.ResolveAction(result);

        }
    }
}
