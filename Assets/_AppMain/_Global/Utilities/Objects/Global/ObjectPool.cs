using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool 
{
    public class PooledObject
    {
        public string key { get; set; }
        public int Hash { get; set; }
        public GameObject source { get; set; }
        public PooledObject(GameObject obj, string objKey)
        {
            source = obj;
            Hash = obj.GetHashCode();
            key = objKey;
        }
    }
    private static List<PooledObject> _items = null;
    public static List<PooledObject> Items
    {
        get
        {
            _items ??= new List<PooledObject>();
            return _items;
        }
    }

    private static bool ContainsGameObject(GameObject obj)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].Hash == obj.GetHashCode()) { return true; }
        }
        return false;
    }
    private static PooledObject FindByGameObject(GameObject obj)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].Hash == obj.GetHashCode()) { return Items[i]; }
        }
        return null;
    }
    private static PooledObject GetByKey(string key)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].key == key) { return Items[i]; }
        }
        return null;
    }
    public static T FindByKey<T>(string key) where T: Component
    {
        PooledObject p = GetByKey(key);
        if (p != null)
        {
            if (p.source.GetComponentInChildren(typeof(T), true) != null)
            {
                return p.source.GetComponent<T>();
            }
        }
        return null;
    }


    public static void Register(GameObject obj, string key)
    {
        if (!ContainsGameObject(obj))
        {
            PooledObject reg = new PooledObject(obj, key);
            Items.Add(reg);
        }
    }
    public static void Remove(GameObject obj)
    {
        PooledObject po = FindByGameObject(obj);
        if (po != null)
        {
            Items.Remove(po);
        }
    }

}
