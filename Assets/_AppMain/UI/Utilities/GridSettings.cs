using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct GridSettings
{
    #region Properties
    public int itemsPerRow { get; set; }
    public float itemSpace { get; set; }
    public int sidePadding { get; set; }
    public float itemRatio { get; set; }
    public int itemsPerPage { get; set; }
    public bool InfiniteMode { get; private set; }
    public Vector2 GridSpacing { get; private set; }

    public event Action OnSettingsUpdate;
    public void SettingsUpdated()
    {
        OnSettingsUpdate?.Invoke();
    }
    #endregion

    #region Empty
    public bool IsEmpty
    {
        get
        {
            return itemsPerRow == 0;
        }
    }
    public static GridSettings Empty
    {
        get
        {
            GridSettings settings = new GridSettings();
            settings.itemsPerRow = 0;
            settings.itemSpace = 0f;
            settings.sidePadding = 0;
            settings.itemRatio = 0f;
            settings.itemsPerPage = 0;
            settings.GridSpacing = Vector2.zero;
            return settings;
        }
    }
    #endregion
    #region Initialization
    public static GridSettings Create(int itemsPerRow, int sidePadding, int perPage, Vector2 gridSpacing, float itemRatio = .75f, float itemSpace = 1f)
    {
        GridSettings settings = new GridSettings();
        settings.itemsPerRow = itemsPerRow;
        settings.itemSpace = itemSpace;
        settings.sidePadding = sidePadding;
        settings.itemRatio = itemRatio;
        settings.InfiniteMode = perPage < 0;
        settings.itemsPerPage = perPage;
        settings.GridSpacing = gridSpacing;
        return settings;
    }
    public static GridSettings CreateInfinite(int itemsPerRow, int sidePadding, Vector2 gridSpacing, float itemRatio = .75f, float itemSpace = 1f)
    {
        return Create(itemsPerRow, sidePadding, -1, gridSpacing, itemRatio, itemSpace);
    }

    public static GridSettings CatalogDefault()
    {
        GridSettings settings = new GridSettings();
        settings.itemsPerRow = 5;
        settings.itemSpace = 1f;
        settings.sidePadding = 8;
        settings.itemRatio = .75f;
        settings.itemsPerPage = 100;
        settings.InfiniteMode = false;
        settings.GridSpacing = new Vector2(5f, 5f);
        return settings;
    }
    #endregion

    public float TotalPadding()
    {
        return (itemSpace * (float)itemsPerRow) + ((float)sidePadding * 2f) + (GridSpacing.x * (float)itemsPerRow);
    }

    
    public void SetGrid(GridLayoutGroup grid)
    {
        grid.spacing = GridSpacing;
        grid.padding = new RectOffset(sidePadding, sidePadding, grid.padding.top, grid.padding.bottom);
        grid.constraintCount = itemsPerRow;
        
    }

    public void SetGridSize(GridLayoutGroup Grid, float freeWidth)
    {
        float cellWidth = freeWidth / (float)itemsPerRow;
        float cellHeight = (cellWidth / itemRatio);

        Grid.constraintCount = itemsPerRow;

        Grid.cellSize = new Vector2(cellWidth, cellHeight);
        Grid.spacing = GridSpacing;
        Grid.padding.left = sidePadding;
        Grid.padding.right = sidePadding;


    }

    public void ChangeCellSpacing(Vector2 newSpacing)
    {
        GridSpacing = newSpacing;
        SettingsUpdated();
    }


}
