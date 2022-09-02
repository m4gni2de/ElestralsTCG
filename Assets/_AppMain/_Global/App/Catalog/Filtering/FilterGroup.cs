using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CardsUI.Filtering
{
    public class FilterGroup : MonoBehaviour
    {
        public enum Joiner
        {
            And = 0,
            Or = 1
        };
        public enum FilterValueType
        {
            String = 0,
            Integer = 1,
        }
        public enum FilterColumnType
        {
            Generic = 0,
            IntGeneric = 1,
            Element = 2,
            CardType = 3,
            CardCost = 4,
        }

        public static readonly string ValChar = "{val}";

        public Joiner joinWord;
        public FilterValueType valueType;
        public FilterColumnType colType;
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

        public FilterToggle FilterWithValue(string val)
        {
            for (int i = 0; i < toggles.Length; i++)
            {
                if (val.ToLower() == toggles[i].valName.ToLower())
                {
                    return toggles[i];
                }
            }
            return null;
        }

        private Dictionary<string, bool> _LastFilters = null;
        public Dictionary<string, bool> LastFilters { get { _LastFilters ??= new Dictionary<string, bool>(); return _LastFilters; } }

        /// <summary>
        /// If there is only 1 Toggle Checked and it is set to be unchecked, Check everything in the Group instead of allowing 0 items to be checked.
        /// </summary>
        public bool NoneIsAll;

        public string ValueClause;

        public bool AllChecked { get { return CheckedCount == toggles.Length; } }
        protected string LastQuery;
        #endregion

        public virtual bool Validate()
        {
            return true;
        }
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
                FilterToggle t = toggles[i];

                if (t.IsChecked)
                {
                    list.Add(GetColumnValue(t.valName));
                }
            }
            return list;
        }
        public List<string> GetIntValues()
        {
            List<string> list = new List<string>();

            for (int i = 0; i < toggles.Length; i++)
            {
                FilterToggle t = toggles[i];

                if (t.IsChecked)
                {
                    list.Add(GetColumnValue(t.valName));
                }
            }
            return list;
        }
        #endregion

        #region Commands
        public void SelectAllCommand()
        {
            for (int i = 0; i < toggles.Length; i++)
            {
                toggles[i].SetChecked(true);
            }
        }
        #endregion

        #region Column Types
        protected string GetColumnValue(string val)
        {
            switch (colType)
            {
                case FilterColumnType.Generic:
                    return $"'{val}'";
                case FilterColumnType.IntGeneric:
                    return $"{val}";
                case FilterColumnType.Element:
                    int eleCode = Element.ElementInt(val);
                    return $"{eleCode}";
                case FilterColumnType.CardCost:
                    return CardLibrary.CostString(val);             
                default:
                    return $"'{val}'";
            }
        }

        #endregion

       
        protected IEnumerator AwaitValidate()
        {

            do
            {

            } while (true);
        }
        public virtual string GetQuery()
        {
            string where = "";
            if (AllChecked) { return where; }

            List<string> vals = GetStringValues();
            if (valueType == FilterValueType.Integer) { vals = GetIntValues(); }

          
            int count = 0;

            for (int i = 0; i < CheckedCount; i++)
            {
                if (i == 0) { where += "("; }
                string valString = $" {vals[i]} ";
                where += ValueClause.Replace(ValChar, valString);
                count += 1;
                if (count == CheckedCount)
                {
                    where += ")";
                }
                else
                {
                    where += $" {joinWord.ToString()} ";
                }
                
            }

            return where;
        }

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

