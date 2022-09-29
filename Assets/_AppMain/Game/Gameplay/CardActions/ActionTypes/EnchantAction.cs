using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Gameplay.CardActions.DrawAction;

namespace Gameplay.CardActions
{
    public enum EnchantActionType
    {
        Set = -1,
        Normal = 0,
        ReEnchant = 1,
        DisEnchant = 2,
        FromFaceDown = 3,
    }

    public class EnchantAction : CardAction, iEnchant
    {
        

        #region Interface
        public bool IsNormalEnchantment()
        {
            return enchantType == EnchantActionType.Normal;
        }
        #endregion

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
        protected override ActionCategory GetCategory()
        {
            return ActionCategory.Enchant;
        }

        protected override CardActionData GetActionData()
        {
            int spiritCount = spirits.Count;

            CardActionData data = new CardActionData(this);
            data.SetPlayer(player);
            data.SetSourceCard(sourceCard);
            data.AddData("enchant_type", (int)enchantType);
            data.AddData("card_mode", (int)cardMode);
            data.AddData("slot_to", toSlot.slotId);
            for (int i = 0; i < spiritCount; i++)
            {
                data.SetSpirit(i + 1, spirits[i].cardId);
            }
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
            player = Game.FindPlayer(data.Value<string>(CardActionData.PlayerKey));
            sourceCard = Game.FindCard(data.Value<string>(CardActionData.SourceKey));
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

            
            yield return DoMovements();

        }

        
        protected override IEnumerator DoMove(GameCard card, CardSlot to, float time = .65f)
        {
            yield return base.DoMove(card, to, time);
            card.SetCardMode(cardMode);
            card.CurrentSlot.RemoveCard(card);
            to.AllocateTo(card);
        }


        #region Action Building
        public static EnchantActionType ParseSource(CardSlot from)
        {
            switch (from.slotType)
            {
                
                case CardLocation.Elestral:
                    return EnchantActionType.ReEnchant;
                case CardLocation.Rune:
                    return EnchantActionType.ReEnchant;
                case CardLocation.Stadium:
                    return EnchantActionType.ReEnchant;
                case CardLocation.Underworld:
                    return EnchantActionType.Normal;
                case CardLocation.Deck:
                    return EnchantActionType.Normal;
                case CardLocation.SpiritDeck:
                    return EnchantActionType.Normal;
                case CardLocation.Hand:
                    return EnchantActionType.Normal;
                default:
                    return EnchantActionType.Set;
            }
        }
        #endregion
    }
}
