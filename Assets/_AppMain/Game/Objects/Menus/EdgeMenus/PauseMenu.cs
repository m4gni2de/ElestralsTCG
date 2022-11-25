using System.Collections;
using System.Collections.Generic;
using Gameplay.Menus;
using UnityEngine;

public class PauseMenu : GameMenu, iBlocker, iOwnCanvas
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
        ShowBlocker(GetSortLayer, -500);
    }
    public override void Close()
    {
        base.Close();
        HideBlocker();
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
}
