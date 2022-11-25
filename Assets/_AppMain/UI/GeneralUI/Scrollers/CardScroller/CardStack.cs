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
        private FormattedText txtQuantity;
        [SerializeField]
        private GameObject qtyObject;
        [SerializeField]
        private Button upQtyButton;
        [SerializeField]
        private Button downQtyButton;


        private List<Renderer> _buttonRenderers = null;
        protected List<Renderer> buttonRenderers
        {
            get
            {
                if (_buttonRenderers == null)
                {
                    _buttonRenderers = new List<Renderer>();
                    Renderer[] rends = qtyObject.GetComponentsInChildren<Renderer>();
                    _buttonRenderers.AddRange(rends);
                }
                return _buttonRenderers;
            }
        }
        #endregion
        #region Events & Button Listeners
        public static event Action<CardView, int> OnQuantityChanged;
        private void DoChangeQuantity()
        {
            OnQuantityChanged?.Invoke(this, quantity);
        }

        public void ToggleUpButton(bool isInteractable)
        {
            upQtyButton.interactable = isInteractable;
        }
        public void ToggleDownButton(bool isInteractable)
        {
            downQtyButton.interactable = isInteractable;
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
                    DoChangeQuantity();
                }
            }
        }

        public void ChangeQuantity(int changeVal)
        {
            quantity = _quantity + changeVal;
        }

        private void UpdateQuantity(int newQty)
        {
            if (newQty > 0)
            {
                txtQuantity.SetText(newQty.ToString());
            }
            else
            {
                txtQuantity.SetText("0");
                Hide();
            }

            
            
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
        public override void SetSortingLayer(string sortLayer)
        {
            base.SetSortingLayer(sortLayer);
            for (int i = 0; i < buttonRenderers.Count; i++)
            {
                buttonRenderers[i].sortingLayerName = sortLayer;
            }

        }

        public override void SetSortingOrder(int order)
        {

            base.SetSortingOrder(order);
            for (int i = 0; i < buttonRenderers.Count; i++)
            {
                buttonRenderers[i].sortingOrder = CurrentConfig.raritySp.sortingOrder + order;
            }
        }
        #endregion

      
    }
}

