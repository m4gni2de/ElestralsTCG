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

    public GameObject objectPool;
    public GameObject confirmBase, cancelBase, messageBase;

    public YesNoBox YesNo;
    public DisplayBox Message;
    public DropdownPopup Dropdown;

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
        objectPool.SetActive(false);
    }
    public void Show(bool useObjectPool = true)
    {
        gameObject.SetActive(true);
        objectPool.SetActive(useObjectPool);
        confirmBase.SetActive(useObjectPool);
        cancelBase.SetActive(useObjectPool);
        messageBase.SetActive(useObjectPool);
    }

    
    public static void SetActivePopup(BasePopup pop = null, bool useObjectPool = true)
    {
        if (pop == null)
        {
            if (ActivePopup != null)
            {
                ActivePopup.ForceClose();
            }
            ActivePopup = null;
            Instance.Close();
        }
        else
        {
            Instance.Show(useObjectPool);
            ActivePopup = pop;
        }
        
    }

    #region Popup Types
    public void AskYesNo(string msg, Action<bool> callback)
    {
        SetActivePopup(YesNo);
        YesNo.Show(msg, callback);
        StartCoroutine(AwaitPopup());
    }
    
    public void DisplayNewMessage(string msg,Action callback, bool showConfirm, bool showCancel)
    {
        StopCoroutine(AwaitPopup());
        SetActivePopup(Message);
        Message.Show(msg, callback,showConfirm, showCancel);
        StartCoroutine(AwaitPopup());

    }
    public void DisplayMessage(string msg, Action callback, bool showConfirm, bool showCancel)
    {
        SetActivePopup(Message);
        Message.Show(msg, callback, showConfirm, showCancel);
        StartCoroutine(AwaitPopup());
    }
    public void DisplayTimedMessage(string msg, Action callback, float time)
    {
        SetActivePopup(Message);
        Message.ShowPersistent(msg, callback, time);
        StartCoroutine(AwaitPopup());
    }
    public void DisplayConditionalMessage(string msg, Action callback, Func<bool> func, bool reqValue)
    {
        SetActivePopup(Message);
        Message.ShowUntilCondition(msg, func, reqValue);
    }

    public void ShowDropdown(string title, List<string> options, Action<string> callback)
    {
        SetActivePopup(Dropdown, false);
        Dropdown.ShowStrings(title, options, callback);
        StartCoroutine(AwaitPopup());
    }
    public void ShowDropdown<T>(string title, List<T> options, Action<T> callback, string propName)
    {
        SetActivePopup(Dropdown, false);
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

   
}
