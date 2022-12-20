using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Defective.JSON;

[System.Serializable]
public class Archive<T> where T : iArchive
{
    [System.Serializable]
    public class Entry
    {

        public T Item { get; set; }
        public string Title { get; set; }
        public int Index { get; set; }
        public DateTime When { get; set; }
        [SerializeField]
        private List<string> _Tags = null;
        public List<string> Tags
        {
            get
            {
                _Tags ??= new List<string>();
                return _Tags;
            }
        }

        public Entry(T obj, string title, int index, params string[] tags)
        {
            Item = obj;
            Title = title;
            Index = index;
            if (tags != null)
            {
                foreach (string tag in tags)
                {
                    Tags.Add(tag);
                }
            }
            When = DateTime.Now;
        }
        public static Entry Basic(T obj)
        {
            return new Entry(obj, "", -1, null);
        }

    }


    #region Properties
    public int Count => Entries.Count;

    private bool _Lock = false;
    public bool IsLocked => _Lock;
    private bool CanAddEntry(Entry entry)
    {
        if (IsLocked) { return App.DisplayError($"The Archive cannot add new Entries while Locked."); }
        if (Entries.Contains(entry)) return false;
        return true;
    }
    private DateTime _Creation = DateTime.MinValue;
    public DateTime Creation => _Creation;

    public Entry LastEntry
    {
        get
        {
            Entry entry = Entries[Entries.Count - 1];
            if (entry != null)
            {
                return entry;
            }

            return null;
        }
    }
    public T LastItem
    {
        get
        {
            Entry entry = Entries[Entries.Count - 1];
            if (entry != null)
            {
                return entry.Item;
            }

            return default(T);
        }
    }

    public DateTime MostRecent
    {
        get
        {
            if (LastEntry != null)
            {
                return LastEntry.When;
            }
            return DateTime.MinValue;
        }
    }
    #endregion

    #region Entries Collection
    public Entry this[int index]
    {
        get
        {
            return Entries[index];
        }
        set
        {
            Entries[index] = value;
        }
    }

    protected List<Entry> _Entries { get; set; }
    protected List<Entry> Entries
    {
        get
        {
            if (_Entries == null) { _Entries = new List<Entry>(); }
            return _Entries;
        }
    }
    #endregion
    #region Initialization
    public Archive()
    {
        _Creation = DateTime.Now;
        _Entries = new List<Entry>();
    }
    public Archive(T obj) : this()
    {
        Add(obj);
    }
    public Archive(IEnumerable<T> objs) : this()
    {
        foreach (T obj in objs)
        {
            Add(obj);
        }
    }
    public Archive(IEnumerable<Entry> entries)
    {
        foreach (Entry entry in entries)
        {
            AddEntry(entry);
        }
    }
    #endregion

    #region Collection Management

    public void Lock(bool isLock = true)
    {
        _Lock = isLock;
    }

    public void Add(T obj)
    {
        Add(obj, "", null);
    }
    public void Add(T obj, string title)
    {
        Add(obj, title, null);
    }
    public void Add(T obj, string title, params string[] tags)
    {
        
        int index = Count;
        Entry entry = new Entry(obj, title, index, tags);
        AddEntry(entry);
    }
    public void AddEntry(Entry entry)
    {
        if (CanAddEntry(entry))
        {
            Add(entry);
        }
    }

    private void Add(Entry entry)
    {
        Entries.Add(entry);
        App.Log($"Entry Added: {entry.Title}");
    }

    public void Remove(Entry entry)
    {
        if (IsLocked) { App.DisplayError($"The Archive cannot add new Entries while Locked."); return; }
        if (Entries.Contains(entry)) { Entries.Remove(entry); }
    }
    public void Remove(T obj)
    {
        for (int i = 0; i < Entries.Count; i++)
        {
            object item = (object)Entries[i].Item;
            if (item == (object)obj)
            {
                Entries.Remove(Entries[i]); 
            }
        }
    }
    public void RemoveLatest()
    {
        Remove(LastEntry);
    }

    public void Clear()
    {
        Entries.Clear();
        _Creation = DateTime.Now;
        _Entries = new List<Entry>();
    }
    #endregion

    public string Print(bool prettyPrint = false)
    {
        JSONObject obj = new JSONObject();
        for (int i = 0; i < Entries.Count; i++)
        {
            Entry e = Entries[i];
            obj.AddField("Item", e.Item.Print);
        }
        return obj.Print(prettyPrint); 
    }


}
