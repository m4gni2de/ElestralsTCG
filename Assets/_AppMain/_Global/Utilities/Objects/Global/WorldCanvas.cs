using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldCanvas : GlobalObject
{
    public static WorldCanvas Instance { get; set; }
    public Canvas canvas;

    private Canvas _scalerCanvas = null;
    public Canvas scalerCanvas { get { _scalerCanvas ??= GetComponentInChildren<Canvas>(); return _scalerCanvas; } }

    public static float scaleX { get => Instance.canvas.transform.localScale.x; }
    public static float scaleY { get => Instance.canvas.transform.localScale.y; }

    public static float Height { get => Instance.canvas.transform.GetComponent<RectTransform>().rect.height; }
    public static float Width { get => Instance.canvas.transform.GetComponent<RectTransform>().rect.width; }


    public static Vector2 ReferenceRes
    {
        get
        {
            if (Instance.canvasScaler == null)
            {
                Instance.canvasScaler = Instance.GetComponentInParent<CanvasScaler>();
            }
            return Instance.canvasScaler.referenceResolution;
        }
    }

    public CanvasScaler canvasScaler;
    public Vector2 ScreenScale
    {
        get
        {
            if (canvasScaler == null)
            {
                canvasScaler = GetComponentInParent<CanvasScaler>();
            }

            if (canvasScaler)
            {
                Scale = new Vector2(canvasScaler.referenceResolution.x / Screen.width, canvasScaler.referenceResolution.y / Screen.height);
                return new Vector2(canvasScaler.referenceResolution.x / Screen.width, canvasScaler.referenceResolution.y / Screen.height);
            }
            else
            {
                Scale = Vector2.one;
                return Vector2.one;
            }
        }
    }

    public Vector2 Scale;

    protected override void Register()
    {
        Instance = this;
        this.Scale = ScreenScale;
        //StartCoroutine(RegisterObject());
    }


    public static void FindCamera()
    {
        Instance.canvas.renderMode = RenderMode.ScreenSpaceCamera;
        Instance.canvas.worldCamera = Camera.main;
    }
    public static void SetOverlay()
    {
        Instance.canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    }

    
}
