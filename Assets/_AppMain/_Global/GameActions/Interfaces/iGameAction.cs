using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iGameAction 
{
    void Invoke();
    void Invoke(params object[] args);
    void SetAction(Delegate ac, params object[] args);
    string uniqueId { get; set; }
    bool IsEqual(Delegate a);
}
