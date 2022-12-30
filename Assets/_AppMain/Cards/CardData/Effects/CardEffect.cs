using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameplay.Abilities;
using Databases;
using Cards;
using Gameplay.Commands;
using Gameplay.Turns;
using System;
using Gameplay.CardActions;
using GameEvents;
using UnityEngine.UIElements;

namespace Gameplay
{
    [System.Serializable]
    public class CardEffect : iLog
    {
        #region Static Indexing
        private static Dictionary<CardEffect, GameCard> _cardsByEffect = null;
        public static Dictionary<CardEffect, GameCard> CardsByEffect
        {
            get
            {
                _cardsByEffect ??= new Dictionary<CardEffect, GameCard>();
                return _cardsByEffect;
            }
        }
        #endregion

        #region Operators
        //public static implicit operator CardEffect(EffectDTO dto)
        //{
        //    CardEffect eff = new CardEffect(dto);
        //}
        #endregion


        #region Properties
        private string _key = "";
        public string key { get { return _key; } protected set { _key = value; }  }
        public bool IsDirty { get; protected set; }
        private EffectData _effectData = null;
        public EffectData effectData { get { return _effectData; } protected set { _effectData = value; } }
        public bool canActivate { get; set; }

        public Ability Ability { get { return effectData.ability; } }
        public Trigger Trigger { get { return effectData.trigger; } }
        public List<ElementCode> CastCost { get { return effectData.castCost; } }

        private bool _isExhausted = false;
        public bool IsExhausted { get { return _isExhausted; } protected set { _isExhausted = value; } }

        private List<string> _costSpirits = null;
        public List<string> CostSpirits { get { _costSpirits ??= new List<string>(); return _costSpirits; } set { _costSpirits = value; } }
        protected List<GameCard> GetCostSpiritCards()
        {
            List<GameCard> cards = new List<GameCard>();
            if (_costSpirits == null) { return cards; }
            for (int i = 0; i < CostSpirits.Count; i++)
            {
                GameCard spirit = Game.FindCard(CostSpirits[i]);
                cards.Add(spirit);
            }
            return cards;
        }


        private EffectAction _effectAction = null;
        public EffectAction EffectAction
        {
            get
            {
                return _effectAction;
            }
            set
            {
                _effectAction = value;
            }
        }
        #endregion

        #region Functions
        public bool IsEmpty { get { return effectData == null; } }

        
        public void CheckEffects(GameCard source)
        {
            canActivate = CanActivate(source);
        }

        protected bool CanActivate(GameCard source)
        {
            //if (Ability == null) { return false; }
            //if (Ability.CanActivate())
            //{
            //    return Validate(source);
            //}
            return false;
        }


        #endregion

        #region Do Effect/Ability
        public void Try(GameCard source)
        {
            EffectAction = new EffectAction(source.Owner, source, this);
            Ability.OnAbilityTry += AwaitAbilityTry;
            CostSpirits.Clear();

            List<GameCard> spiritCards = new List<GameCard>();
            

            if (CastCost.Count > 0)
            {
                CostSpirits = GetCostSpirits(source);
                spiritCards = GetCostSpiritCards();
                for (int i = 0; i < spiritCards.Count; i++)
                {
                    spiritCards[i].isBlackout = true;
                }
            }
            EffectAction.AddSpiritsCost(spiritCards);
            Ability.TryAbility(source);
        }
        public void AwaitAbilityTry(Ability abi, bool didTry)
        {
            abi.OnAbilityTry -= AwaitAbilityTry;
            if (didTry)
            {
                for (int i = 0; i < abi.abilityActions.Count; i++)
                {
                    EffectAction.AddAction(abi.abilityActions[i]);
                }
                SetUsed(true);
                GameManager.Instance.CardEffectActivate(EffectAction);
            }
        }

        private List<string> GetCostSpirits(GameCard source)
        {
            
            Dictionary<ElementCode, int> req = new Dictionary<ElementCode, int>();
            for (int i = 0; i < CastCost.Count; i++)
            {
                if (req.ContainsKey(CastCost[i]))
                {
                    req[CastCost[i]]++;
                }
                else
                {
                    req.Add(CastCost[i], 1);
                }
            }


            List<GameCard> results = source.Owner.GetSpiritsOfType(req);
            List<string> ids = new List<string>();
            for (int i = 0; i < results.Count; i++)
            {
                ids.Add(results[i].cardId);
            }
            return ids;

        }

        #endregion

        #region Initialization
        public static CardEffect GetEffect(string cardKey)
        {
            List<EffectDTO> list = EffectService.FindCardEffects(cardKey);
            if (list.Count > 0)
            {
                return new CardEffect(list[0]);
            }
            return new CardEffect(null);
        }
        public static List<CardEffect> GetCardEffects(string cardKey)
        {
            List<EffectDTO> list = EffectService.FindCardEffects(cardKey);
            List<CardEffect> effects = new List<CardEffect>();
            for (int i = 0; i < list.Count; i++)
            {
                CardEffect cf = new CardEffect(list[i]);
                effects.Add(cf);
            }

            return effects;
        }
        public static CardEffect Empty
        {
            get
            {
                return new CardEffect(null);
            }
        }
       
