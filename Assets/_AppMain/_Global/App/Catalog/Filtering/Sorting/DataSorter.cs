using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        Age = -2,
        Property = -1,
        Name = 0,
        Cost = 1,
        Rarity = 2,
        CardSetName = 3,
        CardSetDate = 4,
        CardSetNumber = 5,
        CardType = 6,
        Attack = 7,
        Defense = 8,
        CardElement = 9,
        Quantity = 10,


    }
    #endregion

    public class DataSorter
    {
        public SortBy sortBy { get; set; }
       public SortDirection sortDirection { get; set; }

        public DataSorter(SortBy sorter, SortDirection sortDirection)
        {
            this.sortBy = sorter;
            this.sortDirection = sortDirection;
        }
       
        public void ChangeDirection(SortDirection dir)
        {
            sortDirection = dir;
        }



    }

    
}

