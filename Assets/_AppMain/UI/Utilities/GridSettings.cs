using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct GridSettings
{
    public int cardsPerRow { get; set; }
    public float cardSpace { get; set; }
    public int sidePadding { get; set; }
    public float cardRatio { get; set; }
    public int cardsPerPage { get; set; }

    public Vector2 GridSpacing { get; private set; }



    public static GridSettings Create(int cardsPerRow, int sidePadding, float cardRatio, int perPage, Vector2 gridSpace, float cardSpace = 1f)
    {
        GridSettings settings = new GridSettings();
        settings.cardsPerRow = cardsPerRow;
        settings.cardSpace = cardSpace;
        settings.sidePadding = sidePadding;
        settings.cardRatio = cardRatio;
        settings.cardsPerPage = perPage;
        settings.GridSpacing = gridSpace;
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

    public float TotalPadding()
    {
        return (cardSpace * (float)cardsPerRow) + ((float)sidePadding * 2f) + (GridSpacing.x * (float)cardsPerRow);
    }

    

    public void UpdateGrid(GridLayoutGroup Grid, float freeWidth)
    {
        float cellWidth = freeWidth / (float)cardsPerRow;
        float cellHeight = (cellWidth / cardRatio);

        Grid.constraintCount = cardsPerRow;

        Grid.cellSize = new Vector2(cellWidth, cellHeight);
        Grid.spacing = GridSpacing;
        Grid.padding.left = sidePadding;
        Grid.padding.right = sidePadding;
    }



}
