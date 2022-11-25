using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gameplay
{
    [System.Serializable]
    public class EffectData
    {

        public enum EffectTrigger
        {
            OnCast = 0,
            OncePerTurn = 1,
            ActionResponse = 2,

        }

        public enum TargetType
        {
            Self = 0,

        }

        #region Properties
        protected string key;
        protected string name;

        protected ActionCategory responseCategory = ActionCategory.None;
        protected EffectTrigger trigger;
        #endregion
        
    }
}

