using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cards;

namespace CardsUI.Filtering
{
    public enum FilterValue
    {
        CardType = 0,
        CardElement = 1

    }

    public class FiltersMenu : MonoBehaviour
    {

        #region Functions
        public bool IsOpen { get { return gameObject.activeSelf; } }

        private FilterGroup[] Groups
        {
            get
            {
                return GetComponentsInChildren<FilterGroup>();
            }
        }

        #endregion

        #region Filter Groups
        
        public FilterGroup elementGroup;
        private List<int> m_elementFilters = null;
        protected List<int> _elementFilters
        {
            get
            {
                m_elementFilters ??= new List<int>();
                return m_elementFilters;
            }
        }

        public FilterGroup cardTypeGroup;
        public List<int> m_typeFilters = null;
        protected List<int> _cardTypeFilters { get { m_typeFilters ??= new List<int>(); return m_typeFilters; } }
        #endregion
        //public DataFilter dataFilter;


        private Dictionary<string, object> _LastFilters = null;
        protected Dictionary<string, object> LastFilters { get { _LastFilters ??= new Dictionary<string, object>(); return _LastFilters; } }

        private void Awake()
        {
            if (IsOpen)
            {
                Close();
            }
        }

        public void SetFilter<T>()
        {
            //dataFilter = new DataFilter<T>();
        }
        public void Toggle(bool open)
        {
            if (open) { Open(); } else { Close(); }
        }
        void Open()
        {
            gameObject.SetActive(true);
            for (int i = 0; i < Groups.Length; i++)
            {
                Groups[i].LoadFilters();
            }
        }
        void Close()
        {
            for (int i = 0; i < Groups.Length; i++)
            {
                Groups[i].SaveFilters();
            }
            gameObject.SetActive(false);
        }


        public string GenerateQuery()
        {
            string query = $"SELECT * FROM {CardService.CardTable} ";

            List<string> wheres = GetQueryWheres();

            if (wheres.Count == 0) { return query; }

            query += "where ";
            for (int i = 0; i < wheres.Count; i++)
            {
                query += wheres[i];
                if (i < wheres.Count - 1)
                {
                    query += " AND ";
                }
                else
                {
                    query += ";";
                }
            }

            return query;
        }
        protected List<string> GetQueryWheres()
        {
            List<string> wheres = new List<string>();

            int filterCount = _elementFilters.Count + _cardTypeFilters.Count;

            if (filterCount == 0) { return wheres; }
            string where = "";

            for (int i = 0; i < _elementFilters.Count; i++)
            {
                if (i == 0) { where += "("; }
                where += $"{_elementFilters[i]} in (cost1, cost2, cost3)";
                if (i < _elementFilters.Count - 1)
                {
                    { where += " or "; }
                }
                else
                {
                    where += ")";
                    wheres.Add(where);
                }
            }

            where = "";

            for (int i = 0; i < _cardTypeFilters.Count; i++)
            {
                if (i == 0) { where += "("; }
                where += $"cardClass = {_cardTypeFilters[i]}";
                if (i < _cardTypeFilters.Count - 1)
                {
                    where += " or ";
                }
                else
                {
                    where += ")";
                    wheres.Add(where);
                }
            }


            return wheres;
        }
        public void DoFilters()
        {
            _elementFilters.Clear();
            _cardTypeFilters.Clear();

            List<string> elementValues = elementGroup.GetStringValues();
            for (int i = 0; i < elementValues.Count; i++)
            {
                int eleCode = Element.ElementInt(elementValues[i]);
                _elementFilters.Add(eleCode);
            }

            List<int> cardTypeValues = cardTypeGroup.GetIntValues();
            for (int i = 0; i < cardTypeValues.Count; i++)
            {
                _cardTypeFilters.Add(cardTypeValues[i]);
            }


        }

       


    }
}
