using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardsUI.Filtering
{
    public class CardCostFilterGroup : FilterGroup
    {
        private FilterToggle Cost0
        {
            get
            {
                return FilterWithValue("0");
            }

        }
        private FilterToggle Cost1
        {
            get
            {
                return FilterWithValue("1");
            }

        }
        private FilterToggle Cost2
        {
            get
            {
                return FilterWithValue("2");
            }

        }
        private FilterToggle Cost3
        {
            get
            {
                return FilterWithValue("3");
            }

        }
        public override string GetQuery()
        {
            string query = "";
            bool isEmpty = true;

            if (CheckedCount > 1) { query = "("; }

            if (Cost3.IsChecked)
            {
                query += "(cost3 is not null)";
                isEmpty = false;
            }
            if (Cost2.IsChecked)
            {
                if (!isEmpty) { query += " or "; }
                query += "(cost2 is not null and cost3 is null)";
                isEmpty = false;
            }
            if (Cost1.IsChecked)
            {
                if (!isEmpty) { query += " or "; }
                query += "(cost1 is not null and cost2 is null and cost3 is null and cardClass <> 0)";
                isEmpty = false;
            }
            if (Cost0.IsChecked)
            {
                if (!isEmpty) { query += " or "; }
                query += "(cardClass = 0)";
            }

            if (CheckedCount > 1) { query += ")"; }

            return query;
        }
    }
}
