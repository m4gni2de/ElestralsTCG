using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Gameplay.CardActions.DrawAction;

namespace Gameplay.CardActions
{
    public enum CastActionType
    {
        Set = -1,
        Cast = 0,
        Enchant = 1,
        DisEnchant = 2,
        FromFaceDown = 3,
    }

    public class CastAction : CardAction, iCast
    {

        #region Operators
        public static EmpowerAction operator +(CastAction ac, GameCard ca)
        {

            if (ca.cardStats.cardType != CardType.Elestral) { return null; }
            CardActionData data = ac.ActionData;
            data.SetData(CardActionData.CategoryKey, (int)ActionCategory.Empower);
            data.SetData("empowered_elestral", ca.cardId);
            return EmpowerAction.FromData(data);
        }
        #endregion

        #region Interface
        public bool IsNormalCast()
        {
            return castType == CastActionType.Cast;
        }
        #endregion

        protected CardSlot toSlot;
        public CardSlot enchantingSlot { get { return toSlot; } }
        public CastActionType castType;
        private List<GameCard> _spirits = null;
        public List<GameCard> spirits { get { _spirits ??= new List<GameCard>(); return _spirits; } }
        
        
        
        protected CardMode cardMode;
        protected bool doesSourceMove = true;
        protected override ActionCategory GetCategory()
        {
            return ActionCategory.Cast;
        }

        protected override string LocalActionMessage
        {
            get
            {
                string msg = "";
                switch (castType)
                {
                    case CastActionType.Set:
                        msg = $"{sourceCard.cardStats.title} is played Face-Down!";
                        break;
                    case CastActionType.Cast:
                        msg = $"{sourceCard.cardStats.title} is Cast with {spirits.SpiritUnicode()}!";
                        break;
                    case CastActionType.Enchant:
                        msg = $"{sourceCard.cardStats.title} is Enchanted with {spirits.SpiritUnicode()}!";
                        break;
                    case CastActionType.DisEnchant:
                        msg = $"{sourceCard.cardStats.title} has been DisEnchanted from its {spirits.SpiritUnicode()}!";
                        break;
                    case CastActionType.FromFaceDown:
                        msg = $"{sourceCard.cardStats.title} is flipped face up and Cast by {spirits.SpiritUnicode()}!";
                        break;
                }
                return msg;
            }
        }
        protected override string RemoteActionMessage
        {
            get
            {
                
                
                if (castType == CastActionType.Set)
                {
                    return $"{player.username} plays a card Face-Down!";
                }
                else
                {
                    return LocalActionMessage;
                }
               
            }
        }
        protected override string LocalDeclareMessage
        {
            get
            {
                string msg = "";
                switch (castType)
                {
                    case CastActionType.Set:
                        msg = $"Play {sourceCard.cardStats.title} Face-Down!";
                        break;
                    case CastActionType.Cast:
                        msg = $"Cast {sourceCard.cardStats.title} with {spirits.SpiritUnicode()}!";
                        break;
                    case CastActionType.Enchant:
                        msg = $"Enchanted {sourceCard.cardStats.title} with {spirits.SpiritUnicode()}!";
                        break;
                    case CastActionType.DisEnchant:
                        msg = $"DisEnchant {spirits.SpiritUnicode()} from {sourceCard.cardStats.title}!";
                        break;
                    case CastActionType.FromFaceDown:
                        msg = $"Flip up and Cast {sourceCard.cardStats.title} with {spirits.SpiritUnicode()}!";
                        break;
                }
                return msg;

            }
        }
        protected override string RemoteDeclareMessage
        {
            get
            {
                if (castType == CastActionType.Set)
                {
                    return $"{player.username} plays a card Face-Down!";
                }
                else
                {
                    return LocalDeclareMessage;
                }
            }
        }

        protected override CardActionData GetActionData()
        {
            int spiritCount = spirits.Count;

            CardActionData data = new CardActionData(this);
            data.SetPlayer(player);
            data.SetSourceCard(sourceCard);
            data.AddData("cast_type", (int)castType);
            data.AddData("card_mode", (int)cardMode);
            data.AddData("slot_to", toSlot.slotId);
            for (int i = 0; i < spiritCount; i++)
            {
                data.SetSpirit(i + 1, spirits[i].cardId);
            }
            data.AddData("result", (int)actionResult);

            return data;
        }



