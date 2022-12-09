using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Blocker : MonoBehaviour, IPointerClickHandler, iOwnCanvas
{

    private string prevSortLayer = "";
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

    private Canvas _canvas = null;
    public Canvas canvas { get { _canvas ??= GetComponent<Canvas>(); return _canvas; } }
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        return;
    }

    public void SetBlocker(string sortingLayer, int sortOrder)
    {
        prevSortLayer = canvas.sortingLayerName;
        SortLayer = sortingLayer;
        SortOrder = sortOrder;

    }
    private void Show()
    {
        gameObject.SetActive(true);
        canvas.overrideSorting = true;
        canvas.sortingLayerName = SortLayer;
        canvas.sortingOrder = SortOrder;
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
    public void ShowBlocker()
    {
        Show();
    }
    public void HideBlocker(bool rollbackSort = true)
    {
        if (rollbackSort)
        {
            SortLayer = prevSortLayer;
        }
        Hide();
    }
}