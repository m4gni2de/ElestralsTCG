using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Blocker : MonoBehaviour, IPointerClickHandler
{
    
    protected string SortLayer { get; set; }
    protected int SortOrder { get; set; }

    private Canvas _canvas = null;
    protected Canvas canvas { get { _canvas ??= GetComponent<Canvas>(); return _canvas; } }
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        return;
    }

    public void SetBlocker(string sortingLayer, int sortOrder)
    {
        SortLayer = sortingLayer;
        SortOrder = sortOrder;
        canvas.overrideSorting = true;
        canvas.sortingLayerName = SortLayer;
        canvas.sortingOrder = SortOrder;

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
    public void HideBlocker()
    {
        Hide();
    }
}