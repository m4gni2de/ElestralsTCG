using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StoneVariants
{

    public class ElestralVariant : StoneVariant
    {
        public SpriteRenderer[] SubClassSp = new SpriteRenderer[2];

        protected override void SetSprite(Card card)
        {
            base.SetSprite(card);

            for (int i = 0; i < SubClassSp.Length; i++)
            {
                SubClassSp[i].sprite = null;
            }

            SetSubClassSprite(card);
        }



        protected void SetSubClassSprite(Card card)
        {
            Elestral e = (Elestral)card;
            Elestral.SubClass es1 = e.Data.subType1;
            Elestral.SubClass es2 = e.Data.subType2;

            SubClassSp[0].sprite = es1.ElestralSubClassSprite();

            if (es2 != Elestral.SubClass.None)
            {
                SubClassSp[1].sprite = es2.ElestralSubClassSprite();
            }

        }
    }
}
