using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    [System.Serializable]
    public struct SlotData
    {
        public string slotId;
        public string fieldId;
        public string owner;
        public int index;
        public string name;
        public string[] cardsOn;
    }

    [System.Serializable]
    public class GameMap
    {
        #region Properties
        private List<SlotData> _Slots = null;
        public List<SlotData> Slots
        {
            get
            {
                _Slots ??= new List<SlotData>();
                return _Slots;
            }
        }
        #endregion
    }
}

