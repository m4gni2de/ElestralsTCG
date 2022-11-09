using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalUtilities
{
    #region Enums
    public enum SortDirection
    {
        ASC = 0,
        DESC = 1,
    }

    public enum SortBy
    {
        Name = 0,
        Cost = 1,
        Rarity = 2,
        CardSetName = 3,
        CardSetDate = 4,
        CardSetNumber = 5,

    }
    #endregion

    public class DataSorter
    {
        public SortBy sortBy { get; set; }
        public string sortColumn { get; set; }
       public SortDirection sortDirection { get; set; }

        public DataSorter(string column, SortDirection sortDirection)
        {
            this.sortColumn = column;
            this.sortDirection = sortDirection;
        }

        public void ChangeDirection(SortDirection dir)
        {
            sortDirection = dir;
        }
    }

    
}

