using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using GameActions;


public class DisplayManager : MonoBehaviour
{

    public static DisplayManager Instance
    {
        get
        {
            if (AppManager.Instance && AppManager.Instance.displayManager != null)
            {
                return AppManager.Instance.displayManager;
            }
            return null;
        }
    }

    #region Properties
    [SerializeField]
    private Button BackButton;
    [SerializeField]
    private Canvas canvas;

    private bool isActive { get; set; }

    private static List<iGameAction> _actionOrder = null;
    public static List<iGameAction> actionOrder
    {
        get
        {
            _actionOrder ??= new List<iGameAction>();
            return _actionOrder;
        }
    }

    private static iGameAction HasAction(Delegate ac)
    {
        for (int i = 0; i < actionOrder.Count; i++)
        {
            iGameAction ga = actionOrder[i];
            if (ga.IsEqual(ac)) { return ga; }

        }
        return null;
    }


    #endregion

    #region Default Action
    private static iGameAction _default = null;
    public static iGameAction DefaultAction
    {
        get
        {
            if (_default == null)
            {
               SetDefault(AppManager.Instance.TryQuit);
            }
            return _default;
        }
    }
    private static void SetDefaultAction(Delegate ac, params object[] args)
    {
        _default = new GameAction(ac, args);
        
    }
    public static void SetDefault(MultiAction ac)
    {
        _default = ac;
    }
    public static void SetDefault(GameAction ac)
    {
        _default = ac;
    }

    public static void SetDefault(Action ac)
    {
        SetDefaultAction(ac);
    }
    public static void SetDefault<T>(Action<T> ac, T para0)
    {
        SetDefaultAction(ac, para0);
    }
    public static void SetDefault<T0, T1>(Action<T0, T1> ac, T0 para0, T1 para1)
    {
        SetDefaultAction(ac, para0, para1);
    }





    //    public static void SetDefault(UnityAction ac)
    //{
    //    _default = ac;
    //}
   
    private static iGameAction CurrentAction
    {
        get
        {
            if (actionOrder.Count > 0)
            {
                return actionOrder[0];
            }
            return DefaultAction;
        }
    }
    #endregion

    private void OnEnable()
    {
        canvas.overrideSorting = true;
        canvas.sortingLayerName = "AppManager";
        canvas.sortingOrder = 5;
    }
    public static void ClearButton()
    {
        actionOrder.Clear();
        _default = null;
    }
   
    #region Action Adding
    private static void AddNewAction(Delegate ac, params object[] args)
    {
        GameAction gameAction = new GameAction(ac, args);
        actionOrder.Add(gameAction);
        
    }

    public static void AddAction(Action ac)
    {
        iGameAction g = HasAction(ac);
        if (g == null)
        {
            AddNewAction(ac);
        }
    }
    public static void AddAction<T>(Action<T> ac, T para0)
    {
        iGameAction g = HasAction(ac);
        if (g == null)
        {
            AddNewAction(ac, para0);
        }
    }
    public static void AddAction<T0, T1>(Action<T0, T1> ac, T0 para0, T1 para1)
    {
        iGameAction g = HasAction(ac);
        if (g == null)
        {
            AddNewAction(ac, para0, para1);
        }
    }
    public static void AddAction<T0, T1, T2>(Action<T0, T1> ac, T0 para0, T1 para1, T2 para2)
    {
        iGameAction g = HasAction(ac);
        if (g == null)
        {
            AddNewAction(ac, para0, para1, para2);
        }
    }

    private static void RemoveGameAction(Delegate ac)
    {
        iGameAction g = new GameAction(ac);
        actionOrder.Remove(g);

    }

    public static void RemoveAction(Action ac)
    {
        iGameAction g = HasAction(ac);
        if (g != null)
        {
            RemoveGameAction(ac);
        }
    }

    #endregion

    #region Multi Action Adding
    public static void AddMultiAction(MultiAction ac)
    {
        actionOrder.Add(ac);
    }
    public static void RemoveMultiAction(MultiAction ac)
    {
        actionOrder.Remove(ac);
    }

    #endregion


    public static void ToggleVisible(bool visible)
    {
        Instance.gameObject.SetActive(visible);
        ToggleActive(visible);
    }
    public static void ToggleActive(bool active)
    {
        if (Instance)
        {
            Instance.isActive = active;
        }
    }
    public void ClickButton()
    {

        iGameAction ac = CurrentAction;
        if (actionOrder.Count > 0)
        {
            actionOrder.RemoveAt(0);
        }
        ac.Invoke();
    }

   
    #region Device Back Button
    private void Update()
    {
        if (isActive)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                ClickButton();
            }
        }
    }
    #endregion


}


