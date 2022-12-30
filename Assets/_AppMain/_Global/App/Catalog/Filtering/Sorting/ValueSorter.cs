using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GlobalUtilities;
using UnityEngine;
using static Sorter;
using static UnityEditor.Progress;

public class ValueSorter : iSorter
{

  
    public class ValueSortCriteria
    {
        public string property { get; set; }
        public SortDirection direction { get; set; }
        public ValueSortCriteria(string prop, SortDirection dir)
        {
            this.property = prop;
            this.direction = dir;
        }
    }

    #region Interface
    public List<T> SortItems<T>(List<T> items)
    {
        List<T> sortedItems = SortAllItems(items);
        return sortedItems;

    }
    #endregion

    #region Properties

    private List<ValueSortCriteria> _sorters = null;
    public List<ValueSortCriteria> Sorters
    {
        get
        {
            _sorters ??= new List<ValueSortCriteria>();
            return _sorters;
        }
    }
    #endregion

    #region Initialzation
    public ValueSorter(string prop, SortDirection dir)
    {
        Sorters.Clear();

        Sorters.Add(new ValueSortCriteria(prop, dir));
    }
    #endregion

    #region Sorting

    public string GetPropName(int index)
    {
        return Sorters[index].property;
    }
    private object GetValue(object obj, int propIndex)
    {
        string propName = Sorters[propIndex].property;
        return obj.GetPropertyOrFieldValue(propName);
    }

    private List<T> SortAllItems<T>(List<T> toSort)
    {
        List<T> values = new List<T>();
        values.AddRange(toSort);

        if (Sorters.Count == 0) { return values; }
        int sortCount = Sorters.Count;

        SortDirection direction = Sorters[0].direction;

        switch (sortCount)
        {
            case 1:
                return values.OrderBy(x => GetValue(x, 0), Sorters[0].direction).ToList();
            case 2:
                return values.OrderBy(x => GetValue(x, 0), Sorters[0].direction).ThenBy(x => GetValue(x, 1), Sorters[1].direction).ToList();
            case 3:
                return values.OrderBy(x => GetValue(x, 0), Sorters[0].direction).
                    ThenBy(x => GetValue(x, 1), Sorters[1].direction).
                        ThenBy(x => GetValue(x, 2), Sorters[2].direction).ToList();

            case 4:
                return values.OrderBy(x => GetValue(x, 0), Sorters[0].direction).
                    ThenBy(x => GetValue(x, 1), Sorters[1].direction).
                        ThenBy(x => GetValue(x, 2), Sorters[2].direction).
                            ThenBy(x => GetValue(x, 3), Sorters[3].direction).ToList();
        }
        return values;
    }   
    #endregion


}
