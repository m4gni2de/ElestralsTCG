using System.Collections;
using System.Collections.Generic;
using GlobalUtilities;
using UnityEngine;

public interface iGridCell
{
    GameObject GetGameObject();
    int Index { get; }
    void LoadData(object data, int index);
    void Clear();
    void Hide();
    void Show();
    void Remove();
    void SetInsideView(bool isInside);
}

public static class GridCellExtensions
{
    public static bool IsWithinView(this iGridCell cell, RectTransform parent)
    {
        RectTransform rect = cell.GetGameObject().GetComponent<RectTransform>();
        return rect.DoesIntersect(parent);
    }
}
