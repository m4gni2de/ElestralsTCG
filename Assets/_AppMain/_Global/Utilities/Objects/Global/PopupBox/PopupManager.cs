using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PopupBox;
using System;
using UnityEngine.Events;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; set; }
    public static BasePopup ActivePopup;


    public YesNoBox YesNo;
    public DisplayBox Message;
    public DropdownPopup Dropdown;


    private static List<BasePopup> _popList = null;
    public static List<BasePopup> PopList
    {
        get
        {
            _popList ??= new List<BasePopup>();
            return _popList;
        }
    }

    #region Event Watchers
    private static UnityEvent _OnCloseWatcher = null;
    protected static UnityEvent OnCloseWatchr
    {
        get
        {
            _OnCloseWatcher ??= new UnityEvent();
            return _OnCloseWatcher;
        }
    }

    public void ClosePopupWatcher()
    {
        _OnCloseWatcher.RemoveAllListeners();
        SetActivePopup();
        _OnCloseWatcher = null;
    }
    public void AddCloseWatcher(BasePopup popup, UnityEvent ev)
    {
        _OnCloseWatcher = ev;
        _OnCloseWatcher.AddListener(ClosePopupWatcher);
    }
    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
         
    }
    public void Close()
    {
        gameObject.SetActive(false);
        AppManager.Instance.appBlocker.HideBlocker();
    }
    public void Show()
    {
        gameObject.SetActive(true);
        AppManager.Instance.appBlocker.ShowBlocker();
        
    }

    
    public static void SetActivePopup(BasePopup pop = null, bool closeOnBack = true)
    {
        if (pop == null)
        {
            if (ActivePopup != null)
            {
                ActivePopup.ForceClose();
                ActivePopup.Refresh();
                ActivePopup.gameObject.SetActive(false);
                
                if (PopList.Contains(ActivePopup)) { PopList.Remove(ActivePopup); }


            }
            
            ActivePopup = null;
            Instance.Close();
            //DisplayManager.RemoveAction(() => ActivePopup.Cancel());
            
        }
        else
        {
            Instance.Show();
            if (!PopList.Contains(pop))
            {
                PopList.Add(pop);
            }
            AddPopup(pop, closeOnBack);
            ActivePopup = pop;
            
        }
        
    }

    public static void AddPopup(BasePopup pop, bool closeOnBack)
    {
        if (!PopList.Contains(pop))
        {
            PopList.Add(pop);
            if (closeOnBack)
            {
                DisplayManager.AddAction(pop.Cancel);
            }
            
        }
    }
    public static void RemovePopup(BasePopup pop)
    {
        if (PopList.Contains(pop)) { PopList.Remove(pop); DisplayManager.RemoveAction(pop.Cancel); }
    }

    #region Popup Types
    public void AskYesNo(string msg, Action<bool> callback, bool createClone = false)
    {
        if (createClone)
        {
            YesNoBox clone = Instantiate(YesNo, YesNo.transform.parent);
            YesNo.gameObject.SetActive(false);
            clone.Show(msg, callback);
            AddPopup(clone, true);
            StartCoroutine(AwaitPopup(clone));
        }
        else
        {
            SetActivePopup(YesNo);
            YesNo.Show(msg, callback);
            StartCoroutine(AwaitPopup());
        }
        
    }
    public void CloneYesNo(string msg, Action<bool> callback)
    {
        YesNoBox clone = Instantiate(YesNo, YesNo.transform.parent);
        YesNo.gameObject.SetActive(false);
        clone.Show(msg, callback);
        StartCoroutine(AwaitPopup(clone));
        
    }

    public void DisplayNewMessage(string msg,Action callback, bool showConfirm, bool showCancel, bool closeOnBack = true)
    {
        StopCoroutine(AwaitPopup());
        SetActivePopup(Message, closeOnBack);
        Message.Show(msg, callback,showConfirm, showCancel);
        StartCoroutine(AwaitPopup());

    }
    public void DisplayMessage(string msg, Action callback, bool showConfirm, bool showCancel, bool closeOnBack = true)
    {
        SetActivePopup(Message, closeOnBack);
        Message.Show(msg, callback, showConfirm, showCancel);
        StartCoroutine(AwaitPopup());
    }
    public void DisplayTimedMessage(string msg, Action callback, float time, bool closeOnBack = true)
    {
        SetActivePopup(Message, closeOnBack);
        Message.ShowPersistent(msg, callback, time);
        StartCoroutine(AwaitPopup());
    }
    public void DisplayConditionalMessage(string msg, Action callback, Func<bool> func, bool reqValue, bool closeOnBack = true)
    {
        SetActivePopup(Message, closeOnBack);
        Message.ShowUntilCondition(msg, func, reqValue);
    }

    public void ShowDropdown(string title, List<string> options, Action<string> callback, bool closeOnBack = true)
    {
        SetActivePopup(Dropdown, closeOnBack);
        Dropdown.ShowStrings(title, options, callback);
        StartCoroutine(AwaitPopup());
    }
    public void ShowDropdown<T>(string title, List<T> options, Action<T> callback, string propName, bool closeOnBack = true)
    {
        SetActivePopup(Dropdown, closeOnBack);
        Dropdown.Show(title, options, callback, propName);
        StartCoroutine(AwaitPopup());
    }
   

    #endregion
    protected IEnumerator AwaitPopup()
    {
        
        do
        {
            yield return new WaitForEndOfFrame();

        } while (true && !ActivePopup.Handled);

        SetActivePopup();
    }
    protected IEnumerator AwaitPopup(BasePopup clone)
    {

        do
        {
            yield return new WaitForEndOfFrame();

        } while (true && !clone.Handled);

        RemovePopup(clone);
        Destroy(clone);

        if (ActivePopup)
        {
            ActivePopup.gameObject.SetActive(true);
        }
    }


}
