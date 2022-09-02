using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardsUI.Filtering
{
    public class QueryFilterToggle : FilterToggle
    {
        public string colName;
        public string valString;


        public string QueryString
        {
            get
            {
                string query = "";
                query = $"{colName} {valString}";
                return query;
            }
        }
    }
}
