using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InputBox : MonoBehaviour, iFreeze
{
    [SerializeField]
    protected TMP_InputField _input;
    [SerializeField]
    protected TMP_Text placeHolder;
    [SerializeField]
    protected TMP_Text titleText;
    public Button confirmButton, cancelButton;
    public bool IsHandled;

    protected int _minVal;
    public int minVal { get { return _minVal; } set { _minVal = value; } }
    protected int _maxVal;
    public int maxVal { get { return _maxVal; } set { _maxVal = value; } }


    protected CanvasGroup canvasGroup;

    public enum InputResult
    {
        None = 0,
        Cancel = 1,
        Confirm = 2,
    }
    public InputResult Result;
    protected void SetHandled(InputResult response)
    {
        Result = response;
        IsHandled = true;
        
    }

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void Open()
    {
        IsHandled = false;
        
    }
    public virtual void Close()
    {
        gameObject.SetActive(false);
    }

    public virtual void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    public virtual void Show()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
    public virtual void Confirm()
    {
        SetHandled(InputResult.Confirm);
    }
    public virtual void Cancel()
    {
        SetHandled(InputResult.Cancel);
    }
}
