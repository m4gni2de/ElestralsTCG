using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace TouchControls
{
    public enum StackOrder
    {
        ByZ = 0,
        ByAge = 1,
        BySortLayer = 2,
    }
    public class TouchGroup : ValidationObject
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

        private bool _blockTouch = false;
        public bool BlockTouch
        {
            get
            {
                return _blockTouch;
            }
            set
            {
                _blockTouch = value;
            }
        }
        #endregion

        #region Events
        public override bool Validate()
        {
            ErrorList.Clear();

            if (BlockTouch) { return false; }
            return ErrorList.Count == 0;
        }

        private List<TouchObject> _clickedObjects = null;
        protected List<TouchObject> ClickedObjects
        {
            get
            {
                _clickedObjects ??= new List<TouchObject>();
                return _clickedObjects;
            }
        }
        protected void OnClick(TouchObject button)
        {
            if (Buttons.Count == 1) { button.DoClick(); Refresh();  return; }
            if (WaitForClick == null) { ClickedObjects.Clear();  WaitForClick = StartCoroutine(AwaitingClicks()); }
            ClickedObjects.Add(button);

            //_clickCount += 1;
            
            //if (Validate())
            //{
            //    List<TouchObject> clicked = GetClickedButtons();
            //    expectedCount = clicked.Count;                
            //    if (clicked.Count > 0)
            //    {
            //        clicked.Sort(Compare);
            //        if (SingleClick)
            //        {
            //            clicked[0].DoClick();
            //        }
            //    }
            //}

            //if (_clickCount >= expectedCount)
            //{
            //    _clickCount = 0;
            //    expectedCount = 0;
            //}

           
           
        }

        private IEnumerator AwaitingClicks()
        {
            int prevCount = ClickedObjects.Count;
            int newCount = prevCount;
            do
            {
                prevCount = ClickedObjects.Count;
                yield return new WaitForEndOfFrame();
                newCount = ClickedObjects.Count;

            } while (prevCount != newCount);

            DoClick();
        }

        private void DoClick()
        {
            if (ClickedObjects.Count > 0)
            {
                ClickedObjects.Sort(Compare);
                if (SingleClick)
                {
                    ClickedObjects[0].DoClick();
                }
            }
            Refresh();
        }
        protected void OnHold(TouchObject button)
        {
            if (!Validate()) { return; }
            List<TouchObject> clicked = GetClickedButtons();
           
            if (clicked.Count > 0)
            {
                clicked.Sort(Compare);
                if (SingleClick) { clicked[0].DoHold(); }
            }
        }
        #endregion


        #region Click Processing
        private Coroutine WaitForClick = null;
        private void Refresh()
        {
            ClickedObjects.Clear();
            WaitForClick = null;
        }
      
        #endregion


        protected bool Validate(TouchObject button)
        {
           
            List<TouchObject> clicked = GetClickedButtons();
            
            if (clicked.Count > 0)
            {
                clicked.Sort(Compare);
                if (SingleClick) { return ClickOrder.IndexOf(button) == 0; }
            }
            

            return true;
        }

        public int Compare(TouchObject x, TouchObject y)
        {
            float a = 0f;
            float b = 0f;

            switch (OrderBy)
            {
                case StackOrder.ByZ:
                    a = x.gameObject.transform.localPosition.z;
                    b = y.gameObject.transform.localPosition.z;
                    if (a > b) { return -1; } else if (a < b) { return 1; } return 0;
                case StackOrder.ByAge:
                    a = Buttons.IndexOf(x);
                    b = Buttons.IndexOf(y);
                    if (a < b) { return -1; } else if (a > b) { return 1; } return 0;
                case StackOrder.BySortLayer:
                    a = x.GetSortValue();
                    b = y.GetSortValue();
                    if (a > b) { return -1; } else if (a < b) { return 1; } return 0;
                default:
                    return 0;
            }
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
                if (obj.IsPointerOverMe())
                {
                    buttons.Add(obj);
                }
                //if (UIHelpers.IsPointerOverMe(obj.GetComponent<RectTransform>()))
                //{
                //    buttons.Add(obj);
                //}

            }
            return buttons;
        }


       
    }
}

