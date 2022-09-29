using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TouchControls
{
    public enum StackOrder
    {
        ByZ = 0,
        ByAge = 1,
    }
    public class TouchGroup : MonoBehaviour
    {
       
        #region Properties
        public string groupName { get; set; }
        [SerializeField]
        private List<TouchObject> _buttons = null;
        public List<TouchObject> Buttons
        {
            get
            {
                _buttons ??= new List<TouchObject>();
                return _buttons;
            }
        }

        protected bool isDirty = true;

        #region Customization
        /// <summary>
        /// If checked, only 1 button in the group can be available for click, even if multiple are clicked at once
        /// </summary>
        public bool SingleClick;
        [SerializeField]
        private StackOrder _OrderBy;
        /// <summary>
        /// Determines the order the buttons are clicked if multiple are clicked at once. Or, if Single Click, determine which button gets click priority
        /// </summary>
        public StackOrder OrderBy
        {
            get
            {
                return _OrderBy;
            }
            set
            {
                _OrderBy = value;
                isDirty = true;
            }
        }

        //make it so if you click on a staggered stack of buttons when ordered by z, get the highest Z button in the uncovered stack
        private List<TouchObject> _ClickOrder = null;
        protected List<TouchObject> ClickOrder
        {
            get
            {
                if(_ClickOrder == null)
                {
                    isDirty = true;
                    _ClickOrder = new List<TouchObject>();
                }
                if (isDirty)
                {
                    Sort();
                }
                return _ClickOrder;
            }
        }
       
        protected void Sort()
        {
            _ClickOrder.Clear();
            _ClickOrder.AddRange(Buttons);
            _ClickOrder.Sort(Compare);
            isDirty = false;
        }
        
        #endregion

        #region Events
        protected void OnClick(TouchObject button)
        {
            List<TouchObject> clicked = GetClickedButtons();
            clicked.Sort(Compare);
            if (SingleClick) { clicked[0].DoClick(); }
        }
        protected void OnHold(TouchObject button)
        {
            List<TouchObject> clicked = GetClickedButtons();
            clicked.Sort(Compare);
            if (SingleClick) { clicked[0].DoHold(); }
        }
        #endregion

        protected bool Validate(TouchObject button)
        {
            Debug.Log(ClickOrder.IndexOf(button));
            List<TouchObject> clicked = GetClickedButtons();
            clicked.Sort(Compare);
            if (SingleClick) { return ClickOrder.IndexOf(button) == 0; }

            return true;
        }

        public int Compare(TouchObject x, TouchObject y)
        {
            float a = 0f;
            float b = 0f;
            if (OrderBy == StackOrder.ByZ)
            {
                a = x.gameObject.transform.localPosition.z;
                b = y.gameObject.transform.localPosition.z;
                if (a > b) { return -1; } else if (a < b) { return 1; }
            }
            if (OrderBy == StackOrder.ByAge)
            {
                a = Buttons.IndexOf(x);
                b = Buttons.IndexOf(y);
                if (a < b) { return -1; } else if (a > b) { return 1; }
            }
            
            return 0;

        }
        #endregion

        #region Button Management
        public void Add(TouchObject tb)
        {
            if (!Buttons.Contains(tb))
            {
                Buttons.Add(tb);
                tb.AddToGroup(this);
                tb.OnThisClicked += OnClick;
                tb.OnThisHeld += OnHold;
                isDirty = true;
            }
        }
        public void Remove(TouchObject tb)
        {
            if (Buttons.Contains(tb))
            {
                Buttons.Remove(tb);
                tb.OnThisClicked -= OnClick;
                tb.OnThisHeld -= OnHold;
                isDirty = true;
            }
        }
        #endregion

        private void Awake()
        {
           
            groupName = UniqueString.Create("tbg", 6);
            ObjectPool.Register(gameObject, groupName);
        }


        protected List<TouchObject> GetClickedButtons()
        {
            List<TouchObject> buttons = new List<TouchObject>();    

            for (int i = 0; i < Buttons.Count; i++)
            {
                TouchObject obj = Buttons[i];
                if (UIHelpers.IsPointerOverMe(obj.GetComponent<RectTransform>()))
                {
                    buttons.Add(obj);
                }

            }
            return buttons;
        }
    }
}
