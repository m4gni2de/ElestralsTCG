using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Pipeline;
using UnityEngine;

public class WorldCanvas : GlobalObject
{
    public static WorldCanvas Instance { get; set; }
    public Canvas canvas;

    private Canvas _scalerCanvas = null;
    Canvas scalerCanvas { get { _scalerCanvas ??= GetComponentInChildren<Canvas>(); return _scalerCanvas; } }

    public static float scaleX { get => Instance.canvas.transform.localScale.x; }
    public static float scaleY { get => Instance.canvas.transform.localScale.y; }

    public static float Height { get => Instance.canvas.transform.localScale.y; }
    public static float Width { get => Instance.canvas.transform.localScale.x; }

    protected override void Register()
    {
        Instance = this;
        //StartCoroutine(RegisterObject());
    }

    
}
