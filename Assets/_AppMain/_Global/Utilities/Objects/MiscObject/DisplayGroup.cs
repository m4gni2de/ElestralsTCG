using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayGroup : MonoBehaviour
{
    private CanvasGroup _group = null;
    public CanvasGroup Group
    {
        get
        {
            _group ??= GetComponent<CanvasGroup>();
            return _group;
        }
    }

    private List<GameObject> _AddedObjects = null;
    public List<GameObject> AddedObjects
    {
        get
        {
            _AddedObjects ??= new List<GameObject>();
            return _AddedObjects;
        }
    }

    public void AddObject(GameObject obj)
    {
        obj.transform.SetParent(transform);
        AddedObjects.Add(obj);
    }
}
