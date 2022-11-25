using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldCanvas : GlobalObject, iInvert
{
    #region Properties
    public static WorldCanvas Instance { get; set; }
    public Canvas canvas;
    public CanvasScaler worldCanvasScaler;
    public Vector2 Scale;

    [SerializeField]
    private Canvas _scalerCanvas = null;
    public Canvas scalerCanvas { get { _scalerCanvas ??= GetComponentInChildren<Canvas>(); return _scalerCanvas; } }

    #endregion
    #region Functions
    public static float scaleX { get => Instance.canvas.transform.localScale.x; }
    public static float scaleY { get => Instance.canvas.transform.localScale.y; }

    public static float Height { get => Instance.canvas.transform.GetComponent<RectTransform>().rect.height; }
    public static float Width { get => Instance.canvas.transform.GetComponent<RectTransform>().rect.width; }

    public static Vector2 ReferenceRes
    {
        get
        {
            if (Instance.worldCanvasScaler == null)
            {
                Instance.worldCanvasScaler = Instance.GetComponentInParent<CanvasScaler>();
            }
            return Instance.worldCanvasScaler.referenceResolution;
        }
    }
    public Vector2 ScreenScale
    {
        get
        {
            if (worldCanvasScaler == null)
            {
                worldCanvasScaler = GetComponentInParent<CanvasScaler>();
            }

            if (worldCanvasScaler)
            {
                Scale = new Vector2(worldCanvasScaler.referenceResolution.x / Screen.width, worldCanvasScaler.referenceResolution.y / Screen.height);
                return new Vector2(worldCanvasScaler.referenceResolution.x / Screen.width, worldCanvasScaler.referenceResolution.y / Screen.height);
            }
            else
            {
                Scale = Vector2.one;
                return Vector2.one;
            }
        }
    }
    #endregion
    #region Interface
    public void Invert(bool doInvert)
    {
        if (doInvert)
        {
            scalerCanvas.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
        }
        else
        {
            scalerCanvas.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        }
    }
    #endregion

    #region Overrides
    protected override void Register()
    {
        Instance = this;
        this.Scale = ScreenScale;
    }
    #endregion

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
