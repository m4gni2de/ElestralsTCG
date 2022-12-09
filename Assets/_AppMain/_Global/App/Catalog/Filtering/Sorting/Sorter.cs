using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using GlobalUtilities;
using UnityEngine;

public class Sorter
{

    #region SortValueMap
    public class SortValueMap
    {
        private object source { get; set; }
        private Dictionary<SortBy, object> _values = null;
        private Dictionary<SortBy, object> Values
        {
            get
            {
                _values ??= new Dictionary<SortBy, object>();
                return _values;
            }
        }

        public SortValueMap(object _source, Dictionary<SortBy, object> vals)
        {
            this.source = _source;
            this._values = vals;
        }

        public object GetValue(SortBy sortBy)
        {
            if (Values.ContainsKey(sortBy))
            {
                return Values[sortBy];
            }
            return null;
        }

        public int Compare<T>(T x, T y)
        {
            ComparedTo compare = x.CompareTo(y);
            return (int)compare;
        }

        public T SourceAs<T>()
        {
            return (T)source;
        }

    }
    #endregion


    #region Properties

    private List<DataSorter> _sorters = null;
    public List<DataSorter> Sorters { get { _sorters ??= new List<DataSorter>(); return _sorters; } }
    private bool HasSorter(SortBy by)
    {
        for (int i = 0; i < Sorters.Count; i++)
        {
            if (Sorters[i].sortBy == by) { return true; }
        }
        return false;
    }


    private List<SortValueMap> _items = null;
    protected List<SortValueMap> Items
    {
        get
        {
            _items ??= new List<SortValueMap>();
            return _items;
        }
    }
    #endregion

    private static DataSorter[] DefaultSorters
    {
        get
        {
            DataSorter[] sort = new DataSorter[2];
            sort[0] = new DataSorter(SortBy.CardType, SortDirection.ASC);
            sort[1] = new DataSorter(SortBy.Cost, SortDirection.DESC);
            return sort;
        }
    }
    #region Initialization
    public Sorter(params DataSorter[] sortOrder)
    {
        if (sortOrder == null || sortOrder.Length == 0) { App.LogWarning("No DataSorter parameters given, using defaults."); sortOrder = DefaultSorters; }

        for (int i = 0; i < sortOrder.Length; i++)
        {
            AddSorter(sortOrder[i], i);
        }
    }

    public void AddSorter(DataSorter sort, int index = -1)
    {
        if (!HasSorter(sort.sortBy))
        {
            Insert(sort, index);
        }
    }

    private void Insert(DataSorter sort, int index)
    {
        if (Sorters.Count == 0) { Sorters.Add(sort); return; }
        if (index <= 0) { Sorters.Insert(0, sort); return; }
        //if (index >= Sorters.Count)
        //{
        //    Sorters.Insert(Sorters.Count - 1, sort);
        //    return;
        //}

        Sorters.Insert(index, sort);

    }

    #endregion


    #region Do Sorting
    public List<T> SortItems<T>(List<T> items)
    {
        Items.Clear();
        List<T> sorted = new List<T>();

        foreach (T item in items)
        {
            Dictionary<SortBy, object> itemVals = GetSortMappings(item);
            SortValueMap map = new SortValueMap(item, itemVals);
            Items.Add(map);
        }

        List<SortValueMap> sortedMaps = SortAllItems();

        for (int i = 0; i < sortedMaps.Count; i++)
        {
            sorted.Add(sortedMaps[i].SourceAs<T>());
        }


        return sorted;
    }


    private Dictionary<SortBy, object> GetSortMappings<T>(T obj)
    {
        Dictionary<SortBy, object> values = new Dictionary<SortBy, object>();
        //var sortObject =obj .GetType().GetCustomAttribute<SortableObjectAttribute>(true);
        //if (sortObject == null) { return values; }


        PropertyInfo[] props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Default | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        foreach (var prop in props)
        {
            foreach (var att in prop.GetCustomAttributes(typeof(SortableProxyAttribute), true))
            {
                if (att is SortableProxyAttribute)
                {
                    SortableProxyAttribute p = att as SortableProxyAttribute;
                    Dictionary<SortBy, object> proxyDict = p.GetProxyValues(prop.GetValue(obj));

                    foreach (var item in proxyDict)
                    {
                        if (!values.ContainsKey(item.Key))
                        {
                            values.Add(item.Key, item.Value);
                        }
                    }
                    break;
                }
            }

            foreach (var att in prop.GetCustomAttributes(typeof(SortableValueAttribute), true))
            {
                if (att is SortableValueAttribute)
                {
                    SortableValueAttribute val = att as SortableValueAttribute;
                    if (!values.ContainsKey(val.Sorter))
                    {
                        values.Add(val.Sorter, prop.GetValue(obj));
                    }

                }
            }
        }

        return values;

    }

    private List<SortValueMap> SortAllItems()
    {
        List<SortValueMap> values = new List<SortValueMap>();
        values.AddRange(Items);

        if (Sorters.Count == 0) { return values; }
        int sortCount = Sorters.Count;

        SortDirection direction = Sorters[0].sortDirection;

        switch (sortCount)
        {
            case 1:
                return values.OrderBy(x => x.GetValue(Sorters[0].sortBy), Sorters[0].sortDirection).ToList();
            case 2:
                return values.OrderBy(x => x.GetValue(Sorters[0].sortBy), Sorters[0].sortDirection).ThenBy(x => x.GetValue(Sorters[1].sortBy), Sorters[1].sortDirection).ToList();
            case 3:
                return values.OrderBy(x => x.GetValue(Sorters[0].sortBy), Sorters[0].sortDirection).
                    ThenBy(x => x.GetValue(Sorters[1].sortBy), Sorters[1].sortDirection).
                        ThenBy(x => x.GetValue(Sorters[2].sortBy), Sorters[2].sortDirection).ToList();
            case 4:
                return values.OrderBy(x => x.GetValue(Sorters[0].sortBy), Sorters[0].sortDirection).
                    ThenBy(x => x.GetValue(Sorters[1].sortBy), Sorters[1].sortDirection).
                        ThenBy(x => x.GetValue(Sorters[2].sortBy), Sorters[2].sortDirection).
                            ThenBy(x => x.GetValue(Sorters[3].sortBy), Sorters[3].sortDirection).ToList();
        }
        return values;
    }


    #endregion
}
