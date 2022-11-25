using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct GridSettings
{
    #region Properties
    public int cardsPerRow { get; set; }
    public float cardSpace { get; set; }
    public int sidePadding { get; set; }
    public float cardRatio { get; set; }
    public int cardsPerPage { get; set; }

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
            return cardsPerRow == 0;
        }
    }
    public static GridSettings Empty
    {
        get
        {
            GridSettings settings = new GridSettings();
            settings.cardsPerRow = 0;
            settings.cardSpace = 0f;
            settings.sidePadding = 0;
            settings.cardRatio = 0f;
            settings.cardsPerPage = 0;
            settings.GridSpacing = Vector2.zero;
            return settings;
        }
    }
    #endregion
    #region Initialization
    public static GridSettings Create(int cardsPerRow, int sidePadding, int perPage, Vector2 gridSpacing, float cardRatio = .75f, float cardSpace = 1f)
    {
        GridSettings settings = new GridSettings();
        settings.cardsPerRow = cardsPerRow;
        settings.cardSpace = cardSpace;
        settings.sidePadding = sidePadding;
        settings.cardRatio = cardRatio;
        settings.cardsPerPage = perPage;
        settings.GridSpacing = gridSpacing;
        return settings;
    }

    public static GridSettings CatalogDefault()
    {
        GridSettings settings = new GridSettings();
        settings.cardsPerRow = 5;
        settings.cardSpace = 1f;
        settings.sidePadding = 8;
        settings.cardRatio = .75f;
        settings.cardsPerPage = 100;
        settings.GridSpacing = new Vector2(5f, 5f);
        return settings;
    }
    #endregion

    public float TotalPadding()
    {
        return (cardSpace * (float)cardsPerRow) + ((float)sidePadding * 2f) + (GridSpacing.x * (float)cardsPerRow);
    }

    
    public void SetGrid(GridLayoutGroup grid)
    {
        grid.spacing = GridSpacing;
        grid.padding = new RectOffset(sidePadding, sidePadding, grid.padding.top, grid.padding.bottom);
        grid.constraintCount = cardsPerRow;
        
    }

    public void SetGridSize(GridLayoutGroup Grid, float freeWidth)
    {
        float cellWidth = freeWidth / (float)cardsPerRow;
        float cellHeight = (cellWidth / cardRatio);

        Grid.constraintCount = cardsPerRow;

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
