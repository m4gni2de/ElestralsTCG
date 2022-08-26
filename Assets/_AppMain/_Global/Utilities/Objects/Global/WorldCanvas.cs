using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCanvas : GlobalObject
{
    public static WorldCanvas Instance { get; set; }
    public Canvas canvas;

    private Canvas _scalerCanvas = null;
    Canvas scalerCanvas { get { _scalerCanvas ??= GetComponentInChildren<Canvas>(); return _scalerCanvas; } }

    protected override void Register()
    {
        Instance = this;
        //StartCoroutine(RegisterObject());
    }

    
}
