using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iTouchContainer
{
    List<TouchButton> touchButtons { get; }
    bool IsClicked { get; set; }
    void OnClickStart<T>(T obj);
    void OnClickEnd<T>(T obj);
    
    
}