        CardEffect(EffectDTO dto = null)
        {
            
            if (dto != null)
            {
                
                _effectData = new EffectData(dto);
                _effectData.LoadEffect();
            }
            else
            {
                _effectData = null;
            }
        }
        #endregion

        #region Toggle Event Triggers
        private void OnGameStateChanged(GameCard source)
        {

        }
        public void SetEvents(GameCard card, CardEventSystem ev)
        {
            //key = card.cardId;
            //if (!CardsByEffect.ContainsKey(this))
            //{
            //    CardsByEffect.Add(this, card);
            //}

            //if (effectData != null)
            //{
            //    SetTrigger(effectData, card, ev);
            //}
            
        }

        private void SetTrigger(EffectData data, GameCard card, CardEventSystem ev)
        {
            Trigger t = data.trigger; 

            if (t.isLocal)
            {
                t.EventWatching = ev.FindLocal(t.index);
            }
            else
            {
                t.EventWatching = CardEventSystem.FindGlobal(t.index);
            }

            if (t.EventWatching != null)
            {
                t.EventWatching.AddArgWatcher(OnWatchedEventTriggered);

                this.Log($"{card.workingName} has started Watching the '{t.EventWatching.key}' GameEvent for its Effect.");

            }
        }

        private void OnWatchedEventTriggered(GameEventArgs args)
        {
            GameCard card = CardsByEffect[this];

            if (!Ability.CanActivate()) { return; }
            if (Trigger.whenActivate == ActivationEvent.OnEvent)
            {
                if (!ValidateArgs(Trigger, args, card)) { return; }
            }
            if (!effectData.autoUse)
            {
                GameManager.Instance.AskCardEffect(this, card);
                this.Log($"{card.workingName} can activate its Effect after being Triggered by the '{Trigger.EventWatching.key}' GameEvent.");
            }
            else
            {
                Try(card);
                this.Log($"{card.workingName} will activate its Effect after being Triggered by the '{Trigger.EventWatching.key}' GameEvent.");
            }

          
        }
      
        private bool ValidateArgs(Trigger t, GameEventArgs args, GameCard source)
        {
            if (t.triggerArgs == null) { return true; }


            for (int i = 0; i < t.triggerArgs.keys.Count; i++)
            {
                string triggerKey = t.triggerArgs.keys[i];
                var triggerValue = t.GetTriggerArgsValue(triggerKey); 



                var KeyProp = t.ParseKeyPropFromArgs(triggerKey);
                string key = KeyProp.Item1;
                string prop = KeyProp.Item2;

                object argValue = null;
                if (key.ToLower() == "this")
                {
                    argValue = source.GetPropertyOrFieldValue(prop);
                }
                else
                {
                    argValue = args.GetArgProperty(key, prop);
                }

                
                if (argValue.GetType() == typeof(bool))
                {
                    bool boolVal = (bool) argValue;
                    argValue = boolVal.BoolToInt();
                }
                if (!argValue.Equals(triggerValue)) { return false; }
            }
            return true;
        }

       

        #endregion

        #region Validation
        private List<string> _errorList = null;
        public List<string> ErrorList { get { _errorList ??= new List<string>(); return _errorList; } }
        public void AddError(string msg)
        {
            ErrorList.Add(msg);
        }
        public bool Validate(GameCard sourceCard)
        {
            ErrorList.Clear();
            Player owner = sourceCard.Owner;

            if (IsExhausted) { AddError("Effect has already been used."); return false; }

            if (!CanBeUsedInGameState(owner, sourceCard)) { AddError($"{sourceCard.cardName} cannot active its effect from its Location."); return false; }

            if (CastCost.Count > 0)
            {
                if (!owner.HasAvailableSpirits(CastCost)) { AddError($"Not enough Spirits to pay for the cost!"); return false; }
            }

            return ErrorList.Count == 0;
        }

        private bool CanBeUsedInGameState(Player p, GameCard card)
        {
            if (Game.GameStarted)
            {
                bool isInPlay = card.CurrentSlot.IsInPlay;
                if (Trigger.whenActivate == ActivationEvent.Perpetual) { return Trigger.ValidateCardLocation(card); }
                if (Trigger.whenActivate == ActivationEvent.OnEvent) { return Trigger.ValidateCardLocation(card); }
                if (Trigger.whenActivate == ActivationEvent.YouCan)
                {
                    Turn activeTurn;
                    bool isActivePlayer = Game.IsPlayerTurn(p, out activeTurn);
                    if (isActivePlayer && activeTurn.IsYours)
                    {
                        if (!Trigger.ValidateCardLocation(card)) { return false; }
                        return activeTurn.ActivePhase == activeTurn.MainPhase;
                    }
                    return false;
                }
            }
            return false;
        }
        #endregion

        public void SetUsed(bool isUsed)
        {
            IsExhausted = isUsed;
        }

    }
}

