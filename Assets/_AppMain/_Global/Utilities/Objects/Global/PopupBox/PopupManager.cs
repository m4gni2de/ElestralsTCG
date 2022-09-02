using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PopupBox;
using System;
public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }
    public static BasePopup ActivePopup;

    public GameObject objectPool;

    public YesNoBox YesNo;
    public DisplayBox Message;


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
    public void Show()
    {
        gameObject.SetActive(true);
        objectPool.SetActive(true);
    }
    public static void SetActivePopup(BasePopup pop = null)
    {
        if (pop == null)
        {
            ActivePopup = null;
            Instance.Close();
        }
        else
        {
            Instance.Show();
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
