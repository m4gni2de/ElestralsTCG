using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay
{
    #region Enums
    public enum StatusType
    {
        None = 0,
        Active = 1,
        Passive = 2,
        Dynamic = 3
    }
    public enum ActiveStatusType
    {
        None = -1,
        Enchanted = 0,
        MisEnchanted = 1,
        NoAbility = 2,
        NoAttack = 3,
        NoAscend = 4,
        NoEmpower = 5,
        AttackPosition = 6,
        DefensePosition = 7,
        PositionStuck = 8,
        Empowered = 9,
        NotAttackTarget = 10,
    }


    public enum PassiveStatusTypes
    {
        None = 0,
        EnchantedWith = 1,
        InLocation = 2,
    }

    public enum DynamicStatusTypes
    {
        None = 0,
        Untargettable = 1,
    }
    #endregion
    public class CardStatus
    {
       
        protected List<ActiveStatusType> _activeStatusList = null;
        public List<ActiveStatusType> ActiveStatusList { get { _activeStatusList ??= new List<ActiveStatusType>(); return _activeStatusList; } }

        protected CardTargetArgs _untargetableBy = null;
        public CardTargetArgs UnTargetableBy { get { return _untargetableBy; } }

        private List<ElementCode> _enchantedWith = null;
        public List<ElementCode> EnchantedWith
        {
            get
            {
                _enchantedWith ??= new List<ElementCode>();
                return _enchantedWith;
            }
        }

        protected CardLocation location;

        public CardStatus(GameCard card)
        {
            SetPassiveStatus(card);
            SetDefaultStatus(card);
            
        }

        #region Default Statuses
        private void SetDefaultStatus(GameCard card)
        {
            if (card.CurrentSlot != null && card.CurrentSlot.IsInPlay)
            {
                AddActiveStatus(ActiveStatusType.Enchanted);
                if (IsMisEnchanted(card)) { AddActiveStatus(ActiveStatusType.MisEnchanted); AddActiveStatus(ActiveStatusType.NoAbility); }
                if (card.mode == CardMode.Attack) { AddActiveStatus(ActiveStatusType.AttackPosition); RemoveActiveStatus(ActiveStatusType.DefensePosition); }
                else if (card.mode == CardMode.Defense) { AddActiveStatus(ActiveStatusType.DefensePosition); RemoveActiveStatus(ActiveStatusType.AttackPosition); }
                else { RemoveActiveStatus(ActiveStatusType.AttackPosition); RemoveActiveStatus(ActiveStatusType.DefensePosition); }
            }
        }


        private bool IsMisEnchanted(GameCard card)
        {
            Dictionary<ElementCode, int> activeElements = new Dictionary<ElementCode, int>();
            for (int i = 0; i < card.EnchantingSpiritTypes.Count; i++)
            {
                if (activeElements.ContainsKey(card.EnchantingSpiritTypes[i]))
                {
                    activeElements[card.EnchantingSpiritTypes[i]] += 1;
                }
                else
                {
                    activeElements.Add(card.EnchantingSpiritTypes[i], 1);
                }
            }

            Dictionary<ElementCode, int> reqElements = new Dictionary<ElementCode, int>();
            for (int i = 0; i < card.card.SpiritsReq.Count; i++)
            {
                if (reqElements.ContainsKey(card.card.SpiritsReq[i].Code))
                {
                    reqElements[card.card.SpiritsReq[i].Code] += 1;
                }
                else
                {
                    reqElements.Add(card.card.SpiritsReq[i].Code, 1);
                }
            }

            foreach (var item in activeElements)
            {
                if (reqElements.ContainsKey(item.Key))
                {
                    if (item.Value != reqElements[item.Key]) { return true; }
                }
            }
            return false;
        }
        #endregion
        #region Active Statuses
        public bool HasActiveStatus(ActiveStatusType type)
        {
            return ActiveStatusList.Contains(type);
        }
        public bool AddActiveStatus(ActiveStatusType type)
        {
            if (!ActiveStatusList.Contains(type))
            {
                ActiveStatusList.Add(type);
                return true;
            }
            return false;
        }
        public void RemoveActiveStatus(ActiveStatusType type)
        {
            if (ActiveStatusList.Contains(type))
            {
                ActiveStatusList.Remove(type);
            }
        }
        #endregion
        #region Passive Statuses
        private void SetPassiveStatus(GameCard card)
        {
            for (int i = 0; i < card.EnchantingSpiritTypes.Count; i++)
            {
                if (EnchantedWith.Contains(card.EnchantingSpiritTypes[i]))
                {
                    EnchantedWith.Add(card.EnchantingSpiritTypes[i]);
                }
            }
            if (card.CurrentSlot != null)
            {
                location = card.CurrentSlot.slotType;
            }
        }
        public bool IsElementEnchanted(ElementCode code)
        {
            return EnchantedWith.Contains(code);
        }
        public bool IsInLocation(CardLocation loc)
        {
            return location == loc;
        }
        #endregion

        public bool CanTarget(GameCard card)
        {
            if (_untargetableBy == null) { return true; }
            return UnTargetableBy.MeetsTargetCriteria(card);
        }
    }
}

