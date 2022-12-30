using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iShowHide 
{
    event Action<bool> OnDisplayChanged;
    void Show();
    void Hide();

}
