using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameplay;

public interface iSelectSlot 
{
    TouchObject touch { get; }
    void ClickSlot();
    void Optimize();
}
