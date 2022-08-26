using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class Arena : MonoBehaviour
    {
        public Field NearField, FarField;


        #region Player Properties
        public Field GetPlayerField(Player p)
        {
            if (NearField._player == p) { return NearField; }
            return FarField;
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
        public void SetPlayer(Player p)
        {
            Field f = NearField;
            if (NearField._player != null) { f = FarField; }
            f.SetPlayer(p);
        }


    }
}

