using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameplay.Abilities;
using Databases;
using Cards;

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
        private List<EffectData> _effectsList = null;
        public List<EffectData> EffectsList
        {
            get { _effectsList ??= new List<EffectData>(); return _effectsList; }
        }

        #endregion

        #region Functions
        public bool IsEmpty { get { return _effectsList.Count == 0; } }

        
        #endregion



        #region Initialization
        public static CardEffect GetEffect(string cardKey)
        {
            List<EffectDTO> list = EffectService.FindCardEffects(cardKey);
            if (list.Count > 0)
            {
                return new CardEffect(list);
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
        CardEffect(List<EffectDTO> list = null)
        {
            EffectsList.Clear();
           if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    EffectData data = new EffectData(list[i]);
                    AddEffectData(data);
                }
            }
        }
        private void AddEffectData(EffectData data)
        {
            data.LoadEffect();
            EffectsList.Add(data);
        }
        #endregion

        #region Toggle Event Triggers
        public void SetEvents(GameCard card, CardEventSystem ev)
        {
            for (int i = 0; i < EffectsList.Count; i++)
            {
                EffectData data = EffectsList[i];
                SetTrigger(data, card, ev);
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
                t.EventWatching.SetWatcher(() => OnEventTriggered(data, card), true);
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
                    GameManager.Instance.AskCardEffect(data, card);
                }
            }
        }
        #endregion
    }
}

