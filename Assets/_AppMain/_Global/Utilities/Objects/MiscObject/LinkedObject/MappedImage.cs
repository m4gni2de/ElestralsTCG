using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using UnityEngine;

public class MappedImage 
{
    public int SortDistance;
    private int _orderBase;
    private string _layerBase;
    private SpriteRenderer sp;

    public MappedImage(int baseOrder, SpriteRenderer renderer, int sortOrder, string sortLayer)
    {
        _orderBase = sortOrder;
        _layerBase = sortLayer;

        SortDistance = sortOrder - baseOrder;
        sp = renderer;
    }


    public void UpdateSortOrder(int newBaseOrder)
    {
        int newOrder = newBaseOrder + SortDistance;
        sp.sortingOrder = newOrder;
    }

    public void UpdateSortLayer(string layerName)
    {
        sp.sortingLayerName = layerName;
    }


}
