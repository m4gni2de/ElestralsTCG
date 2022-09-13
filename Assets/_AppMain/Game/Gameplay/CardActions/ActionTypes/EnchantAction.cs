using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.CardActions
{
    public class EnchantAction : CardAction
    {
        public enum EnchantType
        {
            Set = -1,
            Normal = 0,
            Ascend = 1,
            ReEnchant = 2,
            DisEnchant = 3,
            FromFaceDown = 4,
        }

        protected CardSlot toSlot;
        public EnchantType enchantType;
        private List<GameCard> _spirits = null;
        protected List<GameCard> spirits { get { _spirits ??= new List<GameCard>(); return _spirits; } }
        protected CardMode cardMode;
        protected bool doesSourceMove = true;

        protected override CardActionData GetActionData()
        {
            int spiritCount = spirits.Count;

            CardActionData data = new CardActionData(this);
            data.AddData("player", player.userId);
            data.AddData("card", sourceCard.cardId);
            data.AddData("action_type", "enchant");
            data.AddData("enchant_type", (int)enchantType);
            data.AddData("card_mode", (int)cardMode);
            data.AddData("slot_to", toSlot.index);
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
        protected EnchantAction(Player p, GameCard source, CardSlot to, EnchantType enchantType, GameCard[] spiritsUsed, CardMode cMode) : base(p, source)
        {
            toSlot = to;

            for (int i = 0; i < spiritsUsed.Length; i++)
            {
                spirits.Add(spiritsUsed[i]);
            }
            this.enchantType = enchantType;
            cardMode = cMode;

            if (enchantType == EnchantType.ReEnchant || enchantType == EnchantType.FromFaceDown) { doesSourceMove = false; } else { doesSourceMove = true; }
        }


        public static EnchantAction Normal(Player p, GameCard source, List<GameCard> spirits, CardSlot to, CardMode cMode)
        {
            EnchantType en = EnchantType.Normal;
            return new EnchantAction(p, source, to, en, spirits.ToArray(), cMode);
        }
        public static EnchantAction Set(Player p, GameCard source, CardSlot to)
        {
            EnchantType en = EnchantType.Set;
            return new EnchantAction(p, source, to, en, new GameCard[0], CardMode.Defense);
        }
        public static EnchantAction ReEnchant(Player p, GameCard source, List<GameCard> spirits)
        {
            EnchantType en = EnchantType.ReEnchant;
            return new EnchantAction(p, source, source.CurrentSlot, en, spirits.ToArray(), source.mode);
        }
        public static EnchantAction FromFaceDown(Player p, GameCard source, List<GameCard> spirits)
        {
            EnchantType en = EnchantType.FromFaceDown;
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
                sourceCard.Enchant(spirits[i]);
            }

            if (enchantType == EnchantType.FromFaceDown)
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

            } while (IsValid(acumTime, time) || card.cardObject.GetScale().x > targetVal);
            card.SetCardMode(cardMode);
            card.cardObject.SetScale(startScale);
            card.cardObject.Flip(toFaceDown);
        }

        protected IEnumerator DoEnchant()
        {
            do
            {
                yield return Movements[0];
                yield return new WaitForEndOfFrame();
                Movements.RemoveAt(0);

            } while (Movements.Count > 0);
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
    }
}
