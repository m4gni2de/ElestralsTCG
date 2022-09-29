using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gameplay.CardActions
{
    public class AscendAction : CardAction, iEnchant
    {
        #region Properties
        protected CardSlot toSlot;
        private List<GameCard> _spiritsTaken = null;
        protected List<GameCard> spiritsTaken { get { _spiritsTaken ??= new List<GameCard>(); return _spiritsTaken; } }
        protected GameCard tributedCard;
        public GameCard TributedCard { get { return tributedCard; } }
        protected CardMode cardMode;
        protected GameCard CatalystSpirit;

        protected override ActionCategory GetCategory()
        {
            return ActionCategory.Ascend;
        }
        #endregion

        #region Data Keys
        public static readonly string TributedCardKey = "tributed_card";
        public static readonly string SlotToKey = "slot_to";
        #endregion

        #region Interface
        public bool IsNormalEnchantment()
        {
            return true;
        }
        #endregion

        #region CardData
        protected override CardActionData GetActionData()
        {
            int spiritCount = spiritsTaken.Count;

            CardActionData data = new CardActionData(this);
            data.SetPlayer(player);
            data.SetSourceCard(sourceCard);
            data.AddData("card_mode", (int)cardMode);
            data.AddData(TributedCardKey, tributedCard.cardId);
            data.AddData(SlotToKey, toSlot.slotId);
            data.AddData("catalyst_spirit", CatalystSpirit.cardId);
            data.SetSpiritList(spiritsTaken);
            data.AddData("result", (int)actionResult);

            return data;
        }

        public static AscendAction FromData(CardActionData data)
        {
            return new AscendAction(data);
        }
        protected AscendAction(CardActionData data) : base(data)
        {

        }
        protected override void ParseData(CardActionData data)
        {
            base.ParseData(data);
            player = Game.FindPlayer(data.Value<string>(CardActionData.PlayerKey));
            sourceCard = Game.FindCard(data.Value<string>(CardActionData.SourceKey));
            tributedCard = Game.FindCard(data.Value<string>(TributedCardKey));
            cardMode = (CardMode)data.Value<int>("card_mode");
            toSlot = Game.FindSlot(data.Value<string>(SlotToKey));
            actionResult = (ActionResult)data.Value<int>("result");
            CatalystSpirit = Game.FindCard(data.Value<string>("catalyst_spirit"));
            int spiritCount = data.CountOfSpiritFields();
            for (int i = 0; i < spiritCount; i++)
            {
                string fieldName = $"spirit_{i + 1}";
                string spirit = data.Value<string>(fieldName);
                if (!string.IsNullOrEmpty(spirit))
                {
                    GameCard spiritCard = Game.FindCard(spirit);
                    spiritsTaken.Add(spiritCard);
                }
            }
            SetDetails();

        }
        #endregion

        AscendAction(Player p, GameCard newCard, GameCard tributedTarget, List<GameCard> spiritsTaking, GameCard spiritAdded, CardMode cMode) : base(p, newCard)
        {
            toSlot = tributedTarget.CurrentSlot;
            tributedCard = tributedTarget;
            spiritsTaken.AddRange(spiritsTaking);
            CatalystSpirit = spiritAdded;
            cardMode = cMode;

            SetDetails();
        }
        protected void SetDetails()
        {
            actionTime = .65f;
            _declaredMessage = $"{sourceCard.cardStats.title} wants to Ascend from {tributedCard.cardStats.title}!";
            _actionMessage = $"{sourceCard.cardStats.title} Ascends from {tributedCard.cardStats.title}!";

        }

        public override IEnumerator PerformAction()
        {
            Movements.Clear();

            Movements.Add(DoMove(CatalystSpirit, toSlot, .45f));

            for (int i = 0; i < spiritsTaken.Count; i++)
            {
                GameCard c = spiritsTaken[i];
                if (c.CurrentSlot != toSlot)
                {
                    Movements.Add(DoMove(c, toSlot, .45f));
                }
            }

            Movements.Add(DoMove(tributedCard, player.gameField.UnderworldSlot, .45f));
            Movements.Add(DoMove(sourceCard, toSlot, .6f));
            yield return DoMovements();

            sourceCard.SetCardMode(cardMode);
            GameManager.SelectedCard = sourceCard;
        }

        

        protected override IEnumerator DoMove(GameCard card, CardSlot to, float time = .65f)
        {
            yield return base.DoMove(card, to, time);
            card.CurrentSlot.RemoveCard(card);
            to.AllocateTo(card);
        }


        

    }
}

