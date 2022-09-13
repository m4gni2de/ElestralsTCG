using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TouchController : MonoBehaviour
{
    public List<TouchObject> touchObjects = new List<TouchObject>();

    public void AddObject(TouchObject obj)
    {
        if (!touchObjects.Contains(obj))
        {
            touchObjects.Add(obj);
        }
    }

    //WORK ON MAKING TOUCH BUTTONS WORK WITH CONTROLLERS THAT CAN TOGGLE EVENTS RATHER THAN MAKING THE TOUCH BUTTONS THEMSELVES HAVE EVENT LISTENERS
}
