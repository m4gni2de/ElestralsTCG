using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public string key;

    private bool isSet = false;
    private Vector3 _position { get; set; }
    public Vector2 Position
    {
        get
        {
            if (!isSet)
            {
                _position = transform.localPosition;
                isSet = true;
            }
            return _position;
        }
    }

    
    
}
