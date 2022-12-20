using System.Collections;
using System.Collections.Generic;
using Database;
using Databases;
using UnityEngine;

namespace Cards
{
    public class EffectService : DataService
    {
        private static readonly string EffectTable = "CardsByEffect";

        #region Ability Table Info
        private static readonly string AbilityTable = "AbilityDTO";
        #endregion

        #region Trigger Table Info
        private static readonly string TriggerTable = "EventTrigger";
        private static readonly string TriggerPk = "triKey";
        #endregion


        public static List<EffectDTO> FindCardEffects(string cardKey)
        {
            string qWhere = $"cardKey = '{cardKey}' ORDER BY effOrder ASC";
            List<EffectDTO> list = ListByQuery<EffectDTO>(EffectTable, qWhere);
            return list;
        }
        public static EffectDTO FindCardEffect(string baseKey)
        {
            EffectDTO dto = ByKey<EffectDTO>(EffectTable, "effKey", baseKey);
            if (dto != null) { return dto; }
            return null;
        }


        #region EffectData
        #region Abilities
        public static AbilityDTO FindAbility(string abiKey)
        {
            AbilityDTO dto = ByPk<AbilityDTO>(AbilityTable, abiKey);
            if (dto != null) { return dto; }
            return null;
        }
        #endregion

        #region Triggers
        public static TriggerDTO FindTrigger(string triggerKey)
        {
            TriggerDTO dto = ByKey<TriggerDTO>(TriggerTable, TriggerPk, triggerKey);
            if (dto != null) { return dto; }
            return null;
        }
        #endregion
        #endregion
    }
}

