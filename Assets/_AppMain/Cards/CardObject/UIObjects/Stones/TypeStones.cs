using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardsUI.Stones;

namespace CardsUI
{
    public class TypeStones : MonoBehaviour
    {
        public enum StoneLayout
        {
            Standard = 0,
            Linear = 1
        }

        public bool UseLargeImages = true;
        public StoneLayout _layout = StoneLayout.Standard;
        public List<TypeStone> Stones;

        
        
        protected TypeStone[] UseStones(int count)
        {
            TypeStone[] stones = new TypeStone[count];

          
            if (count == 1)
            {
                stones[0] = Stones[0];
                return stones;
            }
            if (count == 2)
            {
                if (_layout == StoneLayout.Standard)
                {
                    stones[0] = Stones[3];
                    stones[1] = Stones[4];
                }
                else
                {
                    stones[0] = Stones[0];
                    stones[1] = Stones[1];
                }
            }
            else
            {
                stones[0] = Stones[0];
                stones[1] = Stones[1];
                stones[2] = Stones[2];
            }

            return stones;
        }

        private void Awake()
        {
            HideAll();
        }

        private void HideAll()
        {
            for (int i = 0; i < Stones.Count; i++)
            {
                Stones[i].Hide();
            }
        }

        public void SetStones(Card card)
        {
            HideAll();
            int count = card.SpiritsReq.Count;

            if (count == 0)
            {
                Debug.Log(card.cardData.cardName);
            }
            TypeStone[] stones = UseStones(count);

            for (int i = 0; i < stones.Length; i++)
            {
                TypeStone s = stones[i];
                SetTypeSprite(s, (int)card.SpiritsReq[i].BaseData.Code);
            }

            //if (count != 2)
            //{
            //    if (count == 1)
            //    {
            //        SetTypeSprite(Stones[0], (int)card.SpiritsReq[0].BaseData.Code);
            //    }
            //    else if (count == 3)
            //    {
            //        SetTypeSprite(Stones[0], (int)card.SpiritsReq[0].BaseData.Code);
            //        SetTypeSprite(Stones[3], (int)card.SpiritsReq[1].BaseData.Code);
            //        SetTypeSprite(Stones[4], (int)card.SpiritsReq[2].BaseData.Code);
            //    }
            //}
            //else
            //{
            //    SetTypeSprite(Stones[1], (int)card.SpiritsReq[0].BaseData.Code);
            //    SetTypeSprite(Stones[2], (int)card.SpiritsReq[1].BaseData.Code);
            //}
            
        }

        public void SetBlank()
        {
            HideAll();
        }

        public void SetLargeStone(Card card)
        {
            HideAll();
            //Stones[0].SetLargeStone((int)card.SpiritsReq[0].BaseData.Code);
            SetTypeSprite(Stones[0], (int)card.SpiritsReq[0].BaseData.Code);
           
        }

        protected void SetTypeSprite(TypeStone stone, int elementCode)
        {
            if (UseLargeImages)
            {
                stone.SetLargeStone(elementCode);
            }
            else
            {
                stone.SetStone(elementCode);
            }
        }



    }
}

