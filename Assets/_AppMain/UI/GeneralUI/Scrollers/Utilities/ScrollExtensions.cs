using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class ScrollExtensions 
{
    public struct ScrollCoordinates
    {
        public int page { get; set; }
        public int row { get; set; }
        public int column { get; set; }
        public float scrollValue { get; set; }
        public int indexOnPage { get; set; }
    }

    public static ScrollCoordinates FindCoordinates(this ScrollRect rect, int index, int totalResults, GridSettings sett)
    {
        ScrollCoordinates coords = new ScrollCoordinates();

        int pageIndex = GetPageOfIndex(index, GetTotalPages(totalResults, sett), sett);
        coords.page = pageIndex;
        GetRowColumnOfIndex(index, sett, ref coords);

        return coords;
    }
   
    private static int GetTotalPages(int cardCount, GridSettings sett)
    {
        if (cardCount <= sett.itemsPerPage) { return 1; }
        if (cardCount % sett.itemsPerPage > 0)
        {
            return (cardCount / sett.itemsPerPage) + 1;
        }
        return cardCount / sett.itemsPerPage;
    }

    private static int GetPageOfIndex(int index, int totalPages, GridSettings sett)
    {

        for (int i = 0; i < totalPages; i++)
        {
            int maxIndex = (i + 1) * sett.itemsPerPage;
            if (index <= maxIndex) { return i + 1; }
        }
        return totalPages;
    }

    private static void GetRowColumnOfIndex(int index, GridSettings sett, ref ScrollCoordinates scrollCoords)
    {
        int minVal = (scrollCoords.page - 1) * sett.itemsPerPage;

        int rows;
        if (sett.itemsPerPage % sett.itemsPerRow > 0) { rows = (sett.itemsPerPage / sett.itemsPerRow) + 1; } else { rows = (sett.itemsPerPage / sett.itemsPerRow); }

        int cellOffset = index - minVal;
        scrollCoords.indexOnPage = cellOffset;
        int itemCount = 0;

        int rowVal = -1;
        for (int i = 0; i < rows; i++)
        {
            itemCount += (i + 1) * sett.itemsPerRow;
            if (cellOffset <= itemCount) { rowVal = i; break; }
        }
        if (rowVal == -1) { rowVal = rows - 1; }
        scrollCoords.row = rowVal;


        int colVal = -1;
        for (int i = 0; i < sett.itemsPerRow; i++)
        {
            int cell = ((rowVal * sett.itemsPerRow) - 1) + i;
            if (index == cell) { colVal = i; break; }
        }
        if (colVal == -1) { colVal = sett.itemsPerRow - 1; }
        scrollCoords.column = colVal;


        float scrollPerc = 1f - (float)rowVal / (float)rows;
        scrollCoords.scrollValue = scrollPerc;
        


    }
}
