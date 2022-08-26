using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;

namespace Packs
{
    public class BlisterPack
    {
        public PackData data;

        public List<Card> cards
        {
            get
            {
                return data.cards;
            }
        }

        public BlisterPack(BoosterSet bSet)
        {
            data = bSet.GeneratePack();

        }
    }
}

