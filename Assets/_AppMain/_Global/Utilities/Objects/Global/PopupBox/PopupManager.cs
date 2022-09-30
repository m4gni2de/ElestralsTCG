using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PopupBox;
using System;
public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; set; }
    public static BasePopup ActivePopup;

    public GameObject objectPool;
    public GameObject confirmBase, cancelBase, messageBase;

    public YesNoBox YesNo;
    public DisplayBox Message;
    public DropdownPopup Dropdown;



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
    public void DisplayMessage(string msg, Action callback)
    {
        SetActivePopup(Message);
        Message.Show(msg, callback);
        StartCoroutine(AwaitPopup());
    }
    public void ShowDropdown(string title, List<string> options, Action<string> callback)
    {
        SetActivePopup(Dropdown, false);
        Dropdown.Show(title, options, callback);
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
