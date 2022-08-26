using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CardsUI.Filtering
{
    public class FilterGroup : MonoBehaviour
    {

        #region Properties
        //private ToggleGroup toggleGroup;
        private FilterToggle[] _toggles = null;
        public FilterToggle[] toggles
        {
            get
            {
                if (_toggles == null)
                {
                    _toggles = GetComponentsInChildren<FilterToggle>();
                }
                return _toggles;
            }
        }

        public int CheckedCount
        {
            get
            {
                int count = 0;
                for (int i = 0; i < toggles.Length; i++)
                {
                    if (toggles[i].IsChecked) { count += 1; }
                }
                return count;
            }
        }

        private Dictionary<string, bool> _LastFilters = null;
        public Dictionary<string, bool> LastFilters { get { _LastFilters ??= new Dictionary<string, bool>(); return _LastFilters; } }

        /// <summary>
        /// If there is only 1 Toggle Checked and it is set to be unchecked, Check everything in the Group instead of allowing 0 items to be checked.
        /// </summary>
        public bool NoneIsAll;
        #endregion

        #region Opening/Closing
        private void Awake()
        {
            Refresh();
        }

        public void Refresh()
        {
            for (int i = 0; i < toggles.Length; i++)
            {
                toggles[i].Refresh();
                if (NoneIsAll)
                {
                    toggles[i].toggle.onValueChanged.AddListener(delegate {
                        OnNoneSelectAll();
                    });
                }

            }
        }

        
        public void LoadFilters()
        {
            if (LastFilters.Count > 0)
            {
                for (int i = 0; i < toggles.Length; i++)
                {
                   if (LastFilters.ContainsKey(toggles[i].valName))
                    {
                        toggles[i].SetChecked(LastFilters[toggles[i].valName]);
                    }
                }
            }
        }
        public void SaveFilters()
        {
            LastFilters.Clear();
            for (int i = 0; i < toggles.Length; i++)
            {
                LastFilters.Add(toggles[i].valName, toggles[i].IsChecked);
            }
        }
        #endregion


        #region Query
        public List<string> GetStringValues()
        {
            List<string> list = new List<string>();

            for (int i = 0; i < toggles.Length; i++)
            {
                FilterToggle t = toggles[0];

                if (t.IsChecked)
                {
                    list.Add(t.valName);
                }
            }
            return list;
        }
        public List<int> GetIntValues()
        {
            List<int> list = new List<int>();

            for (int i = 0; i < toggles.Length; i++)
            {
                FilterToggle t = toggles[0];

                if (t.IsChecked)
                {
                    list.Add(int.Parse(t.valName));
                }
            }
            return list;
        }
        #endregion

        protected void CheckAll()
        {
            for (int i = 0; i < toggles.Length; i++)
            {
                toggles[i].SetChecked(true);
            }
        }
        
        #region CheckListeners
        public void OnNoneSelectAll()
        {
            if (CheckedCount == 0)
            {
                CheckAll();
            }
        }
        #endregion
    }

}

