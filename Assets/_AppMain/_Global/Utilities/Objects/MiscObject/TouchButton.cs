using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TouchButton
{
    #region Static Properties
    public static List<TouchButton> _buttons = null;
    public static List<TouchButton> buttons { get { _buttons ??= new List<TouchButton>(); return _buttons; } }
    #endregion

    #region Properties
    public GameObject Source { get; set; }
    public Delegate OnClick { get; set; }
    #endregion

    #region Initialization
    protected TouchButton(GameObject obj, Delegate de)
    {
        Source = obj;
        OnClick = de;
    }
    #endregion

    public void Do()
    {
        OnClick.DynamicInvoke();
    }

    public static bool CheckTouch(TouchButton tb)
    {
        if (tb.Source.GetComponent<RectTransform>().IsPointerOverMe())
        {
            return true;
        }
        return false;
    }
    public static void CheckTouch()
    {

        RemoveNullObjects();
        for (int i = 0; i < buttons.Count; i++)
        {
            RectTransform r = buttons[i].Source.GetComponent<RectTransform>();
            if (r.IsPointerOverMe())
            {
                Debug.Log(buttons[i].Source.gameObject.name);
            }
        }
    }
    protected static void RemoveNullObjects()
    {
        List<TouchButton> goners = new List<TouchButton>();
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i].Source == null)
            {
                goners.Add(buttons[i]);
            }
        }

        for (int i = 0; i < goners.Count; i++)
        {
            buttons.Remove(goners[i]);
        }
    }

    protected static bool HasButton(GameObject obj)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i].Source.GetInstanceID() == obj.GetInstanceID()) { return true; }
        }
        return false;
    }

    #region Registering Actions
    public static void Register(GameObject obj, Action ac)
    {
        if (obj.GetComponent<RectTransform>() == null) { return; }

        
        if (!HasButton(obj))
        {
            if (obj != null)
            {
                TouchButton t = new TouchButton(obj, ac);
                buttons.Add(t);
            }
            
        }
    }

    public static void Register<T>(GameObject obj, Action<T> ac)
    {
        if (obj.GetComponent<RectTransform>() == null) { return; }


        if (!HasButton(obj))
        {
            if (obj != null)
            {
                TouchButton t = new TouchButton(obj, ac);
                buttons.Add(t);
            }

        }
    }
    #endregion

    public static TouchButton Create(GameObject obj, Action ac)
    {
        if (obj.GetComponent<RectTransform>() == null) { return null; }

        return new TouchButton(obj, ac);
    }
    
}



public class TouchButton<T> : TouchButton
{
    public new T Source { get { return default(T); } }

    TouchButton(GameObject obj, Delegate de) : base(obj, de)
    {

    }

    public void Do(T obj)
    {
        OnClick.DynamicInvoke(obj);
    }

    public static TouchButton<T> Create(GameObject obj, Action<T> ac)
    {
        if (obj.GetComponent<RectTransform>() == null) { return null; }

        return new TouchButton<T>(obj, ac);
    }
}




