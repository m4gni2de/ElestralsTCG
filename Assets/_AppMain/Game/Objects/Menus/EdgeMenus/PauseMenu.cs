using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.Menus;
using nsSettings;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : GameMenu, iBlocker, iOwnCanvas, iFreeze
{

    #region Interface
    [SerializeField]
    private Blocker m_Blocker;
    public Blocker blocker { get { return m_Blocker; } }
    public void ShowBlocker(string sortingLayer, int sortOrder)
    {
        blocker.SetBlocker(sortingLayer, sortOrder);
        blocker.ShowBlocker();
    }
    public void HideBlocker()
    {
        blocker.HideBlocker();
    }

    private Canvas _canvas;
    public Canvas canvas
    {
        get
        {
            _canvas ??= GetComponent<Canvas>();
            return _canvas;
        }
    }
    public string SortLayer
    {
        get { return canvas.sortingLayerName; }
        set { canvas.overrideSorting = true; canvas.sortingLayerName = value; }
    }

    public int SortOrder
    {
        get { return canvas.sortingOrder; }
        set { canvas.overrideSorting = true; canvas.sortingOrder = value; }
    }
    #endregion

    #region Properties
    [SerializeField] private Slider zoomSlider;
    [SerializeField] private MagicTextBox txtZooom;
    #endregion

    #region Overrides
    protected override string GetSortLayer { get => "InputMenus"; }
    protected override void Setup()
    {
        canvas.sortingLayerName = CanvasSortLayer;
        base.Setup();
    }
    public override void Open()
    {
        base.Open();
        this.Freeze();
        ShowBlocker(GetSortLayer, -500);
        SetZoomSlider();
    }
    public override void Close()
    {
        base.Close();
        HideBlocker();
        if (isDirty)
        {
            SettingsManager.Graphics.Save();
            _isDirty = false;
        }
        this.ThawOnRelease();
    }
    #endregion

    public void LeaveGame()
    {
        App.AskYesNo("Do you want to forfeit this game and leave?", TryLeave);
    }

    protected void TryLeave(bool isLeaving)
    {
        Close();
        if (isLeaving)
        {
            GameManager.Instance.LeaveGame();
        }
    }


    #region Zoom
    private void SetZoomSlider()
    {
        zoomSlider.maxValue = 1.3f;
        zoomSlider.minValue = .7f;
        zoomSlider.value = SettingsManager.Graphics.Settings.cameraZoom;

        float val = 1f - (zoomSlider.value - 1f);

        txtZooom.SetText($"x{string.Format("{0:0.00}", val)}");
        zoomSlider.onValueChanged.RemoveAllListeners();
        zoomSlider.onValueChanged.AddListener(OnZoomSliderValueChanged);
    }
    
    private void OnZoomSliderValueChanged(float newVal)
    {
        txtZooom.SetText($"x{string.Format("{0:0.00}", newVal)}");
        SettingsManager.Graphics.Settings.cameraZoom = (float)Math.Round(newVal, 2);
        CameraMotion.ReScale();
        _isDirty = true;
    }
    #endregion
}
