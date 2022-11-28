using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace CardsUI.Filtering
{
    public class CardStack : CardView
    {
        #region Properties
        [SerializeField]
        private MagicTextBox txtQuantity;
        public MagicTextBox QuantityText { get { return txtQuantity; } }
        [SerializeField]
        private GameObject qtyObject;
        #endregion

        #region Events & Button Listeners
        public static event Action<CardView, int, bool> OnQuantityChanged;
        private void DoChangeQuantity(bool addHistory)
        {
            OnQuantityChanged?.Invoke(this, quantity, addHistory);
        }

        #endregion


        #region Quantity
        private int _quantity;
        public int quantity
        {
            get
            {
                return _quantity;
            }
            set
            {
                if (value != _quantity)
                {
                    _quantity = value;
                    UpdateQuantity(value);
                    
                }
            }
        }

        public void SetQuantity(float changeVal)
        {
            SetQuantity((int)changeVal, true);
        }
        public void SetQuantity(int newVal, bool addHistory)
        {
            quantity = newVal;
            DoChangeQuantity(addHistory);
        }
        public void ChangeQuantity(int changeVal, bool addHistory)
        {
            quantity = _quantity + changeVal;
            DoChangeQuantity(addHistory);
        }

        private void UpdateQuantity(int newQty)
        {
            txtQuantity.SetText(newQty.ToString());
            //if (newQty > 0)
            //{
            //    txtQuantity.SetText(newQty.ToString());
            //}
            //else
            //{
            //    txtQuantity.SetText("0");
            //    Hide();
            //}

            
            
        }


        #endregion

        #region Overrides
        public override void Clear()
        {
            base.Clear();
            _quantity = 0;
        }
        protected override float GetRenderHeight()
        {
            return GetComponent<RectTransform>().rect.height + qtyObject.GetComponent<RectTransform>().rect.height;
        }
        //public override void SetSortingLayer(string sortLayer)
        //{
        //    base.SetSortingLayer(sortLayer);
        //    for (int i = 0; i < buttonRenderers.Count; i++)
        //    {
        //        buttonRenderers[i].sortingLayerName = sortLayer;
        //    }

        //}

        //public override void SetSortingOrder(int order)
        //{

        //    base.SetSortingOrder(order);
        //    for (int i = 0; i < buttonRenderers.Count; i++)
        //    {
        //        buttonRenderers[i].sortingOrder = CurrentConfig.raritySp.sortingOrder + order;
        //    }
        //}
        #endregion

      
    }
}

