using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameplay.Abilities;
using Databases;
using Cards;
using Database;
using Gameplay.Turns;

namespace Gameplay
{
    [System.Serializable]
    public class EffectData
    {


        #region Properties
        private EffectDTO rawData { get; set; }

        private Ability _ability = null;
        public Ability ability { get { return _ability; } protected set { _ability = value; } }

        private Trigger _trigger = null;
        public Trigger trigger { get { return _trigger; } protected set { _trigger = value; } }

        public string effectKey { get; set; }
        public int index { get; set; }
        public bool autoUse { get; set; }
        private List<ElementCode> _castCost = null;
        public List<ElementCode> castCost { get { _castCost ??= new List<ElementCode>(); return _castCost; } protected set { _castCost = value; } }


        #endregion


       

        #region Initialization
        public EffectData(EffectDTO dto)
        {
            rawData = dto;
        }
        public void LoadEffect()
        {
            if (rawData == null) { return; }
            effectKey = rawData.cardKey;
            index = rawData.effOrder;
            autoUse = rawData.autoUse.IntToBool();
            ability = SetAbility(rawData.abiKey);
            trigger = SetTrigger(rawData.triggerKey);
            castCost = SetCost(rawData.castCost);
        }

        private Ability SetAbility(string abiKey)
        {
            AbilityDTO dto = EffectService.FindAbility(abiKey);
            if (dto != null)
            {
                return Ability.Get(dto);
            }
            return null;
        }

        private Trigger SetTrigger(string triKey)
        {
            TriggerDTO dto = EffectService.FindTrigger(triKey);
            if (dto != null)
            {
                bool isLocal = dto.isLocal.IntToBool();
                return new Trigger(isLocal, dto.activation, dto.result, dto.timing, dto.triggerArgs);
            }
            return null;
        }

        private List<ElementCode> SetCost(string costs)
        {
            List<int> costList = costs.AsList(",").StringToInt();
            List<ElementCode> results = new List<ElementCode>();
            for (int i = 0; i < costList.Count; i++)
            {
                results.Add((ElementCode)costList[i]);
            }
            return results;
        }
        #endregion

      
    }
}

