using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameplay.Abilities;
using Databases;
using Cards;
using Gameplay.Commands;
using Gameplay.Turns;
using System;
using static UnityEditor.Progress;
using Gameplay.CardActions;

namespace Gameplay
{
   
    public class CardEffect
    {
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

        private List<GameCard> _costSpirits = null;
        public List<GameCard> CostSpirits { get { _costSpirits ??= new List<GameCard>(); return _costSpirits; } set { _costSpirits = value; } }


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
            if (Ability == null) { return false; }
            if (Ability.CanActivate())
            {
                return Validate(source);
            }
            return false;
        }


        #endregion

        #region Do Effect/Ability
        public void Try(GameCard source)
        {
            EffectAction = new EffectAction(source.Owner, source, this);
            Ability.OnAbilityTry += AwaitAbilityTry;
            CostSpirits.Clear();
            if (CastCost.Count > 0)
            {
                CostSpirits = GetCostSpirits(source);
                for (int i = 0; i < CostSpirits.Count; i++)
                {
                    CostSpirits[i].isBlackout = true;
                }
            }
            EffectAction.AddSpiritsCost(CostSpirits);
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

        private List<GameCard> GetCostSpirits(GameCard source)
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
            return results;

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
            key = card.cardId;
            if (effectData != null)
            {
                SetTrigger(effectData, card, ev);
            }
            
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
                t.EventWatching = CardEventSystem.Find(t.index);
            }

            if (t.EventWatching != null)
            {
                t.EventWatching.AddWatcher(() => OnEventTriggered(data, card), true);

            }
        }

        private void OnEventTriggered(EffectData data, GameCard card)
        {
            Trigger t = data.trigger;
            Ability a = data.ability;

            if (!data.autoUse && a.CanActivate())
            {
                if (t.whenActivate == ActivationEvent.OnEvent)
                {
                    if (ValidateTriggerArgs(t, card))
                    {
                        GameManager.Instance.AskCardEffect(this, card);
                    }
                   
                }
            }
        }

        private bool ValidateTriggerArgs(Trigger t, GameCard card)
        {
            if (t.triggerArgs == null) { return true; }

            
            for (int i = 0; i < t.triggerArgs.keys.Count; i++)
            {
                string key = t.triggerArgs.keys[i];
                var value = t.triggerArgs[key];

                for (int j = 0; j < t.EventWatching.Parameters.Count; j++)
                {
                    if (t.EventWatching.Parameters[i].Name.ToLower() == key.ToLower())
                    {
                        var pValue = t.EventWatching.Parameters[i].GetValue();
                        if (value != pValue)
                        {
                            return false;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    
                }


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

            if (!CanBeUsedInGameState(owner, sourceCard)) { return false; }

            if (CastCost.Count > 0)
            {
                if (!owner.HasAvailableSpirits(CastCost)) { AddError($"Not enough Spirits to pay for the cost!"); }
            }

            return ErrorList.Count == 0;
        }

        private bool CanBeUsedInGameState(Player p, GameCard card)
        {
            if (Game.GameStarted)
            {
                bool isInPlay = card.CurrentSlot.IsInPlay;
                if (Trigger.whenActivate == ActivationEvent.Perpetual) { return isInPlay; }
                if (Trigger.whenActivate == ActivationEvent.OnEvent) { return true; }
                if (Trigger.whenActivate == ActivationEvent.YouCan)
                {
                    Turn activeTurn;
                    bool isActivePlayer = Game.IsPlayerTurn(p, out activeTurn);
                    if (isActivePlayer)
                    {
                        if (!isInPlay) { return false; }
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

