using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iHold
{
    void HoldCallback(bool startHold);
    bool isHeld { get; set; }
    bool isClicked { get; set; }
}
