using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class Arena : MonoBehaviour
    {
        public Field NearField, FarField;

        private List<Field> _Fields = null;
        private List<Field> Fields
        {
            get
            {
                if (_Fields == null)
                {
                    _Fields = new List<Field>();
                    _Fields.Add(NearField);
                    _Fields.Add(FarField);
                }
                return _Fields;
            }
        }

        #region Player Properties
        public Field GetPlayerField(Player p)
        {
            if (NearField._player == p) { return NearField; }
            return FarField;
        }
        public Player GetSlotOwner(CardSlot slot)
        {
            for (int i = 0; i < Fields.Count; i++)
            {
                if (Fields[i].cardSlots.Contains(slot))
                {
                    return Fields[i]._player;
                }
            }
            return null;
        }

   
        public void RemoveSelector()
        {

        }

        //public CardSlot GetCardSlot(Player p, CardLocation location)
        //{
        //    Field f = GetPlayerField(p);

        //    switch (location)
        //    {
        //        case CardLocation.removed:
        //            break;
        //        case CardLocation.Elestral:
        //            break;
        //        case CardLocation.Rune:
        //            break;
        //        case CardLocation.Stadium:
        //            break;
        //        case CardLocation.Underworld:
        //            break;
        //        case CardLocation.Deck:
        //            break;
        //        case CardLocation.SpiritDeck:
        //            break;
        //        case CardLocation.Hand:
        //            break;
        //        default:
        //            break;
        //    }
        //}
        #endregion

        private void Awake()
        {
            
        }
        public void SetPlayer(Player p, string fieldId)
        {
            Field f = NearField;
            if (!p.IsLocal) { f = FarField; }
            //if (NearField._player != null) { f = FarField; }
            f.SetPlayer(p, fieldId);
        }
        public void SetPlayer(Player p)
        {
            Field f = NearField;
            if (!p.IsLocal) { f = FarField; }
            //if (NearField._player != null) { f = FarField; }
            f.SetPlayer(p);
        }



    }
}

