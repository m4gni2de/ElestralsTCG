using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputBox : MonoBehaviour
{
    public Button confirmButton, cancelButton;
    public bool IsHandled;

    

    public void Open()
    {
        IsHandled = false;
    }

    public void Confirm()
    {
        IsHandled = true;
    }
    public void Cancel()
    {
        IsHandled = true;
    }
}