        #region Initialization
        public static CastAction FromData(CardActionData data)
        {
            return new CastAction(data);
        }
        protected CastAction(CardActionData data) : base(data)
        {

        }
        protected override void ParseData(CardActionData data)
        {
            id = data.Value<string>("actionKey");
            player = Game.FindPlayer(data.Value<string>(CardActionData.PlayerKey));
            sourceCard = Game.FindCard(data.Value<string>(CardActionData.SourceKey));
            castType = (CastActionType)data.Value<int>("cast_type");
            cardMode = (CardMode)data.Value<int>("card_mode");
            toSlot = Game.FindSlot(data.Value<string>("slot_to"));
            actionResult = data.GetResult();
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
        protected virtual void SetDetails()
        {
            actionTime = .65f;
            if (castType == CastActionType.Enchant || castType == CastActionType.FromFaceDown || castType == CastActionType.DisEnchant) { doesSourceMove = false; } else { doesSourceMove = true; }
            //_declaredMessage = $"Cast {sourceCard.cardStats.title} with {SpiritString}";
            //_actionMessage = $"{sourceCard.cardStats.title} is Cast with {SpiritString}!";
            
        }
        protected CastAction(Player p, GameCard source, CardSlot to, CastActionType castType, GameCard[] spiritsUsed, CardMode cMode, ActionResult ac = ActionResult.Pending) : base(p, source, ac)
        {
            toSlot = to;
            for (int i = 0; i < spiritsUsed.Length; i++)
            {
                spirits.Add(spiritsUsed[i]);
            }
            this.castType = castType;
            cardMode = cMode;

            SetDetails();
        }

       
        public static CastAction Cast(Player p, GameCard source, List<GameCard> spirits, CardSlot to, CardMode cMode)
        {
            CastActionType en = CastActionType.Cast;
            return new CastAction(p, source, to, en, spirits.ToArray(), cMode, ActionResult.Pending);
        }
        public static CastAction Set(Player p, GameCard source, CardSlot to)
        {
            CastActionType en = CastActionType.Set;
            return new CastAction(p, source, to, en, new GameCard[0], CardMode.Defense);
        }
        public static CastAction Enchant(Player p, GameCard source, List<GameCard> spirits)
        {
            CastActionType en = CastActionType.Enchant;
            return new CastAction(p, source, source.CurrentSlot, en, spirits.ToArray(), source.mode);
        }
        public static CastAction DisEnchant(Player p, GameCard source, List<GameCard> spirits, CardSlot to)
        {
            CastActionType en = CastActionType.DisEnchant;
            return new CastAction(p, source, to, en, spirits.ToArray(), source.mode);
        }
        public static CastAction FromFaceDown(Player p, GameCard source, List<GameCard> spirits)
        {
            CastActionType en = CastActionType.FromFaceDown;
            return new CastAction(p, source, source.CurrentSlot, en, spirits.ToArray(), CardMode.Attack);
        }

        
        
        #endregion

        public override IEnumerator PerformAction()
        {
            Movements.Clear();

            for (int i = 0; i < spirits.Count; i++)
            {
                Movements.Add(DoMove(spirits[i], toSlot));
            }

            if (castType == CastActionType.FromFaceDown)
            {
                Movements.Add(DoFlip(sourceCard, CardMode.Attack));
            }

            if (doesSourceMove)
            {
                Movements.Add(DoMove(sourceCard, toSlot));

            }

            
            yield return DoMovements();

           
        }


        public override IEnumerator DisplayRemoteAction()
        {
            this.Freeze();
            
            
            float waitTime = .45f;
            GameMessage message = GameMessage.FromAction(RemoteActionMessage, this, true, waitTime);
            GameManager.Instance.messageControl.ShowMessage(message);

            float acumTime = 0;
            do
            {
                acumTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            } while (acumTime <= waitTime && !isResolved);
            this.Thaw();
        }
        protected override IEnumerator DoMove(GameCard card, CardSlot to, float time = .65f)
        {
            yield return base.DoMove(card, to, time);
            card.SetCardMode(cardMode);
            card.CurrentSlot.RemoveCard(card);
            to.AllocateTo(card);
        }


        #region Action Building
        public static CastActionType ParseSource(CardSlot from)
        {
            switch (from.slotType)
            {
                
                case CardLocation.Elestral:
                    return CastActionType.Cast;
                case CardLocation.Rune:
                    return CastActionType.Cast;
                case CardLocation.Stadium:
                    return CastActionType.Cast;
                case CardLocation.Underworld:
                    return CastActionType.Enchant;
                case CardLocation.Deck:
                    return CastActionType.Enchant;
                case CardLocation.SpiritDeck:
                    return CastActionType.Enchant;
                case CardLocation.Hand:
                    return CastActionType.Enchant;
                default:
                    return CastActionType.Set;
            }
        }
        #endregion

        
    }
}
