using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InputBox : MonoBehaviour
{
    

    public Button confirmButton, cancelButton;
    public bool IsHandled;

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

    public void Open()
    {
        IsHandled = false;
    }
    public virtual void Close()
    {
        gameObject.SetActive(false);
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
