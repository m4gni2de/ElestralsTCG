using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalObject : MonoBehaviour
{
    private void Awake()
    {
        Register();
    }

    protected virtual void Register()
    {
        //throw new System.Exception("Object name not registered.");
    }
}
