using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iBlocker 
{
    Blocker blocker { get; }
    void ShowBlocker(string sortingLayer, int sortOrder);
    void HideBlocker();
}
